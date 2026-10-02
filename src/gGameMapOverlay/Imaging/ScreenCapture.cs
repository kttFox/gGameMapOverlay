using System.Drawing;
using System.Drawing.Imaging;

namespace gGameMapOverlay.Imaging;

public static class ScreenCapture
{
    /// <summary>スクリーン座標の矩形を GDI (BitBlt) でキャプチャする。</summary>
    public static BgrImage Capture(Rectangle screenRect)
    {
        using var bitmap = new Bitmap(screenRect.Width, screenRect.Height, PixelFormat.Format24bppRgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(screenRect.Location, System.Drawing.Point.Empty, screenRect.Size, CopyPixelOperation.SourceCopy);
        }
        return BgrImage.FromBitmap(bitmap);
    }
}
