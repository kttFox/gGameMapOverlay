using System.Text;
using System.Text.RegularExpressions;

namespace gGameMapOverlay.Parsing;

public readonly record struct GameCoordinate(int X, int Y)
{
    public override string ToString() => $"{X}, {Y}";
}

public static partial class TextParsing
{
    // 数字と誤認識されやすい文字。数字を含むトークンの中でのみ置換する。
    private static readonly Dictionary<char, char> DigitLookalikes = new()
    {
        ['O'] = '0', ['o'] = '0', ['〇'] = '0', ['○'] = '0',
        ['l'] = '1', ['I'] = '1', ['|'] = '1', ['!'] = '1',
    };

    [GeneratedRegex(@"[0-9Oo〇○lI|!]+")]
    private static partial Regex DigitTokenRegex();

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    /// <summary>"123, 456" や "X:123 Y:456" などから最初の2つの数値を取り出す。</summary>
    public static GameCoordinate? ParseCoordinates(string? text)
    {
        var normalized = (text ?? "").Normalize(NormalizationForm.FormKC);
        var fixedText = DigitTokenRegex().Replace(normalized, match =>
            match.Value.Any(char.IsAsciiDigit)
                ? new string(match.Value.Select(ch => DigitLookalikes.GetValueOrDefault(ch, ch)).ToArray())
                : match.Value);
        var numbers = NumberRegex().Matches(fixedText);
        if (numbers.Count < 2
            || !int.TryParse(numbers[0].Value, out var x)
            || !int.TryParse(numbers[1].Value, out var y))
        {
            return null;
        }
        return new GameCoordinate(x, y);
    }

    /// <summary>表示用に全角英数を半角化し、空白を整える。</summary>
    public static string CleanMapName(string? text) =>
        WhitespaceRegex().Replace((text ?? "").Normalize(NormalizationForm.FormKC), " ").Trim();

    /// <summary>
    /// 同じマップ名かどうかを判定するための正規化。記号と空白を除き、OCR で混同しやすい文字を寄せる。
    /// GodiNavi (MIT License, Copyright (c) 2026 GD-fandev) の canonicalize_for_match を移植したもの。
    /// </summary>
    public static string CanonicalizeForMatch(string? value)
    {
        var substitutions = new Dictionary<char, char>
        {
            ['o'] = '0', ['〇'] = '0', ['○'] = '0',
            ['l'] = '1', ['i'] = '1', ['|'] = '1', ['!'] = '1',
            ['굴'] = '글', ['충'] = '층',
        };
        var builder = new StringBuilder();
        foreach (var ch in (value ?? "").Normalize(NormalizationForm.FormKC).ToLowerInvariant())
        {
            if (substitutions.TryGetValue(ch, out var replacement))
            {
                builder.Append(replacement);
            }
            else if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
        }
        return builder.ToString();
    }
}
