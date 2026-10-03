using System.Drawing;
using gGameMapOverlay.Imaging;
using gGameMapOverlay.Native;
using gGameMapOverlay.Ocr;
using gGameMapOverlay.Parsing;

namespace gGameMapOverlay;

public enum ReadingStatus
{
    Ok,
    NoWindow,
    Background,
    NeedNameRegion,
    NeedCoordinateRegion,
    BlackFrame,
    CaptureFailed,
}

public enum RegionKind
{
    Name,
    Coordinates,
}

public sealed record Reading
{
    public required ReadingStatus Status { get; init; }
    public string Message { get; init; } = "";
    public string? MapName { get; init; }
    public GameCoordinate? Coordinates { get; init; }
    public string? RawName { get; init; }
    public double NameScore { get; init; }
    public string? RawCoordinates { get; init; }
    public bool NameStale { get; init; } = true;
    public bool CoordinatesStale { get; init; } = true;

    /// <summary>UI の横の拡大率 (通常 1.0)。領域はこの倍率を掛けて読み取った。</summary>
    public double UiScale { get; init; } = 1.0;

    /// <summary>UI の縦の拡大率 (設定で固定した場合など、横と違うことがある)。</summary>
    public double UiScaleY { get; init; } = 1.0;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    public BgrImage? NameImage { get; init; }

    public BgrImage? CoordinateImage { get; init; }

    public bool Ok => Status == ReadingStatus.Ok;
}

/// <summary>ゲームウィンドウのキャプチャから、マップ名と座標の読み取りまでをまとめる。</summary>
public sealed class MapReader : IDisposable
{
    /// <summary>これ未満の認識スコアの結果は確定に使わない。</summary>
    public const double MinRawNameScore = 0.5;

    private static readonly Dictionary<ReadingStatus, string> Messages = new()
    {
        [ReadingStatus.Ok] = "読み取り中",
        [ReadingStatus.NoWindow] = "ゲームウィンドウが見つかりません",
        [ReadingStatus.Background] = "ゲームが隠れているため一時停止中",
        [ReadingStatus.NeedNameRegion] = "マップ名の領域を設定してください",
        [ReadingStatus.NeedCoordinateRegion] = "座標の領域を設定してください",
        [ReadingStatus.BlackFrame] = "画面が黒くキャプチャされました (ウィンドウモードで起動してください)",
        [ReadingStatus.CaptureFailed] = "キャプチャに失敗しました",
    };

    private readonly AppConfig config;
    private readonly bool ownsOcr;
    private nint hwnd;
    private double lastNameOcrAt = double.NegativeInfinity;
    private string? lastCoordinateSignature;
    private string? lastRawName;
    private double lastNameScore;
    private string? lastRawCoordinates;
    private UiTransform lastTransform = UiTransform.Identity;

    /// <param name="ownsOcr">false なら Dispose で OCR エンジンを解放しない (他の MapReader と共有する場合)。</param>
    public MapReader(AppConfig config, IOcrEngine ocr, bool ownsOcr = true)
    {
        this.config = config;
        this.ownsOcr = ownsOcr;
        Ocr = ocr;
        Tracker = new ReadingTracker(config.NameConfirmHits, config.NameHoldSeconds);
    }

    public IOcrEngine Ocr { get; private set; }

    /// <summary>
    /// 手動で選んだマップ名。null でなければマップ名を OCR で読まず、この名前を確定したものとして返す
    /// (OCR でマップ名をうまく読めないときのため)。
    /// </summary>
    public string? ManualMapName { get; set; }
    public ReadingTracker Tracker { get; }

    public void ReplaceOcr(IOcrEngine ocr)
    {
        var old = Ocr;
        Ocr = ocr;
        old.Dispose();
        Invalidate();
    }

    /// <summary>設定変更後、次回は必ずマップ名と座標の両方を読み直す。</summary>
    public void Invalidate()
    {
        lastNameOcrAt = double.NegativeInfinity;
        lastCoordinateSignature = null;
    }

