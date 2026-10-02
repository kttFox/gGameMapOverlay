using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using gGameMapOverlay.Imaging;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using WinOcrEngine = Windows.Media.Ocr.OcrEngine;

namespace gGameMapOverlay.Ocr;

/// <summary>
/// Windows 標準の OCR (Windows.Media.Ocr)。追加ファイル不要だが、
/// 使う言語の OCR 機能が Windows にインストールされている必要がある。
/// </summary>
public sealed partial class WindowsOcrEngine : IOcrEngine
{
    // Windows OCR は日本語の文字ごとに空白を挟んで返すので、和文字どうしの間の空白を詰める。
    [GeneratedRegex(@"(?<=[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}ー])\s+(?=[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}ー])")]
    private static partial Regex JapaneseSpacingRegex();

    private static readonly Dictionary<string, string> LanguageTags = new()
    {
        ["ja"] = "ja-JP",
        ["ko"] = "ko-KR",
        ["en"] = "en-US",
    };

    private readonly Dictionary<string, WinOcrEngine?> engines = [];
    private readonly Lock gate = new();

    public string Name => "Windows OCR";

    public void WarmUp(string language)
    {
        if (Engine(language) is null)
        {
            throw new NotSupportedException(
                $"Windows に {LanguageTags[language]} の OCR 言語がインストールされていません (設定 > 時刻と言語 > 言語 から追加できます)。");
        }
    }

    public OcrResult Recognize(BgrImage image, string language)
    {
        var engine = Engine(language);
        if (engine is null)
        {
            return OcrResult.Empty;
        }
        using var bitmap = ToSoftwareBitmap(Prepare(image, (int)WinOcrEngine.MaxImageDimension));
        var result = engine.RecognizeAsync(bitmap).AsTask().GetAwaiter().GetResult();
        // Windows OCR は認識スコアを返さないため、文字が取れたら 1 とする。
        var text = result.Text.Trim();
        if (language == "ja")
        {
            text = JapaneseSpacingRegex().Replace(text, "");
        }
        return new OcrResult(text, text.Length > 0 ? 1 : 0);
    }

    private WinOcrEngine? Engine(string language)
    {
        lock (gate)
        {
            if (!engines.TryGetValue(language, out var engine))
            {
                var tag = new Language(LanguageTags[language]);
                engine = WinOcrEngine.IsLanguageSupported(tag) ? WinOcrEngine.TryCreateFromLanguage(tag) : null;
                engines[language] = engine;
            }
            return engine;
        }
    }

    /// <summary>小さなゲーム内文字は拡大した方が認識しやすい (最近傍で 4 倍、最大寸法以内)。</summary>
    private static BgrImage Prepare(BgrImage image, int maxDimension)
    {
        var cropped = ImageAnalysis.TightenTextCrop(image);
        var scale = Math.Max(1, Math.Min(4, maxDimension / Math.Max(cropped.Width, cropped.Height)));
        if (scale == 1)
        {
            return cropped;
        }
        var enlarged = new BgrImage(cropped.Width * scale, cropped.Height * scale);
        for (var y = 0; y < enlarged.Height; y++)
        {
            for (var x = 0; x < enlarged.Width; x++)
            {
                Buffer.BlockCopy(cropped.Pixels, ((y / scale) * cropped.Width + x / scale) * 3, enlarged.Pixels, (y * enlarged.Width + x) * 3, 3);
            }
        }
        return enlarged;
    }

    private static SoftwareBitmap ToSoftwareBitmap(BgrImage image)
    {
        var bgra = new byte[image.Width * image.Height * 4];
        for (int i = 0, j = 0; i < image.Pixels.Length; i += 3, j += 4)
        {
            bgra[j] = image.Pixels[i];
            bgra[j + 1] = image.Pixels[i + 1];
            bgra[j + 2] = image.Pixels[i + 2];
            bgra[j + 3] = 255;
        }
        return SoftwareBitmap.CreateCopyFromBuffer(bgra.AsBuffer(), BitmapPixelFormat.Bgra8, image.Width, image.Height, BitmapAlphaMode.Premultiplied);
    }

    public void Dispose()
    {
    }
}
