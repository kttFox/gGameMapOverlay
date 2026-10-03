using System.Text.Encodings.Web;
using gGameMapOverlay.Imaging;
using gGameMapOverlay.Overlay;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace gGameMapOverlay;

/// <summary>実行ファイルと同じフォルダーの config.json に保存する設定。</summary>
public sealed class AppConfig
{
    /// <summary>ダウンロードした OCR モデルなど、設定以外のデータの置き場所。</summary>
    public static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gGameMapOverlay");

    /// <summary>設定ファイル。フォルダーごとコピーすれば設定も持ち運べるよう、実行ファイルの横に置く。</summary>
    public static readonly string DefaultPath = Path.Combine(AppContext.BaseDirectory, "config.json");

    public static readonly string[] Languages = ["ja", "ko", "en"];

    // 初期値の領域。日本語版クライアント (Ver 7.80.5054) のマップ名欄・座標欄の内側で、
    // 6 種類のウィンドウサイズ (tests/gGameMapOverlay.Tests/TestData/Screenshots と実機) で位置が同じことを確認済み。
    public static readonly ClientRegion DefaultNameRegion = new(66, 3, 349, 23);
    public static readonly ClientRegion DefaultCoordinateRegion = new(66, 27, 139, 47);

    /// <summary>
    /// キャプチャ対象のウィンドウのプロセス名とタイトル (部分一致)。メイン画面で選んだウィンドウのものを覚えておき、
    /// 次に起動したときもこれで探す (GameWindow.Find)。
    /// </summary>
    public string ProcessName { get; set; } = "";
    public string WindowTitle { get; set; } = "";

    // 領域はゲームのクライアント領域の左上を原点とするピクセル座標で保持する。
    // gGame の UI はウィンドウサイズに関係なく左上から同じピクセル位置に描かれるので、
    // サイズ比で換算してはいけない (tests の実画面スクリーンショットで確認済み)。
    // gGame は画面の高さが 1080 より大きいと UI を拡大して描くので、読み取るときにその倍率を掛ける (UiTransform.ForScreen)。
    public ClientRegion? NameRegion { get; set; } = DefaultNameRegion;
    public ClientRegion? CoordinateRegion { get; set; } = DefaultCoordinateRegion;

    /// <summary>マップ名の OCR 言語 (ja / ko / en)。座標は常に en モデルで読む。</summary>
    public string OcrLanguage { get; set; } = "ja";

    /// <summary>paddle: PaddleOCR (ONNX) / windows: Windows 標準 OCR / none: OCR なし (グリッド・プレイヤー・カスタムだけ出す)。</summary>
    public string OcrBackend { get; set; } = "paddle";

    /// <summary>
    /// 座標を、OCR で読めた結果から覚えた文字の画像でも読む (CoordinateGlyphs)。
    /// 覚えた文字だけで読めれば OCR を省くので速く、読めなければ OCR に戻る。
    /// </summary>
    public bool CoordinateGlyphCache { get; set; } = true;

    /// <summary>
    /// ゲームが前面にあるときだけ読み取る。画面キャプチャは見えている内容を読むため、
    /// 他のウィンドウが重なっているときに誤った文字を拾わないようにする。
    /// </summary>
    public bool RequireForeground { get; set; } = true;

    /// <summary>
    /// UI の倍率を固定する。null (既定) なら、ゲームが表示されているモニターの解像度から計算する (UiTransform.ForScreen)。
    /// gGame は画面の高さが 1080 より大きいと UI を拡大して描く (高さ 1280 の画面で約 1.18 倍) ので、
    /// 計算が合わない環境のために固定もできるようにしておく。
    /// </summary>
    public double? UiScale { get; set; }

    /// <summary>
    /// 読み取り間隔。座標欄の見た目が変わったときだけ OCR するので、変わっていない間は小さな範囲のキャプチャ (数 ms) だけで済む。
    /// 長いほど、移動してからオーバーレイが追従するまで遅れる。
    /// </summary>
    public int IntervalMs { get; set; } = 15;

    /// <summary>
    /// キャラクターの移動速度を、座標欄の見た目が変わった間隔から判定する (WalkSpeed)。1 マスのスライドにその等級の 1 歩の時間をかける。
    /// 判定するたびに MoveSpeedGrade に覚えておき、判定できるまではそれを使う。オフなら MoveSpeedGrade に固定する。
    /// </summary>
    public bool MoveSpeedAuto { get; set; } = true;

