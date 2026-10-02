using System.Drawing;
using System.Runtime.InteropServices;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

namespace gGameMapOverlay.Imaging;

/// <summary>
/// Windows.Graphics.Capture で、ゲームのウィンドウ (DWM が合成し終えたもの) を受け取り、クライアント領域を切り出す。
/// 他のウィンドウ (このツールのオーバーレイも) が重なっていても写らない。Windows 10 では撮影中のウィンドウに黄色い枠が出る。
/// </summary>
internal sealed class WindowsGraphicsCaptureCapture : D3DCapture
{
    private static readonly Guid GraphicsCaptureItemId = new("79C3F95B-31F7-4EC2-A464-632EF5D30760");

    [ComImport]
    [Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IGraphicsCaptureItemInterop
    {
        nint CreateForWindow(nint window, in Guid iid);

        nint CreateForMonitor(nint monitor, in Guid iid);
    }

    [ComImport]
    [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IDirect3DDxgiInterfaceAccess
    {
        nint GetInterface(in Guid iid);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left, Top, Right, Bottom;
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(nint hwnd, uint attribute, out Rect value, int size);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(nint hwnd, out Rect rect);

    private const uint DwmwaExtendedFrameBounds = 9;

    // Vortice の同名のメソッドは WinRT のオブジェクトに変換できないので、d3d11.dll を直接呼ぶ。
    [DllImport("d3d11.dll")]
    private static extern int CreateDirect3D11DeviceFromDXGIDevice(nint dxgiDevice, out nint graphicsDevice);

    private nint window;
    private IDirect3DDevice? winrtDevice;
    private GraphicsCaptureItem? item;
    private Direct3D11CaptureFramePool? pool;
    private GraphicsCaptureSession? session;
    private Windows.Graphics.SizeInt32 poolSize;
    private volatile bool closed;

    public override ClientShot Capture(nint hwnd, Rectangle client)
    {
        lock (gate)
        {
            if (session is null || hwnd != window || closed)
            {
                Reset();
                Open(hwnd);
            }
            // まだ画面を受け取っていなければ来るまで少し待つ (始めた直後はすぐ来る)。
            var deadline = Environment.TickCount64 + 500;
            while (!TakeFrames() && latest is null && Environment.TickCount64 < deadline)
            {
                Thread.Sleep(5);
            }
            if (latest is null)
            {
                throw new InvalidOperationException("Windows.Graphics.Capture から画面を受け取れませんでした");
            }
            // 受け取る画面はウィンドウの見えている範囲 (影などの見えない枠を除いた範囲)。
            var frame = DwmGetWindowAttribute(hwnd, DwmwaExtendedFrameBounds, out var bounds, Marshal.SizeOf<Rect>()) == 0 || GetWindowRect(hwnd, out bounds)
                ? bounds
                : default;
            return new WholeShot(ReadLatest(client with { X = client.X - frame.Left, Y = client.Y - frame.Top }));
        }
    }

    private void Open(nint hwnd)
    {
        if (!GraphicsCaptureSession.IsSupported())
        {
            throw new InvalidOperationException("この Windows では Windows.Graphics.Capture を使えません");
        }
        (device, context) = CreateDevice(null);
        using (var dxgiDevice = device.QueryInterface<IDXGIDevice>())
        {
            Marshal.ThrowExceptionForHR(CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.NativePointer, out var inspectable));
            try
            {
                winrtDevice = WinRT.MarshalInterface<IDirect3DDevice>.FromAbi(inspectable);
            }
            finally
            {
                Marshal.Release(inspectable);
            }
        }

        var factory = WinRT.ActivationFactory.Get("Windows.Graphics.Capture.GraphicsCaptureItem");
        var interop = (IGraphicsCaptureItemInterop)Marshal.GetObjectForIUnknown(factory.ThisPtr);
        var itemPointer = interop.CreateForWindow(hwnd, GraphicsCaptureItemId);
        try
        {
            item = GraphicsCaptureItem.FromAbi(itemPointer);
        }
        finally
        {
            Marshal.Release(itemPointer);
        }
        closed = false;
        item.Closed += (_, _) => closed = true;
        poolSize = item.Size;
        pool = Direct3D11CaptureFramePool.CreateFreeThreaded(winrtDevice, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, poolSize);
        session = pool.CreateCaptureSession(item);
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
        {
            session.IsCursorCaptureEnabled = false;
        }
        session.StartCapture();
        window = hwnd;
    }

    /// <summary>届いている画面を受け取り、最後のものを latest に写す。受け取れたら true。</summary>
    private bool TakeFrames()
    {
        var taken = false;
        while (pool!.TryGetNextFrame() is { } frame)
        {
            using (frame)
            {
                var surface = WinRT.MarshalInterface<IDirect3DSurface>.FromManaged(frame.Surface);
                nint texturePointer;
                try
                {
                    var access = (IDirect3DDxgiInterfaceAccess)Marshal.GetObjectForIUnknown(surface);
                    texturePointer = access.GetInterface(typeof(ID3D11Texture2D).GUID);
                    Marshal.ReleaseComObject(access);
                }
                finally
                {
                    Marshal.Release(surface);
                }
                using var texture = new ID3D11Texture2D(texturePointer);
                StoreLatest(texture);
                taken = true;
                if (frame.ContentSize.Width != poolSize.Width || frame.ContentSize.Height != poolSize.Height)
                {
                    // ウィンドウの大きさが変わった: 次からその大きさで受け取る。
                    poolSize = frame.ContentSize;
                    pool.Recreate(winrtDevice, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, poolSize);
                }
            }
        }
        return taken;
    }

    protected override void Reset()
    {
        session?.Dispose();
        session = null;
        pool?.Dispose();
        pool = null;
        item = null;
        winrtDevice?.Dispose();
        winrtDevice = null;
        window = 0;
        base.Reset();
    }
}
