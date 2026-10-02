using gGameMapOverlay.Imaging;

namespace gGameMapOverlay.Ocr;

public readonly record struct OcrResult(string Text, double Score)
{
    public static readonly OcrResult Empty = new("", 0);
}

public interface IOcrEngine : IDisposable
{
    string Name { get; }

    /// <summary>言語モデルを先に読み込んでおく (初回の読み取りが遅れないように)。</summary>
    void WarmUp(string language);

    /// <summary>1行分の文字が写った画像を認識する。language は ja / ko / en。</summary>
    OcrResult Recognize(BgrImage image, string language);
}

/// <summary>OCR を使わない (マップ名と座標を読まず、グリッド・プレイヤー・カスタムだけを画面の中央に合わせて出す)。</summary>
public sealed class NoOcrEngine : IOcrEngine
{
    public const string Backend = "none";

    public string Name => "OCR なし";

    public void WarmUp(string language) { }

    public OcrResult Recognize(BgrImage image, string language) => OcrResult.Empty;

    public void Dispose() { }
}

public static class OcrEngineFactory
{
    /// <param name="threads">PaddleOCR の推論スレッド数 (Windows OCR では使わない)。省略時は自動。</param>
    public static IOcrEngine Create(string backend, OcrModelStore? models = null, int? threads = null) => backend switch
    {
        "windows" => new WindowsOcrEngine(),
        NoOcrEngine.Backend => new NoOcrEngine(),
        _ => new PaddleOcrEngine(models ?? new OcrModelStore(), threads),
    };
}