    /// <summary>移動速度の等級 (0〜4、WalkSpeed.Grades)。</summary>
    public int MoveSpeedGrade { get; set; } = WalkSpeed.IndexOf(WalkSpeed.DefaultGrade);

    /// <summary>移動速度の等級の代わりに、OverlaySlideMs を 1 マスのスライドにかける。</summary>
    public bool OverlaySlideManual { get; set; }

    /// <summary>OverlaySlideManual のとき、1 マスのスライドにかける時間 (ミリ秒)。</summary>
    public int OverlaySlideMs { get; set; } = WalkSpeed.DefaultGrade.StepMs;

    /// <summary>今使う 1 歩の時間 (ミリ秒)。</summary>
    [JsonIgnore]
    public int StepMs => OverlaySlideManual ? OverlaySlideMs : WalkSpeed.Grade(MoveSpeedGrade).StepMs;

    /// <summary>移動に合わせてマスを滑らかに動かす (スライドでの追従)。オフなら読み取った座標へすぐ移す (キー入力での先読みもしない)。</summary>
    public bool OverlaySlide { get; set; } = true;

    /// <summary>
    /// OverlaySlide がオフのとき、座標が変わってからマスを移すまでの時間 (ミリ秒)。座標はゲームが 1 歩を歩き出した瞬間に変わるので、
    /// すぐ移すとキャラクターの絵より先に動いてずれる。0 ならすぐ移す。OverlayJumpDelayManual (完全手動) のときだけ使う。
    /// </summary>
    public int OverlayJumpDelayMs { get; set; } = 170;

    /// <summary>
    /// true (完全手動) なら OverlayJumpDelayMs をそのまま使う。false (自動+手動) なら AutoOverlayJumpDelayMs に OverlayJumpDelayExtraMs を足す。
    /// </summary>
    public bool OverlayJumpDelayManual { get; set; }

    /// <summary>自動+手動のとき、自動の値に足す時間 (ミリ秒、負なら引く)。</summary>
    public int OverlayJumpDelayExtraMs { get; set; } = 40;

    /// <summary>自動の値: 移動速度の 1 歩の半分 (キャラクターの絵が次のマスへ半分進むころ) を 10 ミリ秒単位に丸めたもの。</summary>
    [JsonIgnore]
    public int AutoOverlayJumpDelayMs =>
        (int)Math.Round(WalkSpeed.Grade(MoveSpeedGrade).StepMs / 2.0 / 10, MidpointRounding.AwayFromZero) * 10;

    /// <summary>今使う、オフのときの遅延 (ミリ秒)。</summary>
    [JsonIgnore]
    public int EffectiveOverlayJumpDelayMs => OverlayJumpDelayManual
        ? OverlayJumpDelayMs
        : Math.Max(0, AutoOverlayJumpDelayMs + OverlayJumpDelayExtraMs);

    /// <summary>描いている位置が読み取った座標からこのマス数以上離れたら、滑らせずにすぐ移す。</summary>
    public double OverlaySnapTiles { get; set; } = 1.5;

    /// <summary>
    /// 止まっているときに移動キー (WASD・矢印キー) を押したら、座標が変わるのを待たずにオーバーレイを押した方向へ 1 マス動かし始める (歩き始めだけ。2 歩目からは座標の変化で動かす)。
    /// キーはグローバルな低レベルキーボードフックで見る (移動キーの押し下げ・離しだけ)。
    /// 右クリックで歩くときも同じように動かす (グローバルな低レベルマウスフックで、右ボタンと押している間のカーソルの位置だけを見る)。
    /// </summary>
    public bool MoveKeyPrediction { get; set; } = true;

    /// <summary>
    /// オーバーレイが座標の変化で動いている途中 (方向を変えるときにキーを一度離したなど) に移動キーを押したときも、
    /// 着いたマスから押した方向へ、座標が変わるのを待たずに動かす。MoveKeyPrediction がオンのときだけ使う。
    /// </summary>
    public bool MoveKeyRepredict { get; set; } = true;

