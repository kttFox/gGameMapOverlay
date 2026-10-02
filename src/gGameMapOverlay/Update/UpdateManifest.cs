using System.Text.Json;
using System.Text.Json.Serialization;

namespace gGameMapOverlay.Update;

/// <summary>更新の単位。本体 (実行ファイル一式) とマップ情報 (data フォルダー) は別々に更新する。</summary>
public enum UpdateComponent
{
    App,
    Data,
}

/// <summary>マニフェストの 1 コンポーネント分。url はマニフェストの URL からの相対でもよい。</summary>
public sealed record UpdatePackage
{
    [JsonPropertyName("version")]
    public string Version { get; init; } = "";

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    [JsonPropertyName("sha256")]
    public string Sha256 { get; init; } = "";

    [JsonPropertyName("size")]
    public long Size { get; init; }

    /// <summary>更新内容 (確認のダイアログに表示する)。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

/// <summary>
/// 更新情報 (update_url に置く manifest.json)。形式:
/// { "schema": 1, "app": { version, url, sha256, size, notes }, "data": { ... } }
/// app / data はどちらかだけでもよい。tools/make-release.ps1 で作る。
/// </summary>
public sealed record UpdateManifest
{
    public const int SupportedSchema = 1;

    /// <summary>マニフェストの上限。これより大きい応答は読まない。</summary>
    public const int MaxBytes = 1024 * 1024;

    [JsonPropertyName("schema")]
    public int Schema { get; init; }

    [JsonPropertyName("app")]
    public UpdatePackage? App { get; init; }

    [JsonPropertyName("data")]
    public UpdatePackage? Data { get; init; }

    public UpdatePackage? Get(UpdateComponent component) => component == UpdateComponent.App ? App : Data;

    /// <summary>JSON を読み、形式を検証する。おかしければ InvalidDataException。</summary>
    public static UpdateManifest Parse(string json)
    {
        UpdateManifest manifest;
        try
        {
            manifest = JsonSerializer.Deserialize<UpdateManifest>(json) ?? throw new InvalidDataException("マニフェストが空です。");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"マニフェストを読めません ({exception.Message})", exception);
        }
        if (manifest.Schema != SupportedSchema)
        {
            throw new InvalidDataException($"対応していないマニフェストの形式です (schema {manifest.Schema})。");
        }
        foreach (var component in Enum.GetValues<UpdateComponent>())
        {
            if (manifest.Get(component) is { } package)
            {
                Validate(component, package);
            }
        }
        return manifest;
    }

    private static void Validate(UpdateComponent component, UpdatePackage package)
    {
        if (VersionOrder.Parts(package.Version).Length == 0)
        {
            throw new InvalidDataException($"{component} の version が不正です ({package.Version})。");
        }
        if (package.Url.Length == 0)
        {
            throw new InvalidDataException($"{component} の url がありません。");
        }
        if (package.Sha256.Length != 64 || !package.Sha256.All(Uri.IsHexDigit))
        {
            throw new InvalidDataException($"{component} の sha256 が不正です。");
        }
        if (package.Size <= 0)
        {
            throw new InvalidDataException($"{component} の size が不正です。");
        }
    }
}

/// <summary>"1.2.3" や "100" のような、数字を区切ったバージョンの比較。</summary>
public static class VersionOrder
{
    /// <summary>数字の並び。"+" 以降 (ビルド情報) は無視する。数字がなければ空。</summary>
    public static int[] Parts(string? version)
    {
        var core = (version ?? "").Split('+')[0];
        return core.Split('.', '-')
            .Select(part => new string(part.TakeWhile(char.IsAsciiDigit).ToArray()))
            .TakeWhile(digits => digits.Length > 0)
            .Select(digits => int.TryParse(digits, out var value) ? value : int.MaxValue)
            .ToArray();
    }

    /// <summary>latest が current より新しいか。足りない桁は 0 とみなす (1.2 == 1.2.0)。</summary>
    public static bool IsNewer(string latest, string? current)
    {
        var (a, b) = (Parts(latest), Parts(current));
        for (var i = 0; i < Math.Max(a.Length, b.Length); i++)
        {
            var (x, y) = (i < a.Length ? a[i] : 0, i < b.Length ? b[i] : 0);
            if (x != y)
            {
                return x > y;
            }
        }
        return false;
    }
}
