using System.Text;
using System.Text.RegularExpressions;
using gGameMapOverlay.Imaging;

namespace gGameMapOverlay.Ocr;

/// <summary>
/// 座標欄を 1 文字ずつの画像に切り分け、OCR で読めた結果から各文字の画像を覚えて、次からは画像の一致だけで読む。
/// 座標欄はいつも同じフォントと背景で描かれるので、0〜9 と「:」を一度覚えれば初めての座標も OCR なしで読める。
/// 覚えていない文字が 1 つでもあれば null を返し、呼び出し側は OCR に戻る。
/// </summary>
public sealed partial class CoordinateGlyphs
{
    // 文字とみなす明るさ (灰色の値)。背景は暗く、文字は白っぽい。縁のにじみも文字に含める。
    private const int InkThreshold = 70;
    // 文字の区切りを探すときの明るさ。隣の文字とはにじみ同士で接することがあるので、にじみは区切りに数えない。
    private const int CoreThreshold = 130;
    // 明るさをこの幅で区切って比べる (にじみのわずかな揺れで別の文字にならないように)。
    private const int GrayStep = 64;
    // 覚える文字の種類の上限 (UI の拡大率を変えたときなど、増え続けないように)。
    private const int MaxGlyphs = 256;

    private readonly Dictionary<string, char> glyphs = [];
    // 違う文字として 2 回覚えようとした画像。見分けられないので、以後は OCR に任せる。
    private readonly HashSet<string> ambiguous = [];
    private readonly Lock gate = new();

    /// <summary>覚えている文字の画像の数。</summary>
    public int Count
    {
        get
        {
            lock (gate)
            {
                return glyphs.Count;
            }
        }
    }

    public void Clear()
    {
        lock (gate)
        {
            glyphs.Clear();
            ambiguous.Clear();
        }
    }

    /// <summary>覚えた文字だけで読めれば「x:y」の文字列、読めなければ null。</summary>
    public string? Recognize(BgrImage image)
    {
        var keys = Split(image);
        if (keys.Count == 0)
        {
            return null;
        }
        var text = new StringBuilder(keys.Count);
        lock (gate)
        {
            foreach (var key in keys)
            {
                if (!glyphs.TryGetValue(key, out var ch))
                {
                    return null;
                }
                text.Append(ch);
            }
        }
        var result = text.ToString();
        return CoordinateRegex().IsMatch(result) ? result : null;
    }

    /// <summary>
    /// OCR の結果が「数字:数字」の形で、切り分けた文字数と一致したときだけ、それぞれの文字の画像を覚える。
    /// </summary>
    public void Learn(BgrImage image, string? ocrText)
    {
        var text = (ocrText ?? "").Replace(" ", "");
        if (!CoordinateRegex().IsMatch(text))
        {
            return;
        }
        var keys = Split(image);
        if (keys.Count != text.Length)
        {
            return;
        }
        lock (gate)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                if (ambiguous.Contains(key))
                {
                    continue;
                }
                if (glyphs.TryGetValue(key, out var known))
                {
                    if (known != text[i])
                    {
                        glyphs.Remove(key);
                        ambiguous.Add(key);
                    }
                }
                else if (glyphs.Count < MaxGlyphs)
                {
                    glyphs[key] = text[i];
                }
            }
        }
    }

    /// <summary>
    /// 文字のない列で区切り、1 文字ずつの画像を比較用の文字列にする。
    /// 縦の範囲は全文字で共通 (「:」の位置も形の一部として比べる)。
    /// 領域の端に接するかたまりと、横一杯・縦一杯に伸びる線 (UI の枠) は文字に含めない。
    /// </summary>
    internal static List<string> Split(BgrImage image)
    {
        int width = image.Width, height = image.Height;
        var gray = new byte[width * height];
        var frameRows = new bool[height];
        for (var y = 0; y < height; y++)
        {
            var count = 0;
            for (var x = 0; x < width; x++)
            {
                var value = image.Gray(x, y);
                gray[y * width + x] = value;
                count += value >= InkThreshold ? 1 : 0;
            }
            frameRows[y] = count > width * 0.45;
        }
        // 縦の枠は、横の枠の行を除いた高さのほぼ全体に伸びる (文字はそこまで高くない)。
        var innerRows = frameRows.Count(frame => !frame);
        var frameColumns = new bool[width];
        for (var x = 0; x < width; x++)
        {
            var count = 0;
            for (var y = 0; y < height; y++)
            {
                count += !frameRows[y] && gray[y * width + x] >= InkThreshold ? 1 : 0;
            }
            frameColumns[x] = count > innerRows * 0.9;
        }
        bool Ink(int x, int y) => gray[y * width + x] >= InkThreshold && !frameRows[y] && !frameColumns[x];
        bool Core(int x, int y) => Ink(x, y) && gray[y * width + x] >= CoreThreshold;

        var inkColumns = new bool[width];
        int top = height, bottom = -1;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (Core(x, y))
                {
                    inkColumns[x] = true;
                }
            }
        }

        var spans = new List<(int Left, int Right)>();
        for (var x = 0; x < width;)
        {
            if (!inkColumns[x])
            {
                x++;
                continue;
            }
            var left = x;
            while (x < width && inkColumns[x])
            {
                x++;
            }
            // 端に接するかたまりは、隣の UI の断片とみなす。
            if (left > 0 && x < width)
            {
                spans.Add((left, x - 1));
            }
        }
        foreach (var (left, right) in spans)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = left; x <= right; x++)
                {
                    if (Ink(x, y))
                    {
                        top = Math.Min(top, y);
                        bottom = Math.Max(bottom, y);
                    }
                }
            }
        }

        var keys = new List<string>(spans.Count);
        foreach (var (left, right) in spans)
        {
            var key = new StringBuilder();
            key.Append(right - left + 1).Append('x').Append(bottom - top + 1).Append(':');
            for (var y = top; y <= bottom; y++)
            {
                for (var x = left; x <= right; x++)
                {
                    key.Append((char)('a' + (Ink(x, y) ? gray[y * width + x] / GrayStep + 1 : 0)));
                }
            }
            keys.Add(key.ToString());
        }
        return keys;
    }

    [GeneratedRegex(@"^\d+:\d+$")]
    private static partial Regex CoordinateRegex();
}
