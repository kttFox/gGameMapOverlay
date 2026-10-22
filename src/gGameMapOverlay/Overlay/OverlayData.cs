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
    // 期間限定で差し替えるマップ (ハロウィン版など)。期間中は、元の id の代わりに差し替え先の id を使う。
    private readonly List<MapReplacement> replacements = [];

    /// <summary>
    /// From 以上 Until 未満の間、マップ Original の代わりに Replacement を使う。
    /// OriginalNames は、期間中に元のマップを手動で選ぶための名前 (ミリタード(通常) など)。期間中だけ一覧に出る。
    /// </summary>
    public sealed record MapReplacement(string Original, string Replacement, DateTimeOffset From, DateTimeOffset Until,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? OriginalNames = null)
    {
        public bool IsActive(DateTimeOffset now) => now >= From && now < Until;
    }

    /// <param name="maps">マップ id と、言語 (ja など) ごとの名前。</param>
    /// <param name="layers">種類ごとの、マップ id → マス。ない種類は何も表示しない。</param>
    /// <param name="replacements">期間限定で差し替えるマップ。</param>
    public OverlayData(IEnumerable<(string Id, IReadOnlyDictionary<string, IReadOnlyList<string>> Names)> maps, IReadOnlyDictionary<OverlayLayer, Dictionary<string, TileRects>> layers,
        IEnumerable<MapReplacement>? replacements = null)
    {
        this.layers = layers;
        this.replacements = replacements?.ToList() ?? [];
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
                Names: ReadNames(map.GetProperty("names"))))
            .Where(map => map.Id.Length > 0)
            .ToList();
        // "replaces" / "from" / "until" があるマップは、その期間だけ "replaces" のマップの代わりに使う。
        var replacements = mapsDocument.RootElement.GetProperty("maps").EnumerateArray()
            .Where(map => map.TryGetProperty("replaces", out _))
            .Select(map => new MapReplacement(
                map.GetProperty("replaces").GetString() ?? "",
                map.GetProperty("id").GetString() ?? "",
                map.GetProperty("from").GetDateTimeOffset(),
                map.GetProperty("until").GetDateTimeOffset(),
                map.TryGetProperty("original_names", out var originalNames) ? ReadNames(originalNames) : null))
            .ToList();
        var layers = new Dictionary<OverlayLayer, Dictionary<string, TileRects>>();
        foreach (var layer in OverlayLayer.All)
        {
            if (DataCipher.ReadJson(Path.Combine(directory, layer.FileName)) is { } json)
            {
                layers[layer] = JsonSerializer.Deserialize<Dictionary<string, TileRects>>(json) ?? [];
            }
        }
        return new OverlayData(maps, layers, replacements);
    }

    // 言語ごとの名前 ({"ja": ["..."], ...}) を読む。
    private static IReadOnlyDictionary<string, IReadOnlyList<string>> ReadNames(JsonElement names) =>
        names.EnumerateObject().ToDictionary(language => language.Name,
            language => (IReadOnlyList<string>)language.Value.EnumerateArray().Select(name => name.GetString() ?? "").Where(name => name.Length > 0).ToList());

    /// <summary>OverlayLayer.All のうち、ファイルがなかったもの。</summary>
    public IEnumerable<OverlayLayer> MissingLayers => OverlayLayer.All.Where(layer => !layers.ContainsKey(layer));

    /// <summary>その種類のデータがあるマップの数。</summary>
    public int MapCountOf(OverlayLayer layer) => layers.TryGetValue(layer, out var maps) ? maps.Count : 0;

    /// <summary>
    /// OCR で読んだマップ名の id。maps.json に無ければ null。
    /// 期間限定の差し替えがあるマップは、期間中なら差し替え先の id を返す。
    /// </summary>
    public string? FindMapId(string? mapName) => FindMapId(mapName, DateTimeOffset.Now);

    /// <summary>now の時点での FindMapId。</summary>
    public string? FindMapId(string? mapName, DateTimeOffset now)
    {
        var key = TextParsing.CanonicalizeForMatch(mapName);
        if (replacements.FirstOrDefault(replacement => replacement.IsActive(now) && NamesOf(replacement.OriginalNames).Any(name => TextParsing.CanonicalizeForMatch(name) == key)) is { } original)
        {
            return original.Original; // ミリタード(通常) などは、差し替えずに元のマップ
        }
        var id = idByName.GetValueOrDefault(key);
        return replacements.FirstOrDefault(replacement => replacement.Original == id && replacement.IsActive(now))?.Replacement ?? id;
    }

    /// <summary>
    /// 手動で選ぶためのマップ名の一覧 (maps.json の順)。その言語の最初の名前を使い、なければ他の言語の名前を使う。
    /// 差し替えの期間中は、差し替え先のすぐ後に元のマップを選ぶ名前 (ミリタード(通常) など) を入れる。
    /// 名前は FindMapId で引ける。
    /// </summary>
    public IReadOnlyList<string> MapNames(string language) => MapNames(language, DateTimeOffset.Now);

    /// <summary>now の時点での MapNames。</summary>
    public IReadOnlyList<string> MapNames(string language, DateTimeOffset now) => maps
        .SelectMany(map => replacements
            .Where(replacement => replacement.Replacement == map.Id && replacement.IsActive(now) && replacement.OriginalNames is not null)
            .Select(replacement => replacement.OriginalNames!)
            .Prepend(map.Names))
        .Select(names => (names.GetValueOrDefault(language) ?? []).Concat(names.Values.SelectMany(list => list)).FirstOrDefault())
        .OfType<string>()
        .Distinct()
        .ToList();

    private static IEnumerable<string> NamesOf(IReadOnlyDictionary<string, IReadOnlyList<string>>? names) =>
        names?.Values.SelectMany(list => list) ?? [];

    /// <summary>そのマップのマス。キーが無ければ (表示するものが無いマップ) null。</summary>
    public TileRects? Find(OverlayLayer layer, string mapId) =>
        layers.TryGetValue(layer, out var maps) ? maps.GetValueOrDefault(mapId) : null;
}
