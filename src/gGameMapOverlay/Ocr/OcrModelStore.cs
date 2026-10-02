using System.Security.Cryptography;

namespace gGameMapOverlay.Ocr;

/// <summary>
/// PaddleOCR PP-OCRv4 認識モデル (ONNX, RapidOCR 配布版) の配置場所を管理する。
/// 文字辞書は ONNX のメタデータ "character" に埋め込まれている。
/// </summary>
public sealed class OcrModelStore
{
    public sealed record ModelInfo(string Language, string FileName, string Url, string Sha256, string ApproximateSize);

    private const string BaseUrl = "https://www.modelscope.cn/models/RapidAI/RapidOCR/resolve/v3.9.2/onnx/PP-OCRv4/rec/";

    public static readonly IReadOnlyDictionary<string, ModelInfo> Models = new Dictionary<string, ModelInfo>
    {
        ["ja"] = new("ja", "japan_PP-OCRv4_rec_mobile.onnx", BaseUrl + "japan_PP-OCRv4_rec_mobile.onnx",
            "e1075a67dba758ecfc7ebc78a10ae61c95ac8fb66a9c86fab5541e33f085cb7a", "9.8MB"),
        ["ko"] = new("ko", "korean_PP-OCRv4_rec_mobile.onnx", BaseUrl + "korean_PP-OCRv4_rec_mobile.onnx",
            "ab151ba9065eccd98f884cf4d927db091be86137276392072edd4f9d43ad7426", "24MB"),
        ["en"] = new("en", "en_PP-OCRv4_rec_mobile.onnx", BaseUrl + "en_PP-OCRv4_rec_mobile.onnx",
            "e8770c967605983d1570cdf5352041dfb68fa0c21664f49f47b155abd3e0e318", "7.7MB"),
    };

    public OcrModelStore(IEnumerable<string>? searchDirectories = null)
    {
        SearchDirectories = searchDirectories?.ToList() ?? [DownloadDirectory];
    }

    public static string DownloadDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "models");

    public IReadOnlyList<string> SearchDirectories { get; }

    public string? FindModel(string language)
    {
        var info = Models[language];
        return SearchDirectories.Select(directory => Path.Combine(directory, info.FileName)).FirstOrDefault(File.Exists);
    }

    /// <summary>未配置のモデル (座標用の en を含む)。</summary>
    public IReadOnlyList<ModelInfo> MissingModels(params string[] languages) =>
        languages.Append("en").Distinct().Where(language => FindModel(language) is null).Select(language => Models[language]).ToList();

    /// <summary>モデルをダウンロードし、SHA-256 を検証してから配置する。</summary>
    public static async Task DownloadAsync(ModelInfo info, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(DownloadDirectory);
        var destination = Path.Combine(DownloadDirectory, info.FileName);
        var temporary = destination + ".download";
        progress?.Report($"{info.FileName} をダウンロード中… ({info.ApproximateSize})");
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        // User-Agent が無いと modelscope.cn は 403 を返す
        client.DefaultRequestHeaders.UserAgent.ParseAdd("gGameMapOverlay/1.0");
        await using (var response = await client.GetStreamAsync(info.Url, cancellationToken))
        await using (var file = File.Create(temporary))
        {
            await response.CopyToAsync(file, cancellationToken);
        }
        string actual;
        await using (var file = File.OpenRead(temporary))
        {
            actual = Convert.ToHexStringLower(await SHA256.HashDataAsync(file, cancellationToken));
        }
        if (actual != info.Sha256)
        {
            File.Delete(temporary);
            throw new InvalidDataException($"{info.FileName} のハッシュが一致しません (expected {info.Sha256}, actual {actual})");
        }
        File.Move(temporary, destination, overwrite: true);
    }
}
