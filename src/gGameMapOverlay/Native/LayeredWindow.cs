using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace gGameMapOverlay.Native;

/// <summary>透明度付きの画像をレイヤードウィンドウにそのまま渡す (UpdateLayeredWindow)。</summary>
internal static class LayeredWindow
{
    private const int ULW_ALPHA = 2;
    private const byte AC_SRC_OVER = 0;
    private const byte AC_SRC_ALPHA = 1;

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint(int x, int y)
    {
        public int X = x, Y = y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeSize(int cx, int cy)
    {
        public int Cx = cx, Cy = cy;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BlendFunction
    {
        public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitmapInfoHeader
    {
        public int Size, Width, Height;
        public short Planes, BitCount;
        public int Compression, SizeImage, XPelsPerMeter, YPelsPerMeter, ClrUsed, ClrImportant;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UpdateLayeredWindow(nint hwnd, nint hdcDst, ref NativePoint pptDst, ref NativeSize psize, nint hdcSrc, ref NativePoint pptSrc, int crKey, ref BlendFunction pblend, int dwFlags);

    [DllImport("user32.dll")]
    private static extern nint GetDC(nint hwnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(nint hwnd, nint hdc);

    [DllImport("gdi32.dll")]
    private static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern nint SelectObject(nint hdc, nint hgdiobj);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(nint hObject);

    [DllImport("gdi32.dll")]
    private static extern nint CreateDIBSection(nint hdc, ref BitmapInfoHeader pbmi, uint usage, out nint bits, nint section, uint offset);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(nint destination, nint source, nint length);

    // 渡すための DIB (32bpp、上から下、乗算済みアルファ)。大きさが変わるまで使い回す (UI のスレッドだけで使う)。
    private static nint dib, dibBits;
    private static Size dibSize;

    /// <summary>
    /// bitmap (Format32bppPArgb) を location に表示する。constantAlpha は全体にかける不透明度 (0〜255)。
    /// 失敗したら Win32 のエラー番号を返す (成功は 0)。
    /// </summary>
    public static int Update(nint hwnd, Bitmap bitmap, Point location, byte constantAlpha)
    {
        // Bitmap.GetHbitmap はアルファを捨てることがあるので、乗算済みの画素をそのまま DIB へ写す。
        if (dib == 0 || dibSize != bitmap.Size)
        {
            if (dib != 0)
            {
                DeleteObject(dib);
            }
            var header = new BitmapInfoHeader { Size = Marshal.SizeOf<BitmapInfoHeader>(), Width = bitmap.Width, Height = -bitmap.Height, Planes = 1, BitCount = 32 };
            dib = CreateDIBSection(0, ref header, 0, out dibBits, 0, 0);
            dibSize = bitmap.Size;
        }
        var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
        try
        {
            var rowBytes = bitmap.Width * 4;
            for (var y = 0; y < bitmap.Height; y++)
            {
                CopyMemory(dibBits + (nint)y * rowBytes, data.Scan0 + (nint)y * data.Stride, (nint)rowBytes);
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
        var screen = GetDC(0);
        var memory = CreateCompatibleDC(screen);
        var old = SelectObject(memory, dib);
        try
        {
            var destination = new NativePoint(location.X, location.Y);
            var size = new NativeSize(bitmap.Width, bitmap.Height);
            var source = new NativePoint(0, 0);
            var blend = new BlendFunction { BlendOp = AC_SRC_OVER, SourceConstantAlpha = constantAlpha, AlphaFormat = AC_SRC_ALPHA };
            return UpdateLayeredWindow(hwnd, screen, ref destination, ref size, memory, ref source, 0, ref blend, ULW_ALPHA) ? 0 : Marshal.GetLastWin32Error();
        }
        finally
        {
            SelectObject(memory, old);
            DeleteDC(memory);
            ReleaseDC(0, screen);
        }
    }
}
