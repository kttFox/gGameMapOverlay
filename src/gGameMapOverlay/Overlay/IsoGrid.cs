using System.Drawing;
using gGameMapOverlay.Parsing;

namespace gGameMapOverlay.Overlay;

/// <summary>
/// gGame のマス (等角菱形。原寸では幅 64px × 高さ 32px) と画面上の位置の対応。
/// キャラクターのいるマス (Player) の中心が画面上の PlayerCenter に来るように置く。
/// ゲームの画面に合わせるときは ForClient で作る (キャラクターは画面の中央、マスはクライアント領域の高さに合わせて拡大)。
/// </summary>
/// <remarks>
/// マスの角を格子点 (gx, gy) で表す。マス (cx, cy) の菱形は、上の頂点が (cx, cy)、右が (cx + 1, cy)、
/// 下が (cx + 1, cy + 1)、左が (cx, cy + 1)。格子点が 1 進むと、x 方向は右下へ (+幅/2, +高さ/2)、y 方向は左下へ (-幅/2, +高さ/2) 動く
/// (原寸なら data/monster-block.md の sx = (cx - cy) * 32, sy = (cx + cy) * 16 と同じ)。
/// </remarks>
public readonly record struct IsoGrid(PointF PlayerCenter, GameCoordinate Player, double TileWidth = 64, double TileHeight = 32)
{
    /// <summary>原寸のマスの大きさ (ゲームのマップ画像と画素単位で一致する)。</summary>
    public const double DefaultTileWidth = 64;
    public const double DefaultTileHeight = 32;

    /// <summary>
    /// gGame はクライアント領域の高さがこれを超えると、地形を「高さ ÷ これ」倍に拡大して描く (1024 × 768 が基準)。
    /// 実機で確認: フルスクリーン 1920 × 1080 で 90 × 45 (1080 ÷ 768 = 1.40625 倍) がぴったり、
    /// ウィンドウ表示のクライアント 1444 × 861 で 71.5〜72 × 36 (861 ÷ 768 倍なら 71.75 × 35.9) が地形と合った。
    /// </summary>
    public const int BaseHeight = 768;

    /// <summary>クライアント領域の高さに対する地形の拡大率。BaseHeight 以下なら等倍。</summary>
    public static double ScaleForClientHeight(int clientHeight) =>
        clientHeight > BaseHeight ? clientHeight / (double)BaseHeight : 1.0;

    /// <summary>ゲームの画面に重ねる格子。キャラクターは常にクライアント領域の中央にいる。</summary>
    public static IsoGrid ForClient(Size client, GameCoordinate player)
    {
        var scale = ScaleForClientHeight(client.Height);
        return new IsoGrid(new PointF(client.Width / 2f, client.Height / 2f), player, DefaultTileWidth * scale, DefaultTileHeight * scale);
    }

    /// <summary>格子点が (dx, dy) 進んだときの画面上の移動量。</summary>
    public PointF Delta(double dx, double dy) =>
        new((float)((dx - dy) * TileWidth / 2), (float)((dx + dy) * TileHeight / 2));

    public PointF GridPoint(double gx, double gy)
    {
        var rx = gx - (Player.X + 0.5);
        var ry = gy - (Player.Y + 0.5);
        return new PointF(
            PlayerCenter.X + (float)((rx - ry) * TileWidth / 2),
            PlayerCenter.Y + (float)((rx + ry) * TileHeight / 2));
    }

    /// <summary>画面上の点があるマス (GridPoint の逆)。</summary>
    public GameCoordinate TileAt(PointF point)
    {
        var u = (point.X - PlayerCenter.X) / (TileWidth / 2);
        var v = (point.Y - PlayerCenter.Y) / (TileHeight / 2);
        return new GameCoordinate(Player.X + (int)Math.Floor((u + v) / 2 + 0.5), Player.Y + (int)Math.Floor((v - u) / 2 + 0.5));
    }

    /// <summary>マスの長方形 [x, y, 幅, 高さ] が画面上で占める平行四辺形 (上・右・下・左の頂点)。</summary>
    public PointF[] Polygon(int x, int y, int width, int height) =>
        [GridPoint(x, y), GridPoint(x + width, y), GridPoint(x + width, y + height), GridPoint(x, y + height)];

    public PointF[] Tile(int x, int y) => Polygon(x, y, 1, 1);

    /// <summary>マス (x, y) の菱形を、中心を保ったまま scale 倍に縮めたもの。</summary>
    public PointF[] Tile(int x, int y, double scale)
    {
        var inset = (1 - scale) / 2;
        return [GridPoint(x + inset, y + inset), GridPoint(x + 1 - inset, y + inset), GridPoint(x + 1 - inset, y + 1 - inset), GridPoint(x + inset, y + 1 - inset)];
    }

    /// <summary>Polygon の外接矩形 (画面外の長方形を描かずに済ませるため)。</summary>
    public static RectangleF Bounds(PointF[] polygon)
    {
        var left = polygon.Min(point => point.X);
        var top = polygon.Min(point => point.Y);
        return RectangleF.FromLTRB(left, top, polygon.Max(point => point.X), polygon.Max(point => point.Y));
    }
}
