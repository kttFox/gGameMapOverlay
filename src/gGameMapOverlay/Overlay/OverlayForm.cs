using System.Drawing.Drawing2D;

namespace gGameMapOverlay.Overlay;

/// <summary>オーバーレイに描く内容。値が同じなら描き直さない。</summary>
/// <param name="ClientRect">ゲームのクライアント領域 (スクリーン座標)。オーバーレイはこの真上に重ねる。</param>
/// <param name="Layers">描くマスとその色。この順に重ねて描く (OverlayLayer.All の順)。</param>
/// <param name="Excluded1">描かない範囲 (クライアント座標)。マップ名欄・座標欄に重ねると OCR が誤読するため。</param>
/// <param name="UiAreas">ゲームの UI の範囲 (クライアント座標)。UI の上には描かない。</param>
/// <param name="GridColor">マスの格子の線の色 (マスの種類より下に描く)。null なら描かない。</param>
/// <param name="PlayerColor">キャラクターのいるマスの枠の色。null なら描かない。</param>
/// <param name="Custom">自分で描いたマス (キャラクターのいるマスからの相対位置) とその色。この順に重ね、プレイヤーの枠と同じくずらさずに描く。</param>
internal sealed record OverlayScene(
    Rectangle ClientRect, IsoGrid Grid, IReadOnlyList<(OverlayLayer Layer, TileRects Tiles, Color Color)> Layers, Rectangle? Excluded1, Rectangle? Excluded2,
    Color? GridColor = null, IReadOnlyList<Rectangle>? UiAreas = null, Color? PlayerColor = null, IReadOnlyList<CustomTiles>? Custom = null)
{
    // Layers はリストなので、既定の (参照による) 比較ではなく中身で比べる。
    public bool Equals(OverlayScene? other) =>
        other is not null && ClientRect == other.ClientRect && Grid == other.Grid && Excluded1 == other.Excluded1 && Excluded2 == other.Excluded2
        && GridColor == other.GridColor && PlayerColor == other.PlayerColor
        && (UiAreas ?? []).SequenceEqual(other.UiAreas ?? []) && Layers.SequenceEqual(other.Layers) && (Custom ?? []).SequenceEqual(other.Custom ?? []);

    public override int GetHashCode() => HashCode.Combine(ClientRect, Grid, Layers.Count, Excluded1, Excluded2);
}

/// <summary>
/// ゲーム画面の上に重ねる、クリックを透過する枠なしウィンドウ。
/// 透明度付きの画像を描いて UpdateLayeredWindow で渡す (描いていないところは透ける)。透明度は色ごと (色のアルファ)。
/// </summary>
internal sealed class OverlayForm : Form
{
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_NOACTIVATE = 0x08000000;
    private const int WS_EX_TOPMOST = 0x00000008;

    private OverlayScene? scene;

    // 座標が変わったときのスライド。見せている位置 (shownX, shownY、マス単位) を、読み取った座標へ
    // 常に 1 マス / StepMs の一定の速さで動かす (遅れを取り戻すために速めたりはしない)。
    // 歩き続けているときは、着く前に次の座標が届くので止まらずに進み続ける。
    // これより大きく動いたとき (ワープなど) はスライドしない。
    private const int MaxSlideTiles = 5;
    private readonly System.Windows.Forms.Timer slideTimer = new() { Interval = 15 };
    private readonly System.Diagnostics.Stopwatch clock = System.Diagnostics.Stopwatch.StartNew();
    private double shownX, shownY, lastFrameAt;
    private PointF offset;

    // キー入力からの先読み (歩き始めの 1 歩だけ)。止まっているときに移動キーを押したら、座標が変わるのを待たずに、
    // 押した方向の隣のマスへ動かし始める。2 歩目からは今までどおり、読み取った座標の変化で動かす。
    // 先読みしたマス (predicted) は、読み取った座標がそのマスになったら確かめられたものとして外す。
    // 読み取った座標が違うマスになったとき、または時間内に確かめられなかったとき (壁で進めなかったなど) は
    // 先読みをやめて、読み取った座標へ戻る。
    private const double ConfirmTimeoutMs = 300;
    private readonly List<(int X, int Y)> predicted = [];
    private double moveStartAt, predictDeadline;

    // 2 歩目から: 先読みで歩いている (walking) 間は、マスに着いたら止まらずに、押しているキーの方向の次のマスへ
    // 同じ速さで滑り続ける。着いたときに座標欄の見た目が変わっていなければ、進みながら StallGraceMs まで待ち
    // (1 歩の区切りはゲームのフレームでばらつく)、それでも変わらなければ
    // (gGame は歩き始めた瞬間に座標の表示を変えるので、変わっていなければ歩いていない) 止まったとみなし、着いたマスへ戻す。
    // 見た目の比べる元は、そのマスへ歩いている途中 (半分進んだところ) の座標欄。
    private const int MaxUnconfirmed = 2;
    // 着いてから座標欄が変わらないとき、止まったとみなすまで進みながら待つ時間。
    private const double StallGraceMs = 60;
    // 次に歩く方向がないとき (キーを離した・壁)、止まったとみなすまでその場で待つ時間。
    private const double StopGraceMs = 60;
    private double? stopWaitUntil;
    private double stopWaitCheckedAt;
    private bool walking;
    private double? continueCheckAt;
    // 判定のときにまだ座標欄が変わっていなかった: 着いたマスへ戻しつつ、1 歩の半分の時間までは変わるのを待つ
    // (変わったらそこから次のマスへ動かす。読み取りを待つより早く動き出せる)。
    private double? stalledUntil;
    // 待っている間に、座標欄がまだ変わっていないと最後に確かめた時刻。変わったのはこの時刻と見つけた時刻の間。
    private double stalledCheckedAt;
    private double baselineAt = double.PositiveInfinity;
    // 1 歩目: キーを押した時の座標欄と、歩き出すまでの時間 (KeyDelayMs) の後に比べる。変わっていなければ歩いていない。
    private double? startCheckAt;
    // 1 歩目: 判定の時刻にまだ座標欄が変わっていなかったら、この時刻まで Tick ごとに変わるのを待つ (その間はその場で止める)。
    private double? startWaitUntil;
    // 1 歩目: 座標欄が変わったのは確かめたが、歩き出した時刻がまだ分からない。分かったら合わせる。
    private bool startAlignPending;
    // 最後に先読みで進めた 1 歩の方向 (着いたときに次の方向がなくても、座標欄が変わっていればこの方向へ続ける)。
    private (int X, int Y)? lastStepDirection;
    // 座標欄の比較元を最後に取った時刻。
    private double baselineTakenAt = double.NegativeInfinity;

    private void CaptureBaseline(string purpose)
    {
        baselineTakenAt = clock.Elapsed.TotalMilliseconds;
        CaptureStepBaseline?.Invoke(purpose);
    }
    private double startWaitCheckedAt, keyPressedAt;
    private (int X, int Y) startOrigin;

    /// <summary>1 歩目 (歩き始め) で、キーを押してから座標欄の見た目を確かめるまでの時間 (ミリ秒)。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int StartCheckMs { get; set; } = 40;

    /// <summary>1 歩の途中で呼ぶ。座標欄の今の見た目を、次に歩き続けたかを比べる元として取っておく。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Action<string>? CaptureStepBaseline { get; set; }

    /// <summary>
    /// マス from から direction へ歩き出したと見ているとき、座標欄の画素の変わり方 (X か Y か) から実際の方向を確かめる。
    /// 違っていれば実際の方向、合っている・分からなければ null。
    /// </summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<(int X, int Y), (int X, int Y), (int X, int Y)?>? ResolveDirection { get; set; }

    /// <summary>
    /// 歩き出したと分かったとき (座標欄が変わった)、変わり方で方向を確かめ、違っていれば先読みのマス (いちばん後ろ) と歩く方向を直す。
    /// 読み取りを待つより早く (歩き出してから数 ms で) 曲がる方向の読み違いを直せる。直した方向を返す。
    /// </summary>
    private (int X, int Y) VerifyStepDirection((int X, int Y) from, (int X, int Y) direction)
    {
        if (predicted.Count == 0 || ResolveDirection?.Invoke(from, direction) is not { } actual)
        {
            return direction;
        }
        predicted[^1] = (from.X + actual.X, from.Y + actual.Y);
        lastStepDirection = actual;
        fastCorrection = true;
        Log($"方向修正 (画素): ({direction.X}, {direction.Y}) ではなく ({actual.X}, {actual.Y})　次マス {predicted[^1]}", 1);
        return actual;
    }

    /// <summary>マス (x, y) から、今効いている移動キーの方向へ歩けるならその方向 (入力の反映遅れを考えない)。なければ null。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<int, int, (int X, int Y)?>? ActiveDirection { get; set; }

    /// <summary>座標欄の見た目が最後に変わってから今までの時間 (ミリ秒)。分からなければ null。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<double?>? CoordinateChangeAgo { get; set; }

    /// <summary>
    /// 今の 1 歩を歩き出した時刻 (座標欄の見た目が変わった時刻、clock の時刻)。分からない、または古すぎて今の 1 歩のものでなければ null。
    /// 座標欄はゲームが 1 歩を歩き出した瞬間に変わるので、これに合わせて描けばゲームと同じ位置になる。
    /// </summary>
    private double? StepStartedAt(double now) =>
        CoordinateChangeAgo?.Invoke() is { } ago && ago >= 0 && ago < StepMs * 0.75 ? now - ago : null;

    /// <summary>from から direction へ歩いている 1 歩を、歩き出した時刻 startedAt に合わせた位置に置く。</summary>
    private void PlaceOnStep((int X, int Y) from, (int X, int Y) direction, double startedAt, double now)
    {
        var ahead = Math.Clamp((now - startedAt) / Math.Max(1, StepMs), 0, 1);
        MoveShown(from.X + direction.X * ahead, from.Y + direction.Y * ahead, now);
        baselineAt = startedAt + StepMs / 2;
    }

    // 位置を合わせ直したときのずれ。描く位置は (shownX, shownY) にこのずれを足したもので、ずれは CorrectionMs かけて 0 にする
    // (一瞬で飛ばすと小刻みに動いて見えるため)。
    private const double CorrectionMs = 70;
    // 方向を直したとき (違う方向へ描いていた分) は、長く残すと目立つので速く吸収する。
    private const double TurnCorrectionMs = 50;
    // 遅れていたのを取り戻すときは、さらに速く吸収する。
    private const double CatchUpCorrectionMs = 40;
    private double correctionX, correctionY, correctionAt, correctionDuration = CorrectionMs;
    private bool fastCorrection;

