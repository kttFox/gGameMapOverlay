using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace gGameMapOverlay;

/// <summary>
/// ゲーム画面のスナップショットの上に読み取り領域の四角形を表示し、移動・サイズ変更で合わせてもらう。
/// ライブ時はクライアント領域の真上に等倍で重ね、画像モードでは画面に収まるよう縮小して表示する。
/// </summary>
internal sealed partial class RegionSelectorForm : Form
{
    private const int HandleSize = 8;
    private const int GripTolerance = 5;
    private const int ZoomFactor = 3;

    private readonly Bitmap frame;
    private readonly Bitmap darkened;
    private readonly Color accent;
    private readonly Rectangle? defaultRegion;
    private readonly double scale;
    private Rectangle region; // frame のピクセル座標
    private Grip dragGrip;
    private Point dragStart;
    private Rectangle dragStartRegion;

    /// <param name="frame">ゲームのクライアント領域全体の画像。フォームを閉じるまで呼び出し側が保持する。</param>
    /// <param name="screenBounds">表示位置。frame と同じ縦横比で、サイズが違えば拡大縮小して表示する。</param>
    /// <param name="currentRegion">最初に表示する領域 (frame のピクセル座標)。</param>
    /// <param name="defaultRegion">「初期値に戻す」で戻す領域。</param>
    public RegionSelectorForm(Bitmap frame, Rectangle screenBounds, string title, Color accent, Rectangle? currentRegion, Rectangle? defaultRegion)
    {
        this.frame = frame;
        this.accent = accent;
        this.defaultRegion = defaultRegion;
        darkened = Darken(frame);

        // デザイナー側で AutoScaleMode = None (スクリーンショットと 1:1 で重ねるため DPI スケーリングしない)。
        InitializeComponent();
        Bounds = screenBounds;
        scale = screenBounds.Width / (double)frame.Width;
        guideLabel.Text = $"{title}の欄に四角形を合わせてください。" +
            "ドラッグで移動、角と辺のハンドルでサイズ変更、矢印キーで 1px (Shift で 10px) 移動、Ctrl+矢印でサイズ変更。";
        resetButton.Enabled = defaultRegion is not null;
        region = Clamp(currentRegion ?? defaultRegion ?? new Rectangle(frame.Width / 2 - 120, frame.Height / 2 - 12, 240, 24));
        OnSelectionChanged();
    }

    /// <summary>決定された領域 (frame のピクセル座標)。キャンセル時は null。</summary>
    public Rectangle? SelectedRegion { get; private set; }

    private static Bitmap Darken(Bitmap frame)
    {
        var darkened = new Bitmap(frame.Width, frame.Height);
        using var graphics = Graphics.FromImage(darkened);
        using var attributes = new ImageAttributes();
        attributes.SetColorMatrix(new ColorMatrix { Matrix00 = 0.55f, Matrix11 = 0.55f, Matrix22 = 0.55f });
        graphics.DrawImage(frame, new Rectangle(0, 0, frame.Width, frame.Height), 0, 0, frame.Width, frame.Height, GraphicsUnit.Pixel, attributes);
        return darkened;
    }

    // ---- イベントハンドラ (デザイナーから接続) ------------------------------------------

