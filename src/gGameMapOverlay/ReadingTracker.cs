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

    public ReadingTracker(int confirmHits = 2, double holdSeconds = 3.0, Func<double>? clock = null)
    {
        ConfirmHits = confirmHits;
        HoldSeconds = holdSeconds;
        this.clock = clock ?? (() => Environment.TickCount64 / 1000.0);
    }

    // 設定画面で変えたときは、読み取り中でもそのまま差し替える。
    public int ConfirmHits { get; set => field = Math.Max(1, value); }
    public double HoldSeconds { get; set; }
    public string? MapName { get; private set; }
    public GameCoordinate? Coordinates { get; private set; }
    public bool HasPendingName => pendingKey is not null;
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

    /// <summary>null は読み取り失敗 (確定値は維持)。値が変わったら true。</summary>
    public bool UpdateCoordinates(GameCoordinate? coordinates)
    {
        if (coordinates is null)
        {
            return false;
        }
        coordinateSeenAt = clock();
        if (coordinates == Coordinates)
        {
            return false;
        }
        Coordinates = coordinates;
        return true;
    }

    private bool IsStale(double? seenAt) => seenAt is null || clock() - seenAt > HoldSeconds;
}
