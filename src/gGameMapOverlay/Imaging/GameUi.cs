using System.Drawing;

namespace gGameMapOverlay.Imaging;

/// <summary>
/// ゲーム画面の端にいつも出ている UI (左上のマップ名・座標・INFO・SKILL 欄、下のチャット欄とボタン類) の範囲。
/// UI の上を右クリックしても歩かないので、右クリックで歩くときにそこを除く。開閉・移動するウィンドウは含まない。
/// 右上の経験値・HP・MP・SP の欄は動かせて、その上を右クリックしても歩くので含まない (オーバーレイを描かない範囲には含める)。
/// </summary>
/// <remarks>
/// 等倍 (UI の倍率 1) のクライアント 1056 × 792 の実画面で測った。ウィンドウの大きさを変えて撮り比べると、
/// 左上の欄は左上の角、下のバーは横は中央・縦は下端に合わせて並ぶ (1500 × 792、1056 × 700 などで確認)。
/// UI が拡大されるとき (UiTransform) は、大きさと角からの位置を同じ倍率で拡大する。
/// </remarks>
public static class GameUi
{
    private enum Anchor
    {
        TopLeft,
        TopRight,
        BottomCenter,
    }

    // 角 (下のバーは下端の中央) からの位置 [左, 上, 右, 下] (等倍)。
    private static readonly (Anchor Anchor, int Left, int Top, int Right, int Bottom)[] Areas =
    [
        (Anchor.TopLeft, 0, 0, 365, 25),          // マップ名欄
        (Anchor.TopLeft, 0, 25, 160, 50),         // 座標欄
        (Anchor.TopLeft, 0, 50, 82, 352),         // INFO・SKILL 欄
        (Anchor.BottomCenter, -454, -69, -288, 0),  // バージョン・パーティ・スキル
        (Anchor.BottomCenter, -288, -114, -251, 0), // V・Z・X のボタン
        (Anchor.BottomCenter, -261, -146, 261, 0),  // チャット欄とタブ
        (Anchor.BottomCenter, 253, -114, 290, 0),   // チャット欄の右のボタン
        (Anchor.BottomCenter, 290, -98, 528, 0),    // 右下 (所持数・アイテム箱など)
    ];

    // オーバーレイだけ描かない範囲 (上を右クリックすると歩くので Contains には含めない)。
    // 右上の経験値・HP・MP・SP の欄 (初期位置。1056 × 792 の実画面で測った)。
    private static readonly (Anchor Anchor, int Left, int Top, int Right, int Bottom)[] StatusAreas =
    [
        (Anchor.TopRight, -210, 0, 0, 102),
    ];

    /// <summary>クライアント座標の点 point が UI の上か。client はクライアント領域の大きさ、transform は UI の倍率 (位置のずれは使わない)。</summary>
    public static bool Contains(Size client, UiTransform transform, Point point) =>
        Rectangles(client, transform).Any(rect => rect.Contains(point));

    /// <summary>UI の範囲 (クライアント座標)。引数は Contains と同じ。includeStatus なら右上の経験値・HP・MP・SP の欄も含める。</summary>
    public static Rectangle[] Rectangles(Size client, UiTransform transform, bool includeStatus = false) =>
        (includeStatus ? Areas.Concat(StatusAreas) : Areas).Select(area =>
        {
            var (originX, originY) = area.Anchor switch
            {
                Anchor.TopLeft => (0.0, 0.0),
                Anchor.TopRight => ((double)client.Width, 0.0),
                _ => (client.Width / 2.0, client.Height),
            };
            // 端の画素が含まれるかは Contains の以前の判定 (左・上は含む、右・下は含まない) と同じになるよう切り上げる。
            var left = (int)Math.Ceiling(originX + area.Left * transform.ScaleX);
            var top = (int)Math.Ceiling(originY + area.Top * transform.ScaleY);
            var right = (int)Math.Ceiling(originX + area.Right * transform.ScaleX);
            var bottom = (int)Math.Ceiling(originY + area.Bottom * transform.ScaleY);
            return Rectangle.FromLTRB(left, top, right, bottom);
        }).ToArray();
}
