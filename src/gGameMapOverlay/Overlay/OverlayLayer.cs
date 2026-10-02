namespace gGameMapOverlay.Overlay;

/// <summary>
/// オーバーレイに重ねるデータの種類。data フォルダーのファイル 1 つに対応する
/// </summary>
/// <param name="Key">設定ファイル (overlay_layers / overlay_colors) での名前。</param>
/// <param name="DefaultColor">色の初期値。設定画面で変えられる (AppConfig.GetOverlayColor)。</param>
public sealed record OverlayLayer(string Key, string FileName, string Label, Color DefaultColor)
{
    /// <summary>このマスがこの種類のマスと重なるときは、下のマスが見えるよう一回り小さく描く。</summary>
    public OverlayLayer? ShrinkOver { get; init; }

    public static readonly OverlayLayer MonsterBlock =
        new("monster_block", "monster-block.json", "モンスター境界", Color.FromArgb(40, 120, 255));

    /// <summary>ゲームのマップデータで「特殊」の印が付いたマス。</summary>
    public static readonly OverlayLayer Special =
        new("special", "special.json", "その他", Color.FromArgb(170, 70, 230)) { ShrinkOver = MonsterBlock };

    /// <summary>別のマップへ移動する出入口のマス。</summary>
    public static readonly OverlayLayer MapMove =
        new("map_move", "map-move.json", "出入口", Color.FromArgb(40, 200, 90));

    public static readonly OverlayLayer ImpassableEdge =
        new("impassable_edge", "impassable-edge.json", "壁", Color.FromArgb(230, 40, 40));

    /// <summary>
    /// 描く順 (後ろほど上に描く)。重なったマスでは後ろの種類が優先して見える。
    /// 後から描く種類はそのマスの色だけになり、半透明でも下の色と混ざらない (OverlayForm.DrawTiles)。
    /// 特殊マスはモンスター境界の上に描くが、重なるマスは小さく描いて両方見えるようにする (ShrinkOver)。
    /// </summary>
    public static readonly IReadOnlyList<OverlayLayer> All = [MonsterBlock, Special, ImpassableEdge, MapMove];
}
