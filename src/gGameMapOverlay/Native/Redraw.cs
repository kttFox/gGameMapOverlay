using System.Runtime.InteropServices;

namespace gGameMapOverlay.Native;

/// <summary>コントロールの描き直しを止めて、まとめて 1 回で描き直す (ログを書き換えるときのちらつきを防ぐ)。</summary>
internal static class Redraw
{
    private const int WM_SETREDRAW = 0x000B;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    /// <summary>update の間は control を描き直さず、終わってから 1 回だけ描き直す。</summary>
    public static void Batch(Control control, Action update)
    {
        if (!control.IsHandleCreated)
        {
            update();
            return;
        }
        SendMessage(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        try
        {
            update();
        }
        finally
        {
            SendMessage(control.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            control.Invalidate(true);
        }
    }

    private const int EM_GETFIRSTVISIBLELINE = 0x00CE;
    private const int EM_LINESCROLL = 0x00B6;

    /// <summary>テキストボックスで、いちばん上に見えている行の番号。</summary>
    public static int FirstVisibleLine(TextBox box) =>
        box.IsHandleCreated ? (int)SendMessage(box.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero) : 0;

    private const int WM_VSCROLL = 0x0115;
    private const int SB_BOTTOM = 7;

    /// <summary>テキストボックスを一番下までスクロールする (描き直しを止めている間は ScrollToCaret が効かないことがあるため)。</summary>
    public static void ScrollToBottom(TextBox box)
    {
        if (box.IsHandleCreated)
        {
            SendMessage(box.Handle, WM_VSCROLL, SB_BOTTOM, IntPtr.Zero);
        }
    }

    /// <summary>テキストボックスを、line 行目がいちばん上に見えるまで縦にスクロールする。</summary>
    public static void ScrollToLine(TextBox box, int line)
    {
        if (box.IsHandleCreated)
        {
            SendMessage(box.Handle, EM_LINESCROLL, IntPtr.Zero, line - FirstVisibleLine(box));
        }
    }

    /// <summary>コントロールをダブルバッファで描くようにする (ListView などは外から設定できないので、リフレクションで設定する)。</summary>
    public static void EnableDoubleBuffer(Control control) =>
        typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(control, true);
}