    /// <summary>今キャプチャしているウィンドウ (見つかっていなければ 0)。</summary>
    public nint Window => hwnd;

    /// <summary>
    /// キャプチャするウィンドウを切り替える。0 なら設定のプロセス名・タイトルで探し直す。
    /// 呼び出し側で設定のプロセス名・タイトルも合わせておくこと (ウィンドウを閉じて開き直したときに探せるように)。
    /// </summary>
    public void SelectWindow(nint window)
    {
        hwnd = window;
        Tracker.Reset();
        Invalidate();
    }

    public Rectangle? FindClientRect()
    {
        if (GameWindow.ClientScreenRect(hwnd) is null)
        {
            hwnd = GameWindow.Find(config.ProcessName, config.WindowTitle);
        }
        return GameWindow.ClientScreenRect(hwnd);
    }

    /// <summary>
    /// ゲームの読み取り領域が画面に見えているか (前面でなくても、最小化されず他のウィンドウに隠れていなければよい)。
    /// 確かめない設定なら常に true。
    /// </summary>
    /// <summary>ゲームのウィンドウがアクティブ (キー入力を受け取っている) か。</summary>
    public bool IsGameActive() => FindClientRect() is not null && GameWindow.IsActive(hwnd);

    /// <summary>ゲームを隠している他のウィンドウの位置 (スクリーン座標)。</summary>
    public List<Rectangle> CoveringWindows() => GameWindow.CoveringWindows(hwnd);

    public bool IsGameForeground() => !config.RequireForeground || FindClientRect() is { } client && IsGameVisible(client);

    private bool IsGameVisible(Rectangle client)
    {
        Rectangle[] areas = FindRegions(client.Size) is { } regions ? [regions.Name, regions.Coordinates] : [new Rectangle(Point.Empty, client.Size)];
        foreach (ref var area in areas.AsSpan())
        {
            area.Offset(client.Location);
        }
        return GameWindow.IsVisibleOnScreen(hwnd, areas);
    }

    /// <summary>直近の読み取りで使った UI の拡大率と位置のずれ。</summary>
    public UiTransform LastTransform => lastTransform;

    /// <summary>
    /// ライブ読み取りで使う UI の倍率。設定で固定していなければ、ゲームが表示されているモニターの解像度から計算する。
    /// Win32 API を 2 回呼ぶだけなので、毎回呼んでも負荷はほとんどない。
    /// </summary>
    public UiTransform CurrentTransform()
    {
        if (config.UiScale is { } fixedScale)
        {
            return new UiTransform(fixedScale);
        }
        return GameWindow.MonitorSize(hwnd) is { } monitor ? UiTransform.ForScreen(monitor.Height) : UiTransform.Identity;
    }

    /// <summary>画面上 (UI の拡大率と位置のずれを反映した) の領域。</summary>
    public Rectangle? RegionInClient(RegionKind kind, Size clientSize, UiTransform? transform = null) =>
        config.GetRegion(kind)?.ClipTo(clientSize.Width, clientSize.Height, transform);

    /// <summary>画面上の領域を、UI の拡大率と位置のずれを戻した等倍のゲーム座標として保存する。</summary>
    public void StoreRegion(RegionKind kind, Rectangle region, UiTransform? transform = null)
    {
        config.SetRegion(kind, region, transform);
        Invalidate();
    }

    /// <summary>ゲームのクライアント領域をキャプチャする。ウィンドウがなければ null。</summary>
    public (Rectangle ClientRect, BgrImage Frame, UiTransform Transform)? CaptureClient()
    {
        var rect = FindClientRect();
        return rect is null ? null : (rect.Value, ScreenCapture.Capture(rect.Value), CurrentTransform());
    }

    /// <summary>直前の Read で、ウィンドウと領域を探すのにかかった時間 (ミリ秒)。</summary>
    public double LastPrepareMs { get; private set; }

    /// <summary>直前の Read で、座標欄の撮影 (キャプチャ) にかかった時間 (ミリ秒)。撮影しなかったら 0。</summary>
    public double LastCaptureMs { get; private set; }

