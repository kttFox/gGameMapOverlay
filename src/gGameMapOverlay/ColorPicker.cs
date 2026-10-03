using gGameMapOverlay.Overlay;

namespace gGameMapOverlay;

/// <summary>色の選択画面。「作成した色」はアプリを閉じるまで覚えておき、次に開いたときも出す (設定ファイルには残さない)。</summary>
internal static class ColorPicker
{
    /// <summary>「作成した色」の初期値 (カスタムのマスに順に使う色と、マップのデータの種類の色)。ColorDialog の形 (0x00BBGGRR) で 16 個。</summary>
    private static int[] customColors =
        CustomTilesForm.Palette.Concat(OverlayLayer.All.Select(layer => layer.DefaultColor))
            .Select(color => Color.FromArgb(255, color))
            .DistinctBy(color => color.ToArgb())
            .Concat(Enumerable.Repeat(Color.White, 16))
            .Take(16)
            .Select(ColorTranslator.ToOle)
            .ToArray();

    /// <summary>色を選ばせる。やめたら null。開いている間に作った色は、やめても覚えておく。</summary>
    public static Color? Choose(IWin32Window owner, Color current)
    {
        using var dialog = new ColorDialog { Color = Color.FromArgb(255, current), FullOpen = true, CustomColors = customColors };
        var result = dialog.ShowDialog(owner);
        customColors = dialog.CustomColors;
        return result == DialogResult.OK ? dialog.Color : null;
    }
}
