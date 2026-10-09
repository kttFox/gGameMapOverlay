using System.Text.Json.Serialization;
using DrawingColor = System.Drawing.Color;

namespace gGameMapOverlay.Overlay;

/// <summary>オーバーレイに渡す、同じレイヤーで描いたマス。</summary>
/// <param name="Fill">塗る長方形 (相対位置、1/Division マス単位。CustomTileGroup.SubGrid の格子で描く)。</param>
/// <param name="Frames">枠を描くマス (相対位置、1 マス単位)。</param>
/// <param name="Low">Gマス (Areas) を広げたマスだけか。マップに固定したものは、マップのデータのマスより下に描く。</param>
public sealed record CustomTiles(TileRects Fill, TileRects Frames, DrawingColor Color, int Division = CustomTileGroup.Division, bool Dotted = false, bool Outlined = true, bool Low = false);

/// <summary>
/// 自分で描くマス (カスタム) の 1 グループ。マスはキャラクターのいるマスからの相対位置で持ち、
/// プレイヤーの枠と同じく画面に固定して描く (歩いてもスライドしない)。
/// 1 マスは縦横 Division 等分した小さいマス (1/3 マス) の集まりで、どこを塗るかを形 (CustomTileShape.Mask) で持つ。
/// 線の太さ (1/thickness マス) と、描いたレイヤー (Layers の番号) もマスごとに持つ。色・ドット・輪郭線はレイヤーで、表示・不透明度はグループでまとめて決める。
/// マップに固定したグループ (MapName) は、マスを Origin (そのマップの座標) からの相対位置で持ち、そのマップにいるときだけ地形と一緒に描く。
/// Gマスは中心のマスだけを Areas に持ち、描くときに AreaSize 四方へ広げる。優先度は最低で、どのマスよりも下に描く。
/// </summary>
public sealed class CustomTileGroup
{
    public static readonly DrawingColor DefaultColor = DrawingColor.FromArgb(0, 188, 212);

    /// <summary>1 マスを縦横何等分して形を作るか。</summary>
    public const int Division = 3;

    /// <summary>1 マス全部を塗る形。</summary>
    public const int FullMask = (1 << (Division * Division)) - 1;

    /// <summary>マスの縁に、プレイヤーの枠と同じ線の枠を描く (塗る形とは別に持つビット)。</summary>
    public const int FrameFlag = 1 << (Division * Division);

    private const int ValueMask = FullMask | FrameFlag;

    /// <summary>Gマスの広さ (中心のマスから AreaSize 四方を塗る)。</summary>
    public const int AreaSize = 15;

    public string Name { get; set; } = "";

    /// <summary>不透明度 (%)。AppConfig.MinOverlayOpacity〜MaxOverlayOpacity。OwnOpacity のときだけ使う。</summary>
    public int Opacity { get; set; } = AppConfig.DefaultOverlayOpacity;

    /// <summary>個別の不透明度 (Opacity) を使う。false なら全体の不透明度 (AppConfig.OverlayOpacity) を使う。</summary>
    public bool OwnOpacity { get; set; } = true;

    public bool Shown { get; set; } = true;

    /// <summary>固定したマップの名前。null ならどのマップでもキャラクターに付いていく。</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MapName { get; set; }

    /// <summary>マップに固定したとき、マスの相対位置の基準にするそのマップの座標 [x, y]。</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Origin { get; set; }

    /// <summary>マップに固定しているか。</summary>
    [JsonIgnore]
    public bool OnMap => MapName is not null;

    /// <summary>マップに固定したときの基準の座標。</summary>
    [JsonIgnore]
    public Parsing.GameCoordinate OriginCoordinate => Origin is [var x, var y] ? new(x, y) : new(0, 0);

    /// <summary>レイヤー (Cells・Areas の layer はこの番号)。この順に重ねる (後が上)。Normalize の後は 1 つ以上ある。</summary>
    public List<CustomTileLayer> Layers { get; set; } = [];

