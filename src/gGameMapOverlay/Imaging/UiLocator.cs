using System.Drawing;

namespace gGameMapOverlay.Imaging;

/// <summary>
/// 画面上の UI の拡大率 (横・縦) と位置のずれ。領域 (等倍のゲーム座標) を画面上の位置に変換する。
/// gGame の UI は縦横同じ倍率で拡大されるが、手動調整などで縦横が違う変換も表せるようにしておく。
/// </summary>
public readonly record struct UiTransform(double ScaleX, double ScaleY, int OffsetX = 0, int OffsetY = 0)
{
    /// <summary>縦横同じ倍率。</summary>
    public UiTransform(double scale) : this(scale, scale)
    {
    }

    public static readonly UiTransform Identity = new(1.0);

    /// <summary>
    /// 画面の高さから、gGame が UI を描く倍率を求める。高さが 1080 より大きいと「高さ ÷ 1080」倍、1080 以下は等倍
    /// (1920x1280 で 1.185 倍、1600x1200 で 1.111 倍、1920x1080 と 1366x768 では等倍を実機で確認)。
    /// </summary>
    public static UiTransform ForScreen(int screenHeight) =>
        screenHeight > 1080 ? new UiTransform(Math.Round(screenHeight / 1080.0, 3)) : Identity;

    public bool IsUniform => Math.Abs(ScaleX - ScaleY) < 0.0005;

    public bool IsIdentity => Math.Abs(ScaleX - 1.0) < 0.0005 && Math.Abs(ScaleY - 1.0) < 0.0005 && OffsetX == 0 && OffsetY == 0;

    /// <summary>等倍の座標の矩形を画面上の矩形にする。端数は外側に広げる (拡大しても文字を切り落とさないように)。</summary>
    public Rectangle ToScreen(int left, int top, int right, int bottom) => Rectangle.FromLTRB(
        (int)Math.Floor(left * ScaleX) + OffsetX, (int)Math.Floor(top * ScaleY) + OffsetY,
        (int)Math.Ceiling(right * ScaleX) + OffsetX, (int)Math.Ceiling(bottom * ScaleY) + OffsetY);

    /// <summary>画面上の矩形を等倍の座標に戻す。</summary>
    public (int Left, int Top, int Right, int Bottom) FromScreen(Rectangle rect) => (
        (int)Math.Round((rect.Left - OffsetX) / ScaleX, MidpointRounding.AwayFromZero),
        (int)Math.Round((rect.Top - OffsetY) / ScaleY, MidpointRounding.AwayFromZero),
        (int)Math.Round((rect.Right - OffsetX) / ScaleX, MidpointRounding.AwayFromZero),
        (int)Math.Round((rect.Bottom - OffsetY) / ScaleY, MidpointRounding.AwayFromZero));

    /// <summary>表示用 (例: "1.184"、縦横が違えば "横 1.186 × 縦 1.218")。</summary>
    public override string ToString() => IsUniform ? $"{ScaleX:0.###}" : $"横 {ScaleX:0.###} × 縦 {ScaleY:0.###}";
}

/// <summary>
/// ゲーム画面からマップ名欄・座標欄を探し、UI の拡大率と位置を推定する。画像モード用
/// (ライブ読み取りでは、撮影したモニターの解像度が分かるので UiTransform.ForScreen で計算する)。
/// 欄は「黒い内側を細い明るい枠線が囲む」形なので、倍率と位置を少しずつ変えながら
/// 「枠線の位置が明るく、そのすぐ内側が暗い」度合いが最も高くなるところを探す。
/// gGame は画面の高さが 1080 より大きいと、UI を「画面の高さ ÷ 1080」倍に拡大して描く
/// (1920x1280 で 1.18 倍、1600x1200 で 1.114 倍、1920x1080 と 1366x768 では等倍)。
/// 画像からは撮影したモニターの解像度が分からないので、画面から直接測る。
/// 倍率は縦横共通で探す (縦は枠線の間隔が短く、別々に探すと数 % ぶれるため)。
/// </summary>
public static class UiLocator
{
    // 等倍のときの枠線の位置 (クライアント座標)。欄の内側は AppConfig の初期値の領域。
    private static readonly (int X0, int Y0, int X1, int Y1)[] Frames = [(65, 2, 350, 23), (65, 26, 140, 47)];

    private const double MinScale = 0.9;
    private const double MaxScale = 2.2;

    /// <summary>これ未満の点数は欄が見えていない (読み込み中の画面など) とみなす。等倍の実画面では 350 前後。</summary>
    private const double MinScore = 150;

    /// <summary>等倍とのずれがこの範囲なら等倍とみなす (枠線を 3px の幅で探すので、1〜2px の揺れが出る)。</summary>
    private const double IdentityScaleTolerance = 0.02;
    private const int IdentityOffsetTolerance = 3;

