using System.Text.Json.Serialization;
using DrawingColor = System.Drawing.Color;

namespace gGameMapOverlay.Overlay;

/// <summary>
/// カスタムのグループの中の 1 レイヤー。描き方 (色・ドットで塗るか・輪郭線を描くか) を持ち、マスはどのレイヤーで描いたかを番号で持つ
/// (CustomTileGroup.Layers)。グループの中では、レイヤーを順に重ねて描く (後のレイヤーが上)。
/// </summary>
public sealed record CustomTileLayer
{
    /// <summary>"#RRGGBB"。読めない値なら CustomTileGroup.DefaultColor。</summary>
    public string Color { get; init; } = ToHtml(CustomTileGroup.DefaultColor);

    /// <summary>マスを 1 画素おきのドットで塗るか。</summary>
    public bool Dotted { get; init; }

    /// <summary>マスの縁 (輪郭線) を描くか。</summary>
    public bool Outlined { get; init; } = true;

    /// <summary>前の形式 (レイヤーごとにマスを持っていたころ) のマス。読み込んだら CustomTileGroup.Normalize でグループのマスに移す。</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int[]>? Cells { get; init; }

    public static CustomTileLayer FromColor(DrawingColor color) => new() { Color = ToHtml(color) };

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

    public CustomTileLayer WithColor(DrawingColor color) => this with { Color = ToHtml(color) };

    /// <summary>読み込んだ値を直す (前の形式のマスは除く)。</summary>
    public CustomTileLayer Normalized() => this with { Color = ToHtml(GetColor()), Cells = null };

    internal static string ToHtml(DrawingColor color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}