    /// <summary>
    /// 2 歩目から、オーバーレイがマスに着いたときに移動キーを押していれば、止めずにその方向の次のマスへ進み続ける
    /// (止まる直前に押し続けているときも)。オフなら 2 歩目からは座標の変化で動かす。MoveKeyPrediction がオンのときだけ使う。
    /// </summary>
    public bool MoveKeyContinue { get; set; } = true;

    /// <summary>
    /// キー入力で先に動かしたあと、座標欄の画素が変わったかで本当に歩いたかを確かめる (変わらなければ戻す)。
    /// オフなら確かめずに歩いたとみなし、座標の読み取りで外れていたら戻す。MoveKeyPrediction がオンのときだけ使う。
    /// </summary>
    public bool MovePixelCheck { get; set; } = true;

    /// <summary>
    /// ゲームが移動キーの状態を反映するまでの遅れ (ミリ秒)。マスに着いたとき、この時間だけ前に押していたキーの方向へ次の 1 歩を歩く
    /// (着く直前に別のキーへ切り替えても、ゲームはまだ前のキーの方向へ歩く)。
    /// </summary>
    public int MoveInputLagMs { get; set; } = 120;

    /// <summary>
    /// 一度に何マスも進むスキルのキー (例: "F")。押したら、向いている方向 (最後に歩いた方向) へ MoveSkillTiles マスを
    /// 1 マス MoveSkillTileMs で、読み取りを待たずに動かす。空ならしない。
    /// </summary>
    public string MoveSkillKey { get; set; } = "Back";

    /// <summary>スキルのキーで先に動かすか。MoveKeyPrediction がオンのときだけ使う。</summary>
    public bool MoveSkillPrediction { get; set; } = false;

    /// <summary>スキルで進むマス数 (gGame のスキルは必ずこのマス数進む)。</summary>
    public const int MoveSkillTiles = 5;

    /// <summary>スキルで 1 マス進む時間 (ミリ秒)。</summary>
    public int MoveSkillTileMs { get; set; } = 53;

    /// <summary>移動キーを押してからキャラクターが歩き出すまでの時間 (ミリ秒)。</summary>
    public int MoveKeyDelayMs { get; set; } = 40;

    /// <summary>
    /// 1 歩目 (歩き始め) で、移動キーを押してから座標欄の見た目を確かめるまでの時間 (ミリ秒)。
    /// このとき変わっていなければ歩いていないとみなし、先に動かした分を戻す。
    /// </summary>
    public int MoveStartCheckMs { get; set; } = 40;

    /// <summary>
    /// 移動キーの組み合わせ (例: "W", "WD") → 歩く方向 [x, y] (マス単位の -1〜1)。
    /// 歩いたときに読み取った座標の変化から覚える (まだ覚えていない組み合わせは先読みしない)。
    /// </summary>
    public Dictionary<string, int[]> MoveKeyDirections { get; set; } = [];

    /// <summary>
    /// PaddleOCR の推論に使うスレッド数。null (既定) なら AutoOcrThreads。
    /// 多いほど 1 回の OCR が速く終わり (オーバーレイの追従が速くなる)、そのぶん CPU を使う。
    /// </summary>
    public int? OcrThreads { get; set; }

    /// <summary>自動のときのスレッド数。ゲームの分の CPU は残しておく。</summary>
    public static int AutoOcrThreads => Math.Clamp(Environment.ProcessorCount / 2, 1, 4);

    /// <summary>設定画面で選べるスレッド数の上限。</summary>
    public static int MaxOcrThreads => Math.Clamp(Environment.ProcessorCount, 1, 8);

    [JsonIgnore]
    public int EffectiveOcrThreads => OcrThreads ?? AutoOcrThreads;
    /// <summary>マップ名が確定した後、読み直す間隔 (秒)。</summary>
    public double NameRefreshSeconds { get; set; } = 1.0;

    /// <summary>同じ結果がこの回数続いたらマップ名を確定する (一瞬の誤読で表示が変わらないように)。</summary>
    public int NameConfirmHits { get; set; } = 2;

    /// <summary>読めない状態がこの秒数を超えたら、最後の結果を古いもの (Stale) として扱う。</summary>
    public double NameHoldSeconds { get; set; } = 3.0;

    /// <summary>設定画面で詳細設定 (処理・マップ名) を表示する。</summary>
    public bool ShowAdvancedSettings { get; set; }