    private (double X, double Y) Correction(double now)
    {
        var rest = Math.Max(0, 1 - (now - correctionAt) / correctionDuration);
        return (correctionX * rest, correctionY * rest);
    }

    /// <summary>描く位置の元 (shownX, shownY) を、(restX, restY) の向きへ move マス (残り distance を超えない) 進める。</summary>
    private void AdvanceShown(double restX, double restY, double distance, double move)
    {
        if (distance > 0)
        {
            var advance = Math.Min(move, distance);
            shownX += restX / distance * advance;
            shownY += restY / distance * advance;
        }
    }

    /// <summary>描く位置の元 (shownX, shownY) を (x, y) に置き直す。見た目は今の位置から滑らかに移る。</summary>
    private void MoveShown(double x, double y, double now)
    {
        var (restX, restY) = Correction(now);
        correctionX = restX + shownX - x;
        correctionY = restY + shownY - y;
        correctionAt = now;
        // 遅れていた (見た目が進む向きの後ろにある) ときは早めに追いつかせる。進みすぎを戻すときは目立たないようゆっくり。
        var behind = lastStepDirection is { } step && correctionX * step.X + correctionY * step.Y < 0;
        correctionDuration = fastCorrection ? TurnCorrectionMs : behind ? CatchUpCorrectionMs : CorrectionMs;
        fastCorrection = false;
        shownX = x;
        shownY = y;
    }

    // 滑らかさの記録 (歩き終わったときにログに出す): 描いた位置の 1 コマごとの動き。
    private double? lastDrawnX, lastDrawnY;
    private double lastDrawnAt;
    private int smoothFrames, smoothBackward, smoothStopped;
    private double smoothMaxStep, smoothMaxSpeed, smoothMaxInterval;

    /// <summary>今描く位置 (ずれの吸収を含む) の、読み取った座標からのずれ。滑らかさも記録する。</summary>
    private PointF DrawOffset(OverlayScene current, double playerX, double playerY, double now)
    {
        var (cx, cy) = Correction(now);
        var drawnX = shownX + cx;
        var drawnY = shownY + cy;
        if (walking && lastDrawnX is { } px && lastDrawnY is { } py && lastStepDirection is { } step)
        {
            var dx = drawnX - px;
            var dy = drawnY - py;
            var along = dx * step.X + dy * step.Y;
            var length = Math.Max(Math.Abs(dx), Math.Abs(dy));
            smoothFrames++;
            if (along < -0.003)
            {
                smoothBackward++;
            }
            else if (length < 0.003)
            {
                smoothStopped++;
            }
            smoothMaxStep = Math.Max(smoothMaxStep, length);
            var interval = now - lastDrawnAt;
            smoothMaxInterval = Math.Max(smoothMaxInterval, interval);
            if (interval > 1)
            {
                // 1 マス / 1 歩の時間を 1 とした速さ (1 を大きく超えると飛んで見える)。
                smoothMaxSpeed = Math.Max(smoothMaxSpeed, length / interval * StepMs);
            }
        }
        lastDrawnX = drawnX;
        lastDrawnY = drawnY;
        lastDrawnAt = now;
        accuracy.AddFrame(now, drawnX, drawnY);
        return current.Grid.Delta(playerX - drawnX, playerY - drawnY);
    }

    // スライドの精度の記録 (止まったときにログに出す)。スキルで動いたときは 1 歩の速さが違うので比べない。
    private readonly SlideAccuracy accuracy = new();
    private bool accuracySkipped;

    private void LogAccuracy()
    {
        if (!accuracySkipped && accuracy.Summarize(StepMs) is { } result)
        {
            Log($"精度: {result.Frames} コマ　ずれ 平均 {result.Mean:0.000} マス　最大 {result.Max:0.000} マス ({result.MaxStep} 歩目 {result.MaxAt:+0;-0;0} ms)　進み 平均 {result.LeadMs:+0;-0;0} ms");
        }
        accuracy.Clear();
        accuracySkipped = false;
    }

    private void LogSmoothness()
    {
        if (smoothFrames > 0)
        {
            Log($"滑らかさ: {smoothFrames} コマ　逆向き {smoothBackward}　停止 {smoothStopped}　1 コマの最大 {smoothMaxStep:0.000} マス　速さの最大 {smoothMaxSpeed:0.00} 倍　コマ間隔の最大 {smoothMaxInterval:0} ms");
        }
        smoothFrames = smoothBackward = smoothStopped = 0;
        smoothMaxStep = smoothMaxSpeed = smoothMaxInterval = 0;
        lastDrawnX = lastDrawnY = null;
    }

    /// <summary>マス (x, y) に着いたとき呼ぶ。次に歩く方向 (キーを押していて、行き先が壁でない)。なければ null。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<int, int, (int X, int Y)?>? ContinueDirection { get; set; }

    /// <summary>
    /// 座標欄の見た目が、CaptureStepBaseline で取っておいたときから変わったか (次の 1 歩を歩き始めたか)。
    /// 引数は目的と、確かめる予定の時刻からの遅れ (ミリ秒、予定がない Tick ごとの確認は null)。
    /// </summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<string, double?, bool>? CoordinateChanged { get; set; }

    /// <summary>描いている位置と読み取った座標がこのマス数以上離れたら、滑らせずにすぐ移す。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public double SnapTiles { get; set; } = 1.5;

    /// <summary>1 マスのスライドにかける時間 (ミリ秒、移動速度の等級の 1 歩の時間)。0 ならスライドしない。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int SlideMs { get; set; }

    /// <summary>移動キーを押してからキャラクターが歩き出すまでの時間 (ミリ秒)。先読みはこの後に動き出す。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int KeyDelayMs { get; set; }

    /// <summary>
    /// 止まっている状態で移動キーを押したとき呼ぶ。direction は押したキーで歩く方向 (マス単位の -1〜1)。
    /// 動いている途中 (スライド中) なら何もしない。
    /// </summary>
    public void PredictMove((int X, int Y) direction)
    {
        if (scene is not { } current || SlideMs <= 0 || slideTimer.Enabled || predicted.Count > 0)
        {
            return;
        }
        var now = clock.Elapsed.TotalMilliseconds;
        predicted.Add((current.Grid.Player.X + direction.X, current.Grid.Player.Y + direction.Y));
        lastStepDirection = direction;
        startOrigin = (current.Grid.Player.X, current.Grid.Player.Y);
        keyPressedAt = now;
        startWaitUntil = null;
        startAlignPending = false;
        predictDeadline = now + KeyDelayMs + StepMs + ConfirmTimeoutMs;
        shownX = current.Grid.Player.X;
        shownY = current.Grid.Player.Y;
        lastFrameAt = now;
        moveStartAt = now + KeyDelayMs;
        walking = true;
        continueCheckAt = null;
        stalledUntil = null;
        baselineAt = moveStartAt + StepMs / 2;
        Log($"先読み開始: ({current.Grid.Player.X}, {current.Grid.Player.Y}) → {predicted[0]}", 1);
        CaptureBaseline("歩き始め");
        startCheckAt = now + StartCheckMs;
        repredictStartAt = null;
        continueBlocked = false;
        slideTimer.Start();
    }

    // スキル (一度に何マスも進む): 方向、マス数、1 マスの時間、押した時刻、出発したマス、動き出した時刻 (座標欄が変わった時刻)。
    private (int X, int Y) skillDirection;
    private int skillTiles;
    private double skillTileMs, skillPressedAt;
    private (int X, int Y) skillOrigin;
    private double? skillStartedAt;
    private bool skillActive;
    // スキル中に座標欄が変わった時刻 (出発から何マス目か、変わった時刻)。ずれを測ってログに出す。
    private readonly List<(double Tile, double At)> skillChanges = [];

    /// <summary>
    /// スキルのキーを押したとき呼ぶ。止まっていれば、座標欄が変わった (動き出した) 時刻から、direction へ tiles マスを
    /// 1 マス tileMs で、読み取りを待たずに動かす。途中の読み取りは、通り道の上なら直さない。
    /// </summary>
    public void PredictSkill((int X, int Y) direction, int tiles, double tileMs)
    {
        if (scene is not { } current || SlideMs <= 0 || slideTimer.Enabled || skillActive || tiles <= 0)
        {
            Log($"スキル: 先読みなし ({(slideTimer.Enabled ? "動いている途中" : "準備なし")})");
            return;
        }
        var now = clock.Elapsed.TotalMilliseconds;
        skillActive = true;
        accuracySkipped = true;
        skillDirection = direction;
        skillTiles = tiles;
        skillTileMs = Math.Max(1, tileMs);
        skillPressedAt = now;
        skillOrigin = (current.Grid.Player.X, current.Grid.Player.Y);
        skillStartedAt = null;
        skillChanges.Clear();
        shownX = skillOrigin.X;
        shownY = skillOrigin.Y;
        lastStepDirection = direction;
        lastFrameAt = now;
        Log($"スキル先読み: ({skillOrigin.X}, {skillOrigin.Y}) から ({direction.X}, {direction.Y}) へ {tiles} マス　1 マス {tileMs:0} ms");
        CaptureBaseline("スキル");
        slideTimer.Start();
    }

    /// <summary>
    /// スキルの出発時刻を、座標欄が変わった時刻に合わせ直す。スキルでは k マス目へ動き始めたときに座標欄が変わるので、
    /// 変わった時刻 - (k - 1) × 1 マスの時間 が出発時刻。読み取りの間隔でばらつくので、それまでの変化の平均を使う。
    /// 動き出しを座標欄の画素で見つける前に読み取りが先に変化を見つけることもあり、そのときはここで動き出す。
    /// </summary>
    private void AlignSkillStart(double now)
    {
        var implied = skillChanges.Where(change => change.Tile >= 1).Select(change => change.At - (change.Tile - 1) * skillTileMs).ToList();
        if (implied.Count == 0)
        {
            return;
        }
        var start = implied.Average();
        if (skillStartedAt is null)
        {
            Log($"スキル: 動き出し (読み取りで確認)　キー入力から {start - skillPressedAt:0} ms", 1);
        }
        else if (Math.Abs(start - skillStartedAt.Value) < 1)
        {
            return;
        }
        skillStartedAt = start;
        // 描く位置はずれごと少しずつ合わせる (一瞬で飛ばさない)。
        var progress = Math.Clamp((now - start) / skillTileMs, 0, skillTiles);
        MoveShown(skillOrigin.X + skillDirection.X * progress, skillOrigin.Y + skillDirection.Y * progress, now);
    }

