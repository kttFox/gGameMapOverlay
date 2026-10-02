using System.Drawing;
using System.Drawing.Text;
using gGameMapOverlay.Imaging;
using gGameMapOverlay.Ocr;
using gGameMapOverlay.Parsing;

namespace gGameMapOverlay.Tests;

public class ReadingTrackerTests
{
    private double now;

    private ReadingTracker Tracker(int hits = 2) => new(hits, holdSeconds: 3.0, clock: () => now);

    [Fact]
    public void Name_IsConfirmedAfterConsecutiveHits()
    {
        var tracker = Tracker();
        Assert.False(tracker.UpdateName("a", "町A"));
        Assert.Null(tracker.MapName);
        Assert.True(tracker.UpdateName("a", "町A"));
        Assert.Equal("町A", tracker.MapName);
    }

    [Fact]
    public void Name_SingleMisreadDoesNotReplaceConfirmedName()
    {
        var tracker = Tracker();
        tracker.UpdateName("a", "町A");
        tracker.UpdateName("a", "町A");
        tracker.UpdateName("b", "誤読");
        tracker.UpdateName(null, null);
        tracker.UpdateName("a", "町A");
        Assert.Equal("町A", tracker.MapName);
    }

    [Fact]
    public void Name_BecomesStaleAfterHoldTime()
    {
        var tracker = Tracker(hits: 1);
        tracker.UpdateName("a", "町A");
        now = 2.0;
        tracker.UpdateName(null, null);
        Assert.False(tracker.NameStale);
        now = 3.5;
        Assert.True(tracker.NameStale);
        Assert.Equal("町A", tracker.MapName); // 値は保持する
    }

    [Fact]
    public void Coordinates_KeepLastValueOnFailure()
    {
        var tracker = Tracker();
        Assert.True(tracker.UpdateCoordinates(new GameCoordinate(1, 2)));
        Assert.False(tracker.UpdateCoordinates(null));
        Assert.Equal(new GameCoordinate(1, 2), tracker.Coordinates);
    }

    [Fact]
    public void Coordinates_ChangeIsAdoptedAfterConfirmHits()
    {
        var tracker = Tracker();
        tracker.CoordinateConfirmHits = 2;
        Assert.True(tracker.UpdateCoordinates(new GameCoordinate(1, 2))); // 最初の座標はすぐ採用する
        Assert.False(tracker.UpdateCoordinates(new GameCoordinate(1, 3)));
        Assert.Equal(new GameCoordinate(1, 2), tracker.Coordinates);
        Assert.Equal(new GameCoordinate(1, 3), tracker.PendingCoordinates);
        Assert.True(tracker.UpdateCoordinates(new GameCoordinate(1, 3)));
        Assert.Equal(new GameCoordinate(1, 3), tracker.Coordinates);
        Assert.Null(tracker.PendingCoordinates);
    }

    [Fact]
    public void Coordinates_SingleMisreadIsIgnoredWithConfirmHits()
    {
        var tracker = Tracker();
        tracker.CoordinateConfirmHits = 2;
        tracker.UpdateCoordinates(new GameCoordinate(1, 2));
        Assert.False(tracker.UpdateCoordinates(new GameCoordinate(9, 9))); // 一瞬の誤読
        Assert.False(tracker.UpdateCoordinates(new GameCoordinate(1, 2)));
        Assert.Null(tracker.PendingCoordinates);
        Assert.False(tracker.UpdateCoordinates(new GameCoordinate(9, 9))); // 続かなければ採用しない
        Assert.Equal(new GameCoordinate(1, 2), tracker.Coordinates);
    }
}

public class ClientRegionTests
{
    [Fact]
    public void NewConfig_HasDefaultRegions()
    {
        var config = new AppConfig();
        Assert.Equal(AppConfig.DefaultNameRegion, config.NameRegion);
        Assert.Equal(AppConfig.DefaultCoordinateRegion, config.CoordinateRegion);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("""{"name_region": null, "coordinate_region": null}""")]
    [InlineData("not json")]
    public void Load_FillsMissingRegionsWithDefaults(string json)
    {
        var path = Path.Combine(Directory.CreateTempSubdirectory().FullName, "config.json");
        File.WriteAllText(path, json);
        var config = AppConfig.Load(path);
        Assert.Equal(AppConfig.DefaultNameRegion, config.NameRegion);
        Assert.Equal(AppConfig.DefaultCoordinateRegion, config.CoordinateRegion);
        File.Delete(path);
    }