    /// <summary>直前の Read で、マップ名欄の撮影にかかった時間 (ミリ秒)。読み直さず撮影しなかったら null。</summary>
    public double? LastNameCaptureMs { get; private set; }

    /// <summary>直前の Read で、文字の認識 (OCR とその前後の処理) にかかった時間 (ミリ秒)。撮影の時間は含まない。</summary>
    public double LastRecognizeMs { get; private set; }

    /// <summary>直前の Read で、マップ名の OCR にかかった時間 (ミリ秒)。読み直さなかったら null。</summary>
    public double? LastNameOcrMs { get; private set; }

    /// <summary>直前の Read で、座標の OCR にかかった時間 (ミリ秒)。見た目が前回と同じで読み直さなかったら null。</summary>
    public double? LastCoordinateOcrMs { get; private set; }

    /// <summary>直前に座標を読み直したとき、覚えた文字の画像だけで読めたか (OCR を省いたか)。</summary>
    public bool LastCoordinateFromGlyphs { get; private set; }

    /// <summary>座標欄の文字の画像 (OCR で読めた結果から覚える)。</summary>
    public CoordinateGlyphs CoordinateGlyphs { get; } = new();

    // 最後に撮影したマップ名欄 (読み直さない回のプレビューに使う)。
    private BgrImage? lastNameImage;

    /// <summary>撮影した座標欄の画素と、撮影した時刻 (Stopwatch.GetTimestamp)。</summary>
    public sealed record CapturedPixels(byte[] Pixels, long Timestamp);

    private volatile CapturedPixels? latestCoordinatePixels;

    /// <summary>
    /// Read で最後に撮影した座標欄の画素 (別のスレッドから読んでよい)。
    /// 座標欄の画素の変化を見る処理は、自分で撮影せずにこれを使い回す (撮影を 1 回にまとめる)。
    /// </summary>
    public CapturedPixels? LatestCoordinatePixels => latestCoordinatePixels;

    private long? previousCaptureTimestamp, currentCaptureTimestamp;
    private long lastCoordinateChangeTimestamp;

    /// <summary>
    /// 座標欄の見た目が最後に変わった時刻 (Stopwatch.GetTimestamp、別のスレッドから読んでよい)。まだなければ 0。
    /// 変わる前の最後の撮影と、変わった最初の撮影の真ん中とみなす (読み取りは数 ms ごとなので、そのくらいの精度)。
    /// </summary>
    public long LastCoordinateChangeTimestamp => Interlocked.Read(ref lastCoordinateChangeTimestamp);

    /// <summary>座標欄の見た目が変わった間隔から判定した移動速度。</summary>
    public WalkSpeed WalkSpeed { get; } = new();

