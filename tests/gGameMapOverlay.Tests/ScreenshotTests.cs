using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using gGameMapOverlay.Imaging;
using gGameMapOverlay.Ocr;
using gGameMapOverlay.Parsing;

namespace gGameMapOverlay.Tests;

/// <summary>TestData/Screenshots/screenshots.json の1件。</summary>
public sealed record ScreenshotCase(
    [property: JsonPropertyName("file")] string File,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("client_rect")] int[] ClientRect,
    [property: JsonPropertyName("map_name")] string MapName,
    [property: JsonPropertyName("x")] int X,
    [property: JsonPropertyName("y")] int Y,
    [property: JsonPropertyName("ui_scale")] double? UiScale,
    [property: JsonPropertyName("ui_scale_y")] double? UiScaleY)
{
    /// <summary>UI の横の拡大率。等倍の画面は 1.0。</summary>
    public double Scale => UiScale ?? 1.0;

    /// <summary>UI の縦の拡大率 (省略時は横と同じ)。</summary>
    public double ScaleY => UiScaleY ?? Scale;

    public static readonly string Directory = Path.Combine(AppContext.BaseDirectory, "TestData", "Screenshots");

    public static IReadOnlyList<ScreenshotCase> All { get; } = Load();

    /// <summary>すべての画像 (ウィンドウ枠の判定に使う)。</summary>
    public static TheoryData<string> AllFiles => new(All.Select(item => item.File));

    /// <summary>UI が等倍 (表示スケール 100%) の画像。初期値の領域で読み取れるもの。</summary>
    public static TheoryData<string> Files => new(All.Where(item => item.Scale == 1.0).Select(item => item.File));

    /// <summary>基準画像と同じ場面 (同じマップ・座標) で、ウィンドウサイズだけが違う等倍の画像。</summary>
    public static TheoryData<string> SameSceneFiles => new(All
        .Where(item => item.Scale == 1.0 && item.MapName == Get("maximized_1920x1080.png").MapName && item.X == 40 && item.Y == 40)
        .Select(item => item.File));

    public static ScreenshotCase Get(string file) => All.Single(item => item.File == file);

    /// <summary>画像からゲームのクライアント領域だけを切り出す (ウィンドウ版は枠線とタイトルバーを除く)。</summary>
    public BgrImage LoadClient() =>
        BgrImage.Load(Path.Combine(Directory, File))
            .Crop(Rectangle.FromLTRB(ClientRect[0], ClientRect[1], ClientRect[2], ClientRect[3]));

    private static List<ScreenshotCase> Load()
    {
        using var document = JsonDocument.Parse(System.IO.File.ReadAllText(Path.Combine(Directory, "screenshots.json")));
        return document.RootElement.GetProperty("screenshots").Deserialize<List<ScreenshotCase>>()!;
    }
}

/// <summary>実際の gGame の画面 (5 種類のウィンドウサイズ) を使ったテスト。</summary>
public class ScreenshotTests
{
    // 初期値の領域 (最大化 1920x1080 で測ったマップ名欄と座標欄の内側)。
    internal static readonly Rectangle NameRegion = AppConfig.DefaultNameRegion.ClipTo(int.MaxValue, int.MaxValue)!.Value;
    internal static readonly Rectangle CoordinateRegion = AppConfig.DefaultCoordinateRegion.ClipTo(int.MaxValue, int.MaxValue)!.Value;

    private const string Reference = "maximized_1920x1080.png";

    /// <summary>初期値のままの設定。領域を設定しなくても読めることを確認する。</summary>
    internal static AppConfig Config() => new();

    private static double MeanAbsoluteDifference(BgrImage a, BgrImage b)
    {
        Assert.Equal(a.Width, b.Width);
        Assert.Equal(a.Height, b.Height);
        long total = 0;
        for (var i = 0; i < a.Pixels.Length; i++)
        {
            total += Math.Abs(a.Pixels[i] - b.Pixels[i]);
        }
        return total / (double)a.Pixels.Length;
    }

    [Fact]
    public void Manifest_ClientRectsAreInsideImages()
    {
        foreach (var item in ScreenshotCase.All)
        {
            using var bitmap = new Bitmap(Path.Combine(ScreenshotCase.Directory, item.File));
            var client = Rectangle.FromLTRB(item.ClientRect[0], item.ClientRect[1], item.ClientRect[2], item.ClientRect[3]);
            Assert.True(new Rectangle(0, 0, bitmap.Width, bitmap.Height).Contains(client), item.File);
            // gGame は最大化するとタイトルバーと枠線がなくなるので、枠があるのは通常のウィンドウだけ。
            Assert.Equal(item.Mode == "maximized", client.Size == bitmap.Size);
        }
    }

    /// <summary>
    /// gGame の UI はウィンドウサイズに関係なく左上から同じピクセル位置に描かれる。
    /// 領域をピクセル位置で保存している根拠となる性質。
    /// </summary>
    [Theory]
    [MemberData(nameof(ScreenshotCase.SameSceneFiles), MemberType = typeof(ScreenshotCase))]
    public void UiIsAtSamePixelPositionForEveryWindowSize(string file)
    {
        var reference = ScreenshotCase.Get(Reference).LoadClient();
        var client = ScreenshotCase.Get(file).LoadClient();
        foreach (var region in new[] { NameRegion, CoordinateRegion })
        {
            var difference = MeanAbsoluteDifference(reference.Crop(region), client.Crop(region));
            Assert.True(difference < 8, $"{file} {region}: 平均差 {difference:0.0}");
        }
    }

