using System.Text.Json.Serialization;
using DrawingColor = System.Drawing.Color;

namespace gGameMapOverlay.Overlay;

/// <summary>オーバーレイに渡す 1 レイヤー分のマス。</summary>
/// <param name="Fill">塗る長方形 (相対位置、1/Division マス単位。CustomTileGroup.SubGrid の格子で描く)。</param>
/// <param name="Frames">枠を描くマス (相対位置、1 マス単位)。</param>
public sealed record CustomTiles(TileRects Fill, TileRects Frames, DrawingColor Color, int Division = CustomTileGroup.Division, bool Dotted = false);

/// <summary>
/// 自分で描くマス (カスタム) の 1 グループ。マスはキャラクターのいるマスからの相対位置で持ち、
/// プレイヤーの枠と同じく画面に固定して描く (歩いてもスライドしない)。
/// マスと色はレイヤー (CustomTileLayer) ごとに持ち、表示・不透明度はグループでまとめて決める。
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

    public string Name { get; set; } = "";

    /// <summary>不透明度 (%)。AppConfig.MinOverlayOpacity〜MaxOverlayOpacity。OwnOpacity のときだけ使う。</summary>
    public int Opacity { get; set; } = AppConfig.DefaultOverlayOpacity;

    /// <summary>個別の不透明度 (Opacity) を使う。false なら全体の不透明度 (AppConfig.OverlayOpacity) を使う。</summary>
    public bool OwnOpacity { get; set; } = true;

    public bool Shown { get; set; } = true;

    /// <summary>レイヤー (この順に重ねて描く)。Normalize の後は 1 つ以上ある。</summary>
    public List<CustomTileLayer> Layers { get; set; } = [];

    /// <summary>レイヤーがなかったころの設定の色。読み込んだら Normalize でレイヤーに移す。</summary>
    [JsonPropertyName("color"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LegacyColor { get; set; }

    /// <summary>レイヤーがなかったころの設定のマス。読み込んだら Normalize でレイヤーに移す。</summary>
    [JsonPropertyName("cells"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int[]>? LegacyCells { get; set; }

    /// <summary>全部のレイヤーのマスの数。</summary>
    [JsonIgnore]
    public int CellCount => Layers.Sum(layer => layer.Cells.Count);

    /// <summary>変えても元に影響しない写し (レイヤーも複製する)。</summary>
    public CustomTileGroup Clone()
    {
        var copy = (CustomTileGroup)MemberwiseClone();
        copy.Layers = Layers.Select(layer => layer.Clone()).ToList();
        copy.LegacyCells = LegacyCells?.Select(cell => (int[])cell.Clone()).ToList();
        return copy;
    }

    /// <summary>使う不透明度 (%)。個別でなければ overallOpacity (全体の不透明度)。</summary>
    public int GetOpacity(int overallOpacity = AppConfig.DefaultOverlayOpacity) =>
        Math.Clamp(OwnOpacity ? Opacity : overallOpacity, AppConfig.MinOverlayOpacity, AppConfig.MaxOverlayOpacity);

    /// <summary>オーバーレイに渡す形 (マスのあるレイヤーごと、重ねる順)。</summary>
    public IEnumerable<CustomTiles> ToCustomTiles(int overallOpacity = AppConfig.DefaultOverlayOpacity) =>
        Layers.Where(layer => layer.Cells.Count > 0).Select(layer => layer.ToCustomTiles(GetOpacity(overallOpacity), Name));

    /// <summary>
    /// キャラクターのいるマスを (0, 0) とした格子から、1/division マス単位の格子を作る (CustomTileLayer.ToTileRects の長方形を描く用)。
    /// 小さいマスの格子点 division * g は、元の格子点 g と同じ位置に来る。
    /// </summary>
    public static IsoGrid SubGrid(IsoGrid grid, int division = Division) => new(
        new PointF(grid.PlayerCenter.X, (float)(grid.PlayerCenter.Y - grid.TileHeight / 2 + grid.TileHeight / division / 2)),
        new Parsing.GameCoordinate(0, 0), grid.TileWidth / division, grid.TileHeight / division);

    /// <summary>読み込んだ値を直す (前の形式の色・マスをレイヤーに移し、壊れたマス・重複を除き、不透明度を範囲に収める)。</summary>
    public void Normalize()
    {
        Name ??= "";
        Opacity = Math.Clamp(Opacity, AppConfig.MinOverlayOpacity, AppConfig.MaxOverlayOpacity);
        Layers = (Layers ?? []).OfType<CustomTileLayer>().ToList();
        if (Layers.Count == 0 || LegacyCells is not null)
        {
            Layers.Insert(0, new CustomTileLayer { Color = LegacyColor ?? CustomTileLayer.ToHtml(DefaultColor), Cells = LegacyCells ?? [] });
        }
        LegacyColor = null;
        LegacyCells = null;
        foreach (var layer in Layers)
        {
            layer.Normalize();
        }
    }
}