    /// <summary>メイン画面にカスタムのマスの編集ボタンを出す (タイトルバーのアイコンの右クリックメニューで切り替える)。</summary>
    public bool ShowCustomTilesButton { get; set; }

    /// <summary>
    /// オーバーレイに表示するデータの種類 (OverlayLayer.Key → 表示するか)。書いていない種類は表示する。
    /// </summary>
    public Dictionary<string, bool> OverlayLayers { get; set; } = [];

    public bool IsOverlayLayerShown(OverlayLayer layer) => OverlayLayers.GetValueOrDefault(layer.Key, true);

    public void SetOverlayLayerShown(OverlayLayer layer, bool shown) => OverlayLayers[layer.Key] = shown;

    /// <summary>オーバーレイにマスの格子 (グリッド) を描く。</summary>
    public bool ShowGrid { get; set; }

    /// <summary>オーバーレイにキャラクターのいるマスの枠を描く。</summary>
    public bool ShowPlayer { get; set; } = true;

    /// <summary>自分で描いたマス (カスタム) のグループ (この順に重ねて描く)。表示はグループごと (CustomTileGroup.Shown)。</summary>
    public List<CustomTileGroup> CustomGroups { get; set; } = [];

    /// <summary>描くカスタムのグループ (表示していて、マスがあるもの)。</summary>
    [JsonIgnore]
    public IEnumerable<CustomTileGroup> ShownCustomGroups => CustomGroups.Where(group => group.Shown && group.Cells.Count > 0);

    /// <summary>カスタムの編集画面で、背景にゲーム画面を映す。</summary>
    public bool CustomEditorShowGame { get; set; }

    [JsonIgnore]
    public bool OverlayEnabled => ShowGrid || ShowPlayer || ShownCustomGroups.Any() || OverlayLayer.All.Any(IsOverlayLayerShown);

    /// <summary>オーバーレイの色 (OverlayLayer.Key → "#RRGGBB")。書いていない種類は OverlayLayer.DefaultColor。</summary>
    public Dictionary<string, string> OverlayColors { get; set; } = [];

    /// <summary>種類の色 (アルファは不透明度から決める)。</summary>
    public Color GetOverlayColor(OverlayLayer layer) => GetOverlayColor(layer.Key, layer.DefaultColor);

    /// <summary>グリッドの線の色の初期値。OverlayColors には GridColorKey で入れる。</summary>
    public static readonly Color DefaultGridColor = Color.FromArgb(200, 200, 200);
    public const string GridColorKey = "grid";

    /// <summary>グリッドの線の色 (アルファは不透明度から決める)。</summary>
    public Color GetGridColor() => GetOverlayColor(GridColorKey, DefaultGridColor);

    public void SetGridColor(Color color) => SetOverlayColor(GridColorKey, DefaultGridColor, color);

    /// <summary>キャラクターのいるマスの枠の色の初期値。OverlayColors には PlayerColorKey で入れる。</summary>
    public static readonly Color DefaultPlayerColor = Color.FromArgb(255, 235, 59);
    public const string PlayerColorKey = "player";

    /// <summary>キャラクターのいるマスの枠の色 (アルファは不透明度から決める)。</summary>
    public Color GetPlayerColor() => GetOverlayColor(PlayerColorKey, DefaultPlayerColor);

    public void SetPlayerColor(Color color) => SetOverlayColor(PlayerColorKey, DefaultPlayerColor, color);

    private Color GetOverlayColor(string key, Color defaultColor)
    {
        var color = defaultColor;
        if (OverlayColors.GetValueOrDefault(key) is { } text)
        {
            try
            {
                color = ColorTranslator.FromHtml(text);
            }
            catch (Exception exception) when (exception is ArgumentException or FormatException)
            {
                // 読めない値は初期値のまま
            }
        }
        var alpha = (int)Math.Round(255 * GetOverlayOpacity(key) / 100.0);
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }

    public const int DefaultOverlayOpacity = 50;
    public const int MinOverlayOpacity = 10;
    public const int MaxOverlayOpacity = 100;

    /// <summary>全体の不透明度 (%)。100 で不透明、小さいほど透ける。見えなくならないよう最小は MinOverlayOpacity。個別の不透明度がない色に使う。</summary>
    public int OverlayOpacity { get; set; } = DefaultOverlayOpacity;

