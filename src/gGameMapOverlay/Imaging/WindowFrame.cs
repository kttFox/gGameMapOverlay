using System.Drawing;

namespace gGameMapOverlay.Imaging;

/// <summary>
/// Alt+PrintScreen などでウィンドウ枠ごと撮ったスクリーンショットから、ゲームのクライアント領域を推定する。
/// 色 (アクセントカラー・非アクティブ時の白・ダークモード) には依存せず、
/// 「上端にほぼ単色の行が続く帯 (タイトルバー)」と「タイトルバーの横から下端まで同じ色で続く左右の列 (枠線)」の形だけで判定する。
/// Windows 10 (表示スケール 100〜175%、枠線 1px) と Windows 11 (上端に暗い縁が 2 行、枠線 2px) で確認済み。
/// </summary>
public static class WindowFrame
{
    private const int MinTitleHeight = 20;
    private const int MaxTitleHeight = 80; // 表示スケール 175% で 52px。200% 以上も見込んで余裕を持たせる
    private const int MaxTopEdgeRows = 3;
    private const int MaxBorderWidth = 4;
    private const int ColorTolerance = 32;

    // タイトルバーの行はアイコン・文字・ボタン以外が背景色なので、細い線を除けばほぼ単色になる。
    private const double TitleRowUniformity = 0.7;
    private const double BorderUniformity = 0.95;

    /// <summary>クライアント領域。ウィンドウ枠が見つからなければ画像全体。</summary>
    public static Rectangle DetectClientArea(BgrImage image)
    {
        var full = new Rectangle(0, 0, image.Width, image.Height);
        var titleBottom = TitleBarBottom(image);
        if (titleBottom == 0 || image.Height - titleBottom < MinTitleHeight)
        {
            return full;
        }
        // 最大化ウィンドウは左右・下の枠線がないので、あるときだけ除く。枠線は左右と下の 3 辺に同じ太さで付くので、
        // 通知などが重なって一部の辺が隠れていても、左右どちらかで見つかれば 3 辺とも除く。
        var border = Math.Max(BorderWidth(image, titleBottom, fromLeft: true), BorderWidth(image, titleBottom, fromLeft: false));
        return Rectangle.FromLTRB(border, titleBottom, image.Width - border, image.Height - border);
    }

    /// <summary>タイトルバーの下端 (クライアント領域の上端)。見つからなければ 0。</summary>
    private static int TitleBarBottom(BgrImage image)
    {
        // Windows 11 などでは、タイトルバーの上に色の違う縁の行がある。
        for (var edge = 0; edge <= MaxTopEdgeRows && edge < image.Height; edge++)
        {
            var reference = Pixel(image, image.Width / 2, edge);
            var rows = 0;
            while (edge + rows < image.Height && rows <= MaxTitleHeight
                && Near(Pixel(image, image.Width / 2, edge + rows), reference)
                && IsUniformRow(image, edge + rows))
            {
                rows++;
            }
            if (rows >= MinTitleHeight)
            {
                return rows <= MaxTitleHeight ? edge + rows : 0; // 長すぎる単色の帯は、読み込み画面など枠以外のもの
            }
        }
        return 0;
    }

    private static bool IsUniformRow(BgrImage image, int y)
    {
        var reference = Pixel(image, image.Width / 2, y);
        var matches = 0;
        for (var x = 0; x < image.Width; x++)
        {
            if (Near(Pixel(image, x, y), reference))
            {
                matches++;
            }
        }
        return matches >= image.Width * TitleRowUniformity;
    }

    /// <summary>左端 (または右端) から、上から下まで単色の列が何列続くか。</summary>
    private static int BorderWidth(BgrImage image, int titleBottom, bool fromLeft)
    {
        var width = 0;
        while (width < MaxBorderWidth && IsBorderColumn(image, fromLeft ? width : image.Width - 1 - width, titleBottom))
        {
            width++;
        }
        return width;
    }

    /// <summary>
    /// 枠線の列か。枠線はタイトルバーの横からウィンドウの下端まで同じ色で続くので、
    /// タイトルバーの範囲とその下の範囲の両方でほぼ単色であることを確かめる。
    /// 下の範囲だけを見ると、黒い背景などクライアント側の単色の列を枠線と取り違える。
    /// </summary>
    private static bool IsBorderColumn(BgrImage image, int x, int titleBottom)
    {
        var reference = Pixel(image, x, (titleBottom + image.Height) / 2);
        return UniformFraction(image, x, 0, titleBottom, reference) >= BorderUniformity
            && UniformFraction(image, x, titleBottom, image.Height, reference) >= BorderUniformity;
    }

    private static double UniformFraction(BgrImage image, int x, int top, int bottom, (byte B, byte G, byte R) reference)
    {
        var matches = 0;
        for (var y = top; y < bottom; y++)
        {
            if (Near(Pixel(image, x, y), reference))
            {
                matches++;
            }
        }
        return matches / (double)Math.Max(1, bottom - top);
    }

    private static (byte B, byte G, byte R) Pixel(BgrImage image, int x, int y)
    {
        var i = (y * image.Width + x) * 3;
        return (image.Pixels[i], image.Pixels[i + 1], image.Pixels[i + 2]);
    }

    private static bool Near((byte B, byte G, byte R) a, (byte B, byte G, byte R) b) =>
        Math.Abs(a.B - b.B) <= ColorTolerance && Math.Abs(a.G - b.G) <= ColorTolerance && Math.Abs(a.R - b.R) <= ColorTolerance;
}
