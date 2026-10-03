using System.Drawing.Drawing2D;
using gGameMapOverlay.Overlay;
using gGameMapOverlay.Parsing;

namespace gGameMapOverlay;

/// <summary>
/// 自分で描くマス (カスタム) を編集する、キャラクターを中央に置いた格子。
/// 左ボタンで選んでいる形 (Shape) を描き、右ボタンで消す (押したままドラッグで続けて)。ホイールで拡大・縮小し、ホイールを押してドラッグするか、スペースを押しながらマウスを動かして動かす。
/// </summary>
internal sealed class CustomTilesCanvas : Control
{
    private static readonly Color BackgroundColor = Color.FromArgb(32, 32, 32);
    private static readonly Color GridLineColor = Color.FromArgb(70, 70, 70);
    private static readonly Color HoverColor = Color.FromArgb(220, 255, 255, 255);
    private static readonly Color PreviewColor = Color.FromArgb(90, 255, 255, 255);

    // キャラクターから上下左右それぞれに見せるマスの数 (横はマスの幅、縦はマスの高さで数える)。
    // ゲームの画面 (1920 × 1080 なら横 10.7・縦 12) が入るくらいを初期値にする。
    // ホイール 1 ノッチで ZoomStep 倍ずつ変える (大きく拡大したときも小刻みに動くように)。
    private const double MinHalfTiles = 0.25;
    private const double MaxHalfTiles = 30;
    private const double ZoomStep = 1.2;
    private const double DefaultHalfTiles = 11;
    private double halfTiles = DefaultHalfTiles;

    // ホイールを押してドラッグで動かした量 (マスの幅を 1 とした画面上の量)。
    private (double X, double Y) pan;
    private Point? panFrom;
    // スペースを押しながらマウスを動かしているときの、前の位置 (ボタンを押さなくても動かす)。
    private Point? spaceFrom;

    private ((int X, int Y) Cell, (int X, int Y) Part)? hover;
    private bool? painting; // ドラッグ中なら、描く (true) か消す (false) か
    private ((int X, int Y) Cell, (int X, int Y) Part)? lastPainted;

