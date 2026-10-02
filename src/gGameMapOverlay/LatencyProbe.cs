using System.Diagnostics;
using gGameMapOverlay.Imaging;

namespace gGameMapOverlay;

/// <summary>
/// 移動の遅れを測る。止まっている状態から移動キー (WASD・矢印キー) を押したとき、次の時刻を記録する:
/// キーを押した → 地形が動き始めた・座標欄の見た目が変わった (どちらも細かくキャプチャし続けて調べる) →
/// 変わった座標を読み取ったキャプチャ → 読み取りが終わった (OCR) → 描画を始めた。
/// キーは MoveKeyWatcher から OnMoveKeys で受け取る。
/// </summary>
internal sealed class LatencyProbe : IDisposable
{
    private const double TimeoutMs = 1500;
    private const int MaxResults = 10;

    /// <summary>1 回分の結果 (ミリ秒、キーを押した時刻から)。</summary>
    public sealed record Result(double Terrain, double Changed, double Captured, double Recognized, double Drawn);

    // 地形が動いたとみなす変化: 画素の色の差がこれを超えた画素が、範囲のこの割合を超えたとき
    // (他のキャラクターやモンスターが少し横切っただけでは反応しないように)。
    private const int TerrainPixelDiff = 30;
    private const double TerrainChangedRatio = 0.10;

    private readonly Stopwatch clock = Stopwatch.StartNew();
    private readonly Func<Rectangle?> coordinateRect;
    private readonly Func<Rectangle?> terrainRect;
    private readonly Func<bool> gameForeground;
    private readonly List<Result> results = [];

    // 測っている 1 回分。changedAt は別スレッドで書く。
    private double? keyAt;
    private double changedAt = double.NaN;
    private double terrainAt = double.NaN;
    private CancellationTokenSource? watchCancel;

    /// <param name="coordinateRect">座標欄の画面上の矩形 (見た目が変わったかを調べる)。</param>
    /// <param name="terrainRect">地形を見る画面上の矩形 (キャラクターから離れたところ)。</param>
    /// <param name="gameForeground">ゲームが前面にあるか (前面でないときのキーは測らない)。</param>
    public LatencyProbe(Func<Rectangle?> coordinateRect, Func<Rectangle?> terrainRect, Func<bool> gameForeground)
    {
        this.coordinateRect = coordinateRect;
        this.terrainRect = terrainRect;
        this.gameForeground = gameForeground;
    }

    public double Now => clock.Elapsed.TotalMilliseconds;

    private bool enabled;

    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            if (!value)
            {
                Cancel();
            }
        }
    }

    public IReadOnlyList<Result> Results => results;

    /// <summary>読み取りで座標が変わったとき呼ぶ。captureAt は読み取りのキャプチャを始めた時刻 (Now)。</summary>
    public void OnRecognized(double captureAt, double recognizedAt, double drawnAt)
    {
        if (keyAt is not { } key)
        {
            return;
        }
        var changed = Volatile.Read(ref changedAt);
        var terrain = Volatile.Read(ref terrainAt);
        Cancel();
        if (double.IsNaN(changed) || double.IsNaN(terrain))
        {
            return; // 座標欄・地形が変わった時刻を捉えられなかった
        }
        results.Add(new Result(terrain - key, changed - key, captureAt - key, recognizedAt - key, drawnAt - key));
        if (results.Count > MaxResults)
        {
            results.RemoveAt(0);
        }
    }

    /// <summary>直近の結果の平均の説明。まだなければ null。</summary>
    public string? Summary()
    {
        if (results.Count == 0)
        {
            return null;
        }
        double Average(Func<Result, double> value) => results.Average(value);
        var terrain = Average(r => r.Terrain);
        var drawn = Average(r => r.Drawn);
        return $"キーを押してからの時刻 (平均 {results.Count} 回): 地形が動く {terrain:0} / 座標が変わる {Average(r => r.Changed):0}"
            + $" / 読み取り開始 {Average(r => r.Captured):0} / 読み取り完了 {Average(r => r.Recognized):0} / 描画開始 {drawn:0} ms"
            + Environment.NewLine
            + $"地形→描画開始 {drawn - terrain:0} ms";
    }

    /// <summary>移動キーの組み合わせが変わったとき呼ぶ。止まっている状態からの最初の押し下げだけ測る。</summary>
    public void OnMoveKeys(int previous, int held)
    {
        if (enabled && previous == 0 && held != 0)
        {
            Start();
        }
    }

    private void Start()
    {
        if (keyAt is not null || !gameForeground() || coordinateRect() is not { } rect || terrainRect() is not { } terrainArea)
        {
            return;
        }
        var key = Now;
        keyAt = key;
        Volatile.Write(ref changedAt, double.NaN);
        Volatile.Write(ref terrainAt, double.NaN);
        var cancel = new CancellationTokenSource();
        watchCancel = cancel;
        // 座標欄を細かくキャプチャし続け、キーを押したときの見た目から変わった時刻を記録する。
        _ = Task.Run(() =>
        {
            try
            {
                var baseline = ScreenCapture.Capture(rect).Pixels;
                var terrainBaseline = ScreenCapture.Capture(terrainArea).Pixels;
                var coordinateDone = false;
                var terrainDone = false;
                while (!cancel.IsCancellationRequested && Now - key < TimeoutMs && !(coordinateDone && terrainDone))
                {
                    var now = Now;
                    if (!coordinateDone && !ScreenCapture.Capture(rect).Pixels.AsSpan().SequenceEqual(baseline))
                    {
                        Volatile.Write(ref changedAt, now);
                        coordinateDone = true;
                    }
                    if (!terrainDone && TerrainMoved(terrainBaseline, ScreenCapture.Capture(terrainArea).Pixels))
                    {
                        Volatile.Write(ref terrainAt, now);
                        terrainDone = true;
                    }
                    Thread.Yield();
                }
            }
            catch (Exception exception) when (exception is not OutOfMemoryException)
            {
                // キャプチャに失敗したら、この回は測らない。
            }
        });
    }

    private static bool TerrainMoved(byte[] baseline, byte[] current)
    {
        var changed = 0;
        for (var i = 0; i + 2 < baseline.Length; i += 3)
        {
            var diff = Math.Max(Math.Abs(baseline[i] - current[i]), Math.Max(Math.Abs(baseline[i + 1] - current[i + 1]), Math.Abs(baseline[i + 2] - current[i + 2])));
            if (diff > TerrainPixelDiff)
            {
                changed++;
            }
        }
        return changed > baseline.Length / 3 * TerrainChangedRatio;
    }

    /// <summary>時間切れの回を捨てる。読み取りのたびに呼ぶ。</summary>
    public void Expire()
    {
        if (keyAt is { } key && Now - key > TimeoutMs)
        {
            Cancel();
        }
    }

    private void Cancel()
    {
        watchCancel?.Cancel();
        watchCancel = null;
        keyAt = null;
    }

    public void Dispose() => Enabled = false;
}
