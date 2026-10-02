using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace gGameMapOverlay.Native;

/// <summary>Win32 API でゲームウィンドウを探し、クライアント領域のスクリーン座標を得る。</summary>
public static class GameWindow
{
    private const uint ProcessQueryLimitedInformation = 0x1000;

    private delegate bool EnumWindowsProc(nint hwnd, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X, Y;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc callback, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLengthW(nint hwnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextW(nint hwnd, StringBuilder text, int maxCount);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint hwnd, out uint processId);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(nint hwnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(nint hwnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(nint hwnd);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(nint hwnd, out Rect rect);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(nint hwnd, ref Point point);

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint hwnd, uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public Rect Monitor;
        public Rect Work;
        public uint Flags;
    }

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfoW(nint monitor, ref MonitorInfo info);

    private const uint MonitorDefaultToNearest = 2;

    [DllImport("user32.dll")]
    private static extern nint GetAncestor(nint hwnd, uint flags);

    private const uint GaRoot = 2;

    private const uint EventSystemForeground = 0x0003;
    // フックの処理を自プロセスで受ける (ゲームのプロセスに DLL を読み込ませない)。
    private const uint WinEventOutOfContext = 0x0000;

    public delegate void WinEventProc(nint hook, uint eventType, nint hwnd, int idObject, int idChild, uint thread, uint time);

    [DllImport("user32.dll")]
    private static extern nint SetWinEventHook(uint eventMin, uint eventMax, nint module, WinEventProc callback, uint processId, uint threadId, uint flags);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(nint hook);

    /// <summary>
    /// 前面のウィンドウが変わるたびに callback を呼ぶフックを登録する。登録したスレッドのメッセージループで呼ばれる。
    /// callback は解除するまで GC されないよう呼び出し側で保持すること。失敗すると 0。
    /// </summary>
    public static nint HookForegroundChanged(WinEventProc callback) =>
        SetWinEventHook(EventSystemForeground, EventSystemForeground, 0, callback, 0, 0, WinEventOutOfContext);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint OpenProcess(uint access, bool inherit, uint processId);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool QueryFullProcessImageNameW(nint process, uint flags, StringBuilder name, ref uint size);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(nint handle);

    public static string WindowTitle(nint hwnd)
    {
        var length = GetWindowTextLengthW(hwnd);
        if (length <= 0)
        {
            return "";
        }
        var buffer = new StringBuilder(length + 1);
        GetWindowTextW(hwnd, buffer, buffer.Capacity);
        return buffer.ToString();
    }

    public static string ProcessName(nint hwnd)
    {
        GetWindowThreadProcessId(hwnd, out var processId);
        var process = OpenProcess(ProcessQueryLimitedInformation, false, processId);
        if (process == 0)
        {
            return "";
        }
        try
        {
            var size = 32768u;
            var buffer = new StringBuilder((int)size);
            return QueryFullProcessImageNameW(process, 0, buffer, ref size) ? Path.GetFileName(buffer.ToString()) : "";
        }
        finally
        {
            CloseHandle(process);
        }
    }

    /// <summary>
    /// タイトル部分一致とプロセス名一致の両方を満たすウィンドウを優先し、なければタイトル部分一致、
    /// それもなければプロセス名一致のウィンドウを返す。
    /// </summary>
    public static nint Find(string processName, string windowTitle)
    {
        nint bothMatch = 0, titleMatch = 0, processMatch = 0;
        EnumWindows((hwnd, _) =>
        {
            if (!IsWindowVisible(hwnd))
            {
                return true;
            }
            var title = !string.IsNullOrEmpty(windowTitle)
                && WindowTitle(hwnd).Contains(windowTitle, StringComparison.OrdinalIgnoreCase);
            var process = !string.IsNullOrEmpty(processName)
                && string.Equals(ProcessName(hwnd), processName, StringComparison.OrdinalIgnoreCase);
            if (title && process)
            {
                bothMatch = hwnd;
                return false;
            }
            if (title && titleMatch == 0)
            {
                titleMatch = hwnd;
            }
            if (process && processMatch == 0)
            {
                processMatch = hwnd;
            }
            return true;
        }, 0);
        return bothMatch != 0 ? bothMatch : titleMatch != 0 ? titleMatch : processMatch;
    }

    /// <summary>キャプチャ対象として選べるウィンドウ。</summary>
    public sealed record Candidate(nint Hwnd, string Title, string ProcessName)
    {
        public override string ToString() => string.IsNullOrEmpty(ProcessName) ? Title : $"{Title} ({ProcessName})";
    }

    private const uint GwOwner = 4;
    private const int WsExToolWindow = 0x80;

    /// <summary>
    /// キャプチャ対象として選べるウィンドウの一覧 (Z オーダー順)。タスクバーに出るような、見えていてタイトルのある
    /// トップレベルウィンドウだけで、自プロセスのウィンドウは除く。
    /// </summary>
    public static IReadOnlyList<Candidate> Candidates()
    {
        var list = new List<Candidate>();
        EnumWindows((hwnd, _) =>
        {
            if (!IsWindowVisible(hwnd) || GetWindow(hwnd, GwOwner) != 0 || (GetWindowLong(hwnd, GwlExStyle) & WsExToolWindow) != 0)
            {
                return true;
            }
            if (DwmGetWindowAttribute(hwnd, DwmwaCloaked, out var cloaked, sizeof(int)) == 0 && cloaked != 0)
            {
                return true; // 別の仮想デスクトップのウィンドウや、中断中のストアアプリなど
            }
            GetWindowThreadProcessId(hwnd, out var processId);
            var title = WindowTitle(hwnd);
            if (processId != Environment.ProcessId && title.Length > 0)
            {
                list.Add(new Candidate(hwnd, title, ProcessName(hwnd)));
            }
            return true;
        }, 0);
        return list;
    }

    /// <summary>
    /// ゲームが前面にあるか。画面キャプチャは「見えているもの」を読むので、
    /// 他のウィンドウが上に重なっている間は読み取ってはいけない。
    /// 自プロセスのウィンドウ (読み取りツール自身) が前面の場合も許可する。
    /// </summary>
    /// <summary>ゲームのウィンドウそのものがアクティブか (IsForeground と違い、本ツールのウィンドウが前面なら false)。</summary>
    public static bool IsActive(nint hwnd) => hwnd != 0 && GetForegroundWindow() == hwnd;

    public static bool IsForeground(nint hwnd)
    {
        var foreground = GetForegroundWindow();
        if (hwnd == 0 || foreground == 0)
        {
            return false;
        }
        static nint Root(nint window) => GetAncestor(window, GaRoot) is var root && root != 0 ? root : window;
        if (Root(foreground) == Root(hwnd))
        {
            return true;
        }
        GetWindowThreadProcessId(foreground, out var processId);
        return processId == Environment.ProcessId;
    }

    [DllImport("user32.dll")]
    private static extern nint GetWindow(nint hwnd, uint command);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(nint hwnd, out Rect rect);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(nint hwnd, uint attribute, out int value, int size);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static extern int GetWindowLong(nint hwnd, int index);

    [DllImport("user32.dll")]
    private static extern bool GetLayeredWindowAttributes(nint hwnd, out uint key, out byte alpha, out uint flags);

    private const int GwlExStyle = -20;
    private const int WsExTransparent = 0x20;
    private const int WsExLayered = 0x80000;
    private const uint LwaColorKey = 1;
    private const uint LwaAlpha = 2;

    /// <summary>
    /// クリックを透過する、完全に透明、または透過色を使う (大部分が透けている) ウィンドウか。
    /// こうしたウィンドウは画面全体を覆っていてもゲームを隠していないことがほとんど。
    /// </summary>
    private static bool IsSeeThrough(nint hwnd)
    {
        var exStyle = GetWindowLong(hwnd, GwlExStyle);
        if ((exStyle & WsExTransparent) != 0)
        {
            return true;
        }
        if ((exStyle & WsExLayered) == 0 || !GetLayeredWindowAttributes(hwnd, out _, out var alpha, out var flags))
        {
            return false;
        }
        return (flags & LwaColorKey) != 0 || (flags & LwaAlpha) != 0 && alpha == 0;
    }

    private const uint GwHwndPrev = 3;
    private const uint DwmwaCloaked = 14;

    /// <summary>
    /// ゲームの指定した領域 (スクリーン座標) が画面に見えているか。前面になくても、最小化されておらず、
    /// 他のプロセスのウィンドウが領域に重なっていなければ見えているとみなす。自プロセスのウィンドウは無視する。
    /// </summary>
    public static bool IsVisibleOnScreen(nint hwnd, IReadOnlyList<Rectangle> areas)
    {
        if (IsForeground(hwnd))
        {
            return true;
        }
        if (hwnd == 0 || !IsWindow(hwnd) || !IsWindowVisible(hwnd) || IsIconic(hwnd))
        {
            return false;
        }
        static nint Root(nint window) => GetAncestor(window, GaRoot) is var root && root != 0 ? root : window;
        // Z オーダーでゲームより上にあるトップレベルウィンドウを順に調べる。
        for (var above = GetWindow(Root(hwnd), GwHwndPrev); above != 0; above = GetWindow(above, GwHwndPrev))
        {
            if (!IsWindowVisible(above) || IsIconic(above))
            {
                continue;
            }
            if (DwmGetWindowAttribute(above, DwmwaCloaked, out var cloaked, sizeof(int)) == 0 && cloaked != 0)
            {
                continue; // 別の仮想デスクトップのウィンドウなど、実際には見えていない
            }
            if (IsSeeThrough(above))
            {
                continue; // マウスジェスチャーの描画用など、透明な全画面オーバーレイは隠していない
            }
            GetWindowThreadProcessId(above, out var processId);
            if (processId == Environment.ProcessId || !GetWindowRect(above, out var rect))
            {
                continue;
            }
            var bounds = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            if (areas.Any(area => area.IntersectsWith(bounds)))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ウィンドウが表示されているモニターの解像度 (物理ピクセル)。取得できなければ null。
    /// 呼び出し側のプロセスは per-monitor DPI 対応である必要がある (表示スケールで換算されない値を得るため)。
    /// </summary>
    public static Size? MonitorSize(nint hwnd)
    {
        var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        var monitor = hwnd == 0 ? 0 : MonitorFromWindow(hwnd, MonitorDefaultToNearest);
        if (monitor == 0 || !GetMonitorInfoW(monitor, ref info))
        {
            return null;
        }
        return new Size(info.Monitor.Right - info.Monitor.Left, info.Monitor.Bottom - info.Monitor.Top);
    }

    /// <summary>クライアント領域のスクリーン座標。最小化中などは null。</summary>
    public static Rectangle? ClientScreenRect(nint hwnd)
    {
        if (hwnd == 0 || !IsWindow(hwnd) || IsIconic(hwnd) || !GetClientRect(hwnd, out var rect))
        {
            return null;
        }
        var origin = new Point();
        if (!ClientToScreen(hwnd, ref origin) || rect.Right <= 0 || rect.Bottom <= 0)
        {
            return null;
        }
        return new Rectangle(origin.X, origin.Y, rect.Right, rect.Bottom);
    }
}
