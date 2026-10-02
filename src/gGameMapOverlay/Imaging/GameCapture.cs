using System.Drawing;
using System.Runtime.InteropServices;

namespace gGameMapOverlay.Imaging;

/// <summary>
/// 1 回の撮影。読み取りに使う欄 (クライアント座標) を切り出す。
/// 画面全体を撮る方式では、どの欄も同じ瞬間の画面から切り出す。
/// </summary>
public abstract class ClientShot
{
    public abstract BgrImage Crop(Rectangle regionInClient);
}

/// <summary>
/// ゲームのクライアント領域を撮る方式。環境によって、ゲームが UI (マップ名・座標欄) を描き終える前の画面を撮ってしまうことがあるため、
/// 撮り方を選べるようにしておく (AppConfig.CaptureMethod)。別のスレッドから同時に呼んでもよい。
/// </summary>
public abstract class GameCapture : IDisposable
{
    public const string GdiRegions = "gdi";
    public const string GdiClient = "gdi_client";
    public const string PrintWindow = "print_window";
    public const string DesktopDuplication = "dxgi";
    public const string WindowsGraphicsCapture = "wgc";

    /// <summary>設定画面の項目と同じ順序。</summary>
    public static readonly string[] Methods = [GdiRegions, GdiClient, PrintWindow, DesktopDuplication, WindowsGraphicsCapture];

    public static GameCapture Create(string method) => method switch
    {
        GdiClient => new GdiClientCapture(),
        PrintWindow => new PrintWindowCapture(),
        DesktopDuplication => new DesktopDuplicationCapture(),
        WindowsGraphicsCapture => new WindowsGraphicsCaptureCapture(),
        _ => new GdiRegionCapture(),
    };

    public static string NameOf(string method) => method switch
    {
        GdiClient => "GDI (画面全体)",
        PrintWindow => "PrintWindow",
        DesktopDuplication => "Desktop Duplication",
        WindowsGraphicsCapture => "Windows.Graphics.Capture",
        _ => "GDI (欄ごと)",
    };

    /// <param name="client">クライアント領域のスクリーン座標。</param>
    public abstract ClientShot Capture(nint hwnd, Rectangle client);

    public virtual void Dispose()
    {
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmFlush();

    /// <summary>DWM が次に画面を合成し終えるまで待つ (最大 1 フレーム)。</summary>
    public static void WaitForComposition() => _ = DwmFlush();

    /// <summary>画面全体を撮る方式の撮影。</summary>
    protected sealed class WholeShot(BgrImage whole) : ClientShot
    {
        public override BgrImage Crop(Rectangle regionInClient) => whole.Crop(regionInClient);
    }
}

/// <summary>従来の方式: 欄ごとに、必要になったときに GDI (BitBlt) で画面から撮る。撮る範囲が小さいので速い。</summary>
internal sealed class GdiRegionCapture : GameCapture
{
    public override ClientShot Capture(nint hwnd, Rectangle client) => new Shot(client.Location);

    private sealed class Shot(Point origin) : ClientShot
    {
        public override BgrImage Crop(Rectangle regionInClient) =>
            ScreenCapture.Capture(regionInClient with { X = origin.X + regionInClient.X, Y = origin.Y + regionInClient.Y });
    }
}

/// <summary>クライアント領域全体を GDI (BitBlt) で 1 回撮り、そこから欄を切り出す (GodiNavi と同じく 1 枚の画面から読む)。</summary>
internal sealed class GdiClientCapture : GameCapture
{
    public override ClientShot Capture(nint hwnd, Rectangle client) => new WholeShot(ScreenCapture.Capture(client));
}

/// <summary>PrintWindow (PW_RENDERFULLCONTENT) で、ゲームのウィンドウ自身にクライアント領域を描かせて撮る。DirectX のゲームでは黒くなることがある。</summary>
internal sealed class PrintWindowCapture : GameCapture
{
    private const uint PwClientOnly = 1;
    private const uint PwRenderFullContent = 2;

    [DllImport("user32.dll", EntryPoint = "PrintWindow")]
    private static extern bool PrintWindowNative(nint hwnd, nint hdc, uint flags);

    public override ClientShot Capture(nint hwnd, Rectangle client)
    {
        using var bitmap = new Bitmap(client.Width, client.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            var hdc = graphics.GetHdc();
            try
            {
                if (!PrintWindowNative(hwnd, hdc, PwClientOnly | PwRenderFullContent))
                {
                    throw new InvalidOperationException("PrintWindow に失敗しました");
                }
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }
        return new WholeShot(BgrImage.FromBitmap(bitmap));
    }
}