    [Fact]
    public void DefaultPath_IsNextToExecutable()
    {
        Assert.Equal(Path.Combine(AppContext.BaseDirectory, "config.json"), AppConfig.DefaultPath);
    }

    [Fact]
    public void Load_KeepsUserRegions()
    {
        var path = Path.Combine(Directory.CreateTempSubdirectory().FullName, "config.json");
        File.WriteAllText(path, """{"coordinate_region": {"left": 1, "top": 2, "right": 30, "bottom": 40}}""");
        var config = AppConfig.Load(path);
        Assert.Equal(new ClientRegion(1, 2, 30, 40), config.CoordinateRegion);
        Assert.Equal(AppConfig.DefaultNameRegion, config.NameRegion);
        File.Delete(path);
    }

    [Fact]
    public void SetRegion_StoresPixelRegion()
    {
        var config = new AppConfig();
        config.SetRegion(RegionKind.Coordinates, Rectangle.FromLTRB(1, 2, 30, 40));
        Assert.Equal(new ClientRegion(1, 2, 30, 40), config.GetRegion(RegionKind.Coordinates));
        Assert.Equal(AppConfig.DefaultNameRegion, config.GetRegion(RegionKind.Name)); // もう一方は初期値のまま
    }

    [Fact]
    public void Clone_IsIndependentOfOriginal()
    {
        var config = new AppConfig { OcrLanguage = "ko", NameRegion = new ClientRegion(1, 2, 3, 4) };
        var clone = config.Clone();
        clone.SetRegion(RegionKind.Name, Rectangle.FromLTRB(10, 10, 20, 20));
        Assert.Equal("ko", clone.OcrLanguage);
        Assert.Equal(new ClientRegion(1, 2, 3, 4), config.NameRegion);
    }

    [Fact]
    public void ClipTo_KeepsPixelPositionWhenClientResizes()
    {
        var region = new ClientRegion(100, 10, 300, 40);
        Assert.Equal(Rectangle.FromLTRB(100, 10, 300, 40), region.ClipTo(800, 600));
        Assert.Equal(Rectangle.FromLTRB(100, 10, 300, 40), region.ClipTo(1920, 1080));
    }

    [Fact]
    public void ClipTo_AppliesUiScale()
    {
        var region = new ClientRegion(66, 27, 139, 47);
        Assert.Equal(Rectangle.FromLTRB(99, 40, 209, 71), region.ClipTo(1920, 1080, new UiTransform(1.5))); // 端数は外側へ
    }

    [Fact]
    public void ClipTo_AppliesUiOffset()
    {
        var region = new ClientRegion(66, 27, 139, 47);
        Assert.Equal(Rectangle.FromLTRB(68, 26, 141, 46), region.ClipTo(1920, 1080, new UiTransform(1.0, 1.0, 2, -1)));
    }

    [Fact]
    public void ClipTo_AppliesSeparateHorizontalAndVerticalScale()
    {
        var region = new ClientRegion(66, 27, 139, 47);
        Assert.Equal(Rectangle.FromLTRB(78, 32, 165, 58), region.ClipTo(1920, 1080, new UiTransform(1.186, 1.218)));
    }

    [Fact]
    public void SetRegion_StoresRegionDividedByUiScale()
    {
        var config = new AppConfig();
        config.SetRegion(RegionKind.Name, Rectangle.FromLTRB(99, 4, 524, 35), new UiTransform(1.5));
        Assert.Equal(new ClientRegion(66, 3, 349, 23), config.NameRegion);
        Assert.Equal(Rectangle.FromLTRB(99, 4, 524, 35), config.NameRegion!.ClipTo(1920, 1080, new UiTransform(1.5))); // 往復で元に戻る
    }

    [Theory]
    [InlineData(1080, 1.0)]   // 1920x1080 (実機で確認)
    [InlineData(768, 1.0)]    // 1366x768: 縮小はされない (実機で確認)
    [InlineData(1280, 1.185)] // 1920x1280: 実機で 1.18 倍
    [InlineData(1200, 1.111)] // 1600x1200: 実機で 1.114 倍
    [InlineData(2160, 2.0)]
    public void ForScreen_ScalesAboveHeight1080(int screenHeight, double expected)
    {
        Assert.Equal(expected, UiTransform.ForScreen(screenHeight).ScaleX);
        Assert.Equal(expected, UiTransform.ForScreen(screenHeight).ScaleY);
    }