    public Reading Read(bool force = false)
    {
        LastNameOcrMs = LastCoordinateOcrMs = LastNameCaptureMs = null;
        LastPrepareMs = LastCaptureMs = LastRecognizeMs = 0;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        if (FindClientRect() is not { } client)
        {
            return Prepared(Result(ReadingStatus.NoWindow));
        }
        lastTransform = CurrentTransform();
        if (config.RequireForeground && !IsGameVisible(client))
        {
            return Prepared(Result(ReadingStatus.Background));
        }
        if (Ocr is NoOcrEngine)
        {
            // OCR なし: 何も読まない。キャラクターは常に画面の中央にいるので、座標は (0, 0) として中央に合わせて描く。
            return Prepared(new Reading { Status = ReadingStatus.Ok, Message = Messages[ReadingStatus.Ok], Coordinates = new GameCoordinate(0, 0),
                CoordinatesStale = false, UiScale = lastTransform.ScaleX, UiScaleY = lastTransform.ScaleY });
        }
        if (FindRegions(client.Size) is not { } regions)
        {
            return Prepared(RegionMissing(client.Size));
        }
        LastPrepareMs = watch.Elapsed.TotalMilliseconds;
        // 座標欄とマップ名欄は別々に撮影する。座標欄は毎回、マップ名欄は読み直すときだけ (歩いている間の撮影を小さくする)。
        BgrImage coordinateImage;
        var captureStart = System.Diagnostics.Stopwatch.GetTimestamp();
        try
        {
            coordinateImage = ScreenCapture.Capture(regions.Coordinates with { X = client.X + regions.Coordinates.X, Y = client.Y + regions.Coordinates.Y });
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            // 画面ロック中や UAC ダイアログ表示中などはキャプチャに失敗する。
            return Result(ReadingStatus.CaptureFailed, $"キャプチャに失敗しました: {exception.Message}");
        }
        LastCaptureMs = watch.Elapsed.TotalMilliseconds - LastPrepareMs;
        // 撮影した画面の時刻は、撮影を始めてから終わるまでの真ん中とみなす。
        var captured = captureStart + (System.Diagnostics.Stopwatch.GetTimestamp() - captureStart) / 2;
        previousCaptureTimestamp = latestCoordinatePixels?.Timestamp;
        currentCaptureTimestamp = captured;
        latestCoordinatePixels = new(coordinateImage.Pixels, captured);
        return ReadImages(coordinateImage, () =>
        {
            var started = watch.Elapsed.TotalMilliseconds;
            try
            {
                return ScreenCapture.Capture(regions.Name with { X = client.X + regions.Name.X, Y = client.Y + regions.Name.Y });
            }
            catch (Exception exception) when (exception is not OutOfMemoryException)
            {
                return null; // 撮影できなかった: 今回はマップ名を読み直さない
            }
            finally
            {
                LastNameCaptureMs = watch.Elapsed.TotalMilliseconds - started;
            }
        }, force);

        Reading Prepared(Reading reading)
        {
            LastPrepareMs = watch.Elapsed.TotalMilliseconds;
            return reading;
        }
    }

    /// <summary>クライアント領域全体の画像からマップ名と座標を読み取る。transform は UI の倍率 (省略時は等倍)。</summary>
    public Reading ReadFrame(BgrImage frame, bool force = false, UiTransform? transform = null)
    {
        lastTransform = transform ?? UiTransform.Identity;
        currentCaptureTimestamp = previousCaptureTimestamp = null; // 画像ファイルには撮影の時刻がない
        var clientSize = new Size(frame.Width, frame.Height);
        if (FindRegions(clientSize) is not { } regions)
        {
            return RegionMissing(clientSize);
        }
        return ReadImages(frame.Crop(regions.Coordinates), () => frame.Crop(regions.Name), force);
    }

    /// <summary>lastTransform で見た、マップ名と座標の画面上の領域。どちらかが収まらなければ null。</summary>
    private (Rectangle Name, Rectangle Coordinates)? FindRegions(Size clientSize) =>
        RegionInClient(RegionKind.Name, clientSize, lastTransform) is { } name
        && RegionInClient(RegionKind.Coordinates, clientSize, lastTransform) is { } coordinates
            ? (name, coordinates)
            : null;

    private Reading RegionMissing(Size clientSize) =>
        Result(RegionInClient(RegionKind.Name, clientSize, lastTransform) is null ? ReadingStatus.NeedNameRegion : ReadingStatus.NeedCoordinateRegion);

