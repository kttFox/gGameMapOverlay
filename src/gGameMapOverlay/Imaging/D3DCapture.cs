using System.Drawing;
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace gGameMapOverlay.Imaging;

/// <summary>
/// Direct3D 11 のテクスチャ (BGRA) に受け取った画面を、CPU で読める BgrImage にする部分 (Desktop Duplication と Windows.Graphics.Capture で共通)。
/// 受け取った画面は latest に写しておく (どちらの方式も、画面が変わったときだけ新しい画面が来るため)。
/// </summary>
internal abstract class D3DCapture : GameCapture
{
    protected readonly object gate = new();
    protected ID3D11Device? device;
    protected ID3D11DeviceContext? context;

    // 最後に受け取った画面 (GPU 上)。
    protected ID3D11Texture2D? latest;
    private ID3D11Texture2D? staging;

    protected static (ID3D11Device Device, ID3D11DeviceContext Context) CreateDevice(IDXGIAdapter? adapter)
    {
        D3D11.D3D11CreateDevice(adapter, adapter is null ? DriverType.Hardware : DriverType.Unknown, DeviceCreationFlags.BgraSupport,
            [FeatureLevel.Level_11_0, FeatureLevel.Level_10_1, FeatureLevel.Level_10_0], out ID3D11Device device, out ID3D11DeviceContext context).CheckError();
        return (device, context);
    }

    /// <summary>source の内容を latest に写す (大きさが変わったら作り直す)。</summary>
    protected void StoreLatest(ID3D11Texture2D source)
    {
        var description = source.Description;
        if (latest is null || latest.Description.Width != description.Width || latest.Description.Height != description.Height)
        {
            latest?.Dispose();
            latest = device!.CreateTexture2D(new Texture2DDescription(description.Format, description.Width, description.Height, 1, 1,
                BindFlags.None, ResourceUsage.Default, CpuAccessFlags.None, 1, 0, ResourceOptionFlags.None));
        }
        context!.CopyResource(latest, source);
    }

    /// <summary>latest の rect (latest の中の座標) を切り出す。latest からはみ出す部分は黒にする。</summary>
    protected BgrImage ReadLatest(Rectangle rect)
    {
        var image = new BgrImage(rect.Width, rect.Height);
        var source = Rectangle.Intersect(rect, new Rectangle(0, 0, (int)latest!.Description.Width, (int)latest.Description.Height));
        if (source.Width <= 0 || source.Height <= 0)
        {
            return image;
        }
        if (staging is null || staging.Description.Width < source.Width || staging.Description.Height < source.Height)
        {
            staging?.Dispose();
            staging = device!.CreateTexture2D(new Texture2DDescription(latest.Description.Format,
                (uint)Math.Max(source.Width, staging?.Description.Width ?? 0), (uint)Math.Max(source.Height, staging?.Description.Height ?? 0), 1, 1,
                BindFlags.None, ResourceUsage.Staging, CpuAccessFlags.Read, 1, 0, ResourceOptionFlags.None));
        }
        context!.CopySubresourceRegion(staging, 0, 0, 0, 0, latest, 0, new Box(source.Left, source.Top, 0, source.Right, source.Bottom, 1));
        var mapped = context.Map(staging, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None);
        try
        {
            var row = new byte[source.Width * 4];
            for (var y = 0; y < source.Height; y++)
            {
                Marshal.Copy(mapped.DataPointer + y * (nint)mapped.RowPitch, row, 0, row.Length);
                var offset = ((source.Top - rect.Top + y) * rect.Width + source.Left - rect.Left) * 3;
                for (var x = 0; x < source.Width; x++)
                {
                    image.Pixels[offset + x * 3] = row[x * 4];
                    image.Pixels[offset + x * 3 + 1] = row[x * 4 + 1];
                    image.Pixels[offset + x * 3 + 2] = row[x * 4 + 2];
                }
            }
        }
        finally
        {
            context.Unmap(staging, 0);
        }
        return image;
    }

    /// <summary>デバイスとテクスチャを捨てる (次の撮影で作り直す)。</summary>
    protected virtual void Reset()
    {
        staging?.Dispose();
        staging = null;
        latest?.Dispose();
        latest = null;
        context?.Dispose();
        context = null;
        device?.Dispose();
        device = null;
    }

    public override void Dispose()
    {
        lock (gate)
        {
            Reset();
        }
    }
}

/// <summary>
/// Desktop Duplication API で、ゲームが表示されているモニターの画面 (DWM が合成し終えたもの) を受け取り、クライアント領域を切り出す。
/// GodiNavi (dxcam) と同じ方式。
/// </summary>
internal sealed class DesktopDuplicationCapture : D3DCapture
{
    private IDXGIOutputDuplication? duplication;
    private nint monitor;
    private Rectangle outputBounds;

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint hwnd, uint flags);

    private const uint MonitorDefaultToNearest = 2;

    public override ClientShot Capture(nint hwnd, Rectangle client)
    {
        lock (gate)
        {
            var target = MonitorFromWindow(hwnd, MonitorDefaultToNearest);
            if (duplication is null || target != monitor)
            {
                Reset();
                Open(target);
            }
            try
            {
                // まだ画面を受け取っていなければ来るまで少し待つ (始めた直後は今の画面がすぐ来る)。
                Acquire(latest is null ? 500u : 0u);
            }
            catch (SharpGen.Runtime.SharpGenException exception) when (exception.ResultCode == Vortice.DXGI.ResultCode.AccessLost)
            {
                // 解像度の変更や UAC の画面などで切れた: 開き直す。
                Reset();
                Open(target);
                Acquire(500);
            }
            if (latest is null)
            {
                throw new InvalidOperationException("Desktop Duplication から画面を受け取れませんでした");
            }
            return new WholeShot(ReadLatest(client with { X = client.X - outputBounds.X, Y = client.Y - outputBounds.Y }));
        }
    }

    private void Open(nint target)
    {
        using var factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
        for (var a = 0u; factory.EnumAdapters1(a, out var adapter).Success; a++)
        {
            using (adapter)
            {
                for (var o = 0u; adapter.EnumOutputs(o, out var output).Success; o++)
                {
                    using (output)
                    {
                        var description = output.Description;
                        if (description.Monitor != target)
                        {
                            continue;
                        }
                        (device, context) = CreateDevice(adapter);
                        using var output1 = output.QueryInterface<IDXGIOutput1>();
                        duplication = output1.DuplicateOutput(device);
                        monitor = target;
                        var bounds = description.DesktopCoordinates;
                        outputBounds = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
                        return;
                    }
                }
            }
        }
        throw new InvalidOperationException("ゲームが表示されているモニターが見つかりません");
    }

    private void Acquire(uint timeoutMs)
    {
        var result = duplication!.AcquireNextFrame(timeoutMs, out _, out var resource);
        if (result.Code == Vortice.DXGI.ResultCode.WaitTimeout.Code)
        {
            return; // 画面が変わっていない: 前に受け取った画面のまま
        }
        result.CheckError();
        try
        {
            using var texture = resource!.QueryInterface<ID3D11Texture2D>();
            StoreLatest(texture);
        }
        finally
        {
            resource?.Dispose();
            duplication.ReleaseFrame();
        }
    }

    protected override void Reset()
    {
        duplication?.Dispose();
        duplication = null;
        monitor = 0;
        base.Reset();
    }
}
