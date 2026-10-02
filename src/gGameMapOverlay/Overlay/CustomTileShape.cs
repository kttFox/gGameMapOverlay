namespace gGameMapOverlay.Overlay;

/// <summary>
/// カスタムのマスの形。1 マスを縦横 3 等分した小さいマス (1/3 マス) のどれを塗るかを、ビットで持つ
/// (小さいマス (sx, sy) が 1 &lt;&lt; (sy * 3 + sx))。sx はマップの x の向き、sy は y の向き。
/// 上下左右は、マスを x が右・y が下の正方形として見たときの向き (画面では、上 = 右上、右 = 右下、下 = 左下、左 = 左上)。
/// 縦・横は、真ん中の小さいマスから辺の真ん中へ伸ばした線で、隣のマスの同じ形とつながる。
/// 丁字 (上など) は、その辺に沿った 1 列と、そこから反対の辺へ伸ばした真ん中の 1 列 (上なら上の 1 列と真ん中の縦の 1 列)。
/// 角 (左上など) は、その角で交わる 2 辺に沿った L 字 (左上なら上の 1 列と左の 1 列)。
/// 枠はプレイヤーの枠と同じ線 (CustomTileGroup.FrameFlag)、太枠は真ん中以外の小さいマスを塗る。
/// 線の太さは 1/thickness マスにできる (Fill)。3 つの列は、端の列が縁から 1/thickness、真ん中の列がマスの中央の 1/thickness の幅になり、
/// 隣り合う小さいマスを両方塗るなら、その間 (太さ 1/3 より細いときにできる隙間) も塗ってつなげる。
/// </summary>
/// <param name="Mask">0 なら決まった形ではなく、小さいマスを 1 つずつ描く (Free)。</param>
public sealed record CustomTileShape(string Name, int Mask)
{
    private const int N = CustomTileGroup.Division;

    /// <summary>線の太さ (1/thickness マス) の初期値。小さいマスがちょうど 3 等分になる。</summary>
    public const int DefaultThickness = N;

    /// <summary>選べる線の太さ (1/MinThickness〜1/MaxThickness マス)。</summary>
    public const int MinThickness = 2;
    public const int MaxThickness = 16;

    private static int Bit(int sx, int sy) => 1 << (sy * N + sx);

    private static readonly int Center = Bit(1, 1);
    private static readonly int Up = Bit(1, 0);
    private static readonly int Down = Bit(1, 2);
    private static readonly int Left = Bit(0, 1);
    private static readonly int Right = Bit(2, 1);

    private static CustomTileShape Line(string name, params int[] arms) => new(name, arms.Aggregate(Center, (mask, arm) => mask | arm));

    /// <summary>辺に沿った 1 列 (row なら sy = index の横の列、そうでなければ sx = index の縦の列)。</summary>
    private static int Edge(bool row, int index) => Enumerable.Range(0, N).Aggregate(0, (mask, i) => mask | (row ? Bit(i, index) : Bit(index, i)));

    /// <summary>角の L 字。top なら上の列 (でなければ下の列)、left なら左の列 (でなければ右の列)。</summary>
    private static CustomTileShape Corner(string name, bool top, bool left) => new(name, Edge(true, top ? 0 : N - 1) | Edge(false, left ? 0 : N - 1));

    public static readonly CustomTileShape Full = new("1 マス", CustomTileGroup.FullMask);

    /// <summary>小さいマスを 1 つずつ描く (左クリックで足す、右クリックで消す)。</summary>
    public static readonly CustomTileShape Free = new("小さいマスずつ", 0);

    /// <summary>選べる形 (編集画面に並べる順)。</summary>
    public static readonly IReadOnlyList<CustomTileShape> All =
    [
        Full,
        Free,
        Line("点"),
        Line("縦", Up, Down),
        Line("横", Left, Right),
        Corner("左上", top: true, left: true),
        Corner("右上", top: true, left: false),
        Corner("右下", top: false, left: false),
        Corner("左下", top: false, left: true),
        new("丁字 (上)", Edge(true, 0) | Edge(false, 1)),
        new("丁字 (右)", Edge(false, N - 1) | Edge(true, 1)),
        new("丁字 (下)", Edge(true, N - 1) | Edge(false, 1)),
        new("丁字 (左)", Edge(false, 0) | Edge(true, 1)),
        new("枠", CustomTileGroup.FrameFlag),
        new("太枠", CustomTileGroup.FullMask & ~Center),
    ];