    /// <summary>
    /// 個別の不透明度 (OverlayLayer.Key・GridColorKey・PlayerColorKey → %)。書いてある色だけ全体の不透明度の代わりに使う
    /// (設定画面でチェックを入れた色)。
    /// </summary>
    public Dictionary<string, int> OverlayOpacities { get; set; } = [];

    public int GetOverlayOpacity(OverlayLayer layer) => GetOverlayOpacity(layer.Key);

    public int GetGridOpacity() => GetOverlayOpacity(GridColorKey);

    /// <summary>key は OverlayLayer.Key・GridColorKey・PlayerColorKey。</summary>
    public int GetOverlayOpacity(string key) =>
        Math.Clamp(OverlayOpacities.TryGetValue(key, out var value) ? value : OverlayOpacity, MinOverlayOpacity, MaxOverlayOpacity);

    /// <summary>個別の不透明度を使っているか。key は OverlayLayer.Key・GridColorKey・PlayerColorKey。</summary>
    public bool HasOwnOpacity(string key) => OverlayOpacities.ContainsKey(key);

    /// <summary>個別の不透明度にする (全体と同じ値でも個別のまま残す)。key は OverlayLayer.Key・GridColorKey・PlayerColorKey。</summary>
    public void SetOwnOpacity(string key, int opacity) =>
        OverlayOpacities[key] = Math.Clamp(opacity, MinOverlayOpacity, MaxOverlayOpacity);

    /// <summary>個別の不透明度をやめて、全体の不透明度に従う。</summary>
    public void ClearOwnOpacity(string key) => OverlayOpacities.Remove(key);

    /// <summary>オーバーレイにアンチエイリアスをかけるか。</summary>
    public bool OverlayAntiAlias { get; set; } = true;

    /// <summary>地形の画像のチャンクの境目と番号をオーバーレイに描くか (確かめる用)。</summary>
    public bool OverlayShowChunks { get; set; }

    /// <summary>色を変える。初期値と同じなら設定から消す (初期値が変わったときに追従するように)。</summary>
    public void SetOverlayColor(OverlayLayer layer, Color color) => SetOverlayColor(layer.Key, layer.DefaultColor, color);

    private void SetOverlayColor(string key, Color defaultColor, Color color)
    {
        if (color.ToArgb() == defaultColor.ToArgb())
        {
            OverlayColors.Remove(key);
        }
        else
        {
            OverlayColors[key] = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }

    /// <summary>GitHub の最新リリースの manifest.json (.github/workflows/release.yml が添付する)。</summary>
    public const string DefaultUpdateUrl = "https://github.com/kttFox/gGameMapOverlay/releases/latest/download/manifest.json";

    /// <summary>
    /// 更新情報 (manifest.json) の URL。null (空) なら DefaultUpdateUrl。
    /// 本体とマップ情報 (data フォルダー) は別々に確認・更新する (Update.Updater)。
    /// </summary>
    public string? UpdateUrl { get; set; } = DefaultUpdateUrl;

    /// <summary>起動時に更新を確認する。false なら確認しない。</summary>
    public bool CheckUpdatesOnStartup { get; set; } = true;

    /// <summary>タスクトレイに常駐する。true ならメイン画面を閉じてもタスクトレイに残り、終了はタスクトレイのメニューから行う。</summary>
    public bool StayInTray { get; set; }

    public int? WindowX { get; set; }
    public int? WindowY { get; set; }

    /// <summary>設定画面の位置。null ならメイン画面の中央に出す。</summary>
    public int? SettingsWindowX { get; set; }
    public int? SettingsWindowY { get; set; }

    /// <summary>情報画面の位置。null なら設定画面 (なければメイン画面) の右に出す。</summary>
    public int? InfoWindowX { get; set; }
    public int? InfoWindowY { get; set; }

    /// <summary>情報画面の大きさ。null なら既定の大きさ。</summary>
    public int? InfoWindowWidth { get; set; }
    public int? InfoWindowHeight { get; set; }

    /// <summary>設定画面を開いていたか。true なら起動時に開く。</summary>
    public bool SettingsWindowOpen { get; set; }

    /// <summary>情報画面を開いていたか。true なら起動時に開く。</summary>
    public bool InfoWindowOpen { get; set; }

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <param name="path">設定ファイル。null なら DefaultPath。</param>
    public static AppConfig Load(string? path)
    {
        path ??= DefaultPath;

        AppConfig config;
        try
        {
            config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(path), JsonOptions) ?? new AppConfig();
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            config = new AppConfig();
        }
        if (!Languages.Contains(config.OcrLanguage))
        {
            config.OcrLanguage = "ja";
        }
        // 以前の版で領域を設定しないまま保存された設定 (null) にも初期値を補う。
        config.NameRegion ??= DefaultNameRegion;
        config.CoordinateRegion ??= DefaultCoordinateRegion;
        // 既定値を入れる前の版で保存された設定 (null) にも補う。止めるときは check_updates_on_startup を false にする
        if (string.IsNullOrWhiteSpace(config.UpdateUrl))
        {
            config.UpdateUrl = DefaultUpdateUrl;
        }
        config.ProcessName ??= "";
        config.WindowTitle ??= "";
        config.IntervalMs = config.IntervalMs;
        config.MoveSpeedGrade = Math.Clamp(config.MoveSpeedGrade, 0, WalkSpeed.Grades.Count - 1);
        config.OverlaySnapTiles = Math.Max(0, config.OverlaySnapTiles);
        config.MoveSkillKey ??= "";
        config.MoveSkillTileMs = Math.Clamp(config.MoveSkillTileMs, 10, 1000);
        config.MoveKeyDirections ??= [];
        config.CustomGroups = (config.CustomGroups ?? []).OfType<CustomTileGroup>().ToList();
        foreach (var group in config.CustomGroups)
        {
            group.Normalize();
        }
        if (config.OcrThreads is { } threads)
        {
            config.OcrThreads = Math.Clamp(threads, 1, MaxOcrThreads);
        }
        return config;
    }

