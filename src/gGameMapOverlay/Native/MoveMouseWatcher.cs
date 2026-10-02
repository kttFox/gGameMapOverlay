using System.Runtime.InteropServices;

namespace gGameMapOverlay.Native;

/// <summary>
/// 右クリックで歩くのを見る。グローバルな低レベルマウスフックを使うが、右ボタンの押し下げ・離しと、押している間のカーソルの位置だけを見る。
/// フックはインストールしたスレッド (UI スレッド) のメッセージループで呼ばれるので、イベントも UI スレッドで起きる。
/// </summary>
internal sealed class MoveMouseWatcher : IDisposable
{
    private const int WH_MOUSE_LL = 14;
    private const int WM_MOUSEMOVE = 0x0200;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;

    private delegate nint HookProc(int code, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, HookProc callback, nint module, uint threadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(nint hook);

    [DllImport("user32.dll")]
    private static extern nint CallNextHookEx(nint hook, int code, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? name);

    private readonly HookProc hookProc; // フックを解除するまで GC されないようフィールドで持つ
    private nint hook;

    public MoveMouseWatcher() => hookProc = OnMouse;

    /// <summary>右ボタンを押している。</summary>
    public bool Held { get; private set; }

    /// <summary>右ボタンを押した (カーソルの画面座標)。</summary>
    public event Action<Point>? Pressed;

    /// <summary>右ボタンを押している間にカーソルが動いた (カーソルの画面座標)。</summary>
    public event Action<Point>? Moved;

    /// <summary>右ボタンを離した (押している間にフックを外したときは起きない)。</summary>
    public event Action? Released;

    public bool Enabled
    {
        get => hook != 0;
        set
        {
            if (value == Enabled)
            {
                return;
            }
            if (value)
            {
                hook = SetWindowsHookEx(WH_MOUSE_LL, hookProc, GetModuleHandle(null), 0);
            }
            else
            {
                UnhookWindowsHookEx(hook);
                hook = 0;
                Held = false;
            }
        }
    }

    private nint OnMouse(int code, nint wParam, nint lParam)
    {
        if (code >= 0)
        {
            // MSLLHOOKSTRUCT の先頭はカーソルの画面座標 (Per-Monitor のスクリーン座標)。
            switch ((int)wParam)
            {
                case WM_RBUTTONDOWN:
                    Held = true;
                    Pressed?.Invoke(new Point(Marshal.ReadInt32(lParam), Marshal.ReadInt32(lParam, 4)));
                    break;
                case WM_MOUSEMOVE when Held:
                    Moved?.Invoke(new Point(Marshal.ReadInt32(lParam), Marshal.ReadInt32(lParam, 4)));
                    break;
                case WM_RBUTTONUP when Held:
                    Held = false;
                    Released?.Invoke();
                    break;
            }
        }
        return CallNextHookEx(hook, code, wParam, lParam);
    }

    public void Dispose() => Enabled = false;
}
