using DrawingColor = System.Drawing.Color;

namespace gGameMapOverlay.Overlay;

/// <summary>オーバーレイに渡す 1 グループ分のマス。</summary>
/// <param name="Fill">塗る長方形 (相対位置、1/Division マス単位。CustomTileGroup.SubGrid の格子で描く)。</param>
/// <param name="Frames">枠を描くマス (相対位置、1 マス単位)。</param>
public sealed record CustomTiles(TileRects Fill, TileRects Frames, DrawingColor Color, int Division = CustomTileGroup.Division);

/// <summary>
/// 自分で描くマス (カスタム) の 1 グループ。マスはキャラクターのいるマスからの相対位置で持ち、
/// プレイヤーの枠と同じく画面に固定して描く (歩いてもスライドしない)。
/// 1 マスは縦横 Division 等分した小さいマス (1/3 マス) の集まりで、どこを塗るかを形 (CustomTileShape.Mask) で持つ。
/// 線の太さ (1/thickness マス) もマスごとに持つ (CustomTileShape.Fill)。
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

    public string Name { get; set; } = "";

    /// <summary>"#RRGGBB"。読めない値なら DefaultColor。</summary>
    public string Color { get; set; } = ToHtml(DefaultColor);

    /// <summary>不透明度 (%)。AppConfig.MinOverlayOpacity〜MaxOverlayOpacity。OwnOpacity のときだけ使う。</summary>
    public int Opacity { get; set; } = AppConfig.DefaultOverlayOpacity;

    /// <summary>個別の不透明度 (Opacity) を使う。false なら全体の不透明度 (AppConfig.OverlayOpacity) を使う。</summary>
    public bool OwnOpacity { get; set; } = true;

    public bool Shown { get; set; } = true;

    /// <summary>
    /// キャラクターのいるマスからの相対位置と形の一覧。[dx, dy] は 1 マス全部、[dx, dy, mask] は形 (CustomTileShape.Mask) の部分だけ
    /// (FrameFlag があれば枠も描く)、[dx, dy, mask, thickness] は線の太さを 1/thickness マスにした形
    /// (太さ 1/3 (CustomTileShape.DefaultThickness) と、太さで変わらない形 (1 マス全部・枠だけ) は thickness を書かない)。
    /// </summary>
    public List<int[]> Cells { get; set; } = [];

    /// <summary>使う不透明度 (%)。個別でなければ overallOpacity (全体の不透明度)。</summary>
    public int GetOpacity(int overallOpacity = AppConfig.DefaultOverlayOpacity) =>
        Math.Clamp(OwnOpacity ? Opacity : overallOpacity, AppConfig.MinOverlayOpacity, AppConfig.MaxOverlayOpacity);

    /// <summary>色 (アルファは不透明度 (GetOpacity) から決める)。</summary>
    public DrawingColor GetColor(int overallOpacity = AppConfig.DefaultOverlayOpacity)
    {
        var color = DefaultColor;
        try
        {
            color = ColorTranslator.FromHtml(Color);
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            // 読めない値は初期値のまま
        }
        var alpha = (int)Math.Round(255 * GetOpacity(overallOpacity) / 100.0);
        return DrawingColor.FromArgb(alpha, color.R, color.G, color.B);
    }

    public void SetColor(DrawingColor color) => Color = ToHtml(color);

    private static int MaskOf(int[] cell) => cell.Length >= 3 ? cell[2] & ValueMask : FullMask;

    private static int ThicknessOf(int[] cell) =>
        cell.Length >= 4 ? Math.Clamp(cell[3], CustomTileShape.MinThickness, CustomTileShape.MaxThickness) : CustomTileShape.DefaultThickness;

    /// <summary>太さで塗る部分が変わる形か (1 マス全部・枠だけは変わらない)。</summary>
    private static bool HasThickness(int mask) => (mask & FullMask) is not (0 or FullMask);

    /// <summary>保存する形 ([dx, dy]・[dx, dy, mask]・[dx, dy, mask, thickness] のうち一番短いもの)。</summary>
    private static int[] MakeCell(int dx, int dy, int mask, int thickness) =>
        mask == FullMask ? [dx, dy]
        : !HasThickness(mask) || thickness == CustomTileShape.DefaultThickness ? [dx, dy, mask]
        : [dx, dy, mask, thickness];

    private int[]? CellAt(int dx, int dy) => Cells.FirstOrDefault(cell => cell[0] == dx && cell[1] == dy);

    /// <summary>そのマスの形。描いていなければ 0。</summary>
    public int MaskAt(int dx, int dy) => CellAt(dx, dy) is { } cell ? MaskOf(cell) : 0;

    /// <summary>そのマスの線の太さ (1/thickness マス)。描いていなければ初期値。</summary>
    public int ThicknessAt(int dx, int dy) => CellAt(dx, dy) is { } cell ? ThicknessOf(cell) : CustomTileShape.DefaultThickness;

    /// <summary>マスの形を mask、線の太さを 1/thickness マスにする (mask が 0 なら消す)。変わったら true。</summary>
    public bool SetCell(int dx, int dy, int mask, int thickness = CustomTileShape.DefaultThickness)
    {
        mask &= ValueMask;
        thickness = Math.Clamp(thickness, CustomTileShape.MinThickness, CustomTileShape.MaxThickness);
        var cell = mask == 0 ? null : MakeCell(dx, dy, mask, thickness);
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

    /// <summary>ToTileRects の単位 (1 マスを縦横何等分するか)。どのマスの形・太さも割り切れる数 (Division の倍数)。</summary>
    public int FillDivision() =>
        Cells.Aggregate(Division, (division, cell) => CustomTileShape.Lcm(division, CustomTileShape.Fill(MaskOf(cell), ThicknessOf(cell)).Division));

    /// <summary>
    /// 描く用の長方形 (相対位置、1/FillDivision マス単位)。マス (dx, dy) は (dx * FillDivision, dy * FillDivision) から FillDivision 四方。
    /// SubGrid(grid, FillDivision) の格子で描く。
    /// </summary>
    public TileRects ToTileRects() => ToTileRects(FillDivision());

    private TileRects ToTileRects(int division) => new()
    {
        Name = Name,
        Rects = Cells.SelectMany(cell =>
        {
            var (cellDivision, rects) = CustomTileShape.Fill(MaskOf(cell), ThicknessOf(cell));
            var scale = division / cellDivision;
            return rects.Select(rect => new[] { cell[0] * division + rect[0] * scale, cell[1] * division + rect[1] * scale, rect[2] * scale, rect[3] * scale });
        }).ToArray(),
    };

    /// <summary>枠を描くマス (相対位置、1 マス単位)。</summary>
    public TileRects ToFrameRects() => new()
    {
        Name = Name,
        Rects = Cells.Where(cell => (MaskOf(cell) & FrameFlag) != 0).Select(cell => new[] { cell[0], cell[1], 1, 1 }).ToArray(),
    };

    /// <summary>オーバーレイに渡す形。</summary>
    public CustomTiles ToCustomTiles(int overallOpacity = AppConfig.DefaultOverlayOpacity)
    {
        var division = FillDivision();
        return new(ToTileRects(division), ToFrameRects(), GetColor(overallOpacity), division);
    }

    /// <summary>
    /// キャラクターのいるマスを (0, 0) とした格子から、1/division マス単位の格子を作る (ToTileRects の長方形を描く用)。
    /// 小さいマスの格子点 division * g は、元の格子点 g と同じ位置に来る。
    /// </summary>
    public static IsoGrid SubGrid(IsoGrid grid, int division = Division) => new(
        new PointF(grid.PlayerCenter.X, (float)(grid.PlayerCenter.Y - grid.TileHeight / 2 + grid.TileHeight / division / 2)),
        new Parsing.GameCoordinate(0, 0), grid.TileWidth / division, grid.TileHeight / division);

    /// <summary>読み込んだ値を直す (壊れたマス・重複を除き、不透明度を範囲に収める)。</summary>
    public void Normalize()
    {
        Name ??= "";
        Color ??= ToHtml(DefaultColor);
        Opacity = Math.Clamp(Opacity, AppConfig.MinOverlayOpacity, AppConfig.MaxOverlayOpacity);
        Cells = (Cells ?? [])
            .Where(cell => cell is { Length: 2 or 3 or 4 } && MaskOf(cell) != 0)
            .DistinctBy(cell => (cell[0], cell[1]))
            .Select(cell => MakeCell(cell[0], cell[1], MaskOf(cell), ThicknessOf(cell)))
            .ToList();
    }

    private static string ToHtml(DrawingColor color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}