    /// <summary>
    /// スキルで座標欄が変わった時刻を、マス数に対して直線で当てはめ、実際の 1 マスの時間と動き出しのずれをログに出す
    /// (スキルでは、座標欄はそのマスへ動き始めたときに変わる)。
    /// </summary>
    private void LogSkillTiming()
    {
        if (skillStartedAt is not { } started || skillChanges.Count < 2)
        {
            return;
        }
        double n = skillChanges.Count, sx = 0, sy = 0, sxx = 0, sxy = 0;
        foreach (var (tile, at) in skillChanges)
        {
            var x = tile - 1.0;
            var y = at - started;
            sx += x; sy += y; sxx += x * x; sxy += x * y;
        }
        var slope = (n * sxy - sx * sy) / Math.Max(1e-9, n * sxx - sx * sx);
        var intercept = (sy - slope * sx) / n;
        Log($"スキル精度: 実測 1 マス {slope:0.0} ms (設定 {skillTileMs:0} ms)　動き出しのずれ {intercept:+0;-0} ms　変化 {skillChanges.Count} 回", 1);
    }

    /// <summary>スキル中の 1 コマ。動き出すのを座標欄で待ち、動き出したら決まった速さで進める。終わったら false。</summary>
    private bool UpdateSkill(OverlayScene current, double now)
    {
        double playerX = current.Grid.Player.X, playerY = current.Grid.Player.Y;
        if (skillStartedAt is null)
        {
            if (CoordinateChanged?.Invoke("スキルの動き出し", null) == true)
            {
                skillStartedAt = StepStartedAt(now) is { } started && started >= skillPressedAt ? started : now;
                Log($"スキル: 動き出し　キー入力から {skillStartedAt - skillPressedAt:0} ms", 1);
            }
            else if (now - skillPressedAt > 400)
            {
                Log("スキル: 400 ms 動かず　先読み中止", 1);
                skillActive = false;
                return false;
            }
            offset = DrawOffset(current, playerX, playerY, now);
            return true;
        }
        var progress = Math.Clamp((now - skillStartedAt.Value) / skillTileMs, 0, skillTiles);
        var end = (X: skillOrigin.X + skillDirection.X * skillTiles, Y: skillOrigin.Y + skillDirection.Y * skillTiles);
        // 読み取った座標が通り道から外れた・途中で止まった (壁など): 先読みをやめて読み取った座標へ。
        var along = (playerX - skillOrigin.X) * skillDirection.X + (playerY - skillOrigin.Y) * skillDirection.Y;
        var onPath = Math.Abs((playerX - skillOrigin.X) * skillDirection.Y - (playerY - skillOrigin.Y) * skillDirection.X) < 0.5 && along >= -0.5 && along <= skillTiles + 0.5;
        var stoppedEarly = along < skillTiles && now - skillStartedAt.Value > (along + 1) * skillTileMs + 150;
        if (!onPath || stoppedEarly)
        {
            Log($"スキル: 読み取り ({playerX}, {playerY}) が先読みと合わない　先読み中止", 1);
            skillActive = false;
            return false;
        }
        shownX = skillOrigin.X + skillDirection.X * progress;
        shownY = skillOrigin.Y + skillDirection.Y * progress;
        if (progress >= skillTiles && playerX == end.X && playerY == end.Y)
        {
            Log($"スキル: 到着 ({end.X}, {end.Y})", 1);
            LogSkillTiming();
            skillActive = false;
            return false;
        }
        offset = DrawOffset(current, playerX, playerY, now);
        return true;
    }

    // 動いている途中のキー入力 (RepredictMove) で、次の 1 歩を歩き出せるいちばん早い時刻 (キーを押した時刻 + KeyDelayMs)。
    private double? repredictStartAt;

    /// <summary>スライド中 (止まっていない) か。</summary>
    public bool IsSliding => slideTimer.Enabled;

    private bool suppressed;

    /// <summary>true の間は、描く内容があっても隠しておく (ゲーム画面を撮るときにオーバーレイが写り込まないように)。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public bool Suppressed
    {
        get => suppressed;
        set
        {
            suppressed = value;
            if (value && Visible)
            {
                Hide();
            }
            else if (!value && !Visible && scene is not null)
            {
                Show();
            }
        }
    }

    /// <summary>最後に先読みで進めた 1 歩の方向 (なければ null)。</summary>
    public (int X, int Y)? LastStepDirection => lastStepDirection;

    /// <summary>動いている途中のキー入力 (RepredictMove) を受けて、着いたら次のマスへ進むのを待っているか。</summary>
    public bool RepredictPending => repredictStartAt is not null;

    // 歩いていないと分かった (座標欄が変わらなかった・先読みを確かめられなかった): 座標が次に変わるまでは、
    // 止まる直前にキーを押し続けていても歩き続けるとみなさない (壁などで進めないのに進む・戻るを繰り返さないように)。
    private bool continueBlocked;

    /// <summary>
    /// 座標の変化で動いている途中 (先読みで歩いていない) に、移動キーを押し下げたとき呼ぶ。
    /// 今向かっているマスに着いたら、そのとき押しているキーの方向へ、座標が変わるのを待たずに動かす
    /// (キーを押してから KeyDelayMs 経つまでは着いたマスで待つ)。
    /// </summary>
    public void RepredictMove()
    {
        if (scene is null || SlideMs <= 0 || !slideTimer.Enabled || walking || predicted.Count > 0)
        {
            if (scene is not null && SlideMs > 0 && slideTimer.Enabled)
            {
                Log($"途中キー入力: 無視 (先読み中 {predicted.Count} マス)", 1);
            }
            return;
        }
        var now = clock.Elapsed.TotalMilliseconds;
        walking = true;
        continueCheckAt = null;
        stalledUntil = null;
        startCheckAt = null;
        baselineAt = double.PositiveInfinity;
        // 座標欄はもう今向かっているマスを表しているので、今の見た目を次に歩き出したかを比べる元にする。
        Log($"途中キー入力: 受付　描画位置 ({shownX:0.00}, {shownY:0.00})　到着予定 ({scene.Grid.Player.X}, {scene.Grid.Player.Y})　歩き出し {KeyDelayMs} ms 後", 1);
        CaptureBaseline("途中キー入力");
        repredictStartAt = now + KeyDelayMs;
    }

    /// <summary>スライドの出来事 (すぐ移した・先読みをやめた・止まったなど) の説明を受け取る。情報画面のログに出す。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Action<string, int>? EventLogged { get; set; }

    /// <summary>depth はログの字下げの深さ (きっかけになった出来事の下に、その結果を 1 段下げて出す)。</summary>
    private void Log(string message, int depth = 0) => EventLogged?.Invoke(message, depth);

    // 描いている位置と読み取った座標の差 (マス) のこれまでの最大 (情報画面に出す)。
    private double maxGap;

    /// <summary>差の最大を 0 に戻す。</summary>
    public void ResetMaxGap() => maxGap = 0;

    /// <summary>今の移動の状態の説明 (情報画面に出す)。</summary>
    public string MotionStatus
    {
        get
        {
            if (scene is not { } current)
            {
                return "オーバーレイなし";
            }
            var (px, py) = (current.Grid.Player.X, current.Grid.Player.Y);
            var sliding = slideTimer.Enabled;
            var (sx, sy) = sliding ? (shownX, shownY) : (px, py);
            var gap = Math.Max(Math.Abs(px - sx), Math.Abs(py - sy));
            return $"読み取った座標: ({px}, {py})" + Environment.NewLine
                + $"描いている位置: ({sx:0.00}, {sy:0.00})　差 {gap:0.00} マス (最大 {maxGap:0.00})" + Environment.NewLine
                + $"スライド: {(sliding ? "o" : "x")}　歩き続け: {(walking ? "o" : "-")}" + Environment.NewLine
                + $"先読み: {(predicted.Count == 0 ? "なし" : string.Join(" → ", predicted.Select(p => $"({p.X}, {p.Y})")))}";
        }
    }

    // 1 マス進む時間は移動速度の等級の 1 歩の時間 (SlideMs として MainForm が設定する)。
    private double StepMs => SlideMs;