    [Fact]
    public void CurrentTransform_UsesFixedScaleFromConfig()
    {
        using var reader = new MapReader(new AppConfig { UiScale = 1.25 }, new FakeOcrForScale());
        Assert.Equal(new UiTransform(1.25), reader.CurrentTransform());
    }

    private sealed class FakeOcrForScale : IOcrEngine
    {
        public string Name => "fake";
        public void WarmUp(string language) { }
        public OcrResult Recognize(BgrImage image, string language) => OcrResult.Empty;
        public void Dispose() { }
    }

    [Fact]
    public void ClipTo_ReturnsNullWhenOutsideClient()
    {
        Assert.Null(new ClientRegion(900, 10, 1000, 40).ClipTo(800, 600));
    }

    [Fact]
    public void Load_ReadsLegacyRatioRegionAsPixels()
    {
        var path = Path.Combine(Directory.CreateTempSubdirectory().FullName, "config.json");
        File.WriteAllText(path, """
            {"name_region": {"left": 66, "top": 3, "right": 349, "bottom": 23, "reference_width": 1056, "reference_height": 660}}
            """);
        Assert.Equal(new ClientRegion(66, 3, 349, 23), AppConfig.Load(path).NameRegion);
        File.Delete(path);
    }

    [Fact]
    public void ClipTo_ClampsToClient()
    {
        var region = new ClientRegion(700, 580, 900, 700);
        Assert.Equal(Rectangle.FromLTRB(700, 580, 800, 600), region.ClipTo(800, 600));
    }
}

public class MapReaderTests
{
    /// <summary>領域の左上ピクセルの色で「どの領域か」を判定して決まった文字を返す偽 OCR。</summary>
    private sealed class FakeOcr(Func<string, string> nameText, string coordinateText) : IOcrEngine
    {
        public int Calls { get; private set; }
        public string Name => "fake";
        public void WarmUp(string language) { }
        public void Dispose() { }

        public OcrResult Recognize(BgrImage image, string language)
        {
            Calls++;
            return language == "en" && image.Pixels[0] == 200 ? new(coordinateText, 0.99) : new(nameText(language), 0.95);
        }
    }

    private static BgrImage Frame()
    {
        var frame = new BgrImage(800, 600);
        for (var y = 0; y < 600; y++)
        {
            for (var x = 0; x < 800; x++)
            {
                var value = y >= 560 ? (byte)200 : (byte)100;
                frame.Pixels[(y * 800 + x) * 3] = value;
                frame.Pixels[(y * 800 + x) * 3 + 1] = (byte)(x % 50); // 真っ黒判定を避ける
            }
        }
        return frame;
    }

    private static AppConfig Config() => new()
    {
        NameRegion = new ClientRegion(300, 10, 500, 36),
        CoordinateRegion = new ClientRegion(330, 560, 470, 584),
    };

    [Fact]
    public void ReadFrame_ConfirmsNameAndParsesCoordinates()
    {
        using var reader = new MapReader(Config(), new FakeOcr(_ => "レゲ地下洞窟 ９Ｆ", "123, 456"));
        var first = reader.ReadFrame(Frame(), force: true);
        Assert.True(first.Ok);
        Assert.Null(first.MapName); // 1回目は確認中
        Assert.Equal(new GameCoordinate(123, 456), first.Coordinates);

        var second = reader.ReadFrame(Frame(), force: true);
        Assert.Equal("レゲ地下洞窟 9F", second.MapName);
    }

    [Fact]
    public void ReadFrame_SkipsOcrForUnchangedCoordinates()
    {
        var ocr = new FakeOcr(_ => "町", "1, 2");
        var config = Config();
        config.NameRefreshSeconds = 3600;
        using var reader = new MapReader(config, ocr);
        reader.ReadFrame(Frame());
        reader.ReadFrame(Frame()); // マップ名確定 (名前は未確定の間は毎回読む)
        var calls = ocr.Calls;
        reader.ReadFrame(Frame());
        Assert.Equal(calls, ocr.Calls);
    }

    [Fact]
    public void Dispose_KeepsSharedOcrEngine()
    {
        var ocr = new CountingDisposeOcr();
        new MapReader(Config(), ocr, ownsOcr: false).Dispose();
        Assert.Equal(0, ocr.Disposed);
        new MapReader(Config(), ocr).Dispose();
        Assert.Equal(1, ocr.Disposed);
    }

