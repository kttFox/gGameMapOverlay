using System.Drawing;
using gGameMapOverlay.Imaging;

namespace gGameMapOverlay.Tests;

public class WindowFrameTests
{
    /// <summary>Alt+PrintScreen で撮った実画面と、最大化 (タイトルバーなし) のスクリーンショットで、クライアント領域を正しく推定できる。</summary>
    [Theory]
    [MemberData(nameof(ScreenshotCase.AllFiles), MemberType = typeof(ScreenshotCase))]
    public void DetectClientArea_MatchesManifest(string file)
    {
        var item = ScreenshotCase.Get(file);
        var image = BgrImage.Load(Path.Combine(ScreenshotCase.Directory, item.File));
        var expected = Rectangle.FromLTRB(item.ClientRect[0], item.ClientRect[1], item.ClientRect[2], item.ClientRect[3]);
        Assert.Equal(expected, WindowFrame.DetectClientArea(image));
    }

    /// <summary>ゲーム画面の代わりに、ランダムな模様で埋めたクライアント領域を持つウィンドウ画像を作る。</summary>
    private static BgrImage Window(int width, int height, Color title, Color? border, int titleHeight = 31)
    {
        using var bitmap = new Bitmap(width, height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            var random = new Random(1);
            for (var y = 0; y < height; y += 4)
            {
                for (var x = 0; x < width; x += 4)
                {
                    using var brush = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                    graphics.FillRectangle(brush, x, y, 4, 4);
                }
            }
            using var titleBrush = new SolidBrush(title);
            graphics.FillRectangle(titleBrush, 0, 0, width, titleHeight);
            graphics.DrawString("gGame Client", new Font("Segoe UI", 9), Brushes.Black, 30, 8); // タイトル文字
            graphics.FillRectangle(Brushes.Gray, width - 40, 10, 12, 12); // 閉じるボタンの代わり
            if (border is { } color)
            {
                using var pen = new Pen(color);
                graphics.DrawLine(pen, 0, 0, 0, height - 1);
                graphics.DrawLine(pen, width - 1, 0, width - 1, height - 1);
                graphics.DrawLine(pen, 0, height - 1, width - 1, height - 1);
            }
        }
        return BgrImage.FromBitmap(bitmap);
    }

    [Fact]
    public void DetectClientArea_InactiveWhiteTitleBar()
    {
        var image = Window(640, 480, Color.White, Color.FromArgb(170, 170, 170));
        Assert.Equal(Rectangle.FromLTRB(1, 31, 639, 479), WindowFrame.DetectClientArea(image));
    }

    [Fact]
    public void DetectClientArea_MaximizedWindowOfOtherAppsHasNoSideBorders()
    {
        var image = Window(640, 480, Color.FromArgb(32, 32, 32), border: null, titleHeight: 23);
        Assert.Equal(Rectangle.FromLTRB(0, 23, 640, 480), WindowFrame.DetectClientArea(image));
    }

    [Fact]
    public void DetectClientArea_ReturnsWholeImageWithoutFrame()
    {
        var image = ScreenshotCase.Get("maximized_1920x1080.png").LoadClient(); // gGame の最大化はタイトルバーなし
        Assert.Equal(new Rectangle(0, 0, image.Width, image.Height), WindowFrame.DetectClientArea(image));

        var client = ScreenshotCase.Get("window_1056x660.png").LoadClient(); // 枠を除いた後の画像
        Assert.Equal(new Rectangle(0, 0, client.Width, client.Height), WindowFrame.DetectClientArea(client));
    }

    [Fact]
    public void DetectClientArea_IgnoresUniformImage()
    {
        // 真っ黒な読み込み画面などは、上端が単色でもタイトルバーとはみなさない
        var image = new BgrImage(640, 480);
        Assert.Equal(new Rectangle(0, 0, 640, 480), WindowFrame.DetectClientArea(image));
    }
}
