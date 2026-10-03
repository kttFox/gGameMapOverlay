using DrawingColor = System.Drawing.Color;

namespace gGameMapOverlay.Overlay;

/// <summary>
/// カスタムのグループの中の 1 レイヤー (1 色分のマス)。グループの中では、レイヤーを順に重ねて描く (後のレイヤーが上)。
/// 1 マスは縦横 CustomTileGroup.Division 等分した小さいマス (1/3 マス) の集まりで、どこを塗るかを形 (CustomTileShape.Mask) で持つ。
/// 線の太さ (1/thickness マス) もマスごとに持つ (CustomTileShape.Fill)。
/// </summary>
public sealed class CustomTileLayer
{
    private const int Division = CustomTileGroup.Division;
    private const int FullMask = CustomTileGroup.FullMask;
    private const int FrameFlag = CustomTileGroup.FrameFlag;
    private const int ValueMask = FullMask | FrameFlag;

    /// <summary>"#RRGGBB"。読めない値なら CustomTileGroup.DefaultColor。</summary>
    public string Color { get; set; } = ToHtml(CustomTileGroup.DefaultColor);

    /// <summary>マスを 1 画素おきのドットで塗るか。</summary>
    public bool Dotted { get; set; }

    /// <summary>
    /// キャラクターのいるマスからの相対位置と形の一覧。[dx, dy] は 1 マス全部、[dx, dy, mask] は形 (CustomTileShape.Mask) の部分だけ
    /// (FrameFlag があれば枠も描く)、[dx, dy, mask, thickness] は線の太さを 1/thickness マスにした形
    /// (太さ 1/3 (CustomTileShape.DefaultThickness) と、太さで変わらない形 (1 マス全部・枠だけ) は thickness を書かない)。
    /// </summary>
    public List<int[]> Cells { get; set; } = [];

    /// <summary>変えても元に影響しない写し (マスの一覧も複製する)。</summary>
    public CustomTileLayer Clone()
    {
        var copy = (CustomTileLayer)MemberwiseClone();
        copy.Cells = Cells.Select(cell => (int[])cell.Clone()).ToList();
        return copy;
    }

    /// <summary>色 (アルファは不透明度 opacity (%) から決める)。</summary>
    public DrawingColor GetColor(int opacity = AppConfig.DefaultOverlayOpacity)
    {
        var color = CustomTileGroup.DefaultColor;
        try
        {
            color = ColorTranslator.FromHtml(Color);
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            // 読めない値は初期値のまま
        }
        var alpha = (int)Math.Round(255 * Math.Clamp(opacity, 0, 100) / 100.0);
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
    /// CustomTileGroup.SubGrid(grid, FillDivision) の格子で描く。
    /// </summary>
    public TileRects ToTileRects(string name = "") => ToTileRects(FillDivision(), name);

    private TileRects ToTileRects(int division, string name) => new()
    {
        Name = name,
        Rects = Cells.SelectMany(cell =>
        {
            var (cellDivision, rects) = CustomTileShape.Fill(MaskOf(cell), ThicknessOf(cell));
            var scale = division / cellDivision;
            return rects.Select(rect => new[] { cell[0] * division + rect[0] * scale, cell[1] * division + rect[1] * scale, rect[2] * scale, rect[3] * scale });
        }).ToArray(),
    };

    /// <summary>枠を描くマス (相対位置、1 マス単位)。</summary>
    public TileRects ToFrameRects(string name = "") => new()
    {
        Name = name,
        Rects = Cells.Where(cell => (MaskOf(cell) & FrameFlag) != 0).Select(cell => new[] { cell[0], cell[1], 1, 1 }).ToArray(),
    };

    /// <summary>オーバーレイに渡す形 (不透明度 opacity (%))。</summary>
    public CustomTiles ToCustomTiles(int opacity = AppConfig.DefaultOverlayOpacity, string name = "")
    {
        var division = FillDivision();
        return new(ToTileRects(division, name), ToFrameRects(name), GetColor(opacity), division, Dotted);
    }

    /// <summary>読み込んだ値を直す (壊れたマス・重複を除く)。</summary>
    public void Normalize()
    {
        Color ??= ToHtml(CustomTileGroup.DefaultColor);
        Cells = (Cells ?? [])
            .Where(cell => cell is { Length: 2 or 3 or 4 } && MaskOf(cell) != 0)
            .DistinctBy(cell => (cell[0], cell[1]))
            .Select(cell => MakeCell(cell[0], cell[1], MaskOf(cell), ThicknessOf(cell)))
            .ToList();
    }

    internal static string ToHtml(DrawingColor color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}