    private sealed class CountingDisposeOcr : IOcrEngine
    {
        public int Disposed { get; private set; }
        public string Name => "counting";
        public void WarmUp(string language) { }
        public OcrResult Recognize(BgrImage image, string language) => OcrResult.Empty;
        public void Dispose() => Disposed++;
    }

    [Fact]
    public void ReadFrame_ReportsMissingRegions()
    {
        using var reader = new MapReader(new AppConfig { NameRegion = null }, new FakeOcr(_ => "", ""));
        Assert.Equal(ReadingStatus.NeedNameRegion, reader.ReadFrame(Frame()).Status);
    }

    [Fact]
    public void ReadFrame_ReportsRegionOutsideSmallClient()
    {
        // 初期値の領域がクライアントに収まらないほど小さいウィンドウ
        using var reader = new MapReader(new AppConfig(), new FakeOcr(_ => "", ""));
        Assert.Equal(ReadingStatus.NeedNameRegion, reader.ReadFrame(new BgrImage(60, 40)).Status);
    }

    [Fact]
    public void ReadFrame_DetectsBlackFrame()
    {
        using var reader = new MapReader(Config(), new FakeOcr(_ => "", ""));
        Assert.Equal(ReadingStatus.BlackFrame, reader.ReadFrame(new BgrImage(800, 600)).Status);
    }
}

public class ImageAnalysisTests
{
    [Fact]
    public void TightenTextCrop_RemovesBorderAndMargins()
    {
        using var bitmap = new Bitmap(200, 40);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.FromArgb(15, 15, 25));
            graphics.DrawRectangle(Pens.LightGray, 0, 0, 199, 39); // UI の枠
            graphics.FillRectangle(Brushes.White, 80, 12, 30, 14); // 文字の代わり
        }
        var cropped = ImageAnalysis.TightenTextCrop(BgrImage.FromBitmap(bitmap));
        Assert.Equal(30 + 12, cropped.Width);
        Assert.Equal(14 + 12, cropped.Height);
    }

    [Fact]
    public void TightenTextCrop_IgnoresFragmentsCutBySides()
    {
        using var bitmap = new Bitmap(200, 30);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.FromArgb(15, 15, 25));
            graphics.FillRectangle(Brushes.White, 0, 5, 3, 18); // 隣の UI の縁が左端で切れたもの
            graphics.FillRectangle(Brushes.White, 196, 8, 4, 4); // 右端で切れたもの
            graphics.FillRectangle(Brushes.White, 80, 8, 30, 14); // 文字の代わり
        }
        var cropped = ImageAnalysis.TightenTextCrop(BgrImage.FromBitmap(bitmap));
        Assert.Equal(30 + 12, cropped.Width);
    }

    [Fact]
    public void ResizeBilinear_KeepsUniformColor()
    {
        var image = new BgrImage(3, 3, Enumerable.Repeat((byte)77, 27).ToArray());
        Assert.All(image.ResizeBilinear(10, 7).Pixels, value => Assert.Equal(77, value));
    }
}

/// <summary>ONNX モデルが配置されている場合だけ実行する、実際の OCR を通した確認。</summary>
public class PaddleOcrIntegrationTests
{
    private static readonly OcrModelStore Models = new();

    private static BgrImage RenderText(string text, string font)
    {
        using var bitmap = new Bitmap(320, 34);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.FromArgb(15, 15, 25));
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var typeface = new Font(font, 15, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(Color.FromArgb(235, 235, 235));
            graphics.DrawString(text, typeface, brush, 10, 6);
        }
        return BgrImage.FromBitmap(bitmap);
    }

    [Theory]
    [InlineData("en", "123, 456", "Arial")]
    [InlineData("en", "Rege Cave B10", "Arial")]
    [InlineData("ja", "カールマーニュ洞窟", "Meiryo")]
    [InlineData("ko", "레게 지하동굴", "Malgun Gothic")]
    public void Recognize_ReadsRenderedText(string language, string text, string font)
    {
        if (Models.MissingModels(language).Count > 0)
        {
            return; // モデル未配置の環境ではスキップ
        }
        using var engine = new PaddleOcrEngine(Models);
        var result = engine.Recognize(RenderText(text, font), language);
        Assert.Equal(TextParsing.CanonicalizeForMatch(text), TextParsing.CanonicalizeForMatch(result.Text));
        Assert.True(result.Score > 0.8, $"score {result.Score}");
    }
}