    /// <summary>座標を読み、必要なときだけマップ名も読む。マップ名欄の画像は captureName で必要になってから取る。</summary>
    private Reading ReadImages(BgrImage coordinateImage, Func<BgrImage?> captureName, bool force)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            if (ImageAnalysis.IsBlackFrame(coordinateImage))
            {
                // 座標欄が真っ黒: マップ名欄も黒なら画面の切り替わり中など。
                var name = captureName();
                lastNameImage = name ?? lastNameImage;
                if (name is null || ImageAnalysis.IsBlackFrame(name))
                {
                    return Result(ReadingStatus.BlackFrame, nameImage: name, coordinateImage: coordinateImage);
                }
            }
            ReadCoordinates(coordinateImage, force);
            ReadName(captureName, force);
            return Result(ReadingStatus.Ok, nameImage: lastNameImage, coordinateImage: coordinateImage);
        }
        finally
        {
            LastRecognizeMs = watch.Elapsed.TotalMilliseconds - (LastNameCaptureMs ?? 0);
        }
    }

    private void ReadCoordinates(BgrImage coordinateImage, bool force)
    {
        var signature = ImageAnalysis.Signature(coordinateImage);
        if (signature != lastCoordinateSignature && currentCaptureTimestamp is { } current)
        {
            var changedAt = previousCaptureTimestamp is { } previous ? previous + (current - previous) / 2 : current;
            Interlocked.Exchange(ref lastCoordinateChangeTimestamp, changedAt);
            WalkSpeed.AddChange(changedAt * 1000.0 / System.Diagnostics.Stopwatch.Frequency);
        }
        if (force || signature != lastCoordinateSignature)
        {
            lastCoordinateSignature = signature;
            var coordinateWatch = System.Diagnostics.Stopwatch.StartNew();
            var text = config.CoordinateGlyphCache ? CoordinateGlyphs.Recognize(coordinateImage) : null;
            LastCoordinateFromGlyphs = text is not null;
            if (text is null)
            {
                text = Ocr.Recognize(coordinateImage, "en").Text;
                if (config.CoordinateGlyphCache)
                {
                    CoordinateGlyphs.Learn(coordinateImage, text);
                }
            }
            LastCoordinateOcrMs = coordinateWatch.Elapsed.TotalMilliseconds;
            lastRawCoordinates = text;
            Tracker.UpdateCoordinates(TextParsing.ParseCoordinates(text));
        }
        else
        {
            // 見た目が変わっていないので前回の座標がそのまま有効。
            Tracker.UpdateCoordinates(Tracker.Coordinates);
        }
    }

    private void ReadName(Func<BgrImage?> captureName, bool force)
    {
        var now = Environment.TickCount64 / 1000.0;
        // マップ名はそう頻繁に変わらないので、確定後は一定間隔でのみ読み直す (そのときだけ撮影する)。
        if (ManualMapName is not null || !(force || Tracker.MapName is null || Tracker.HasPendingName || now - lastNameOcrAt >= config.NameRefreshSeconds))
        {
            return;
        }
        var nameImage = LastNameCaptureMs is null ? captureName() : lastNameImage; // 黒判定で撮影済みならそれを使う
        if (nameImage is null)
        {
            return;
        }
        lastNameImage = nameImage;
        lastNameOcrAt = now;
        var nameWatch = System.Diagnostics.Stopwatch.StartNew();
        var name = Ocr.Recognize(nameImage, config.OcrLanguage);
        LastNameOcrMs = nameWatch.Elapsed.TotalMilliseconds;
        lastRawName = name.Text;
        lastNameScore = name.Score;
        AcceptName(name);
    }

    private void AcceptName(OcrResult name)
    {
        var cleaned = TextParsing.CleanMapName(name.Text);
        if (cleaned.Length == 0 || name.Score < MinRawNameScore)
        {
            Tracker.UpdateName(null, null);
            return;
        }
        Tracker.UpdateName(TextParsing.CanonicalizeForMatch(cleaned), cleaned);
    }

    private Reading Result(ReadingStatus status, string? message = null, BgrImage? nameImage = null, BgrImage? coordinateImage = null) => new()
    {
        Status = status,
        Message = message ?? Messages[status],
        MapName = ManualMapName ?? Tracker.MapName,
        Coordinates = Tracker.Coordinates,
        RawName = lastRawName,
        NameScore = lastNameScore,
        RawCoordinates = lastRawCoordinates,
        NameStale = ManualMapName is null && Tracker.NameStale,        UiScale = lastTransform.ScaleX,
        UiScaleY = lastTransform.ScaleY,
        CoordinatesStale = Tracker.CoordinatesStale,
        NameImage = nameImage,
        CoordinateImage = coordinateImage,
    };

    public void Dispose()
    {
        if (ownsOcr)
        {
            Ocr.Dispose();
        }
    }
}
