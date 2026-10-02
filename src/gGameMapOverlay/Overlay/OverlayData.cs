using System.Text.Json;
using System.Text.Json.Serialization;
using gGameMapOverlay.Parsing;

namespace gGameMapOverlay.Overlay;

/// <summary>1 マップ分のマスの集まり (data/monster-block.json などの 1 件)。</summary>
public sealed class TileRects
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    /// <summary>マスを覆う長方形 [x, y, 幅, 高さ] の一覧。長方形同士は重ならない。</summary>
    [JsonPropertyName("rects")]
    public int[][] Rects { get; init; } = [];

    public bool Contains(int x, int y) =>
        Rects.Any(rect => x >= rect[0] && x < rect[0] + rect[2] && y >= rect[1] && y < rect[1] + rect[3]);
}

/// <summary>
/// オーバーレイに使う data フォルダーのデータ。maps.json でマップ名から id を引き、
/// id で各 OverlayLayer のファイル (monster-block.json など) を引く。
/// </summary>
public sealed class OverlayData
{
    public const string MapsFileName = "maps.json";

    /// <summary>実行ファイルの横の data フォルダー (ビルド時にリポジトリの data/ からコピーする)。</summary>
    public static readonly string DefaultDirectory = Path.Combine(AppContext.BaseDirectory, "data");

    // キーは TextParsing.CanonicalizeForMatch で正規化した名前 (OCR の空白・全角半角の揺れを吸収する)。
    private readonly Dictionary<string, string> idByName = [];
    private readonly IReadOnlyDictionary<OverlayLayer, Dictionary<string, TileRects>> layers;
    // maps.json の順の、マップ id と言語ごとの名前 (手動でマップを選ぶ一覧に使う)。
    private readonly List<(string Id, IReadOnlyDictionary<string, IReadOnlyList<string>> Names)> maps;

    /// <param name="maps">マップ id と、言語 (ja など) ごとの名前。</param>
    /// <param name="layers">種類ごとの、マップ id → マス。ない種類は何も表示しない。</param>
    public OverlayData(IEnumerable<(string Id, IReadOnlyDictionary<string, IReadOnlyList<string>> Names)> maps, IReadOnlyDictionary<OverlayLayer, Dictionary<string, TileRects>> layers)
    {
        this.layers = layers;
        this.maps = maps.ToList();
        foreach (var (id, names) in this.maps)
        {
            foreach (var name in names.Values.SelectMany(list => list))
            {
                var key = TextParsing.CanonicalizeForMatch(name);
                if (key.Length > 0)
                {
                    idByName.TryAdd(key, id); // 同じ名前が複数の id にあれば、maps.json で先に書いてあるほう
                }
            }
        }
    }

    /// <summary>
    /// maps.json と、OverlayLayer.All のファイルを読み込む。
    /// maps.json は必須。種類ごとのファイルは、なければその種類を表示しないだけ (MissingFiles に入る)。
    /// どのファイルも、同じ名前の暗号化ファイル (*.pak、DataCipher) があればそちらを使う。
    /// </summary>
    public static OverlayData Load(string directory)
    {
        using var mapsDocument = JsonDocument.Parse(DataCipher.ReadJson(Path.Combine(directory, MapsFileName))
            ?? throw new FileNotFoundException($"{MapsFileName} がありません。", Path.Combine(directory, MapsFileName)));
        var maps = mapsDocument.RootElement.GetProperty("maps").EnumerateArray()
            .Select(map => (
                Id: map.GetProperty("id").GetString() ?? "",
                Names: (IReadOnlyDictionary<string, IReadOnlyList<string>>)map.GetProperty("names").EnumerateObject()
                    .ToDictionary(language => language.Name,
                        language => (IReadOnlyList<string>)language.Value.EnumerateArray().Select(name => name.GetString() ?? "").Where(name => name.Length > 0).ToList())))
            .Where(map => map.Id.Length > 0)
            .ToList();
        var layers = new Dictionary<OverlayLayer, Dictionary<string, TileRects>>();
        foreach (var layer in OverlayLayer.All)
        {
            if (DataCipher.ReadJson(Path.Combine(directory, layer.FileName)) is { } json)
            {
                layers[layer] = JsonSerializer.Deserialize<Dictionary<string, TileRects>>(json) ?? [];
            }
        }
        return new OverlayData(maps, layers);
    }

    /// <summary>OverlayLayer.All のうち、ファイルがなかったもの。</summary>
    public IEnumerable<OverlayLayer> MissingLayers => OverlayLayer.All.Where(layer => !layers.ContainsKey(layer));

    /// <summary>その種類のデータがあるマップの数。</summary>
    public int MapCountOf(OverlayLayer layer) => layers.TryGetValue(layer, out var maps) ? maps.Count : 0;

    /// <summary>OCR で読んだマップ名の id。maps.json に無ければ null。</summary>
    public string? FindMapId(string? mapName) =>
        idByName.GetValueOrDefault(TextParsing.CanonicalizeForMatch(mapName));

    /// <summary>
    /// 手動で選ぶためのマップ名の一覧 (maps.json の順)。その言語の最初の名前を使い、なければ他の言語の名前を使う。
    /// 名前は FindMapId で引ける。
    /// </summary>
    public IReadOnlyList<string> MapNames(string language) => maps
        .Select(map => (map.Names.GetValueOrDefault(language) ?? []).Concat(map.Names.Values.SelectMany(list => list)).FirstOrDefault())
        .OfType<string>()
        .Distinct()
        .ToList();

    /// <summary>そのマップのマス。キーが無ければ (表示するものが無いマップ) null。</summary>
    public TileRects? Find(OverlayLayer layer, string mapId) =>
        layers.TryGetValue(layer, out var maps) ? maps.GetValueOrDefault(mapId) : null;
}