    /// <summary>UI の拡大率と位置。欄が見つからなければ null。</summary>
    public static UiTransform? Locate(BgrImage client)
    {
        // 探す範囲 (左上) だけ、各画素の明るさ (RGB の最大値) にしておく。
        var width = Math.Min(client.Width, (int)(Frames.Max(f => f.X1) * MaxScale) + 8);
        var height = Math.Min(client.Height, (int)(Frames.Max(f => f.Y1) * MaxScale) + 8);
        var bright = new byte[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var i = (y * client.Width + x) * 3;
                bright[y * width + x] = Math.Max(client.Pixels[i], Math.Max(client.Pixels[i + 1], client.Pixels[i + 2]));
            }
        }

        // 1. 縦横同じ倍率・原点固定で、倍率を粗く探す
        var best = (Score: double.MinValue, ScaleX: 1.0, ScaleY: 1.0, X: 0, Y: 0);
        for (var scale = MinScale; scale <= MaxScale + 1e-9; scale += 0.01)
        {
            Consider(ref best, bright, width, height, scale, scale, 0, 0);
        }
        // 2. 倍率を細かく合わせながら、原点のずれも少し探す
        var coarse = best.ScaleX;
        for (var scale = coarse - 0.02; scale <= coarse + 0.02 + 1e-9; scale += 0.002)
        {
            for (var dx = -3; dx <= 3; dx++)
            {
                for (var dy = -3; dy <= 3; dy++)
                {
                    Consider(ref best, bright, width, height, scale, scale, dx, dy);
                }
            }
        }

        if (best.Score < MinScore)
        {
            return null;
        }
        if (Math.Abs(best.ScaleX - 1.0) <= IdentityScaleTolerance
            && Math.Abs(best.X) <= IdentityOffsetTolerance && Math.Abs(best.Y) <= IdentityOffsetTolerance)
        {
            return UiTransform.Identity;
        }
        var scaleFound = Math.Round(best.ScaleX, 3);
        return new UiTransform(scaleFound, scaleFound, best.X, best.Y);
    }

    private static void Consider(
        ref (double Score, double ScaleX, double ScaleY, int X, int Y) best,
        byte[] bright, int width, int height, double scaleX, double scaleY, int offsetX, int offsetY)
    {
        var score = Score(bright, width, height, scaleX, scaleY, offsetX, offsetY);
        if (score > best.Score)
        {
            best = (score, scaleX, scaleY, offsetX, offsetY);
        }
    }

    /// <summary>枠線上が明るく、その少し内側が暗いほど高い点数。欄が画面からはみ出すなら最低点。</summary>
    internal static double Score(byte[] bright, int width, int height, double scaleX, double scaleY, int offsetX, int offsetY)
    {
        var total = 0.0;
        var insetX = Math.Max(2.0, 2.0 * scaleX);
        var insetY = Math.Max(2.0, 2.0 * scaleY);
        foreach (var (fx0, fy0, fx1, fy1) in Frames)
        {
            double x0 = offsetX + fx0 * scaleX, y0 = offsetY + fy0 * scaleY, x1 = offsetX + fx1 * scaleX, y1 = offsetY + fy1 * scaleY;
            int left = (int)(x0 + insetX), right = (int)(x1 - insetX), top = (int)(y0 + insetY), bottom = (int)(y1 - insetY);
            if (x0 < 1 || y0 < 1 || x1 + 2 >= width || y1 + 2 >= height || right - left < 4 || bottom - top < 2)
            {
                return double.MinValue;
            }
            var line = (Row(bright, width, Round(y0), left, right, true) + Row(bright, width, Round(y1), left, right, true)
                + Column(bright, width, Round(x0), top, bottom, true) + Column(bright, width, Round(x1), top, bottom, true)) / 4;
            // 右端の内側は文字がかかることがあるので使わない。
            var inner = (Row(bright, width, Round(y0 + insetY), left, right, false) + Row(bright, width, Round(y1 - insetY), left, right, false)
                + Column(bright, width, Round(x0 + insetX), top, bottom, false)) / 3;
            total += line - inner;
        }
        return total;
    }

    private static int Round(double value) => (int)Math.Round(value);

    /// <summary>行 y の [left, right) の平均の明るさ。spread なら上下 1px の最大値をとる (拡大の補間でぼやけた線も拾う)。</summary>
    private static double Row(byte[] bright, int width, int y, int left, int right, bool spread)
    {
        long sum = 0;
        for (var x = left; x < right; x++)
        {
            var value = bright[y * width + x];
            if (spread)
            {
                value = Math.Max(value, Math.Max(bright[Math.Max(0, y - 1) * width + x], bright[(y + 1) * width + x]));
            }
            sum += value;
        }
        return sum / (double)(right - left);
    }

    private static double Column(byte[] bright, int width, int x, int top, int bottom, bool spread)
    {
        long sum = 0;
        for (var y = top; y < bottom; y++)
        {
            var value = bright[y * width + x];
            if (spread)
            {
                value = Math.Max(value, Math.Max(bright[y * width + Math.Max(0, x - 1)], bright[y * width + x + 1]));
            }
            sum += value;
        }
        return sum / (double)(bottom - top);
    }
}
