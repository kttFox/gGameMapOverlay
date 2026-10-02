using gGameMapOverlay.Imaging;

namespace gGameMapOverlay.Tests;

public class UiLocatorTests
{
    /// <summary>実画面から UI の拡大率を推定できる。等倍の画面は Identity (初期値の領域をそのまま使う)。</summary>
    [Theory]
    [MemberData(nameof(ScreenshotCase.AllFiles), MemberType = typeof(ScreenshotCase))]
    public void Locate_FindsUiScale(string file)
    {
        var item = ScreenshotCase.Get(file);
        var transform = UiLocator.Locate(item.LoadClient());
        Assert.NotNull(transform);
        if (item.Scale == 1.0)
        {
            Assert.True(transform.Value.IsIdentity, $"{file}: {transform}");
        }
        else
        {
            Assert.Equal(item.Scale, transform.Value.ScaleX, 0.02);
            Assert.Equal(item.ScaleY, transform.Value.ScaleY, 0.02);
        }
    }

    /// <summary>画像から測った倍率が、画面の高さからの計算 (ライブ読み取りで使う) と一致する。</summary>
    [Fact]
    public void Locate_AgreesWithScaleFromScreenHeight()
    {
        var measured = UiLocator.Locate(ScreenshotCase.Get("window_win11_compat_1912x910.png").LoadClient())!.Value;
        Assert.Equal(UiTransform.ForScreen(1280).ScaleX, measured.ScaleX, 0.01);
    }

    [Fact]
    public void Locate_ReturnsNullWithoutUi()
    {
        Assert.Null(UiLocator.Locate(new BgrImage(800, 600))); // 真っ黒 (読み込み中など)
        var noise = new BgrImage(800, 600);
        new Random(1).NextBytes(noise.Pixels);
        Assert.Null(UiLocator.Locate(noise));
    }

    [Fact]
    public void Locate_IsFastEnoughForOccasionalUse()
    {
        var frame = ScreenshotCase.Get("window_win11_compat_1912x910.png").LoadClient();
        UiLocator.Locate(frame); // JIT の分を除く
        var watch = System.Diagnostics.Stopwatch.StartNew();
        UiLocator.Locate(frame);
        Assert.True(watch.ElapsedMilliseconds < 500, $"{watch.ElapsedMilliseconds}ms");
    }
}