    public OverlayForm()
    {
        AutoScaleMode = AutoScaleMode.None; // スクリーンショットと同じ物理ピクセルで重ねる
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Text = "gGame Map Overlay";
        // 最前面は TopMost ではなく CreateParams の WS_EX_TOPMOST で付ける。TopMost のフォームは表示するときに
        // フォーカスを移されるので、本ツールが前面のとき (タスクトレイのメニューを開いたときなど) に
        // オーバーレイがアクティブになり、メニューが閉じてしまう。
        slideTimer.Tick += SlideTimer_Tick;
        jumpTimer.Tick += (_, _) => EndJump();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            slideTimer.Dispose();
            jumpTimer.Dispose();
            layeredBitmap?.Dispose();
            ResetTerrainCaches();
        }
        base.Dispose(disposing);
    }

    /// <summary>スライドしないとき (SlideMs が 0)、座標が変わってからマスを移すまでの時間 (ミリ秒)。0 ならすぐ移す。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public double JumpDelayMs { get; set; }

    private readonly System.Windows.Forms.Timer jumpTimer = new();
    // 遅らせて移している間、まだ描いている (前の) マス。
    private (int X, int Y)? jumpFrom;

    /// <summary>
    /// スライドしないとき、座標が変わってから JumpDelayMs 経つまで前のマスのまま描き、経ったら読み取った座標へ移す。
    /// 待っている間にまた変わったら、最後の座標へ、最後に変わった時刻から JumpDelayMs 後に移す。
    /// </summary>
    private void DelayJump(OverlayScene previous, OverlayScene next, double lag)
    {
        jumpFrom ??= (previous.Grid.Player.X, previous.Grid.Player.Y);
        offset = next.Grid.Delta(next.Grid.Player.X - jumpFrom.Value.X, next.Grid.Player.Y - jumpFrom.Value.Y);
        jumpTimer.Stop();
        var wait = JumpDelayMs - Math.Max(0, lag);
        if (wait < 1)
        {
            EndJump();
            return;
        }
        jumpTimer.Interval = (int)Math.Ceiling(wait);
        jumpTimer.Start();
    }

    private void EndJump()
    {
        jumpTimer.Stop();
        jumpFrom = null;
        offset = PointF.Empty;
        Redraw();
    }

    private void StartSlide(OverlayScene previous, OverlayScene next, double lag)
    {
        var dx = next.Grid.Player.X - previous.Grid.Player.X;
        var dy = next.Grid.Player.Y - previous.Grid.Player.Y;
        if (SlideMs <= 0 && JumpDelayMs > 0 && (dx != 0 || dy != 0) && Math.Abs(dx) <= MaxSlideTiles && Math.Abs(dy) <= MaxSlideTiles
            && previous.ClientRect == next.ClientRect && previous.Grid.TileWidth == next.Grid.TileWidth
            && previous.Grid.TileHeight == next.Grid.TileHeight && previous.Layers.SequenceEqual(next.Layers))
        {
            DelayJump(previous, next, lag);
            return;
        }
        if (SlideMs <= 0 || Math.Abs(dx) > MaxSlideTiles || Math.Abs(dy) > MaxSlideTiles
            || previous.ClientRect != next.ClientRect || previous.Grid.TileWidth != next.Grid.TileWidth
            || previous.Grid.TileHeight != next.Grid.TileHeight || !previous.Layers.SequenceEqual(next.Layers))
        {
            if (dx != 0 || dy != 0 || previous.ClientRect != next.ClientRect)
            {
                StopSlide(next);
            }
            return;
        }
        if (dx == 0 && dy == 0)
        {
            return;
        }
        var now = clock.Elapsed.TotalMilliseconds;
        if (skillActive)
        {
            var tile = (next.Grid.Player.X - skillOrigin.X) * skillDirection.X + (next.Grid.Player.Y - skillOrigin.Y) * skillDirection.Y;
            var skillChangedAt = CoordinateChangeAgo?.Invoke() is { } ago && ago >= 0 ? now - ago : now - Math.Max(0, lag);
            skillChanges.Add((tile, skillChangedAt));
            AlignSkillStart(now);
            var timing = skillStartedAt is { } started
                ? $"　出発から {skillChangedAt - started:0} ms (予定 {(tile - 1) * skillTileMs:0} ms)　ずれ {(skillChangedAt - started - (tile - 1) * skillTileMs) / skillTileMs:+0.00;-0.00;0.00} マス"
                : "　動き出し前";
            Log($"座標認識: ({previous.Grid.Player.X}, {previous.Grid.Player.Y}) → ({next.Grid.Player.X}, {next.Grid.Player.Y})　遅れ {lag:0} ms　スキル {tile} マス目{timing}");
            return;
        }
        Log($"座標認識: ({previous.Grid.Player.X}, {previous.Grid.Player.Y}) → ({next.Grid.Player.X}, {next.Grid.Player.Y})　遅れ {lag:0} ms　描画位置 ({shownX:0.00}, {shownY:0.00})　スライド:{(slideTimer.Enabled ? "o" : "x")}　歩行:{(walking ? "o" : "x")}");
        continueBlocked = false; // 座標が変わった: 歩いている
        var tiles = Math.Max(Math.Abs(dx), Math.Abs(dy));
        var changedAt = now - Math.Max(0, lag);
        // 精度の記録: 読み取りが飛んだ (何マスも進んだ) ときは、1 歩ずつ前に歩き出したとみなす。
        var stepStartedAt = StepStartedAt(now) ?? changedAt;
        for (var i = 0; i < tiles; i++)
        {
            var from = (X: previous.Grid.Player.X + dx * i / tiles, Y: previous.Grid.Player.Y + dy * i / tiles);
            var to = (X: previous.Grid.Player.X + dx * (i + 1) / tiles, Y: previous.Grid.Player.Y + dy * (i + 1) / tiles);
            accuracy.AddStep(stepStartedAt - (tiles - 1 - i) * StepMs, from, to);
        }
        // 先読みしたマスに着いたと読み取れたら、そこまでを確かめられたものとして外す。道筋から外れたら先読みをやめる。
        if (predicted.Count > 0)
        {
            var index = predicted.IndexOf((next.Grid.Player.X, next.Grid.Player.Y));
            if (index >= 0)
            {
                var firstStep = index == 0 && (startCheckAt is not null || startWaitUntil is not null || startAlignPending);
                startAlignPending = false;
                predicted.RemoveRange(0, index + 1);
                Log($"先読み確認: ({next.Grid.Player.X}, {next.Grid.Player.Y})　残り {predicted.Count} マス", 1);
                predictDeadline = now + StepMs + ConfirmTimeoutMs;
                if (firstStep && tiles == 1 && StepStartedAt(now) is { } started && started >= keyPressedAt)
                {
                    // 1 歩目を座標欄の画素で確かめる前に読み取りが届いた: 歩いたのは確かなので、実際に歩き出した時刻に合わせる。
                    startCheckAt = null;
                    startWaitUntil = null;
                    PlaceOnStep(startOrigin, (dx, dy), started, now);
                    moveStartAt = now;
                    lastFrameAt = now;
                    Log($"歩き出し合わせ: キー入力から {started - keyPressedAt:0} ms で歩き出し (読み取り)　描画位置 ({shownX:0.00}, {shownY:0.00})", 2);
                }
            }
            else if (walking && tiles == 1)
            {
                // 先読みと違う隣のマスへ歩いた (曲がるタイミングの読み違い。ゲームがキーを反映するタイミングは一定でない):
                // 歩き続けたまま、実際に歩いた方向へ進路を直す。位置は実際に歩き出した時刻に合わせて置き直す。
                var from = (previous.Grid.Player.X, previous.Grid.Player.Y);
                var direction = (dx, dy);
                Log($"進路修正: 先読み {predicted[^1]} ではなく ({next.Grid.Player.X}, {next.Grid.Player.Y}) へ歩いた　方向 ({dx}, {dy})", 1);
                predicted.Clear();
                lastStepDirection = direction;
                continueCheckAt = null;
                stalledUntil = null;
                predictDeadline = now + StepMs + ConfirmTimeoutMs;
                PlaceOnStep(from, direction, StepStartedAt(now) ?? changedAt, now);
                lastFrameAt = now;
                moveStartAt = Math.Min(moveStartAt, now);
            }
            else
            {
                Log($"先読み中止: 不一致 (先読み {predicted[^1]}　認識 ({next.Grid.Player.X}, {next.Grid.Player.Y}))", 1);
                predicted.Clear();
                walking = false; // 先読みと違うところへ歩いた: 読み取った座標で動かす
            }
        }
        if (walking && double.IsPositiveInfinity(baselineAt))
        {
            // 歩き続けていて、着く前に読み取りで次の 1 歩が分かった (着いたときに比較元を取れなかった): この 1 歩の途中で取る。
            baselineAt = changedAt + StepMs / 2;
        }
        if (!slideTimer.Enabled && tiles >= SnapTiles)
        {
            Log($"即時移動: 停止位置から {tiles} マス ({previous.Grid.Player.X}, {previous.Grid.Player.Y}) → ({next.Grid.Player.X}, {next.Grid.Player.Y})", 1);
            StopSlide(next); // 止まっていたところから SnapTiles 以上離れた: 滑らせずにすぐ移す
            return;
        }
        if (!slideTimer.Enabled)
        {
            // 止まっていた (前の scene の位置にいる) ところから動き出す。キャラクターは座標欄が変わった時から歩いているので、
            // そこから今までに歩いた距離だけ進めたところから始める (1 マスを超えない)。
            // 歩き続けている間は、この先は同じ速さで進むので遅れは増えない。
            var ahead = Math.Clamp((now - stepStartedAt) / Math.Max(1, StepMs), 0, 1);
            shownX = previous.Grid.Player.X + (double)dx / tiles * ahead;
            shownY = previous.Grid.Player.Y + (double)dy / tiles * ahead;
            lastFrameAt = now;
            Log($"スライド開始: 座標認識 ({shownX:0.00}, {shownY:0.00})　{now - stepStartedAt:0} ms 前に歩き出し", 1);
            slideTimer.Start();
        }
        UpdateSlide(next);
    }

    /// <summary>スライドをやめて、読み取った位置にそのまま描く。</summary>
    /// <summary>スライド中なら止めて、読み取った座標の位置に描く (スライドをオフにしたときなど)。</summary>
    public void CancelSlide()
    {
        if (slideTimer.Enabled)
        {
            StopSlide(scene);
            Redraw();
        }
    }

    private void StopSlide(OverlayScene? current)
    {
        slideTimer.Stop();
        jumpTimer.Stop();
        jumpFrom = null;
        skillActive = false;
        offset = PointF.Empty;
        correctionX = correctionY = 0;
        LogSmoothness();
        LogAccuracy();
        predicted.Clear();
        walking = false;
        stalledUntil = null;
        startCheckAt = null;
        startWaitUntil = null;
        startAlignPending = false;
        stopWaitUntil = null;
        repredictStartAt = null;
    }

    private void SlideTimer_Tick(object? sender, EventArgs e)
    {
        if (scene is null)
        {
            StopSlide(null);
            return;
        }
        UpdateSlide(scene);
        Redraw();
    }

    private void UpdateSlide(OverlayScene current)
    {
        var now = clock.Elapsed.TotalMilliseconds;
        if (skillActive)
        {
            if (UpdateSkill(current, now))
            {
                return;
            }
            // スキルが終わった: ここからは読み取った座標で動かす。
            lastFrameAt = now;
            moveStartAt = now;
        }
        // キーを押してから歩き出すまでは動かさない。
        var dt = Math.Clamp(now - Math.Max(lastFrameAt, moveStartAt), 0, 100);
        lastFrameAt = now;
        if (predicted.Count > 0 && now > predictDeadline)
        {
            Log($"先読み中止: 確認タイムアウト ({predicted[^1]})");
            predicted.Clear(); // 確かめられなかった: 読み取った座標へ戻る
            walking = false;
            continueBlocked = true;
        }
        if (startCheckAt is { } startAt && now >= startAt)
        {
            startCheckAt = null;
            if (predicted.Count > 0 && CoordinateChanged?.Invoke("歩き出し確認", now - startAt) != true)
            {
                // まだ座標欄が変わらない: キーから歩き出すまでの時間はゲームのフレームの区切りでばらつく (実測 37〜85 ms) ので、
                // すぐにはやめずに、その場で止めて 1 歩の半分の時間まで変わるのを待つ。
                Log($"歩き出し待機: キー入力 {StartCheckMs} ms 後 座標画素変化なし　最大 {StepMs / 2:0} ms 待つ", 1);
                startWaitUntil = now + StepMs / 2;
                startWaitCheckedAt = now;
                predictDeadline = Math.Max(predictDeadline, now + StepMs / 2 + StepMs + ConfirmTimeoutMs);
            }
            else if (predicted.Count > 0)
            {
                Log($"歩き出し確認: キー入力 {StartCheckMs} ms 後 座標画素変化あり", 1);
                if (predicted.Count == 1)
                {
                    var guess = predicted[0];
                    VerifyStepDirection(startOrigin, (guess.X - startOrigin.X, guess.Y - startOrigin.Y));
                }
                if (StepStartedAt(now) is { } started && started >= keyPressedAt)
                {
                    // 実際に歩き出した時刻に合わせる (キーから歩き出しまでの時間は 37〜85 ms とばらつくので、推測より正確)。
                    var first = predicted[0];
                    PlaceOnStep(startOrigin, (first.X - startOrigin.X, first.Y - startOrigin.Y), started, now);
                    moveStartAt = now;
                    lastFrameAt = now;
                    dt = 0;
                    Log($"歩き出し合わせ: キー入力から {started - keyPressedAt:0} ms で歩き出し　描画位置 ({shownX:0.00}, {shownY:0.00})", 2);
                }
                else
                {
                    // 座標欄は変わったが、変わった時刻はまだ分からない (読み取りがまだ見ていない): 分かったら合わせる。
                    startAlignPending = true;
                }
            }
        }
        if (startAlignPending && predicted.Count == 1 && StepStartedAt(now) is { } lateStarted && lateStarted >= keyPressedAt)
        {
            startAlignPending = false;
            var first = predicted[0];
            PlaceOnStep(startOrigin, (first.X - startOrigin.X, first.Y - startOrigin.Y), lateStarted, now);
            moveStartAt = now;
            lastFrameAt = now;
            dt = 0;
            Log($"歩き出し合わせ: キー入力から {lateStarted - keyPressedAt:0} ms で歩き出し (遅れて判明)　描画位置 ({shownX:0.00}, {shownY:0.00})", 1);
        }
        if (startWaitUntil is { } waitUntil && predicted.Count > 0)
        {
            if (CoordinateChanged?.Invoke("歩き出し待機", null) == true)
            {
                // 変わった: このとき歩き出した (前に確かめた時刻と今の真ん中とみなす)。その時刻から今までに進んだはずの位置へ直接移す。
                startWaitUntil = null;
                if (predicted.Count == 1)
                {
                    var guess = predicted[0];
                    VerifyStepDirection(startOrigin, (guess.X - startOrigin.X, guess.Y - startOrigin.Y));
                }
                var startedAt = StepStartedAt(now) is { } known && known >= keyPressedAt ? known : (startWaitCheckedAt + now) / 2;
                var ahead = Math.Min(1, (now - startedAt) / Math.Max(1, StepMs));
                var first = predicted[0];
                MoveShown(startOrigin.X + (first.X - startOrigin.X) * ahead, startOrigin.Y + (first.Y - startOrigin.Y) * ahead, now);
                moveStartAt = now;
                lastFrameAt = now;
                dt = 0;
                baselineAt = startedAt + StepMs / 2;
                Log($"歩き出し確認: 待機中に座標画素変化あり　キー入力から約 {startedAt - keyPressedAt:0} ms で歩き出し　描画位置 ({shownX:0.00}, {shownY:0.00}) へ直接移動", 1);
            }
            else if (now > waitUntil)
            {
                // 待っても変わらなかった: 歩いていない (向きを変えただけなど)。先読みをやめて元のマスへ戻す。
                startWaitUntil = null;
                Log($"先読み中止: 待機しても座標画素変化なし ({predicted[^1]})", 1);
                predicted.Clear();
                walking = false;
                continueBlocked = true;
                moveStartAt = now;
            }
            else
            {
                startWaitCheckedAt = now;
                dt = 0; // 待っている間はその場で止める
            }
        }
        else if (startWaitUntil is not null)
        {
            startWaitUntil = null;
        }
        double playerX = current.Grid.Player.X, playerY = current.Grid.Player.Y;
        maxGap = Math.Max(maxGap, Math.Max(Math.Abs(playerX - shownX), Math.Abs(playerY - shownY)));
        if (Math.Max(Math.Abs(playerX - shownX), Math.Abs(playerY - shownY)) >= SnapTiles)
        {
            // 描いている位置が読み取った座標から SnapTiles 以上離れた: 滑らせずにすぐ移す。
            Log($"即時移動: 描画位置 ({shownX:0.00}, {shownY:0.00}) と座標認識 ({playerX}, {playerY}) の差 {Math.Max(Math.Abs(playerX - shownX), Math.Abs(playerY - shownY)):0.00} マス");
            StopSlide(current);
            return;
        }
        var (targetX, targetY) = predicted.Count > 0 ? ((double)predicted[^1].X, (double)predicted[^1].Y) : (playerX, playerY);
        var restX = targetX - shownX;
        var restY = targetY - shownY;
        // 斜めも 1 歩は 1 マス分の時間なので、x と y の大きいほうで距離を測り、両方が同時に着くように進める。
        var distance = Math.Max(Math.Abs(restX), Math.Abs(restY));
        var move = dt / Math.Max(1, StepMs);
        if (walking && now >= baselineAt && startWaitUntil is null)
        {
            baselineAt = double.PositiveInfinity;
            CaptureBaseline("1 歩の途中");
        }
        if (walking && continueCheckAt is { } checkAt && now >= checkAt)
        {
            // 着いてから少し経った: 座標欄が変わっていなければ歩いていない。続けて進めた分を取り消して、着いたマスへ戻す。
            continueCheckAt = null;
            if (CoordinateChanged?.Invoke("到着後の歩行判定", now - checkAt) != true && predicted.Count > 0)
            {
                // まだ変わらない: 1 歩の時間はゲームのフレームの区切りで少しばらつくので、戻さずにそのまま進みながら StallGraceMs まで待つ
                // (戻してから進め直すと、マスごとに小刻みに動いて見える)。待っても変わらなければ止まったとみなして戻す。
                Log($"歩行判定: 到着後 変化なし　次マス {predicted[^1]} へ進みながら最大 {StallGraceMs:0} ms 待つ", 1);
                stalledUntil = now + StallGraceMs;
                stalledCheckedAt = now;
            }
            else if (lastStepDirection is { } step)
            {
                // 先読みしたマスを読み取りがもう確かめていたら (先読みが空)、向かっている先は読み取った座標。
                (int X, int Y) next = predicted.Count > 0 ? predicted[^1] : ((int)playerX, (int)playerY);
                Log($"歩行判定: 到着後 変化あり　次マス {next}", 1);
                (int X, int Y) from = (next.X - step.X, next.Y - step.Y);
                step = VerifyStepDirection(from, step);
                next = (from.X + step.X, from.Y + step.Y);
                if (StepStartedAt(now) is { } started)
                {
                    // 実際に歩き出した時刻に合わせる。見た目はこのコマで進む分を進めたところからつなぐ (止まって見えないように)。
                    AdvanceShown(restX, restY, distance, move);
                    PlaceOnStep((next.X - step.X, next.Y - step.Y), step, started, now);
                    Log($"歩き出し合わせ: {now - started:0} ms 前に歩き出し　描画位置 ({shownX:0.00}, {shownY:0.00})", 2);
                    (targetX, targetY) = ((double)next.X, (double)next.Y);
                    restX = targetX - shownX;
                    restY = targetY - shownY;
                    distance = Math.Max(Math.Abs(restX), Math.Abs(restY));
                    move = 0; // 今の時刻に合わせて置いたので、このコマではもう進めない
                }
            }
        }
        var stalledResumed = false;
        var resumedElapsed = 0.0;
        if (walking && stalledUntil is { } until)
        {
            if (CoordinateChanged?.Invoke("待機中の歩行判定", null) == true)
            {
                stalledUntil = null; // 遅れて変わった: 歩いている
                // 歩き出した時刻 (分からなければ、前に確かめた時刻と今の真ん中) に合わせる。進みながら待っていたので、少しだけ戻すことになる。
                var started = StepStartedAt(now) ?? (stalledCheckedAt + now) / 2;
                Log($"歩行判定: 遅れて変化あり　{now - started:0} ms 前に歩き出し", 1);
                if (lastStepDirection is { } step && predicted.Count > 0)
                {
                    var next = predicted[^1];
                    (int X, int Y) from = (next.X - step.X, next.Y - step.Y);
                    step = VerifyStepDirection(from, step);
                    next = (from.X + step.X, from.Y + step.Y);
                    AdvanceShown(restX, restY, distance, move);
                    PlaceOnStep(from, step, started, now);
                    (targetX, targetY) = ((double)next.X, (double)next.Y);
                    restX = targetX - shownX;
                    restY = targetY - shownY;
                    distance = Math.Max(Math.Abs(restX), Math.Abs(restY));
                    move = 0; // 今の時刻に合わせて置いたので、このコマではもう進めない
                }
            }
            else if (now > until)
            {
                stalledUntil = null;
                Log("歩行判定: 変化なし　停止", 1);
                walking = false; // 変わらなかった: 止まった。進めた分を取り消して、着いたマスへ戻す
                continueBlocked = true;
                if (predicted.Count > 0)
                {
                    predicted.RemoveAt(predicted.Count - 1);
                }
                (targetX, targetY) = predicted.Count > 0 ? ((double)predicted[^1].X, (double)predicted[^1].Y) : (playerX, playerY);
                restX = targetX - shownX;
                restY = targetY - shownY;
                distance = Math.Max(Math.Abs(restX), Math.Abs(restY));
            }
            else
            {
                stalledCheckedAt = now;
            }
        }
        if ((distance <= move || stalledResumed) && walking && continueCheckAt is null && stalledUntil is null
            && predicted.Count < MaxUnconfirmed && ContinueDirection?.Invoke((int)Math.Round(targetX), (int)Math.Round(targetY)) is { } d)
        {
            // 着いた: 止まらずに次のマスへ同じ速さで進み続ける (歩いているかは少し後に座標欄で確かめる)。
            if (predicted.Count == 0)
            {
                predictDeadline = now + StepMs + ConfirmTimeoutMs;
            }
            predicted.Add(((int)targetX + d.X, (int)targetY + d.Y));
            lastStepDirection = d;
            stopWaitUntil = null;
            Log($"到着: ({targetX:0}, {targetY:0})　方向 ({d.X}, {d.Y})　次マス {predicted[^1]}　先読み {predicted.Count} マス　描画位置 ({shownX:0.00}, {shownY:0.00})　1 歩 {StepMs:0} ms");
            baselineAt = now + StepMs / 2;
            if (stalledResumed)
            {
                // 座標欄が変わるのを確かめてから動き出したので、もう確かめなくてよい。
                // 戻しかけていた位置から滑らせず、歩き出してから今までに進んだはずの位置へ直接移す。
                var ahead = Math.Min(1, resumedElapsed / Math.Max(1, StepMs));
                shownX = targetX + d.X * ahead;
                shownY = targetY + d.Y * ahead;
                Log($"直接移動: 歩き出しから {resumedElapsed:0} ms　描画位置 ({shownX:0.00}, {shownY:0.00})", 2);
            }
            else if (repredictStartAt is { } resumeAt && resumeAt > now)
            {
                // 動いている途中のキー入力: キーを押してから歩き出すまでは、着いたマスで待ってから進む。
                moveStartAt = resumeAt;
                predictDeadline = resumeAt + StepMs + ConfirmTimeoutMs;
                baselineAt = resumeAt + StepMs / 2;
                continueCheckAt = resumeAt;
                shownX = targetX;
                shownY = targetY;
                Log($"到着待機: 歩き出しまで {resumeAt - now:0} ms", 1);
            }
            else
            {
                continueCheckAt = now;
                var over = move - distance;
                shownX = targetX + d.X * over;
                shownY = targetY + d.Y * over;
            }
            repredictStartAt = null;
        }
        else if (distance <= move)
        {
            shownX = targetX;
            shownY = targetY;
            // 次に歩く方向がない (キーを離した・壁) ときも、すぐには止めずに StopGraceMs までその場で座標欄が変わるのを待つ
            // (ゲームはキーを離す少し前の状態で次の 1 歩を決めるうえ、オーバーレイはゲームより少し早く着くことが多い)。
            // 比較元がこの 1 歩の途中で取ったものでなければ (1 歩より古い) 使えない: 前の歩の変化を次の歩と間違える。
            var walkedOn = false;
            if (walking && continueCheckAt is null && stalledUntil is null && lastStepDirection is { } waiting && predicted.Count < MaxUnconfirmed
                && now - baselineTakenAt <= StepMs + StopGraceMs)
            {
                walkedOn = CoordinateChanged?.Invoke(stopWaitUntil is null ? "到着時の歩行確認" : "停止待ちの歩行確認", null) == true;
                if (!walkedOn && stopWaitUntil is null)
                {
                    stopWaitUntil = now + StopGraceMs;
                    stopWaitCheckedAt = now;
                    Log($"到着: ({targetX:0}, {targetY:0})　次の方向なし (キー離し・壁)　止まる前に最大 {StopGraceMs:0} ms 待つ", 1);
                }
                if (!walkedOn && now <= stopWaitUntil)
                {
                    stopWaitCheckedAt = now;
                    offset = DrawOffset(current, playerX, playerY, now);
                    return;
                }
                stopWaitUntil = null;
            }
            if (walkedOn && (ActiveDirection?.Invoke((int)Math.Round(targetX), (int)Math.Round(targetY)) ?? lastStepDirection) is { } last)
            {
                // 歩いた方向は、今効いているキーの方向 (前のキーの方向が壁なら、ゲームは今のキーの方向へ歩く)。キーがなければ前と同じ方向。
                lastStepDirection = last;
                // 次に歩く方向がない (キーを離した・壁) が、座標欄はもう変わっている: ゲームは着く前 (キーを離す直前) に次の 1 歩を
                // 始めていた、または壁のデータが違う。歩いた証拠があるので、同じ方向へ止めずに進み続ける (違えば読み取った座標で戻す)。
                predicted.Add(((int)Math.Round(targetX) + last.X, (int)Math.Round(targetY) + last.Y));
                last = VerifyStepDirection(((int)Math.Round(targetX), (int)Math.Round(targetY)), last);
                if (predicted.Count == 1)
                {
                    predictDeadline = now + StepMs + ConfirmTimeoutMs;
                }
                baselineAt = now + StepMs / 2;
                var over = move - distance;
                shownX = targetX + last.X * over;
                shownY = targetY + last.Y * over;
                if (StepStartedAt(now) is { } started)
                {
                    PlaceOnStep(((int)Math.Round(targetX), (int)Math.Round(targetY)), last, started, now);
                }
                else if (stopWaitCheckedAt > 0 && now - stopWaitCheckedAt < StepMs)
                {
                    // 待っている間に変わった: 前に確かめた時刻と今の真ん中で歩き出したとみなす。
                    PlaceOnStep(((int)Math.Round(targetX), (int)Math.Round(targetY)), last, (stopWaitCheckedAt + now) / 2, now);
                }
                stopWaitCheckedAt = 0;
                Log($"到着: ({targetX:0}, {targetY:0})　次の方向なし (キー離し・壁) だが座標画素変化あり　方向 ({last.X}, {last.Y}) の次マス {predicted[^1]} へ進む");
                offset = DrawOffset(current, playerX, playerY, now);
                return;
            }
            if (walking && continueCheckAt is null && stalledUntil is null)
            {
                Log($"到着: ({targetX:0}, {targetY:0})　次の方向なし (キー離し・壁)");
                walking = false; // 次に歩く方向がない (キーを離した・壁): この先は読み取った座標で動かす
                repredictStartAt = null;
            }
            if (!walking && predicted.Count == 0 && !continueBlocked && ContinueDirection?.Invoke((int)Math.Round(targetX), (int)Math.Round(targetY)) is { } held)
            {
                // 止まる直前: まだ移動キーを押し続けている。歩き続けるとみなして、止めずに次のマスへ進み続ける
                // (歩いているかは、2 歩目と同じく少し後に座標欄で確かめる)。
                walking = true;
                predicted.Add(((int)Math.Round(targetX) + held.X, (int)Math.Round(targetY) + held.Y));
                lastStepDirection = held;
                predictDeadline = now + StepMs + ConfirmTimeoutMs;
                // 座標欄はもう着いたマスを表しているので、今の見た目を比べる元にする。
                baselineAt = double.PositiveInfinity;
                Log($"停止直前キー継続: ({targetX:0}, {targetY:0})　方向 ({held.X}, {held.Y})　次マス {predicted[^1]}");
                CaptureBaseline("停止直前キー継続");
                continueCheckAt = now;
                var over = move - distance;
                shownX = targetX + held.X * over;
                shownY = targetY + held.Y * over;
                offset = DrawOffset(current, playerX, playerY, now);
                return;
            }
            if (!walking && predicted.Count == 0)
            {
                LogSmoothness();
                LogAccuracy();
                Log($"-------------スライド終了-------------: ({targetX:0}, {targetY:0})");
                slideTimer.Stop();
                offset = PointF.Empty;
                return;
            }
            // 着いたマスがまだ確かめられていない、または歩き続けたかを見る前: そこで待つ。
        }
        else
        {
            shownX += restX / distance * move;
            shownY += restY / distance * move;
        }
        // 格子 (Player = 読み取った座標) で、(shownX, shownY) にいるように見えるずれ。
        offset = DrawOffset(current, playerX, playerY, now);
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            var parameters = base.CreateParams;
            // クリックはゲームに素通しし、フォーカスも奪わない。Alt+Tab にも出さない。常に最前面。
            parameters.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TOPMOST;
            return parameters;
        }
    }

    /// <summary>null なら隠す。lagMs は座標が変わってから描くまでの遅れの見積もり (歩く速さを測るのに使う)。</summary>
    public void ShowScene(OverlayScene? next, double lagMs = 0)
    {
        if (next is null)
        {
            scene = null;
            StopSlide(null);
            if (Visible)
            {
                Hide();
            }
            return;
        }
        if (Bounds != next.ClientRect)
        {
            Bounds = next.ClientRect;
            if (next == scene)
            {
                Redraw(); // 大きさも描いた画像で決まるので描き直す
            }
        }
        if (next != scene)
        {
            if (scene is { } previous)
            {
                StartSlide(previous, next, lagMs);
            }
            else
            {
                StopSlide(next);
            }
            scene = next;
            Redraw();
        }
        if (!Visible && !suppressed)
        {
            Show();
        }
    }

    // 計測用: 環境変数 GMO_DRAWLOG があれば、描いたコマごとに (時刻, マス (0, 0) の画面上の位置) を書き出す。
    private static readonly StreamWriter? DrawLog = Environment.GetEnvironmentVariable("GMO_DRAWLOG") is { Length: > 0 } path ? new StreamWriter(path, append: false) { AutoFlush = true } : null;

    // ---- 描画 --------------------------------------------------------------------
    // 透明度付きの画像を描いて UpdateLayeredWindow で渡す。透過色を使わないのでアンチエイリアスをかけられる。
    // 透明度は色ごとに持つ (OverlayScene の色のアルファ)。

    private Bitmap? layeredBitmap;
    private int lastLayeredError;

    /// <summary>アンチエイリアスをかけるか。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public bool AntiAlias
    {
        get => antiAliasEnabled;
        set
        {
            if (value != antiAliasEnabled)
            {
                antiAliasEnabled = value;
                Redraw();
            }
        }
    }

    private bool antiAliasEnabled = true;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Redraw();
    }

    /// <summary>今の scene と offset で描き直す。</summary>
    private void Redraw()
    {
        if (scene is not { } current || !IsHandleCreated || Width <= 0 || Height <= 0)
        {
            return;
        }
        if (layeredBitmap is null || layeredBitmap.Size != Size)
        {
            layeredBitmap?.Dispose();
            layeredBitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        }
        ComposeChunks(current, AntiAlias);
        var error = Native.LayeredWindow.Update(Handle, layeredBitmap, Location, 255);
        if (error != 0 && error != lastLayeredError)
        {
            Log($"オーバーレイ: UpdateLayeredWindow 失敗 (エラー {error})");
        }
        lastLayeredError = error;
        DrawLog?.WriteLine(FormattableString.Invariant($"{System.Diagnostics.Stopwatch.GetTimestamp()},{current.Grid.GridPoint(0, 0).X + offset.X:0.00},{current.Grid.GridPoint(0, 0).Y + offset.Y:0.00}"));
    }

    private void ResetTerrainCaches()
    {
        foreach (var chunk in chunks.Values)
        {
            chunk.Dispose();
        }
        chunks.Clear();
        chunkRenderedAt.Clear();
        chunkKey = null;
    }

    // ---- 地形の画像 (チャンク) ------------------------------------------------------------
    // 地形を「ワールド座標」(格子点 (0, 0) を原点にした画素の位置。キャラクターの位置によらない) で
    // ChunkSize 画素の正方形のチャンクに分けて描いておき、歩いても描き直さずに、貼る位置を変えるだけにする。
    // 画面にかかるチャンクがなければその場で描き、周り 1 チャンクのうち足りないものは 1 コマに 1 つずつ描き足す。
    // 周り ChunkKeep チャンクより遠いものは捨てる。中身 (マスの種類・色・格子・マスの大きさ・アンチエイリアス) が変わったら全部捨てる。
    // チャンクは画素の正方形なので、隣のチャンクとは同じ形を同じ位置で描いて切り分けるだけになり、継ぎ目は出ない。

    /// <summary>地形の中身が同じか (キャラクターの位置や除外範囲は見ない)。</summary>
    private sealed record TerrainKey(IReadOnlyList<(OverlayLayer Layer, TileRects Tiles, Color Color)> Layers, Color? GridColor, double TileWidth, double TileHeight, bool AntiAlias)
    {
        public bool Equals(TerrainKey? other) =>
            other is not null && GridColor == other.GridColor && TileWidth == other.TileWidth && TileHeight == other.TileHeight
            && AntiAlias == other.AntiAlias && Layers.SequenceEqual(other.Layers);

        public override int GetHashCode() => HashCode.Combine(Layers.Count, GridColor, TileWidth, TileHeight, AntiAlias);

        public static TerrainKey Of(OverlayScene scene, bool antiAlias) =>
            new(scene.Layers, scene.GridColor, scene.Grid.TileWidth, scene.Grid.TileHeight, antiAlias);

        /// <summary>previous から何が変わったか (ログ用)。</summary>
        public string DescribeChange(TerrainKey previous)
        {
            var changes = new List<string>();
            if (!Layers.SequenceEqual(previous.Layers))
            {
                changes.Add($"マスの種類 [{string.Join(", ", previous.Layers.Select(Describe))}] → [{string.Join(", ", Layers.Select(Describe))}]");
            }
            if (GridColor != previous.GridColor)
            {
                changes.Add($"格子の色 {previous.GridColor} → {GridColor}");
            }
            if (TileWidth != previous.TileWidth || TileHeight != previous.TileHeight)
            {
                changes.Add(FormattableString.Invariant($"マスの大きさ {previous.TileWidth:R}x{previous.TileHeight:R} → {TileWidth:R}x{TileHeight:R}"));
            }
            if (AntiAlias != previous.AntiAlias)
            {
                changes.Add($"アンチエイリアス {previous.AntiAlias} → {AntiAlias}");
            }
            return string.Join("、", changes);

            static string Describe((OverlayLayer Layer, TileRects Tiles, Color Color) item) =>
                $"{item.Layer.Key}:{item.Tiles.Name}#{System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(item.Tiles)}:{item.Color}";
        }
    }

    /// <summary>画面の左上のワールド座標 (画素、四捨五入)。</summary>
    private Point ViewOrigin(OverlayScene current)
    {
        var grid = current.Grid;
        var player = grid.Delta(grid.Player.X + 0.5, grid.Player.Y + 0.5);
        return new Point((int)Math.Round(player.X - grid.PlayerCenter.X - offset.X), (int)Math.Round(player.Y - grid.PlayerCenter.Y - offset.Y));
    }

    /// <summary>ワールド座標の area を、その大きさの画像に描く (キャラクターの位置によらない)。</summary>
    private static Bitmap RenderWorld(OverlayScene current, Rectangle area, bool antiAlias)
    {
        var bitmap = new Bitmap(area.Width, area.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = antiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
        // Player = (0, 0) の格子で GridPoint(g) = PlayerCenter + Delta(g) - Delta(0.5, 0.5)。これが Delta(g) - area の左上になるように置く。
        var center = current.Grid.Delta(0.5, 0.5);
        var grid = current.Grid with { Player = new Parsing.GameCoordinate(0, 0), PlayerCenter = new PointF(center.X - area.X, center.Y - area.Y) };
        DrawTerrain(graphics, current, grid, area.Size);
        return bitmap;
    }

    /// <summary>重ねた後に、描かない範囲を透明に戻して、自分で描いたマスとキャラクターのマスを描く。</summary>
    private static void FinishCompose(Graphics graphics, OverlayScene current, bool antiAlias)
    {
        graphics.CompositingMode = CompositingMode.SourceCopy;
        using (var clear = new SolidBrush(Color.Transparent))
        {
            foreach (var rect in ExcludedRects(current))
            {
                graphics.FillRectangle(clear, rect);
            }
        }
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.SmoothingMode = antiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
        if (current.Custom is { Count: > 0 })
        {
            var state = graphics.Save();
            foreach (var rect in ExcludedRects(current))
            {
                graphics.ExcludeClip(rect);
            }
            DrawCustom(graphics, current, current.ClientRect.Size);
            graphics.Restore(state);
        }
        DrawPlayer(graphics, current);
    }

    private const int ChunkSize = 256;
    private const int ChunkKeep = 2;
    private readonly Dictionary<Point, Bitmap> chunks = [];
    private readonly Dictionary<Point, double> chunkRenderedAt = []; // チャンクを描いた時刻 (チャンクの表示用)

    /// <summary>チャンクの境目と番号を描くか (確かめる用)。描いたばかりのチャンクは ChunkFlashMs の間だけ色を付ける。</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public bool ShowChunks
    {
        get => showChunks;
        set
        {
            if (value != showChunks)
            {
                showChunks = value;
                Redraw();
            }
        }
    }

    private bool showChunks;
    private const double ChunkFlashMs = 500;

    private void DrawChunkGuides(Graphics graphics, Rectangle view, int left, int top, int right, int bottom)
    {
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.SmoothingMode = SmoothingMode.None;
        var now = clock.Elapsed.TotalMilliseconds;
        using var border = new Pen(Color.FromArgb(220, 0, 255, 255)) { DashStyle = DashStyle.Dash };
        using var flash = new SolidBrush(Color.FromArgb(70, 255, 255, 0));
        using var font = new Font("Segoe UI", 9f, FontStyle.Bold);
        using var text = new SolidBrush(Color.FromArgb(230, 0, 255, 255));
        using var shadow = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
        for (var cy = top; cy <= bottom; cy++)
        {
            for (var cx = left; cx <= right; cx++)
            {
                var rect = new Rectangle(cx * ChunkSize - view.X, cy * ChunkSize - view.Y, ChunkSize, ChunkSize);
                var age = chunkRenderedAt.TryGetValue(new Point(cx, cy), out var at) ? now - at : double.MaxValue;
                if (age < ChunkFlashMs)
                {
                    graphics.FillRectangle(flash, rect);
                }
                graphics.DrawRectangle(border, rect);
                var label = $"({cx}, {cy})";
                graphics.DrawString(label, font, shadow, rect.X + 5, rect.Y + 5);
                graphics.DrawString(label, font, text, rect.X + 4, rect.Y + 4);
            }
        }
    }
    private TerrainKey? chunkKey;

    private void ComposeChunks(OverlayScene current, bool antiAlias)
    {
        if (current.Layers.Count == 0 && current.GridColor is null)
        {
            // 地形に描くものがない (プレイヤーの枠・自分で描いたマスだけ): チャンクは作らない。持っていたものも捨てる。
            if (chunkKey is not null)
            {
                Log("地形: 描画対象なし、全チャンク破棄");
                ResetTerrainCaches();
            }
            using var empty = Graphics.FromImage(layeredBitmap!);
            empty.Clear(Color.Transparent);
            FinishCompose(empty, current, antiAlias);
            return;
        }
        var key = TerrainKey.Of(current, antiAlias);
        if (chunkKey != key)
        {
            Log(chunkKey is null ? "地形: 初回" : $"地形: 内容変更 ({key.DescribeChange(chunkKey)})、全チャンク破棄");
            ResetTerrainCaches();
            chunkKey = key;
        }
        var view = new Rectangle(ViewOrigin(current), Size);
        int Floor(int value) => (int)Math.Floor(value / (double)ChunkSize);
        var (left, top, right, bottom) = (Floor(view.Left), Floor(view.Top), Floor(view.Right - 1), Floor(view.Bottom - 1));
        Bitmap Chunk(int cx, int cy, string reason)
        {
            if (!chunks.TryGetValue(new Point(cx, cy), out var bitmap))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                bitmap = RenderWorld(current, new Rectangle(cx * ChunkSize, cy * ChunkSize, ChunkSize, ChunkSize), antiAlias);
                chunks[new Point(cx, cy)] = bitmap;
                chunkRenderedAt[new Point(cx, cy)] = clock.Elapsed.TotalMilliseconds;
                Log(FormattableString.Invariant($"地形: チャンク描画 ({cx}, {cy}) ({reason})　{watch.Elapsed.TotalMilliseconds:0.0} ms　保有チャンク {chunks.Count}"));
            }
            return bitmap;
        }
        using (var graphics = Graphics.FromImage(layeredBitmap!))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            for (var cy = top; cy <= bottom; cy++)
            {
                for (var cx = left; cx <= right; cx++)
                {
                    graphics.DrawImageUnscaled(Chunk(cx, cy, "表示範囲"), cx * ChunkSize - view.X, cy * ChunkSize - view.Y);
                }
            }
            if (ShowChunks)
            {
                DrawChunkGuides(graphics, view, left, top, right, bottom);
            }
            FinishCompose(graphics, current, antiAlias);
        }
        // 先に描いておく (1 コマに 1 つ)。
        var prefetched = false;
        for (var cy = top - 1; cy <= bottom + 1 && !prefetched; cy++)
        {
            for (var cx = left - 1; cx <= right + 1 && !prefetched; cx++)
            {
                if (!chunks.ContainsKey(new Point(cx, cy)))
                {
                    Chunk(cx, cy, "先読み");
                    prefetched = true;
                }
            }
        }
        var farChunks = chunks.Keys.Where(c => c.X < left - ChunkKeep || c.X > right + ChunkKeep || c.Y < top - ChunkKeep || c.Y > bottom + ChunkKeep).ToList();
        foreach (var far in farChunks)
        {
            chunks[far].Dispose();
            chunks.Remove(far);
            chunkRenderedAt.Remove(far);
        }
        if (farChunks.Count > 0)
        {
            Log($"地形: チャンク破棄 {string.Join(" ", farChunks.Select(c => $"({c.X}, {c.Y})"))}　保有チャンク {chunks.Count}");
        }
    }

    /// <summary>
    /// scene を 1 枚の画像にそのまま描く (背景は描かない、チャンクを使わない)。size はクライアント領域の大きさ。offset はマスをずらす量 (スライド中)。
    /// オーバーレイはチャンクに分けて描く (ComposeChunks) が、描く中身はこれと同じ (テストで使う)。
    /// </summary>
    internal static void Draw(Graphics graphics, OverlayScene current, Size size, PointF offset = default, bool antiAlias = false)
    {
        graphics.SmoothingMode = antiAlias ? SmoothingMode.AntiAlias : SmoothingMode.None;
        foreach (var rect in ExcludedRects(current))
        {
            graphics.ExcludeClip(rect);
        }
        // キャラクターは画面の中央から動かないので、ずらすのはマスだけ。自分で描いたマスもキャラクターに付いていくのでずらさない。
        DrawTerrain(graphics, current, Shift(current.Grid, offset), size);
        DrawCustom(graphics, current, size);
        DrawPlayer(graphics, current);
    }

    private static IsoGrid Shift(IsoGrid grid, PointF offset) =>
        grid with { PlayerCenter = new PointF(grid.PlayerCenter.X + offset.X, grid.PlayerCenter.Y + offset.Y) };

    /// <summary>描かない範囲 (マップ名欄・座標欄は少し広げる、ゲームの UI)。</summary>
    private static IEnumerable<Rectangle> ExcludedRects(OverlayScene current)
    {
        foreach (var excluded in new[] { current.Excluded1, current.Excluded2 })
        {
            if (excluded is { } rect)
            {
                rect.Inflate(4, 4);
                yield return rect;
            }
        }
        foreach (var rect in current.UiAreas ?? [])
        {
            yield return rect;
        }
    }

    /// <summary>格子とマス (地形) を描く。size は描く範囲 (この外のマスは描かない)。</summary>
    private static void DrawTerrain(Graphics graphics, OverlayScene current, IsoGrid grid, Size size)
    {
        var visible = new RectangleF(0, 0, size.Width, size.Height);
        if (current.GridColor is { } gridColor)
        {
            DrawGrid(graphics, grid, size, gridColor);
        }
        foreach (var (layer, tiles, color) in current.Layers)
        {
            // 下に描いた種類と重なるマスを小さく描く種類 (ShrinkOver) なら、その種類のマスを渡す。表示していなければ縮めない。
            var under = layer.ShrinkOver is { } shrinkOver ? current.Layers.FirstOrDefault(entry => entry.Layer == shrinkOver).Tiles : null;
            DrawTiles(graphics, grid, tiles, color, visible, under);
        }
    }

    /// <summary>自分で描いたマス。キャラクターのいるマスからの相対位置なので、キャラクターを (0, 0) にした格子で描く。ずらさない。</summary>
    private static void DrawCustom(Graphics graphics, OverlayScene current, Size size)
    {
        var grid = current.Grid with { Player = new Parsing.GameCoordinate(0, 0) };
        var visible = new RectangleF(0, 0, size.Width, size.Height);
        foreach (var tiles in current.Custom ?? [])
        {
            DrawCustomTiles(graphics, grid, tiles, visible);
        }
    }

    /// <summary>1 グループ分の自分で描いたマス。grid はキャラクターのいるマスを (0, 0) にした格子 (編集画面でも使う)。</summary>
    internal static void DrawCustomTiles(Graphics graphics, IsoGrid grid, CustomTiles tiles, RectangleF visible)
    {
        DrawTiles(graphics, CustomTileGroup.SubGrid(grid, tiles.Division), tiles.Fill, tiles.Color, visible);
        if (tiles.Frames.Rects.Length == 0)
        {
            return;
        }
        // 枠はプレイヤーの枠と同じ太さの線。
        using var pen = new Pen(tiles.Color, 2);
        foreach (var rect in tiles.Frames.Rects)
        {
            var tile = grid.Tile(rect[0], rect[1]);
            if (IsoGrid.Bounds(tile).IntersectsWith(visible))
            {
                graphics.DrawPolygon(pen, tile);
            }
        }
    }

    /// <summary>キャラクターのいるマス。位置合わせの目安にもなる。ずらさない。</summary>
    private static void DrawPlayer(Graphics graphics, OverlayScene current)
    {
        if (current.PlayerColor is not { } color)
        {
            return;
        }
        using var player = new Pen(color, 2);
        graphics.DrawPolygon(player, current.Grid.Tile(current.Grid.Player.X, current.Grid.Player.Y));
    }

    /// <summary>
    /// マスを塗って縁を描く。塗る前にその範囲を透明に戻すので、先に描いた種類 (やグリッド) と重なったマスはこの色だけになる
    /// (半透明でも色が混ざらない)。
    /// </summary>
    /// <param name="under">これと重なるマスは ShrinkScale 倍に縮めて描く (下のマスが周りに見えるように)。</param>
    internal static void DrawTiles(Graphics graphics, IsoGrid grid, TileRects tiles, Color color, RectangleF visible, TileRects? under = null)
    {
        // 隣り合う長方形 (マス) の境目に縁を描かないよう、長方形の辺を集めて 2 回出てきた部分 (内側の境目) を消す。
        // 辺は線 (縦か, 位置) ごとに区間の端を集める。横の線 y は (from, y)-(to, y)、縦の線 x は (x, from)-(x, to)。
        // 端を並べて 2 つずつ組にすると、奇数回覆われた部分 (= 縁) になる (細かい格子でも長方形の数だけで済む)。
        var edges = new Dictionary<(bool Vertical, int Line), List<int>>();
        void Toggle(bool vertical, int line, int from, int to)
        {
            if (!edges.TryGetValue((vertical, line), out var ends))
            {
                edges[(vertical, line)] = ends = [];
            }
            ends.Add(from);
            ends.Add(to);
        }
        var polygons = new List<PointF[]>();
        var smalls = new List<PointF[]>(); // 縮めたマス (縁をそのまま描く)
        foreach (var rect in tiles.Rects)
        {
            var polygon = grid.Polygon(rect[0], rect[1], rect[2], rect[3]);
            if (!IsoGrid.Bounds(polygon).IntersectsWith(visible))
            {
                continue; // 画面外 (マップの大部分) は描かない
            }
            if (under is null || !Cells(rect).Any(cell => under.Contains(cell.X, cell.Y)))
            {
                polygons.Add(polygon);
                Toggle(false, rect[1], rect[0], rect[0] + rect[2]);
                Toggle(false, rect[1] + rect[3], rect[0], rect[0] + rect[2]);
                Toggle(true, rect[0], rect[1], rect[1] + rect[3]);
                Toggle(true, rect[0] + rect[2], rect[1], rect[1] + rect[3]);
                continue;
            }
            // 一部でも重なる長方形は、マスごとに大きさを変えて描く。縮めたマスは離れているので縁をそのまま描く。
            foreach (var (x, y) in Cells(rect))
            {
                if (under.Contains(x, y))
                {
                    smalls.Add(grid.Tile(x, y, ShrinkScale));
                    continue;
                }
                polygons.Add(grid.Tile(x, y));
                Toggle(false, y, x, x + 1);
                Toggle(false, y + 1, x, x + 1);
                Toggle(true, x, y, y + 1);
                Toggle(true, x + 1, y, y + 1);
            }
        }
        if (polygons.Count == 0 && smalls.Count == 0)
        {
            return;
        }
        // 先に全部を透明に戻してから塗る (1 つずつ戻すと、隣の長方形のアンチエイリアスの縁を消して継ぎ目が出る)。
        // 戻すときはアンチエイリアスをかけない (縁の画素が半端に残って下の色と混ざらないように)。
        var (compositing, smoothing) = (graphics.CompositingMode, graphics.SmoothingMode);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.SmoothingMode = SmoothingMode.None;
        using (var clear = new SolidBrush(Color.Transparent))
        {
            foreach (var polygon in polygons.Concat(smalls))
            {
                graphics.FillPolygon(clear, polygon);
            }
        }
        (graphics.CompositingMode, graphics.SmoothingMode) = (compositing, smoothing);
        using var fill = new SolidBrush(color);
        using var edge = new Pen(Color.FromArgb(color.A, ControlPaint.Light(color)));
        foreach (var polygon in polygons)
        {
            graphics.FillPolygon(fill, polygon);
        }
        foreach (var small in smalls)
        {
            graphics.FillPolygon(fill, small);
            graphics.DrawPolygon(edge, small);
        }
        foreach (var ((vertical, line), ends) in edges)
        {
            ends.Sort();
            for (var i = 0; i + 1 < ends.Count; i += 2)
            {
                var (from, to) = (ends[i], ends[i + 1]);
                // 続いている区間 (長方形の辺が並んだところ) は 1 本の線にする。
                while (i + 3 < ends.Count && ends[i + 2] == to)
                {
                    to = ends[i + 3];
                    i += 2;
                }
                if (from < to)
                {
                    graphics.DrawLine(edge, vertical ? grid.GridPoint(line, to) : grid.GridPoint(from, line), vertical ? grid.GridPoint(line, from) : grid.GridPoint(to, line));
                }
            }
        }
    }

    /// <summary>画面に入るマスの境目 (格子線) を描く。</summary>
    private static void DrawGrid(Graphics graphics, IsoGrid grid, Size size, Color color)
    {
        // 画面の四隅があるマスから、格子線を引く範囲を決める (1 マス余分に)。
        var corners = new[] { new PointF(0, 0), new PointF(size.Width, 0), new PointF(0, size.Height), new PointF(size.Width, size.Height) }
            .Select(grid.TileAt).ToArray();
        var minX = corners.Min(tile => tile.X) - 1;
        var maxX = corners.Max(tile => tile.X) + 2;
        var minY = corners.Min(tile => tile.Y) - 1;
        var maxY = corners.Max(tile => tile.Y) + 2;
        using var pen = new Pen(color);
        for (var x = minX; x <= maxX; x++)
        {
            graphics.DrawLine(pen, grid.GridPoint(x, minY), grid.GridPoint(x, maxY));
        }
        for (var y = minY; y <= maxY; y++)
        {
            graphics.DrawLine(pen, grid.GridPoint(minX, y), grid.GridPoint(maxX, y));
        }
    }

    /// <summary>ShrinkOver で重なるマスを縮める倍率。</summary>
    internal const double ShrinkScale = 0.6;

    private static IEnumerable<(int X, int Y)> Cells(int[] rect)
    {
        for (var y = rect[1]; y < rect[1] + rect[3]; y++)
        {
            for (var x = rect[0]; x < rect[0] + rect[2]; x++)
            {
                yield return (x, y);
            }
        }
    }
}
