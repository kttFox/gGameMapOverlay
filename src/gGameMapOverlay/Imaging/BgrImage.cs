using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace gGameMapOverlay.Imaging;

/// <summary>パディングなしの 24bit BGR 画像。PaddleOCR の入力と同じチャネル順。</summary>
public sealed class BgrImage
{
    public BgrImage(int width, int height, byte[]? pixels = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        Width = width;
        Height = height;
        Pixels = pixels ?? new byte[width * height * 3];
        if (Pixels.Length != width * height * 3)
        {
            throw new ArgumentException("pixel buffer size mismatch", nameof(pixels));
        }
    }

    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }

    public static BgrImage FromBitmap(Bitmap bitmap)
    {
        var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try
        {
            var image = new BgrImage(bitmap.Width, bitmap.Height);
            var rowBytes = bitmap.Width * 3;
            for (var y = 0; y < bitmap.Height; y++)
            {
                Marshal.Copy(data.Scan0 + y * data.Stride, image.Pixels, y * rowBytes, rowBytes);
            }
            return image;
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }

    public static BgrImage Load(string path)
    {
        using var bitmap = new Bitmap(path);
        return FromBitmap(bitmap);
    }

    public Bitmap ToBitmap()
    {
        var bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
        var data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        try
        {
            var rowBytes = Width * 3;
            for (var y = 0; y < Height; y++)
            {
                Marshal.Copy(Pixels, y * rowBytes, data.Scan0 + y * data.Stride, rowBytes);
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
        return bitmap;
    }

    public BgrImage Crop(Rectangle rect)
    {
        rect.Intersect(new Rectangle(0, 0, Width, Height));
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            throw new ArgumentException("crop rectangle is outside the image", nameof(rect));
        }
        var cropped = new BgrImage(rect.Width, rect.Height);
        var rowBytes = rect.Width * 3;
        for (var y = 0; y < rect.Height; y++)
        {
            Buffer.BlockCopy(Pixels, ((rect.Top + y) * Width + rect.Left) * 3, cropped.Pixels, y * rowBytes, rowBytes);
        }
        return cropped;
    }

    /// <summary>OpenCV の INTER_LINEAR 相当 (ピクセル中心基準) のバイリニア補間。</summary>
    public BgrImage ResizeBilinear(int width, int height)
    {
        var resized = new BgrImage(width, height);
        var scaleX = Width / (double)width;
        var scaleY = Height / (double)height;
        for (var y = 0; y < height; y++)
        {
            var sy = Math.Clamp((y + 0.5) * scaleY - 0.5, 0, Height - 1);
            var y0 = (int)sy;
            var y1 = Math.Min(y0 + 1, Height - 1);
            var fy = sy - y0;
            for (var x = 0; x < width; x++)
            {
                var sx = Math.Clamp((x + 0.5) * scaleX - 0.5, 0, Width - 1);
                var x0 = (int)sx;
                var x1 = Math.Min(x0 + 1, Width - 1);
                var fx = sx - x0;
                for (var c = 0; c < 3; c++)
                {
                    var top = Pixels[(y0 * Width + x0) * 3 + c] * (1 - fx) + Pixels[(y0 * Width + x1) * 3 + c] * fx;
                    var bottom = Pixels[(y1 * Width + x0) * 3 + c] * (1 - fx) + Pixels[(y1 * Width + x1) * 3 + c] * fx;
                    resized.Pixels[(y * width + x) * 3 + c] = (byte)Math.Round(top * (1 - fy) + bottom * fy);
                }
            }
        }
        return resized;
    }

    public byte Gray(int x, int y)
    {
        var i = (y * Width + x) * 3;
        // ITU-R BT.601 (Pillow の convert("L") と同じ係数)
        return (byte)((Pixels[i + 2] * 299 + Pixels[i + 1] * 587 + Pixels[i] * 114 + 500) / 1000);
    }
}
