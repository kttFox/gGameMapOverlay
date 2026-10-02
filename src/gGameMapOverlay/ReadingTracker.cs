using gGameMapOverlay.Parsing;

namespace gGameMapOverlay;

/// <summary>
/// OCR の揺れを吸収して、確定したマップ名と座標を保持する。
/// マップ名は同じ結果が confirmHits 回連続したら確定する。読み取りに失敗しても確定済みの値は消さず、
/// holdSeconds を超えて読めない状態が続いたら Stale として扱う。
/// </summary>
public sealed class ReadingTracker
{
    private readonly Func<double> clock;
    private string? currentKey;
    private string? pendingKey;
    private int pendingHits;
    private double? nameSeenAt;
    private double? coordinateSeenAt;
    private GameCoordinate? pendingCoordinates;
    private int pendingCoordinateHits;

    public ReadingTracker(int confirmHits = 2, double holdSeconds = 3.0, Func<double>? clock = null)
    {
        ConfirmHits = confirmHits;
        HoldSeconds = holdSeconds;
        this.clock = clock ?? (() => Environment.TickCount64 / 1000.0);
    }

    // 設定画面で変えたときは、読み取り中でもそのまま差し替える。
    public int ConfirmHits { get; set => field = Math.Max(1, value); }
    public double HoldSeconds { get; set; }

    /// <summary>座標が変わったとき、同じ座標がこの回数続いたら採用する (1 ならすぐ)。</summary>
    public int CoordinateConfirmHits { get; set => field = Math.Max(1, value); } = 1;
    public string? MapName { get; private set; }
    public GameCoordinate? Coordinates { get; private set; }
    public bool HasPendingName => pendingKey is not null;

    /// <summary>確かめている途中の座標 (まだ採用していない)。なければ null。</summary>
    public GameCoordinate? PendingCoordinates => pendingCoordinates;
    public bool NameStale => IsStale(nameSeenAt);
    public bool CoordinatesStale => IsStale(coordinateSeenAt);

    public void Reset()
    {
        MapName = null;
        Coordinates = null;
        currentKey = null;
        pendingKey = null;
        pendingHits = 0;
        nameSeenAt = null;
        coordinateSeenAt = null;
        pendingCoordinates = null;
        pendingCoordinateHits = 0;
    }

    /// <summary>key は同一性判定用。null は読み取り失敗。確定値が変わったら true。</summary>
    public bool UpdateName(string? key, string? displayName)
    {
        if (string.IsNullOrEmpty(key))
        {
            pendingKey = null;
            pendingHits = 0;
            return false;
        }
        var now = clock();
        if (key == currentKey)
        {
            pendingKey = null;
            pendingHits = 0;
            nameSeenAt = now;
            return false;
        }
        if (key == pendingKey)
        {
            pendingHits++;
        }
        else
        {
            pendingKey = key;
            pendingHits = 1;
        }
        if (pendingHits < ConfirmHits)
        {
            return false;
        }
        MapName = displayName;
        currentKey = key;
        pendingKey = null;
        pendingHits = 0;
        nameSeenAt = now;
        return true;
    }

    /// <summary>
    /// null は読み取り失敗 (確定値は維持)。値が変わったら true。
    /// 変わった座標は CoordinateConfirmHits 回続けて同じだったら採用する (見た目が変わらず読み直さないときも、前回読んだ座標を渡して数える)。
    /// </summary>
    public bool UpdateCoordinates(GameCoordinate? coordinates)
    {
        if (coordinates is null)
        {
            return false;
        }
        coordinateSeenAt = clock();
        if (coordinates == Coordinates)
        {
            pendingCoordinates = null;
            pendingCoordinateHits = 0;
            return false;
        }
        pendingCoordinateHits = coordinates == pendingCoordinates ? pendingCoordinateHits + 1 : 1;
        pendingCoordinates = coordinates;
        if (pendingCoordinateHits < CoordinateConfirmHits && Coordinates is not null)
        {
            return false; // まだ確かめている (最初の座標はすぐ採用する)
        }
        Coordinates = coordinates;
        pendingCoordinates = null;
        pendingCoordinateHits = 0;
        return true;
    }

    private bool IsStale(double? seenAt) => seenAt is null || clock() - seenAt > HoldSeconds;
}
