namespace gGameMapOverlay;

/// <summary>移動速度の等級。Name は画面に出す名前、StepMs は 1 歩の時間 (ミリ秒)。</summary>
public readonly record struct MoveSpeedGrade(string Name, int StepMs);

/// <summary>
/// キャラクターの移動速度 (1 マス歩く時間) の等級と、座標欄の見た目が変わった間隔からの判定。
/// gGame の移動速度は 5 段階で、等級ごとに 1 歩の時間が決まっている。
/// 座標欄はゲームが 1 歩を歩き出した瞬間に変わるので、歩き続けている間に変わった間隔が 1 歩の時間になる。
/// 1 回ごとの間隔は撮影のタイミングで数 ms ずつ揺れるので、続けて歩いた何歩かの合計から平均を出して、いちばん近い等級に丸める。
/// 読み取りのスレッドから AddChange を呼び、画面のスレッドから結果を読む。
/// </summary>
public sealed class WalkSpeed
{
    public static readonly MoveSpeedGrade Grade0 = new("+0", 294);
    public static readonly MoveSpeedGrade Grade1 = new("+1", 277);
    public static readonly MoveSpeedGrade Grade2 = new("+2", 263);
    public static readonly MoveSpeedGrade Grade3 = new("+3", 250);
    public static readonly MoveSpeedGrade Grade4 = new("+4", 238);

    /// <summary>等級 (0〜4) の並び。設定には何番目かを保存する。</summary>
    public static readonly IReadOnlyList<MoveSpeedGrade> Grades = [Grade0, Grade1, Grade2, Grade3, Grade4];

    /// <summary>まだ判定も設定もしていないときの等級。</summary>
    public static readonly MoveSpeedGrade DefaultGrade = Grade0;

    /// <summary>等級が Grades の何番目か。なければ -1。</summary>
    public static int IndexOf(MoveSpeedGrade grade)
    {
        for (var index = 0; index < Grades.Count; index++)
        {
            if (Grades[index] == grade)
            {
                return index;
            }
        }
        return -1;
    }

    // 1 歩の間隔とみなす範囲。外れたら (止まってから歩き出した・スキルで一度に何マスも進んだなど) 続けて歩いた区切りとする。
    private const double MinIntervalMs = 218;
    private const double MaxIntervalMs = 314;
    // 判定に使う歩数。少ないと揺れの影響が大きく、多いと速度が変わったときに追いつくのが遅い。
    private const int MinSteps = 3;
    private const int MaxSteps = 8;

    private readonly Lock gate = new();
    // 続けて歩いている間に座標欄が変わった時刻 (最大 MaxSteps + 1 個)。
    private readonly Queue<double> run = new();
    private double? lastChangeAt;
    private double? measuredStepMs;
    private int? measuredGrade;

    public static MoveSpeedGrade Grade(int grade) => Grades[Math.Clamp(grade, 0, Grades.Count - 1)];

    /// <summary>1 歩の時間にいちばん近い等級。</summary>
    public static int NearestGrade(double stepMs)
    {
        var best = 0;
        for (var grade = 1; grade < Grades.Count; grade++)
        {
            if (Math.Abs(Grades[grade].StepMs - stepMs) < Math.Abs(Grades[best].StepMs - stepMs))
            {
                best = grade;
            }
        }
        return best;
    }

    /// <summary>判定した等級。まだ判定できていなければ null。止まったあとも最後の判定を残す。</summary>
    public int? MeasuredGrade
    {
        get
        {
            lock (gate)
            {
                return measuredGrade;
            }
        }
    }

    /// <summary>判定に使った 1 歩の時間の平均 (ミリ秒)。まだなければ null。</summary>
    public double? MeasuredStepMs
    {
        get
        {
            lock (gate)
            {
                return measuredStepMs;
            }
        }
    }

    /// <summary>座標欄の見た目が変わった時刻 (ミリ秒、増えていく時刻ならどれでもよい) を渡す。</summary>
    public void AddChange(double atMs)
    {
        lock (gate)
        {
            if (lastChangeAt is not { } last || atMs - last is < MinIntervalMs or > MaxIntervalMs)
            {
                run.Clear();
            }
            lastChangeAt = atMs;
            run.Enqueue(atMs);
            if (run.Count > MaxSteps + 1)
            {
                run.Dequeue();
            }
            var steps = run.Count - 1;
            if (steps >= MinSteps)
            {
                measuredStepMs = (atMs - run.Peek()) / steps;
                measuredGrade = NearestGrade(measuredStepMs.Value);
            }
        }
    }
}
