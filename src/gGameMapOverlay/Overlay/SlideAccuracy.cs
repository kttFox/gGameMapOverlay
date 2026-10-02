namespace gGameMapOverlay.Overlay;

/// <summary>
/// スライドの精度の記録。描いた位置と、ゲームのキャラクターがいるはずの位置を比べる。
/// キャラクターは座標欄が変わった瞬間 (1 歩を歩き出した時刻) から、1 歩の時間をかけて次のマスへ等速で進むとみなす。
/// 1 歩ごとの歩き出した時刻とマスは、読み取った座標が届いてから分かるので、止まったときにまとめて比べる。
/// </summary>
internal sealed class SlideAccuracy
{
    private readonly List<(double At, double X, double Y)> frames = [];
    private readonly List<(double At, (int X, int Y) From, (int X, int Y) To)> steps = [];

    public bool IsEmpty => frames.Count == 0;

    /// <summary>描いた位置 (マス単位) を記録する。</summary>
    public void AddFrame(double at, double x, double y) => frames.Add((at, x, y));

    /// <summary>from から to (隣のマス) へ、startedAt に歩き出した 1 歩を記録する。</summary>
    public void AddStep(double startedAt, (int X, int Y) from, (int X, int Y) to) => steps.Add((startedAt, from, to));

    public void Clear()
    {
        frames.Clear();
        steps.Clear();
    }

    /// <summary>時刻 at にキャラクターがいるはずの位置。歩いていなければ null。</summary>
    public (double X, double Y)? Expected(double at, double stepMs)
    {
        if (steps.Count == 0)
        {
            return null;
        }
        var index = steps.FindLastIndex(step => step.At <= at);
        if (index < 0)
        {
            return steps[0].From;
        }
        var (startedAt, from, to) = steps[index];
        var progress = Math.Clamp((at - startedAt) / Math.Max(1, stepMs), 0, 1);
        return (from.X + (to.X - from.X) * progress, from.Y + (to.Y - from.Y) * progress);
    }

    /// <summary>
    /// 記録したコマのずれ。Mean・Max はずれの大きさ (マス)、LeadMs は進む向きのずれの平均を時間にしたもの (正なら先に進みすぎ)。
    /// MaxStep は最大のずれが何歩目 (0 は歩き出す前) で、MaxAt はその 1 歩を歩き出してからの時間。
    /// 比べられるコマがなければ null。
    /// </summary>
    public (int Frames, double Mean, double Max, double LeadMs, double MaxAt, int MaxStep)? Summarize(double stepMs)
    {
        var count = 0;
        double sum = 0, max = 0, lead = 0, maxAt = 0;
        var maxStep = 0;
        foreach (var (at, x, y) in frames)
        {
            if (Expected(at, stepMs) is not { } expected)
            {
                continue;
            }
            var dx = x - expected.X;
            var dy = y - expected.Y;
            var error = Math.Max(Math.Abs(dx), Math.Abs(dy));
            sum += error;
            // 進む向き: そのときの 1 歩の向き (歩き出す前は最初の 1 歩の向き)。
            var index = Math.Max(0, steps.FindLastIndex(step => step.At <= at));
            if (error > max)
            {
                max = error;
                maxAt = at - steps[index].At;
                maxStep = steps.FindLastIndex(step => step.At <= at) + 1;
            }
            var (_, from, to) = steps[index];
            var (sx, sy) = (to.X - from.X, to.Y - from.Y);
            lead += (dx * sx + dy * sy) / Math.Max(1, Math.Abs(sx) + Math.Abs(sy));
            count++;
        }
        return count == 0 ? null : (count, sum / count, max, lead / count * stepMs, maxAt, maxStep);
    }
}
