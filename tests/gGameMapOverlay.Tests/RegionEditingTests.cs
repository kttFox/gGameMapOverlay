using System.Drawing;
using System.Windows.Forms;

namespace gGameMapOverlay.Tests;

public class RegionEditingTests
{
    private static readonly Rectangle Box = Rectangle.FromLTRB(100, 50, 300, 70);
    private static readonly Size Bounds = new(800, 600);

    [Theory]
    [InlineData(200, 60, Grip.Move)]
    [InlineData(100, 60, Grip.Left)]
    [InlineData(303, 60, Grip.Right)]
    [InlineData(200, 48, Grip.Top)]
    [InlineData(200, 72, Grip.Bottom)]
    [InlineData(99, 49, Grip.Left | Grip.Top)]
    [InlineData(301, 71, Grip.Right | Grip.Bottom)]
    [InlineData(301, 50, Grip.Right | Grip.Top)]
    [InlineData(100, 70, Grip.Left | Grip.Bottom)]
    [InlineData(400, 60, Grip.None)]
    [InlineData(200, 90, Grip.None)]
    public void HitTest_FindsEdgesCornersAndInside(int x, int y, object expected)
    {
        // Grip は internal なので、public なテストメソッドの引数には object で受ける。
        Assert.Equal((Grip)expected, RegionEditing.HitTest(Box, new Point(x, y), tolerance: 5));
    }

    [Fact]
    public void Apply_MoveKeepsSizeAndStaysInBounds()
    {
        Assert.Equal(new Rectangle(110, 45, 200, 20), RegionEditing.Apply(Box, Grip.Move, 10, -5, Bounds));
        Assert.Equal(new Rectangle(0, 0, 200, 20), RegionEditing.Apply(Box, Grip.Move, -500, -500, Bounds));
        Assert.Equal(new Rectangle(600, 580, 200, 20), RegionEditing.Apply(Box, Grip.Move, 900, 900, Bounds));
    }

    [Fact]
    public void Apply_ResizesOnlyGrabbedEdges()
    {
        Assert.Equal(Rectangle.FromLTRB(90, 50, 300, 70), RegionEditing.Apply(Box, Grip.Left, -10, 99, Bounds));
        Assert.Equal(Rectangle.FromLTRB(100, 50, 320, 75), RegionEditing.Apply(Box, Grip.Right | Grip.Bottom, 20, 5, Bounds));
        Assert.Equal(Rectangle.FromLTRB(100, 40, 300, 70), RegionEditing.Apply(Box, Grip.Top, 99, -10, Bounds));
    }

    [Fact]
    public void Apply_KeepsMinimumSizeAndBounds()
    {
        var shrunk = RegionEditing.Apply(Box, Grip.Right | Grip.Bottom, -1000, -1000, Bounds);
        Assert.Equal(new Rectangle(100, 50, RegionEditing.MinWidth, RegionEditing.MinHeight), shrunk);
        Assert.Equal(Rectangle.FromLTRB(0, 0, 300, 70), RegionEditing.Apply(Box, Grip.Left | Grip.Top, -1000, -1000, Bounds));
        Assert.Equal(Rectangle.FromLTRB(100, 50, 800, 600), RegionEditing.Apply(Box, Grip.Right | Grip.Bottom, 1000, 1000, Bounds));
    }

    [Fact]
    public void Clamp_ShrinksOversizedRegion()
    {
        Assert.Equal(new Rectangle(0, 0, 800, 600), RegionEditing.Clamp(new Rectangle(-50, -50, 2000, 2000), Bounds));
        Assert.Equal(new Rectangle(10, 10, RegionEditing.MinWidth, RegionEditing.MinHeight), RegionEditing.Clamp(new Rectangle(10, 10, 1, 1), Bounds));
    }
}

public class RegionSelectorFormTests
{
    /// <summary>実画面の上に初期値の四角形を表示できる (描画の例外や位置ずれがない) ことを確認する。</summary>
    [Fact]
    public void RendersRegionOverScreenshot()
    {
        Exception? error = null;
        Color border = Color.Empty;
        var accent = Color.FromArgb(255, 183, 77);
        var region = AppConfig.DefaultCoordinateRegion.ClipTo(int.MaxValue, int.MaxValue)!.Value;
        var thread = new Thread(() =>
        {
            try
            {
                using var frame = ScreenshotCase.Get("window_1056x660.png").LoadClient().ToBitmap();
                using var form = new RegionSelectorForm(frame, new Rectangle(-5000, -5000, frame.Width, frame.Height), "座標", accent, region, region);
                form.Show();
                Application.DoEvents();
                using var bitmap = new Bitmap(form.Width, form.Height);
                form.DrawToBitmap(bitmap, new Rectangle(0, 0, form.Width, form.Height));
                border = bitmap.GetPixel(region.Left + region.Width / 2, region.Top); // 上辺の中ほど (ハンドルの間)
                form.Close();
            }
            catch (Exception exception)
            {
                error = exception;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        Assert.Null(error);
        Assert.Equal(accent.ToArgb(), border.ToArgb());
    }
}

public class SettingsFormTests
{
    [Fact]
    public void DescribeRegion_ShowsPositionAndSize()
    {
        Assert.Equal("位置 (66, 3)　サイズ 283×20", SettingsForm.DescribeRegion(new ClientRegion(66, 3, 349, 23)));
        Assert.Equal("未設定", SettingsForm.DescribeRegion(null));
    }
}