    /// <summary>形の小さいマス (sx, sy) の一覧。</summary>
    public static IEnumerable<(int X, int Y)> Parts(int mask)
    {
        for (var sy = 0; sy < N; sy++)
        {
            for (var sx = 0; sx < N; sx++)
            {
                if ((mask & Bit(sx, sy)) != 0)
                {
                    yield return (sx, sy);
                }
            }
        }
    }

    /// <summary>小さいマス (sx, sy) だけの形。</summary>
    public static int PartMask(int sx, int sy) => Bit(sx, sy);

    // 形と太さごとの Fill の結果 (オーバーレイの描画と編集画面で何度も使う)。
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<(int Mask, int Thickness), (int Division, int[][] Rects)> fills = new();

    /// <summary>
    /// 形 mask (FullMask の部分) を太さ 1/thickness で塗る長方形 [x, y, 幅, 高さ] の一覧 (1 マスを縦横 Division 等分した単位、重ならない)。
    /// Division はできるだけ小さくする (1 マス全部なら 1)。
    /// </summary>
    public static (int Division, int[][] Rects) Fill(int mask, int thickness)
    {
        mask &= CustomTileGroup.FullMask;
        thickness = Math.Clamp(thickness, MinThickness, MaxThickness);
        return fills.GetOrAdd((mask, thickness), key => Rasterize(key.Mask, key.Thickness));
    }

    private static (int Division, int[][] Rects) Rasterize(int mask, int thickness)
    {
        if (mask == 0)
        {
            return (1, []);
        }
        // 真ん中の列は中央の 1/thickness なので、thickness が偶数なら端が 1/(2 × thickness) 単位になる。
        var unit = thickness % 2 == 0 ? 2 : 1;
        var division = thickness * unit;
        (int From, int To)[] bands = [(0, unit), ((division - unit) / 2, (division + unit) / 2), (division - unit, division)];
        // 位置 u を塗るのに必要な列の組の候補。列の中なら {その列} (太さ 1/2 なら列が重なるので複数)、列の間の隙間なら {両側の列}。
        int[][] Needs(int u)
        {
            var inside = Enumerable.Range(0, N).Where(band => bands[band].From <= u && u < bands[band].To).Select(band => new[] { band }).ToArray();
            if (inside.Length > 0)
            {
                return inside;
            }
            var before = Enumerable.Range(0, N - 1).Last(band => bands[band].To <= u);
            return [[before, before + 1]];
        }
        var needs = Enumerable.Range(0, division).Select(Needs).ToArray();
        bool Filled(int ux, int uy) => needs[ux].Any(xs => needs[uy].Any(ys => xs.All(sx => ys.All(sy => (mask & Bit(sx, sy)) != 0))));

        // 行ごとに塗る範囲を区切り、上の行と同じ範囲なら縦につなげる。
        var rects = new List<int[]>();
        var open = new Dictionary<(int From, int To), int[]>();
        for (var uy = 0; uy < division; uy++)
        {
            var runs = new List<(int From, int To)>();
            for (var ux = 0; ux < division; ux++)
            {
                if (!Filled(ux, uy))
                {
                    continue;
                }
                var from = ux;
                while (ux + 1 < division && Filled(ux + 1, uy))
                {
                    ux++;
                }
                runs.Add((from, ux + 1));
            }
            var next = new Dictionary<(int From, int To), int[]>();
            foreach (var run in runs)
            {
                if (open.Remove(run, out var rect))
                {
                    rect[3]++;
                }
                else
                {
                    rect = [run.From, uy, run.To - run.From, 1];
                    rects.Add(rect);
                }
                next[run] = rect;
            }
            open = next;
        }
        // 全部の長方形の座標が割り切れる分だけ、等分数を小さくする。
        var common = rects.SelectMany(rect => rect).Aggregate(division, Gcd);
        return (division / common, rects.Select(rect => rect.Select(value => value / common).ToArray()).ToArray());
    }

    private static int Gcd(int a, int b) => b == 0 ? Math.Abs(a) : Gcd(b, a % b);

    internal static int Lcm(int a, int b) => a / Gcd(a, b) * b;
}
