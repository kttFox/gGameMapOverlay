using System.Text;
using gGameMapOverlay.Imaging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace gGameMapOverlay.Ocr;

/// <summary>
/// PaddleOCR PP-OCRv4 の文字認識 (rec) モデルを ONNX Runtime で直接実行する。
/// 読み取り領域は 1 行分をユーザーが指定する前提なので、検出 (det) と方向分類 (cls) は行わない。
/// 前処理と CTC デコードは RapidOCR の実装に合わせている。
/// </summary>
public sealed class PaddleOcrEngine : IOcrEngine
{
    private const int InputHeight = 48;
    private const int MinInputWidth = 320;

    private readonly OcrModelStore models;
    private readonly int threads;
    private readonly Dictionary<string, (InferenceSession Session, string[] Characters)> sessions = [];
    private readonly Lock gate = new();

    /// <param name="threads">推論に使うスレッド数。省略時は AppConfig.AutoOcrThreads。</param>
    public PaddleOcrEngine(OcrModelStore models, int? threads = null)
    {
        this.models = models;
        this.threads = Math.Max(1, threads ?? AppConfig.AutoOcrThreads);
    }

    public string Name => "PaddleOCR";

    public void WarmUp(string language) => Session(language);

    public OcrResult Recognize(BgrImage image, string language)
    {
        var (session, characters) = Session(language);
        var input = Preprocess(ImageAnalysis.TightenTextCrop(image));
        using var outputs = session.Run([NamedOnnxValue.CreateFromTensor(session.InputNames[0], input)]);
        var probabilities = outputs[0].AsTensor<float>();
        return Decode(probabilities, characters);
    }

    private (InferenceSession Session, string[] Characters) Session(string language)
    {
        lock (gate)
        {
            if (sessions.TryGetValue(language, out var cached))
            {
                return cached;
            }
            var path = models.FindModel(language)
                ?? throw new FileNotFoundException(
                    $"OCR モデル {OcrModelStore.Models[language].FileName} が見つかりません。", OcrModelStore.Models[language].FileName);
            var options = new SessionOptions
            {
                // 座標の読み取りが遅れるとオーバーレイの追従が遅れるので、複数スレッドで推論する
                // (en モデルで 1 スレッド 59ms → 4 スレッド 20ms)。
                IntraOpNumThreads = threads,
                InterOpNumThreads = 1,
                GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
            };
            // 小さな画像を高頻度で推論するため、スレッドのスピン待ちで CPU を使わせない。
            options.AddSessionConfigEntry("session.intra_op.allow_spinning", "0");
            options.AddSessionConfigEntry("session.inter_op.allow_spinning", "0");
            var session = new InferenceSession(path, options);
            if (!session.ModelMetadata.CustomMetadataMap.TryGetValue("character", out var dictionary))
            {
                throw new InvalidDataException($"{Path.GetFileName(path)} に文字辞書のメタデータがありません。");
            }
            // index 0 は CTC の blank、末尾は空白文字 (use_space_char)。
            // メタデータは末尾が改行で終わるので、空の最終行を辞書に含めない。
            var characters = new[] { "" }
                .Concat(dictionary.TrimEnd('\n').Split('\n').Select(line => line.TrimEnd('\r')))
                .Append(" ")
                .ToArray();
            var classes = session.OutputMetadata.Values.First().Dimensions[^1];
            if (classes > 0 && classes != characters.Length)
            {
                session.Dispose();
                throw new InvalidDataException(
                    $"{Path.GetFileName(path)} の文字辞書 ({characters.Length}) と出力クラス数 ({classes}) が一致しません。");
            }
            sessions[language] = (session, characters);
            return sessions[language];
        }
    }

    /// <summary>高さ 48 に縦横比を保って縮尺し、[-1, 1] に正規化、右側をゼロ詰めする (NCHW, BGR)。</summary>
    internal static DenseTensor<float> Preprocess(BgrImage image)
    {
        var ratio = image.Width / (double)image.Height;
        var inputWidth = Math.Max(MinInputWidth, (int)(InputHeight * Math.Max(MinInputWidth / (double)InputHeight, ratio)));
        var resizedWidth = Math.Min(inputWidth, (int)Math.Ceiling(InputHeight * ratio));
        var resized = image.ResizeBilinear(Math.Max(1, resizedWidth), InputHeight);
        var tensor = new DenseTensor<float>([1, 3, InputHeight, inputWidth]);
        for (var y = 0; y < InputHeight; y++)
        {
            for (var x = 0; x < resized.Width; x++)
            {
                var i = (y * resized.Width + x) * 3;
                for (var c = 0; c < 3; c++)
                {
                    tensor[0, c, y, x] = (resized.Pixels[i + c] / 255f - 0.5f) / 0.5f;
                }
            }
        }
        return tensor;
    }

    /// <summary>CTC の貪欲デコード。連続する同一ラベルと blank を除き、採用した文字の確率平均をスコアにする。</summary>
    internal static OcrResult Decode(Tensor<float> probabilities, string[] characters)
    {
        var steps = probabilities.Dimensions[1];
        var classes = probabilities.Dimensions[2];
        var text = new StringBuilder();
        var confidences = new List<float>();
        var previous = -1;
        for (var t = 0; t < steps; t++)
        {
            var best = 0;
            var bestProbability = float.MinValue;
            for (var k = 0; k < classes; k++)
            {
                var probability = probabilities[0, t, k];
                if (probability > bestProbability)
                {
                    bestProbability = probability;
                    best = k;
                }
            }
            if (best != 0 && best != previous && best < characters.Length)
            {
                text.Append(characters[best]);
                confidences.Add(bestProbability);
            }
            previous = best;
        }
        return new OcrResult(text.ToString().Trim(), confidences.Count > 0 ? confidences.Average() : 0);
    }

    public void Dispose()
    {
        lock (gate)
        {
            foreach (var (session, _) in sessions.Values)
            {
                session.Dispose();
            }
            sessions.Clear();
        }
    }
}
