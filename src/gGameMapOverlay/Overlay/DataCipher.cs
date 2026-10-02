using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace gGameMapOverlay.Overlay;

/// <summary>
/// data フォルダーの暗号化ファイル (*.pak)。中身は JSON を gzip で圧縮し、AES-GCM で暗号化したもの。
/// 形式: "GMO1" (4 バイト) + nonce (12) + tag (16) + 暗号文。
/// 鍵は実行ファイルに埋め込むので、安易に読まれないための難読化であって、秘密を守るものではない。
/// </summary>
public static class DataCipher
{
    public const string Extension = ".pak";

    private static ReadOnlySpan<byte> Magic => "GMO1"u8;

    private const string Passphrase = "Godius_is_god_game";

    private static readonly byte[] Key = SHA256.HashData(Encoding.UTF8.GetBytes(Passphrase));

    private const int NonceSize = 12;
    private const int TagSize = 16;

    public static byte[] Encrypt(string text)
    {
        using var compressed = new MemoryStream();
        using (var gzip = new GZipStream(compressed, CompressionLevel.SmallestSize))
        {
            gzip.Write(Encoding.UTF8.GetBytes(text));
        }
        var plain = compressed.ToArray();
        var result = new byte[Magic.Length + NonceSize + TagSize + plain.Length];
        Magic.CopyTo(result);
        var nonce = result.AsSpan(Magic.Length, NonceSize);
        RandomNumberGenerator.Fill(nonce);
        using var aes = new AesGcm(Key, TagSize);
        aes.Encrypt(nonce, plain, result.AsSpan(Magic.Length + NonceSize + TagSize), result.AsSpan(Magic.Length + NonceSize, TagSize));
        return result;
    }

    public static string Decrypt(byte[] data)
    {
        if (data.Length < Magic.Length + NonceSize + TagSize || !data.AsSpan(0, Magic.Length).SequenceEqual(Magic))
        {
            throw new InvalidDataException("暗号化データの形式が違います。");
        }
        var cipher = data.AsSpan(Magic.Length + NonceSize + TagSize);
        var plain = new byte[cipher.Length];
        using (var aes = new AesGcm(Key, TagSize))
        {
            aes.Decrypt(data.AsSpan(Magic.Length, NonceSize), cipher, data.AsSpan(Magic.Length + NonceSize, TagSize), plain);
        }
        using var gzip = new GZipStream(new MemoryStream(plain), CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// JSON ファイル (path は "*.json") を読む。同じ名前の *.pak があればそちらを復号して使う。
    /// どちらも無ければ null。
    /// </summary>
    public static string? ReadJson(string path)
    {
        var encrypted = Path.ChangeExtension(path, Extension);
        if (File.Exists(encrypted))
        {
            return Decrypt(File.ReadAllBytes(encrypted));
        }
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    /// <summary>sourceDirectory の *.json をすべて暗号化し、outputDirectory に *.pak として書く。書いたファイルの一覧を返す。</summary>
    public static IReadOnlyList<string> EncryptDirectory(string sourceDirectory, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        var written = new List<string>();
        foreach (var source in Directory.EnumerateFiles(sourceDirectory, "*.json").Order())
        {
            var output = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(source) + Extension);
            File.WriteAllBytes(output, Encrypt(File.ReadAllText(source)));
            written.Add(output);
        }
        return written;
    }
}