    private void OkButton_Click(object? sender, EventArgs e) => Confirm();

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        if (defaultRegion is { } value)
        {
            region = Clamp(value);
            OnSelectionChanged();
        }
    }

    // ---- 描画 -------------------------------------------------------------

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        Activate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var graphics = e.Graphics;
        graphics.InterpolationMode = scale < 1 ? InterpolationMode.HighQualityBilinear : InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;
        graphics.DrawImage(darkened, ClientRectangle);

        // 領域の中だけ元の明るさで見せる。
        var display = ToDisplay(region);
        graphics.DrawImage(frame, display, region, GraphicsUnit.Pixel);
        graphics.PixelOffsetMode = PixelOffsetMode.Default;
        using var pen = new Pen(accent, 2);
        graphics.DrawRectangle(pen, display);
        using var brush = new SolidBrush(accent);
        foreach (var handle in Handles(display))
        {
            graphics.FillRectangle(brush, handle);
            graphics.DrawRectangle(Pens.Black, handle);
        }
    }

    private static IEnumerable<Rectangle> Handles(Rectangle display)
    {
        var xs = new[] { display.Left, display.Left + display.Width / 2, display.Right };
        var ys = new[] { display.Top, display.Top + display.Height / 2, display.Bottom };
        foreach (var y in ys)
        {
            foreach (var x in xs)
            {
                if (x != xs[1] || y != ys[1])
                {
                    yield return new Rectangle(x - HandleSize / 2, y - HandleSize / 2, HandleSize, HandleSize);
                }
            }
        }
    }

    // ---- マウス操作 -----------------------------------------------------------

    private Grip HitTest(Point point) => RegionEditing.HitTest(ToDisplay(region), point, GripTolerance);

    private static Cursor CursorFor(Grip grip) => grip switch
    {
        Grip.Move => Cursors.SizeAll,
        Grip.Left or Grip.Right => Cursors.SizeWE,
        Grip.Top or Grip.Bottom => Cursors.SizeNS,
        Grip.Left | Grip.Top or Grip.Right | Grip.Bottom => Cursors.SizeNWSE,
        Grip.Right | Grip.Top or Grip.Left | Grip.Bottom => Cursors.SizeNESW,
        _ => Cursors.Default,
    };

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Right)
        {
            DialogResult = DialogResult.Cancel;
            return;
        }
        if (e.Button != MouseButtons.Left)
        {
            return;
        }
        dragGrip = HitTest(e.Location);
        dragStart = e.Location;
        dragStartRegion = region;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (dragGrip == Grip.None)
        {
            Cursor = CursorFor(HitTest(e.Location));
            return;
        }
        var dx = (int)Math.Round((e.X - dragStart.X) / scale);
        var dy = (int)Math.Round((e.Y - dragStart.Y) / scale);
        region = Apply(dragStartRegion, dragGrip, dx, dy);
        OnSelectionChanged();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        dragGrip = Grip.None;
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        if (e.Button == MouseButtons.Left && HitTest(e.Location) == Grip.Move)
        {
            Confirm();
        }
    }

    private Rectangle Apply(Rectangle start, Grip grip, int dx, int dy) =>
        RegionEditing.Apply(start, grip, dx, dy, frame.Size);

    private Rectangle Clamp(Rectangle value) => RegionEditing.Clamp(value, frame.Size);

    // ---- キー操作 -------------------------------------------------------------

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // 矢印キーはボタン間のフォーカス移動ではなく、領域の微調整に使う。
        var key = keyData & Keys.KeyCode;
        if (key is Keys.Left or Keys.Right or Keys.Up or Keys.Down)
        {
            var step = keyData.HasFlag(Keys.Shift) ? 10 : 1;
            var dx = key == Keys.Left ? -step : key == Keys.Right ? step : 0;
            var dy = key == Keys.Up ? -step : key == Keys.Down ? step : 0;
            region = keyData.HasFlag(Keys.Control)
                ? Apply(region, Grip.Right | Grip.Bottom, dx, dy)
                : Clamp(new Rectangle(region.X + dx, region.Y + dy, region.Width, region.Height));
            OnSelectionChanged();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    // ---- 状態の更新 ------------------------------------------------------------

    private void Confirm()
    {
        SelectedRegion = region;
        DialogResult = DialogResult.OK;
    }

    private void OnSelectionChanged()
    {
        positionLabel.Text = $"X {region.X}  Y {region.Y}  幅 {region.Width}  高さ {region.Height}";
        UpdateZoomPreview();
        PlaceToolPanel();
        Invalidate();
    }

    /// <summary>領域の中身を拡大表示する (文字が枠に収まっているか確認しやすいように)。</summary>
    private void UpdateZoomPreview()
    {
        var zoom = Math.Min(ZoomFactor, Math.Min(
            zoomPreview.Width / (double)region.Width, zoomPreview.Height / (double)region.Height));
        var size = new Size(Math.Max(1, (int)(region.Width * zoom)), Math.Max(1, (int)(region.Height * zoom)));
        var image = new Bitmap(size.Width, size.Height);
        using (var graphics = Graphics.FromImage(image))
        {
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.DrawImage(frame, new Rectangle(Point.Empty, size), region, GraphicsUnit.Pixel);
        }
        var old = zoomPreview.Image;
        zoomPreview.Image = image;
        old?.Dispose();
    }

    /// <summary>操作パネルを、四角形に重ならないよう画面の下端 (重なるなら上端) に置く。</summary>
    private void PlaceToolPanel()
    {
        var display = Rectangle.Inflate(ToDisplay(region), HandleSize, HandleSize);
        var x = Math.Max(0, (ClientSize.Width - toolPanel.Width) / 2);
        var atBottom = new Rectangle(x, Math.Max(0, ClientSize.Height - toolPanel.Height - 8), toolPanel.Width, toolPanel.Height);
        toolPanel.Location = atBottom.IntersectsWith(display) ? new Point(x, 8) : atBottom.Location;
    }

    private Rectangle ToDisplay(Rectangle value) => Rectangle.FromLTRB(
        (int)Math.Round(value.Left * scale), (int)Math.Round(value.Top * scale),
        (int)Math.Round(value.Right * scale), (int)Math.Round(value.Bottom * scale));

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        darkened.Dispose();
        // 先に外してから解放する (閉じる途中で描き直されると、解放済みの画像を描こうとして例外になる)。
        var image = zoomPreview.Image;
        zoomPreview.Image = null;
        image?.Dispose();
    }
}