    /// <summary>
    /// キャラクターのいるマスからの相対位置と形の一覧。[dx, dy] は 1 マス全部、[dx, dy, mask] は形 (CustomTileShape.Mask) の部分だけ
    /// (FrameFlag があれば枠も描く)、[dx, dy, mask, thickness] は線の太さを 1/thickness マスにした形、
    /// [dx, dy, mask, thickness, layer] は Layers[layer] に描いた形 (書いていなければ Layers[0]・太さ 1/3 (CustomTileShape.DefaultThickness))。
    /// </summary>
    public List<int[]> Cells { get; set; } = [];

    /// <summary>Gマスの中心のマスの一覧。[dx, dy] は Layers[0]、[dx, dy, layer] は Layers[layer] に塗る。</summary>
    public List<int[]> Areas { get; set; } = [];

    /// <summary>レイヤーの代わりにペンを持っていたころの設定。読み込んだら Normalize でレイヤーに移す。</summary>
    [JsonPropertyName("pens"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CustomTileLayer>? LegacyPens { get; set; }

    /// <summary>レイヤーがなかったころの設定の色。読み込んだら Normalize でレイヤーに移す。</summary>
    [JsonPropertyName("color"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LegacyColor { get; set; }

    /// <summary>マスの数。</summary>
    [JsonIgnore]
    public int CellCount => Cells.Count + Areas.Count;

    /// <summary>変えても元に影響しない写し (マスの一覧も複製する)。</summary>
    public CustomTileGroup Clone()
    {
        var copy = (CustomTileGroup)MemberwiseClone();
        copy.Layers = [.. Layers];
        copy.Cells = Cells.Select(cell => (int[])cell.Clone()).ToList();
        copy.Areas = Areas.Select(area => (int[])area.Clone()).ToList();
        copy.Origin = (int[]?)Origin?.Clone();
        copy.LegacyPens = null;
        return copy;
    }

    /// <summary>使う不透明度 (%)。個別でなければ overallOpacity (全体の不透明度)。</summary>
    public int GetOpacity(int overallOpacity = AppConfig.DefaultOverlayOpacity) =>
        Math.Clamp(OwnOpacity ? Opacity : overallOpacity, AppConfig.MinOverlayOpacity, AppConfig.MaxOverlayOpacity);

    /// <summary>レイヤー layer に描いたマスの数。</summary>
    public int LayerCellCount(int layer) => Cells.Count(cell => LayerOf(cell) == layer) + Areas.Count(area => AreaLayerOf(area) == layer);

    private static int AreaLayerOf(int[] area) => area.Length >= 3 ? area[2] : 0;

    /// <summary>レイヤー index を消す (そのレイヤーのマスも消す)。最後の 1 つは消さない。</summary>
    public void RemoveLayer(int index)
    {
        if (Layers.Count <= 1 || index < 0 || index >= Layers.Count)
        {
            return;
        }
        Layers.RemoveAt(index);
        Cells = Cells.Where(cell => LayerOf(cell) != index)
            .Select(cell => LayerOf(cell) is var layer && layer > index ? WithLayer(cell, layer - 1) : cell).ToList();
        Areas = Areas.Where(area => AreaLayerOf(area) != index)
            .Select(area => AreaLayerOf(area) is var layer && layer > index ? MakeArea(area[0], area[1], layer - 1) : area).ToList();
    }

    /// <summary>レイヤー layer を index 番目に入れる (それより後のレイヤーのマスは番号をずらす)。</summary>
    public void InsertLayer(int index, CustomTileLayer layer)
    {
        index = Math.Clamp(index, 0, Layers.Count);
        Layers.Insert(index, layer);
        Cells = Cells.Select(cell => LayerOf(cell) is var old && old >= index ? WithLayer(cell, old + 1) : cell).ToList();
        Areas = Areas.Select(area => AreaLayerOf(area) is var old && old >= index ? MakeArea(area[0], area[1], old + 1) : area).ToList();
    }

    /// <summary>レイヤー index と target を入れ替える (マスはレイヤーと一緒に動く)。</summary>
    public void SwapLayers(int index, int target)
    {
        if (index < 0 || index >= Layers.Count || target < 0 || target >= Layers.Count || index == target)
        {
            return;
        }
        (Layers[index], Layers[target]) = (Layers[target], Layers[index]);
        int Swap(int layer) => layer == index ? target : layer == target ? index : layer;
        Cells = Cells.Select(cell => WithLayer(cell, Swap(LayerOf(cell)))).ToList();
        Areas = Areas.Select(area => MakeArea(area[0], area[1], Swap(AreaLayerOf(area)))).ToList();
    }

    private static int[] WithLayer(int[] cell, int layer) => MakeCell(cell[0], cell[1], MaskOf(cell), ThicknessOf(cell), layer);

    private static int[] MakeArea(int dx, int dy, int layer) => layer != 0 ? [dx, dy, layer] : [dx, dy];

    /// <summary>中心 (x, y) の Gマスが塗るマス (中心から前後に AreaSize / 2 マス)。</summary>
    public static IEnumerable<(int X, int Y)> AreaCells(int x, int y)
    {
        const int from = -(AreaSize / 2);
        for (var dy = from; dy < from + AreaSize; dy++)
        {
            for (var dx = from; dx < from + AreaSize; dx++)
            {
                yield return (x + dx, y + dy);
            }
        }
    }

    /// <summary>中心 (dx, dy) に、レイヤー layer の Gマスを置く (同じ中心の前のものは置き換える)。変わったら true。</summary>
    public bool AddArea(int dx, int dy, int layer)
    {
        var area = MakeArea(dx, dy, layer);
        if (Areas.FirstOrDefault(old => old[0] == dx && old[1] == dy) is { } old && old.SequenceEqual(area))
        {
            return false;
        }
        Areas.RemoveAll(old => old[0] == dx && old[1] == dy);
        Areas.Add(area);
        return true;
    }

    /// <summary>マス (x, y) を塗っている Gマスを消す。消したら true。</summary>
    public bool RemoveAreasAt(int x, int y) =>
        Areas.RemoveAll(area => Math.Abs(area[0] - x) <= AreaSize / 2 && Math.Abs(area[1] - y) <= AreaSize / 2) > 0;

    private static int MaskOf(int[] cell) => cell.Length >= 3 ? cell[2] & ValueMask : FullMask;

    private static int ThicknessOf(int[] cell) =>
        cell.Length >= 4 ? Math.Clamp(cell[3], CustomTileShape.MinThickness, CustomTileShape.MaxThickness) : CustomTileShape.DefaultThickness;

    private static int LayerOf(int[] cell) => cell.Length >= 5 ? cell[4] : 0;

    /// <summary>太さで塗る部分が変わる形か (1 マス全部・枠だけは変わらない)。</summary>
    private static bool HasThickness(int mask) => (mask & FullMask) is not (0 or FullMask);

    /// <summary>保存する形 ([dx, dy]・[dx, dy, mask]・[dx, dy, mask, thickness]・[dx, dy, mask, thickness, layer] のうち一番短いもの)。</summary>
    private static int[] MakeCell(int dx, int dy, int mask, int thickness, int layer)
    {
        if (!HasThickness(mask))
        {
            thickness = CustomTileShape.DefaultThickness;
        }
        return layer != 0 ? [dx, dy, mask, thickness, layer]
            : thickness != CustomTileShape.DefaultThickness ? [dx, dy, mask, thickness]
            : mask != FullMask ? [dx, dy, mask]
            : [dx, dy];
    }

    private int[]? CellAt(int dx, int dy) => Cells.FirstOrDefault(cell => cell[0] == dx && cell[1] == dy);

    /// <summary>そのマスの形。描いていなければ 0。</summary>
    public int MaskAt(int dx, int dy) => CellAt(dx, dy) is { } cell ? MaskOf(cell) : 0;

    /// <summary>そのマスの線の太さ (1/thickness マス)。描いていなければ初期値。</summary>
    public int ThicknessAt(int dx, int dy) => CellAt(dx, dy) is { } cell ? ThicknessOf(cell) : CustomTileShape.DefaultThickness;

    /// <summary>そのマスを描いたレイヤーの番号。描いていなければ null。</summary>
    public int? LayerAt(int dx, int dy) => CellAt(dx, dy) is { } cell ? LayerOf(cell) : null;

    /// <summary>マスの形を mask、線の太さを 1/thickness マス、レイヤーを layer にする (mask が 0 なら消す)。変わったら true。</summary>
    public bool SetCell(int dx, int dy, int mask, int thickness, int layer)
    {
        mask &= ValueMask;
        thickness = Math.Clamp(thickness, CustomTileShape.MinThickness, CustomTileShape.MaxThickness);
        var cell = mask == 0 ? null : MakeCell(dx, dy, mask, thickness, layer);
        if (CellAt(dx, dy) is { } old ? cell is not null && old.SequenceEqual(cell) : cell is null)
        {
            return false;
        }
        Cells.RemoveAll(cell => cell[0] == dx && cell[1] == dy);
        if (cell is not null)
        {
            Cells.Add(cell);
        }
        return true;
    }

    /// <summary>
    /// オーバーレイに渡す形 (マスのあるレイヤーごと、Layers の順に重ねる)。low が null ならすべて (Gマスが下)、
    /// true なら Gマスを広げたマスだけ、false ならそれ以外だけ (グループをまたいで Gマスを先に描くため)。
    /// </summary>
    public IEnumerable<CustomTiles> ToCustomTiles(int overallOpacity = AppConfig.DefaultOverlayOpacity, bool? low = null) =>
        ToTiles(overallOpacity, low, 0, 0);

    /// <summary>マップに固定したマスを、そのマップの座標で描くオーバーレイに渡す形 (Origin の分ずらす)。low は ToCustomTiles と同じ。</summary>
    public IEnumerable<CustomTiles> ToMapTiles(int overallOpacity = AppConfig.DefaultOverlayOpacity, bool? low = null) =>
        ToTiles(overallOpacity, low, OriginCoordinate.X, OriginCoordinate.Y);

    private IEnumerable<CustomTiles> ToTiles(int overallOpacity, bool? low, int dx, int dy)
    {
        var opacity = GetOpacity(overallOpacity);
        var areas = low == false ? [] : Areas.GroupBy(AreaLayerOf).OrderBy(areas => areas.Key)
            .Select(areas => ToCustomTiles(
                areas.SelectMany(area => AreaCells(area[0] + dx, area[1] + dy)).Distinct().Select(cell => new[] { cell.X, cell.Y }).ToList(),
                Layers[areas.Key], opacity, Name) with { Low = true });
        var cells = low == true ? [] : Cells.GroupBy(LayerOf).OrderBy(cells => cells.Key)
            .Select(cells => ToCustomTiles(cells.Select(cell => Shift(cell, dx, dy)).ToList(), Layers[cells.Key], opacity, Name));
        return areas.Concat(cells);
    }

    private static int[] Shift(int[] cell, int dx, int dy)
    {
        var shifted = (int[])cell.Clone();
        shifted[0] += dx;
        shifted[1] += dy;
        return shifted;
    }

    /// <summary>マップ mapName の座標 origin に固定する (マスの相対位置はそのまま。origin のキャラクターから見た位置になる)。</summary>
    public void FixToMap(string mapName, Parsing.GameCoordinate origin)
    {
        MapName = mapName;
        Origin = [origin.X, origin.Y];
    }

    /// <summary>マップに固定したマスの位置を変えずに、基準の座標を origin にする (マスの相対位置をずらす)。</summary>
    public void Rebase(Parsing.GameCoordinate origin)
    {
        var old = OriginCoordinate;
        Cells = Cells.Select(cell => Shift(cell, old.X - origin.X, old.Y - origin.Y)).ToList();
        Areas = Areas.Select(area => Shift(area, old.X - origin.X, old.Y - origin.Y)).ToList();
        Origin = [origin.X, origin.Y];
    }

    /// <summary>cells をレイヤー layer の描き方で描く、オーバーレイに渡す形 (不透明度 opacity (%))。</summary>
    public static CustomTiles ToCustomTiles(IReadOnlyList<int[]> cells, CustomTileLayer layer, int opacity = AppConfig.DefaultOverlayOpacity, string name = "")
    {
        // どのマスの形・太さも割り切れる単位 (Division の倍数)。マス (dx, dy) は (dx * division, dy * division) から division 四方。
        var division = cells.Aggregate(Division, (division, cell) => CustomTileShape.Lcm(division, CustomTileShape.Fill(MaskOf(cell), ThicknessOf(cell)).Division));
        var fill = new TileRects
        {
            Name = name,
            Rects = cells.SelectMany(cell =>
            {
                var (cellDivision, rects) = CustomTileShape.Fill(MaskOf(cell), ThicknessOf(cell));
                var scale = division / cellDivision;
                return rects.Select(rect => new[] { cell[0] * division + rect[0] * scale, cell[1] * division + rect[1] * scale, rect[2] * scale, rect[3] * scale });
            }).ToArray(),
        };
        var frames = new TileRects
        {
            Name = name,
            Rects = cells.Where(cell => (MaskOf(cell) & FrameFlag) != 0).Select(cell => new[] { cell[0], cell[1], 1, 1 }).ToArray(),
        };
        return new(fill, frames, layer.GetColor(opacity), division, layer.Dotted, layer.Outlined);
    }

    /// <summary>
    /// キャラクターのいるマスを (0, 0) とした格子から、1/division マス単位の格子を作る (CustomTiles.Fill の長方形を描く用)。
    /// 小さいマスの格子点 division * g は、元の格子点 g と同じ位置に来る。
    /// </summary>
    public static IsoGrid SubGrid(IsoGrid grid, int division = Division) => new(
        new PointF(grid.PlayerCenter.X, (float)(grid.PlayerCenter.Y - grid.TileHeight / 2 + grid.TileHeight / division / 2)),
        new Parsing.GameCoordinate(0, 0), grid.TileWidth / division, grid.TileHeight / division);

    /// <summary>
    /// 読み込んだ値を直す (前の形式のペン・レイヤーごとのマス・色をレイヤーとマスに移し、壊れたマス・重複を除き、不透明度を範囲に収める)。
    /// 前の形式のレイヤーで同じ位置に重ねていたマスは、上 (後) のレイヤーのものを残す。
    /// </summary>
    public void Normalize()
    {
        Name ??= "";
        if (string.IsNullOrEmpty(MapName))
        {
            MapName = null;
            Origin = null;
        }
        else if (Origin is not [_, _])
        {
            Origin = [0, 0];
        }
        Opacity = Math.Clamp(Opacity, AppConfig.MinOverlayOpacity, AppConfig.MaxOverlayOpacity);
        var source = (Layers is { Count: > 0 } ? Layers : LegacyPens ?? []).OfType<CustomTileLayer>().ToList();
        var cells = (Cells ?? []).Where(cell => cell is { Length: >= 2 and <= 5 }).ToList();
        for (var index = 0; index < source.Count; index++)
        {
            // 前の形式: レイヤーごとに持っていたマスを、そのレイヤーの番号にしてグループのマスに移す。
            var layer = index;
            cells.AddRange((source[index].Cells ?? []).Where(cell => cell is { Length: >= 2 and <= 4 })
                .Select(cell => new[] { cell[0], cell[1], MaskOf(cell), ThicknessOf(cell), layer }));
        }
        var layers = source.Select(layer => layer.Normalized()).ToList();
        if (layers.Count == 0)
        {
            layers.Add(new CustomTileLayer { Color = LegacyColor ?? CustomTileLayer.ToHtml(DefaultColor) }.Normalized());
        }
        LegacyPens = null;
        LegacyColor = null;
        Cells = cells.Where(cell => MaskOf(cell) != 0 && LayerOf(cell) >= 0 && LayerOf(cell) < layers.Count)
            .Reverse().DistinctBy(cell => (cell[0], cell[1])).Reverse().Select(cell => WithLayer(cell, LayerOf(cell))).ToList();
        Areas = (Areas ?? []).Where(area => area is { Length: >= 2 and <= 3 } && AreaLayerOf(area) >= 0 && AreaLayerOf(area) < layers.Count)
            .Reverse().DistinctBy(area => (area[0], area[1])).Reverse().Select(area => MakeArea(area[0], area[1], AreaLayerOf(area))).ToList();
        Layers = layers;
    }
}