    public CustomTilesCanvas()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = BackgroundColor;
    }

    /// <summary>描くグループ (この順に重ねる)。表示していないグループは、選んでいるときだけ描く。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<CustomTileGroup> Groups { get; set; } = [];

    /// <summary>選んでいるグループ (表示していなくても描く)。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public CustomTileGroup? SelectedGroup { get; set; }

    /// <summary>描く・消す対象のレイヤー (SelectedGroup の中の 1 つ)。null なら描こうとしたときに RequestLayer で作ってもらう。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public CustomTileLayer? Selected { get; set; }

    private Bitmap? gameImage;
    private double gameTileWidth;

    /// <summary>
    /// 背景に映すゲーム画面 (クライアント領域)。キャラクターは画面の中央にいるので、その中央をキャラクターのマスに合わせ、
    /// マスの幅 tileWidth がこの格子のマスの幅になるよう拡大・縮小して描く。null なら映さない。前の画像は破棄する。
    /// </summary>
    public void SetGameImage(Bitmap? image, double tileWidth)
    {
        gameImage?.Dispose();
        gameImage = image;
        gameTileWidth = tileWidth;
        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            gameImage?.Dispose();
            gameImage = null;
        }
        base.Dispose(disposing);
    }

    /// <summary>描く形。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public CustomTileShape Shape
    {
        get => shape;
        set
        {
            shape = value;
            Invalidate();
        }
    }

    private CustomTileShape shape = CustomTileShape.Full;

    /// <summary>描く線の太さ (1/Thickness マス)。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int Thickness
    {
        get => thickness;
        set
        {
            thickness = value;
            Invalidate();
        }
    }

    private int thickness = CustomTileShape.DefaultThickness;

    /// <summary>キャラクターのいるマスの枠の色。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Color PlayerColor { get; set; } = AppConfig.DefaultPlayerColor;

    /// <summary>全体の不透明度 (個別の不透明度でないグループに使う)。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int OverallOpacity { get; set; } = AppConfig.DefaultOverlayOpacity;

    /// <summary>グループがないときに描こうとしたら呼ぶ。作ったグループのレイヤーを返す。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<CustomTileLayer?>? RequestLayer { get; set; }

    /// <summary>マスを描いた・消したとき。</summary>
    public event EventHandler? CellsChanged;

    /// <summary>描く・消すドラッグを始める直前 (まだマスを変えていない)。</summary>
    public event EventHandler? StrokeStarting;

    /// <summary>マウスの下のマス (キャラクターからの相対位置) が変わったとき。</summary>
    public event EventHandler? HoverChanged;

    public (int X, int Y)? HoverCell => hover?.Cell;

    /// <summary>キャラクターのいるマスを (0, 0) とした、今の大きさの格子。</summary>
    private IsoGrid Grid()
    {
        // 幅は左右 halfTiles マス分 (マスの幅 × 2 × halfTiles)、高さは上下 halfTiles マス分 (マスの高さ × 2 × halfTiles) が入る大きさにする。
        var width = Math.Min(ClientSize.Width / (2.0 * halfTiles), ClientSize.Height / halfTiles);
        width = Math.Max(4, width);
        // 動かした量はマスの数で持つ (拡大・縮小しても同じマスが中央付近に残るように)。
        var center = new PointF(ClientSize.Width / 2f + (float)(pan.X * width), ClientSize.Height / 2f + (float)(pan.Y * width));
        return new IsoGrid(center, new GameCoordinate(0, 0), width, width / 2);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var graphics = e.Graphics;
        graphics.Clear(BackColor);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var grid = Grid();
        var size = ClientSize;
        if (gameImage is { } image && gameTileWidth > 0)
        {
            var scale = grid.TileWidth / gameTileWidth;
            var (width, height) = ((float)(image.Width * scale), (float)(image.Height * scale));
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.DrawImage(image, grid.PlayerCenter.X - width / 2, grid.PlayerCenter.Y - height / 2, width, height);
        }
        var corners = new[] { new PointF(0, 0), new PointF(size.Width, 0), new PointF(0, size.Height), new PointF(size.Width, size.Height) }
            .Select(grid.TileAt).ToArray();
        var (minX, maxX) = (corners.Min(tile => tile.X) - 1, corners.Max(tile => tile.X) + 2);
        var (minY, maxY) = (corners.Min(tile => tile.Y) - 1, corners.Max(tile => tile.Y) + 2);
        using (var pen = new Pen(GridLineColor))
        {
            for (var x = minX; x <= maxX; x++)
            {
                graphics.DrawLine(pen, grid.GridPoint(x, minY), grid.GridPoint(x, maxY));
            }
            for (var y = minY; y <= maxY; y++)
            {
                graphics.DrawLine(pen, grid.GridPoint(minX, y), grid.GridPoint(maxX, y));
            }
        }
        var visible = new RectangleF(0, 0, size.Width, size.Height);
        // マスは塗る前にその範囲を透明に戻す (OverlayForm.DrawTiles) ので、背景と格子を消さないよう透明な画像に描いてから重ねる。
        using (var layer = new Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
        {
            using (var layerGraphics = Graphics.FromImage(layer))
            {
                layerGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                foreach (var tiles in Groups.Where(group => group.Shown || group == SelectedGroup).SelectMany(group => group.ToCustomTiles(OverallOpacity)))
                {
                    OverlayForm.DrawCustomTiles(layerGraphics, grid, tiles, visible);
                }
                if (hover is { } target && panFrom is null)
                {
                    // 描くとどうなるかの見本 (小さいマスずつなら、その小さいマス)。
                    var mask = Shape.Mask == 0 ? CustomTileShape.PartMask(target.Part.X, target.Part.Y) : Shape.Mask;
                    var preview = new CustomTileLayer { Cells = [[target.Cell.X, target.Cell.Y, mask, Thickness]] };
                    OverlayForm.DrawCustomTiles(layerGraphics, grid, preview.ToCustomTiles() with { Color = PreviewColor }, visible);
                }
            }
            graphics.DrawImageUnscaled(layer, 0, 0);
        }
        using (var player = new Pen(PlayerColor, 2))
        {
            graphics.DrawPolygon(player, grid.Tile(0, 0));
        }
        if (hover is { } cell && panFrom is null)
        {
            using var pen = new Pen(HoverColor, 1.5f);
            graphics.DrawPolygon(pen, grid.Tile(cell.Cell.X, cell.Cell.Y));
        }
    }

    /// <summary>点のあるマスと、その中の小さいマス (1/3 マス)。</summary>
    private ((int X, int Y) Cell, (int X, int Y) Part) TargetAt(Point point)
    {
        const int n = CustomTileGroup.Division;
        var sub = CustomTileGroup.SubGrid(Grid()).TileAt(point);
        var cell = ((int)Math.Floor(sub.X / (double)n), (int)Math.Floor(sub.Y / (double)n));
        return (cell, (sub.X - cell.Item1 * n, sub.Y - cell.Item2 * n));
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();
        if (e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && IsSpaceDown()))
        {
            panFrom = e.Location;
            painting = null;
            Capture = true;
            Cursor = Cursors.SizeAll;
            Invalidate();
            return;
        }
        if (e.Button is not (MouseButtons.Left or MouseButtons.Right))
        {
            return;
        }
        var layer = Selected ?? RequestLayer?.Invoke();
        if (layer is null)
        {
            return;
        }
        Selected = layer;
        painting = e.Button == MouseButtons.Left;
        lastPainted = null;
        Capture = true;
        StrokeStarting?.Invoke(this, EventArgs.Empty);
        PaintAt(TargetAt(e.Location));
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (panFrom is null && painting is null && IsSpaceDown())
        {
            if (spaceFrom is { } last)
            {
                var tile = Grid().TileWidth;
                pan = (pan.X + (e.X - last.X) / tile, pan.Y + (e.Y - last.Y) / tile);
                Invalidate();
            }
            spaceFrom = e.Location;
            Cursor = Cursors.SizeAll;
            return;
        }
        if (spaceFrom is not null)
        {
            spaceFrom = null;
            Cursor = Cursors.Default;
        }
        if (panFrom is { } from)
        {
            var width = Grid().TileWidth;
            pan = (pan.X + (e.X - from.X) / width, pan.Y + (e.Y - from.Y) / width);
            panFrom = e.Location;
            Invalidate();
            return;
        }
        var target = ClientRectangle.Contains(e.Location) ? TargetAt(e.Location) : (((int X, int Y) Cell, (int X, int Y) Part)?)null;
        if (target != hover)
        {
            var cellChanged = target?.Cell != hover?.Cell;
            hover = target;
            Invalidate();
            if (cellChanged)
            {
                HoverChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        if (painting is not null && target is { } at)
        {
            PaintAt(at);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (panFrom is not null)
        {
            panFrom = null;
            Cursor = Cursors.Default;
            Invalidate();
        }
        painting = null;
        Capture = false;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (hover is not null)
        {
            hover = null;
            Invalidate();
            HoverChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short GetKeyState(int virtualKey);

    // フォーカスがなくてもスペースを押しながらのドラッグで動かせるよう、キーの状態を直接見る。
    private static bool IsSpaceDown() => GetKeyState((int)Keys.Space) < 0;

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        Zoom(Math.Sign(e.Delta));
    }

    public event EventHandler? ZoomChanged;

    /// <summary>今の倍率 (最初の大きさを 100% とした %)。</summary>
    public int ZoomPercent => (int)Math.Round(DefaultHalfTiles / halfTiles * 100);

    /// <summary>steps 段だけ拡大する (負なら縮小)。</summary>
    public void Zoom(int steps) => SetHalfTiles(halfTiles * Math.Pow(ZoomStep, -steps));

    /// <summary>倍率を 100% に戻す。</summary>
    public void ResetZoom() => SetHalfTiles(DefaultHalfTiles);

    private void SetHalfTiles(double value)
    {
        value = Math.Clamp(value, MinHalfTiles, MaxHalfTiles);
        if (value != halfTiles)
        {
            halfTiles = value;
            Invalidate();
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 左ボタンなら選んでいる形を選んでいる太さで描き (同じマスの前の形は置き換える)、右ボタンならマスを消す。
    /// 小さいマスずつなら、その小さいマスだけを足す・消す (足すときは太さも選んでいる太さにする)。
    /// </summary>
    private void PaintAt(((int X, int Y) Cell, (int X, int Y) Part) target)
    {
        var free = Shape.Mask == 0;
        if (free is false)
        {
            target = (target.Cell, (0, 0)); // 小さいマスの違いは見ない (同じマスで描き直さない)
        }
        if (painting is not { } on || Selected is not { } layer || target == lastPainted)
        {
            return;
        }
        lastPainted = target;
        var (x, y) = target.Cell;
        var mask = !free ? (on ? Shape.Mask : 0)
            : on ? layer.MaskAt(x, y) | CustomTileShape.PartMask(target.Part.X, target.Part.Y)
            : layer.MaskAt(x, y) & ~CustomTileShape.PartMask(target.Part.X, target.Part.Y);
        if (layer.SetCell(x, y, mask, on ? Thickness : layer.ThicknessAt(x, y)))
        {
            Invalidate();
            CellsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
