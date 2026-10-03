using System.Runtime.InteropServices;

namespace gGameMapOverlay.Native;

/// <summary>タイトルバーのアイコンの右クリックメニュー (システムメニュー) に自分の項目を足す。</summary>
internal static class SystemMenu
{
    public const int WM_SYSCOMMAND = 0x0112;

    private const int MF_STRING = 0x0000;
    private const int MF_SEPARATOR = 0x0800;
    private const int MF_BYCOMMAND = 0x0000;
    private const int MF_BYPOSITION = 0x0400;
    private const int MF_CHECKED = 0x0008;
    private const int MF_UNCHECKED = 0x0000;

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string? lpNewItem);

    [DllImport("user32.dll")]
    private static extern int CheckMenuItem(IntPtr hMenu, int uIDCheckItem, int uCheck);

    /// <summary>先頭に項目を足し、その下に区切り線を入れる。id は 0xF000 より小さい 16 の倍数にする (下位 4 ビットは Windows が使う)。</summary>
    public static void InsertFirst(Form form, int id, string text)
    {
        var menu = GetSystemMenu(form.Handle, false);
        InsertMenu(menu, 0, MF_BYPOSITION | MF_SEPARATOR, 0, null);
        InsertMenu(menu, 0, MF_BYPOSITION | MF_STRING, id, text);
    }

    public static void SetChecked(Form form, int id, bool check) =>
        CheckMenuItem(GetSystemMenu(form.Handle, false), id, MF_BYCOMMAND | (check ? MF_CHECKED : MF_UNCHECKED));

    /// <summary>WM_SYSCOMMAND が自分の項目 id のものか。</summary>
    public static bool IsCommand(ref Message m, int id) =>
        m.Msg == WM_SYSCOMMAND && ((int)m.WParam & 0xFFF0) == id;
}