    /// <summary>比率で換算すると、サイズの違うウィンドウでは別の場所を読んでしまうことの確認 (以前の実装の不具合)。</summary>
    [Fact]
    public void RatioScaledRegionWouldMissTheUi()
    {
        var reference = ScreenshotCase.Get(Reference).LoadClient();
        var client = ScreenshotCase.Get("window_1056x660.png").LoadClient();
        var sx = client.Width / (double)reference.Width;
        var sy = client.Height / (double)reference.Height;
        var scaled = Rectangle.FromLTRB(
            (int)Math.Round(CoordinateRegion.Left * sx), (int)Math.Round(CoordinateRegion.Top * sy),
            (int)Math.Round(CoordinateRegion.Right * sx), (int)Math.Round(CoordinateRegion.Bottom * sy));
        var pixelDifference = MeanAbsoluteDifference(reference.Crop(CoordinateRegion), client.Crop(CoordinateRegion));
        var scaledDifference = MeanAbsoluteDifference(
            reference.Crop(CoordinateRegion).ResizeBilinear(scaled.Width, scaled.Height), client.Crop(scaled));
        Assert.True(scaledDifference > pixelDifference * 3, $"pixel {pixelDifference:0.0} / scaled {scaledDifference:0.0}");
    }

    [Theory]
    [MemberData(nameof(ScreenshotCase.Files), MemberType = typeof(ScreenshotCase))]
    public void RegionInClient_UsesSamePixelsForEveryWindowSize(string file)
    {
        var client = ScreenshotCase.Get(file).LoadClient();
        using var reader = new MapReader(Config(), new NullOcr());
        Assert.Equal(NameRegion, reader.RegionInClient(RegionKind.Name, new Size(client.Width, client.Height)));
        Assert.Equal(CoordinateRegion, reader.RegionInClient(RegionKind.Coordinates, new Size(client.Width, client.Height)));
    }

    private sealed class NullOcr : IOcrEngine
    {
        public string Name => "null";
        public void WarmUp(string language) { }
        public OcrResult Recognize(BgrImage image, string language) => OcrResult.Empty;
        public void Dispose() { }
    }
}

/// <summary>PaddleOCR で 1 回だけモデルを読み込み、テスト間で共有する。</summary>
public sealed class PaddleOcrFixture : IDisposable
{
    private static readonly OcrModelStore Models = new();

    public PaddleOcrFixture()
    {
        Available = Models.MissingModels("ja").Count == 0;
        Engine = Available ? new PaddleOcrEngine(Models) : null;
    }

    public bool Available { get; }
    public PaddleOcrEngine? Engine { get; }

    public void Dispose() => Engine?.Dispose();
}

/// <summary>ONNX モデルが配置されている場合だけ実行する、実画面での読み取り確認。</summary>
public class ScreenshotOcrTests(PaddleOcrFixture fixture) : IClassFixture<PaddleOcrFixture>
{
    private Reading Read(string file, AppConfig config)
    {
        using var reader = new MapReader(config, fixture.Engine!, ownsOcr: false);
        var item = ScreenshotCase.Get(file);
        var frame = item.LoadClient();
        // 画像モードと同じく、UI の倍率は画像から測る (撮影したモニターの解像度は分からないので)。
        var transform = UiLocator.Locate(frame) ?? UiTransform.Identity;
        var reading = reader.ReadFrame(frame, force: true, transform);
        for (var i = 1; i < config.NameConfirmHits && reading.Ok; i++)
        {
            reading = reader.ReadFrame(frame, force: true, transform);
        }
        return reading;
    }

    /// <summary>初期値の領域で読める。UI が拡大されている画面 (高さ 1280 の画面) も、倍率を合わせて読む。</summary>
    [Theory]
    [MemberData(nameof(ScreenshotCase.AllFiles), MemberType = typeof(ScreenshotCase))]
    public void ReadsMapNameAndCoordinates(string file)
    {
        if (!fixture.Available)
        {
            return; // モデル未配置の環境ではスキップ
        }
        var expected = ScreenshotCase.Get(file);
        var reading = Read(file, ScreenshotTests.Config());
        Assert.True(reading.Ok, reading.Message);
        Assert.Equal(expected.MapName, reading.MapName);
        Assert.Equal(new GameCoordinate(expected.X, expected.Y), reading.Coordinates);
        Assert.Equal(expected.Scale, reading.UiScale, 0.02);
        Assert.Equal(expected.ScaleY, reading.UiScaleY, 0.02);
    }

    /// <summary>ライブ読み取りで使う「画面の高さからの計算」の倍率でも、Windows 11 (1920x1280) の画面を読める。</summary>
    [Fact]
    public void ReadsScaledScreenWithScaleFromScreenHeight()
    {
        if (!fixture.Available)
        {
            return;
        }
        using var reader = new MapReader(ScreenshotTests.Config(), fixture.Engine!, ownsOcr: false);
        var frame = ScreenshotCase.Get("window_win11_compat_1912x910.png").LoadClient();
        var transform = UiTransform.ForScreen(1280);
        reader.ReadFrame(frame, force: true, transform);
        var reading = reader.ReadFrame(frame, force: true, transform);
        Assert.Equal("ディバル錬金術師ギルド", reading.MapName);
        Assert.Equal(new GameCoordinate(39, 23), reading.Coordinates);
    }

    [Fact]
    public void RoughlyDrawnRegionsStillWork()
    {
        if (!fixture.Available)
        {
            return;
        }
        // 実際の操作では枠より少し大きめに囲むことが多い (枠線は TightenTextCrop が除く)。
        var config = new AppConfig
        {
            NameRegion = new ClientRegion(60, 0, 356, 26),
            CoordinateRegion = new ClientRegion(60, 24, 146, 50),
        };
        var reading = Read("window_1056x660.png", config);
        Assert.Equal("ディーバル", reading.MapName);
        Assert.Equal(new GameCoordinate(40, 40), reading.Coordinates);
    }
}