    public static ClientRegion DefaultRegion(RegionKind kind) => kind == RegionKind.Name ? DefaultNameRegion : DefaultCoordinateRegion;

    public ClientRegion? GetRegion(RegionKind kind) => kind == RegionKind.Name ? NameRegion : CoordinateRegion;

    /// <summary>画面上の矩形を、UI の拡大率と位置のずれを戻した等倍のゲーム座標として保存する。</summary>
    public void SetRegion(RegionKind kind, System.Drawing.Rectangle region, UiTransform? transform = null)
    {
        var (left, top, right, bottom) = (transform ?? UiTransform.Identity).FromScreen(region);
        var stored = new ClientRegion(left, top, right, bottom);
        if (kind == RegionKind.Name)
        {
            NameRegion = stored;
        }
        else
        {
            CoordinateRegion = stored;
        }
    }

    /// <summary>保存せずに一時的に書き換えるための複製 (画像モード用)。</summary>
    public AppConfig Clone() => JsonSerializer.Deserialize<AppConfig>(JsonSerializer.Serialize(this, JsonOptions), JsonOptions)!;

    public void Save(string? path = null)
    {
        path ??= DefaultPath;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(this, JsonOptions));
        File.Move(temporary, path, overwrite: true);
    }
}

/// <summary>
/// クライアント領域の左上を原点とするピクセル座標の矩形 [Left, Right) x [Top, Bottom)。
/// 旧形式 (比率換算用の reference_width / reference_height 付き) の設定も、余分な項目は無視して読み込める。
/// </summary>
public sealed record ClientRegion(int Left, int Top, int Right, int Bottom)
{
    /// <summary>
    /// 画面上の矩形 (UI の拡大率と位置のずれを反映し、クライアント領域からはみ出す部分を切り詰めたもの)。ほぼ収まらなければ null。
    /// </summary>
    public System.Drawing.Rectangle? ClipTo(int clientWidth, int clientHeight, UiTransform? transform = null)
    {
        var screen = (transform ?? UiTransform.Identity).ToScreen(Left, Top, Right, Bottom);
        var left = Math.Max(0, screen.Left);
        var top = Math.Max(0, screen.Top);
        var right = Math.Min(clientWidth, screen.Right);
        var bottom = Math.Min(clientHeight, screen.Bottom);
        if (right - left < 2 || bottom - top < 2)
        {
            return null;
        }
        return System.Drawing.Rectangle.FromLTRB(left, top, right, bottom);
    }
}
