using System.Drawing.Drawing2D;
using gGameMapOverlay.Imaging;
using gGameMapOverlay.Native;
using gGameMapOverlay.Ocr;
using gGameMapOverlay.Overlay;
using gGameMapOverlay.Parsing;
using gGameMapOverlay.Update;

namespace gGameMapOverlay;

internal sealed partial class MainForm : Form, ISettingsHost, IInfoSource {
	private static readonly Color OkColor = Color.FromArgb( 46, 125, 50 );
	private static readonly Color WarnColor = Color.FromArgb( 239, 108, 0 );
	private static readonly Color ErrorColor = Color.FromArgb( 198, 40, 40 );
	private static readonly Color MutedColor = Color.FromArgb( 117, 117, 117 );

	private readonly AppConfig config;
	// 情報画面に出すスライドの出来事のログ (新しいものほど後ろ)。
	private const int MaxEventLog = 200;
	private readonly Queue<string> eventLog = new();
	private long eventLogVersion;
	// ログの経過時間の基準: 止まった状態から移動キーを押した時刻。まだ押していなければ null。
	private readonly System.Diagnostics.Stopwatch eventClock = System.Diagnostics.Stopwatch.StartNew();
	private double? keyDownAt;
	private readonly string configPath;
	private readonly OcrModelStore models = new();
	private readonly MapReader reader;
	// OCR エンジンはスレッドセーフではないので、読み取り・ウォームアップ・差し替えを直列化する。
	private readonly SemaphoreSlim ocrGate = new( 1, 1 );

	private string activeBackend;
	private bool ready;
	private bool running = true;
	private bool busy;
	private bool selecting;
	private bool closing;
	// タスクトレイのメニューで「終了」を選んだ (常駐中でも閉じるときに隠さずに終了する)。
	private bool exiting;
	// タスクトレイに隠したときに開いていた設定画面・情報画面 (表示に戻すときにまた開く)。
	private bool settingsHiddenToTray, infoHiddenToTray, customHiddenToTray;

	// 画像モード (デバッグ用): スクリーンショットをゲーム画面とみなして読み取る。
	// 領域は保存しない複製の設定に持つので、ライブ用の領域設定は変わらない。
	private AppConfig? imageConfig;
	private BgrImage? imageFrame;
	private string? imagePath;
	private bool imageFrameRemoved;
	private UiTransform imageTransform = UiTransform.Identity;
	private readonly string? startupImagePath;

	private SettingsForm? settingsForm;
	private InfoForm? infoForm;
	private CustomTilesForm? customForm;


	// オーバーレイ (モンスター境界・壁)。データが読めなければ overlayData は null で、理由を overlayError に持つ。
	private readonly OverlayForm overlay = new();
	// マップ情報を更新したら読み込み直すので readonly にしない。
	private OverlayData? overlayData;
	private string overlayError = "";
	private readonly Updater updater = new();
	private bool updating;
	// 更新の確認の結果 (一時停止ボタンの右に出す)。読み取りのステータスはすぐ書き換わるので分けて表示する。
	private readonly ToolTip updateStatusTip = new();
	private Reading? lastLiveReading;
	// 移動の遅れの計測 (設定画面でオンにしたときだけキーボードフックを入れる)。
	private readonly LatencyProbe latencyProbe;
	private GameCoordinate? probeCoordinates;
	// 移動キー (WASD・矢印キー)。先読みと遅れの計測で使う。どちらも使わないときはフックを入れない。
	private readonly Native.MoveKeyWatcher moveKeys = new();
	// 右クリックで歩く (押している間、カーソルの方向へ歩き続ける)。移動キーと同じ仕組みで先読みするので、
	// 右クリックの 4 方向を仮想の移動キー (MoveKeyWatcher のビットの上のビット) として扱う。
	private readonly Native.MoveMouseWatcher moveMouse = new();
	private static readonly (int X, int Y)[] MouseDirections = [(1, 0), (-1, 0), (0, 1), (0, -1)];
	private const int FirstMouseKey = 1 << 4; // MoveKeyWatcher.Keys の 4 つの次のビット
											  // 今の右クリックの仮想キー (ビット 1 つ、なければ 0)。
	private int mouseMoveKey;
	// 右ボタンを押したのがゲームのクライアント領域の中 (UI の上を除く) だったときの、押したときのクライアント領域 (外・UI の上で押したなら歩かないので null)。
	private Rectangle? mouseMoveClient;
	private int lastMoveKeys;
	private long lastMoveKeysAt;
	// 2 歩目から歩き続けたかを見るときに比べる、座標欄の見た目。
	private byte[]? stepBaseline;
	// 比較元を取ったときの座標の文字 (例: "13:76")。画素の変わった場所が X と Y のどちらかを見分けるのに使う。
	private string? stepBaselineText;
	private double stepBaselineAt;
	private GameCoordinate? shownCoordinates;
	// 種類ごとの、色の見本 (色は設定画面で変える) と表示を切り替えるチェックボックス (デザイナーで配置)。
	private readonly Dictionary<OverlayLayer, (Panel Swatch, CheckBox Box)> layerControls;
	private bool loadingLayers;
	private bool loadingMaps;
	// オーバーレイに渡す、自分で描いたマス (相対位置) と色。設定を変えたときだけ作り直す
	// (同じ中身でも作り直すと OverlayScene が別物とみなされるので、読み取りごとには作らない)。
	private IReadOnlyList<CustomTiles> customTiles = [];
	// カスタムのグループごとの、色の見本と表示を切り替えるチェックボックス (グリッド・プレイヤーの下に並べる。グループの数が変わったら作り直す)。
	private readonly List<(Panel Swatch, CheckBox Box)> customControls = [];
	private const int CustomFirstRow = 2;

	// 前面のウィンドウが変わったら、次の読み取りを待たずにオーバーレイを隠す・出し直す。
	// デリゲートはフックを解除するまで GC されないようフィールドで持つ。
	private readonly GameWindow.WinEventProc foregroundChanged;
	private nint foregroundHook;

	private bool ImageMode => imageFrame is not null;

	/// <param name="debug">画像モード (スクリーンショットから読み取る) のボタンを表示する。</param>
	public MainForm( AppConfig config, string configPath, string? startupImagePath = null, bool debug = false ) {
		this.config = config;
		this.configPath = configPath;
		this.startupImagePath = startupImagePath;
		foregroundChanged = ( _, _, _, _, _, _, _ ) => ForegroundChanged();
		reader = new MapReader( config, OcrEngineFactory.Create( config.OcrBackend, models, config.EffectiveOcrThreads ) );
		activeBackend = config.OcrBackend;
		LoadOverlayData();

		InitializeComponent();
		trayIcon.Icon = ( Environment.ProcessPath is { } exe ? Icon.ExtractAssociatedIcon( exe ) : null ) ?? SystemIcons.Application;
		trayIcon.Visible = config.StayInTray;
		layerControls = new() {
			[OverlayLayer.MonsterBlock] = (monsterBlockSwatch, monsterBlockBox),
			[OverlayLayer.Special] = (specialSwatch, specialBox),
			[OverlayLayer.ImpassableEdge] = (impassableEdgeSwatch, impassableEdgeBox),
			[OverlayLayer.MapMove] = (mapMoveSwatch, mapMoveBox),
		};
		LoadLayerSettings();
		LoadMapChoices();
		LoadWindowChoices();
		UpdateOcrControls();
		overlay.AntiAlias = config.OverlayAntiAlias;
		overlay.ShowChunks = config.OverlayShowChunks;
		overlay.SlideMs = config.OverlaySlide ? StepMs : 0;
		overlay.JumpDelayMs = config.OverlaySlide ? 0 : config.EffectiveOverlayJumpDelayMs;
		overlay.KeyDelayMs = config.MoveKeyDelayMs;
		overlay.StartCheckMs = config.MoveStartCheckMs;
		overlay.SnapTiles = config.OverlaySnapTiles;

		debugPanel.Visible = debug || startupImagePath is not null;
		timer.Interval = config.IntervalMs;
		latencyProbe = new LatencyProbe( CoordinateScreenRect, TerrainScreenRect, () => running && !ImageMode && reader.IsGameActive() );
		moveKeys.Changed += ( previous, held ) => MoveInput_Changed( previous | mouseMoveKey, held | mouseMoveKey );
		moveKeys.SkillPressed += MoveKeys_SkillPressed;
		moveMouse.Pressed += cursor => {
			// UI の上を押したときは歩かない (押したまま UI の外へ動かしても歩かない)。
			mouseMoveClient = reader.FindClientRect() is { } client && client.Contains( cursor )
				&& !GameUi.Contains( client.Size, reader.LastTransform, new Point( cursor.X - client.X, cursor.Y - client.Y ) )
				? client
				: null;
			SetMouseMoveKey( cursor );
		};
		moveMouse.Moved += cursor => SetMouseMoveKey( cursor );
		moveMouse.Released += () => {
			mouseMoveClient = null;
			SetMouseMoveKey( null );
		};
		overlay.CaptureStepBaseline = CaptureStepBaseline;
		overlay.ContinueDirection = ContinueDirection;
		overlay.CoordinateChanged = CoordinateChanged;
		overlay.ResolveDirection = ( from, predicted ) => ResolveStepDirection( from.X, from.Y, predicted );
		overlay.ActiveDirection = RecentDirection;
		overlay.CoordinateChangeAgo = () => running && !ImageMode && reader.LastCoordinateChangeTimestamp is var changed and not 0
			? System.Diagnostics.Stopwatch.GetElapsedTime( changed ).TotalMilliseconds
			: null;
		overlay.EventLogged = AddEventLog;
		// UI が描かれていない画面を撮ったとき (読み取りのスレッドからも来る)。
		reader.FrameRejected += message => {
			if( IsHandleCreated && !closing ) {
				BeginInvoke( () => AddEventLog( message ) );
			}
		};
		UpdateMoveKeyWatcher();
		if( config.WindowX is { } x && config.WindowY is { } y && Screen.AllScreens.Any( screen => screen.WorkingArea.Contains( x + 40, y + 20 ) ) ) {
			StartPosition = FormStartPosition.Manual;
			Location = new Point( x, y );
		}
	}

	// ---- イベントハンドラ (デザイナーから接続) ------------------------------------------

	private void ToggleButton_Click( object? sender, EventArgs e ) => ToggleRunning();

	private void CloseButton_Click( object? sender, EventArgs e ) => Close();

	private void TrayIcon_DoubleClick( object? sender, EventArgs e ) => ShowFromTray();

	private void TrayShowItem_Click( object? sender, EventArgs e ) => ShowFromTray();

	private void TrayExitItem_Click( object? sender, EventArgs e ) {
		// 隠したまま閉じる。隠した画面の位置はトレイに隠したときに覚えてあり、閉じるときも見えていない画面の位置は書き換えない。
		exiting = true;
		Close();
	}

	private void SettingsButton_Click( object? sender, EventArgs e ) => OpenSettings();

	private void ImageButton_Click( object? sender, EventArgs e ) => OpenImage();

	private void LiveButton_Click( object? sender, EventArgs e ) => ExitImageMode();

	private void Timer_Tick( object? sender, EventArgs e ) => Tick();

	private void MapSelect_SelectedIndexChanged( object? sender, EventArgs e ) => MapSelectionChanged();

	private void WindowSelect_DropDown( object? sender, EventArgs e ) => LoadWindowChoices();

	private void WindowSelect_SelectedIndexChanged( object? sender, EventArgs e ) => WindowSelectionChanged();

	private void LayerBox_CheckedChanged( object? sender, EventArgs e ) {
		if( loadingLayers ) {
			return;
		}
		foreach( var (layer, controls) in layerControls ) {
			if( controls.Box == sender ) {
				config.SetOverlayLayerShown( layer, controls.Box.Checked );
			}
		}
		OverlaySettingsChanged();
	}

	private void GridBox_CheckedChanged( object? sender, EventArgs e ) {
		if( loadingLayers ) {
			return;
		}
		config.ShowGrid = gridBox.Checked;
		OverlaySettingsChanged();
	}

	private void PlayerBox_CheckedChanged( object? sender, EventArgs e ) {
		if( loadingLayers ) {
			return;
		}
		config.ShowPlayer = playerBox.Checked;
		OverlaySettingsChanged();
	}

	private void CustomBox_CheckedChanged( object? sender, EventArgs e ) {
		if( loadingLayers ) {
			return;
		}
		var index = customControls.FindIndex( controls => controls.Box == sender );
		if( index < 0 || index >= config.CustomGroups.Count ) {
			return;
		}
		config.CustomGroups[index].Shown = customControls[index].Box.Checked;
		OverlaySettingsChanged();
		if( customForm is { IsDisposed: false } ) { // 閉じた編集画面 (破棄済み) は更新しない
			customForm.RefreshGroups();
		}
	}

	private void SetStatus( string text, Color color ) {
		statusLabel.Text = text;
		statusLabel.ForeColor = color;
	}

	/// <param name="detail">マウスを乗せたときに出す詳しい内容 (エラーの理由など)。</param>
	private void SetUpdateStatus( string text, Color color, string? detail = null ) {
		updateStatusLabel.Text = text;
		updateStatusLabel.ForeColor = color;
		updateStatusTip.SetToolTip( updateStatusLabel, detail ?? "" );
	}

	private void Save() {
		try {
			config.Save( configPath );
		} catch( Exception exception ) when( exception is IOException or UnauthorizedAccessException ) {
			SetStatus( $"設定を保存できません: {exception.Message}", ErrorColor );
		}
	}

	// ---- OCR の準備 --------------------------------------------------------

	protected override async void OnShown( EventArgs e ) {
		base.OnShown( e );
		foregroundHook = GameWindow.HookForegroundChanged( foregroundChanged ); // 失敗してもタイマーの判定で動く
		// 設定画面と情報画面は、前に終了したときに開いていたものだけ開く。
		if( config.SettingsWindowOpen ) {
			OpenSettings();
		}
		if( config.InfoWindowOpen ) {
			( (ISettingsHost)this ).ShowInfo();
		}
		await PrepareOcrAsync();
		timer.Enabled = running; // 準備中に一時停止されていたら止めたまま
		if( startupImagePath is not null ) {
			await LoadImageAsync( startupImagePath );
		}
		if( config.CheckUpdatesOnStartup ) {
			await CheckUpdatesAsync( quiet: true );
		}
	}

	private void LoadOverlayData() {
		try {
			overlayData = OverlayData.Load( OverlayData.DefaultDirectory );
			overlayError = "";
		} catch( Exception exception ) when( exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or KeyNotFoundException or InvalidOperationException ) {
			overlayData = null;
			overlayError = $"データを読み込めません ({exception.Message})";
		}
	}

	// ---- 自動更新 ------------------------------------------------------------

	/// <summary>
	/// update_url のマニフェストで、本体とマップ情報の新しい版を調べる。見つかったものごとに確認してから更新する。
	/// マップ情報はその場で読み込み直し、本体は更新後に再起動する。
	/// </summary>
	/// <param name="quiet">起動時の確認。更新がないときや確認できないときはステータスに出すだけにする。</param>
	private async Task CheckUpdatesAsync( bool quiet ) {
		if( updating ) {
			return;
		}
		if( !Uri.TryCreate( config.UpdateUrl, UriKind.Absolute, out var manifestUri ) ) {
			if( !quiet ) {
				MessageBox.Show( this, $"更新情報の URL (設定 update_url) が正しくありません。{Environment.NewLine}{config.UpdateUrl}", Text,
					MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			SetUpdateStatus( "更新情報の URL が正しくありません", ErrorColor, config.UpdateUrl );
			return;
		}
		updating = true;
		SetUpdateStatus( "更新を確認中…", MutedColor );
		try {
			IReadOnlyList<AvailableUpdate> updates;
			try {
				updates = await updater.CheckAsync( manifestUri );
			} catch( Exception exception ) when( exception is HttpRequestException or TaskCanceledException or IOException or InvalidDataException or UnauthorizedAccessException ) {
				SetUpdateStatus( "更新を確認できません", WarnColor, exception.Message );
				if( !quiet ) {
					MessageBox.Show( this, $"更新を確認できません。{Environment.NewLine}{exception.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
				return;
			}
			SetUpdateStatus( updates.Count == 0 ? "最新版です" : "新しい版があります", updates.Count == 0 ? MutedColor : WarnColor,
				string.Join( Environment.NewLine, updates.Select( update => $"{( update.Component == UpdateComponent.App ? "本体" : "マップ情報" )} {update.Package.Version}" ) ) );
			if( updates.Count == 0 && !quiet ) {
				MessageBox.Show( this, $"最新の版です。{Environment.NewLine}{VersionSummary}", Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
			// マップ情報は再起動なしで入れ替わるので先に、本体 (再起動する) は最後に
			foreach( var update in updates.OrderBy( update => update.Component == UpdateComponent.App ) ) {
				if( !await ApplyUpdateAsync( update ) ) {
					continue;
				}
				if( update.Component == UpdateComponent.App ) {
					Updater.RestartAfterAppUpdate();
					Close();
					return;
				}
			}
		} finally {
			updating = false;
		}
	}

	/// <summary>ダイアログに出す今の版。「：」の位置をそろえる。</summary>
	private string VersionSummary => AlignedLines( ("本体", Updater.AppVersion), ("マップ情報", updater.DataVersion) );

	/// <summary>
	/// 「項目名：値」の行を、「：」の位置がそろうように作る。MessageBox の文字は幅がそろっていないので、
	/// MessageBox のフォントで幅を測り、足りない分をスペースで埋める (スペースの幅の分だけずれることはある)。
	/// </summary>
	private static string AlignedLines( params (string Caption, string Value)[] lines ) {
		var font = SystemFonts.MessageBoxFont ?? DefaultFont;
		int Width( string text ) => TextRenderer.MeasureText( text, font, Size.Empty, TextFormatFlags.NoPadding ).Width;
		var target = lines.Max( line => Width( line.Caption ) );
		return string.Join( Environment.NewLine, lines.Select( line => {
			var caption = line.Caption;
			// 全角スペースで埋められるだけ埋めてから、残りを半角スペースで埋める
			while( Width( caption + "　" ) <= target ) {
				caption += "　";
			}
			while( Width( caption + " " ) <= target + Width( " " ) / 2 ) {
				caption += " ";
			}
			return $"{caption}：{line.Value}";
		} ) );
	}

	/// <summary>確認して更新する。更新したら true。</summary>
	private async Task<bool> ApplyUpdateAsync( AvailableUpdate update ) {
		var (name, after) = update.Component == UpdateComponent.App ? ("本体", "更新後に再起動します。") : ("マップ情報", "");
		var notes = string.IsNullOrWhiteSpace( update.Package.Notes ) ? "" : $"{Environment.NewLine}{Environment.NewLine}{update.Package.Notes.Trim()}";
		var answer = MessageBox.Show(
			this,
			$"{name}の新しい版があります。更新しますか? {after}{Environment.NewLine}{Environment.NewLine}" +
			$"現在 {update.CurrentVersion} → 新しい版 {update.Package.Version} ({update.Package.Size / 1024.0 / 1024.0:0.0}MB){notes}",
			Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question );
		if( answer != DialogResult.Yes ) {
			return false;
		}
		var progress = new Progress<string>( message => SetUpdateStatus( message, MutedColor ) );
		try {
			await updater.ApplyAsync( update, progress );
		} catch( Exception exception ) when( exception is HttpRequestException or TaskCanceledException or IOException or InvalidDataException
			  or UnauthorizedAccessException or System.Text.Json.JsonException or KeyNotFoundException or InvalidOperationException ) {
			MessageBox.Show( this, $"{name}を更新できませんでした (元の版のままです)。{Environment.NewLine}{exception.Message}", Text,
				MessageBoxButtons.OK, MessageBoxIcon.Warning );
			SetUpdateStatus( $"{name}を更新できませんでした", ErrorColor, exception.Message );
			return false;
		}
		if( update.Component == UpdateComponent.Data ) {
			LoadOverlayData();
			LoadMapChoices();
			lastLiveReading = null;
			settingsForm?.RefreshValues();
		}
		SetUpdateStatus( $"{name}を {update.Package.Version} に更新しました", OkColor );
		return true;
	}

	/// <summary>必要ならモデルをダウンロードし、使う言語のモデルを読み込む。</summary>
	private async Task PrepareOcrAsync() {
		ready = false;
		if( config.OcrBackend == "paddle" && !await EnsureModelsAsync() ) {
			await ChangeBackendAsync( "windows" ); // 準備し直しもここで行う
			return;
		}
		if( !NoOcr ) {
			SetStatus( $"{reader.Ocr.Name} のモデルを読み込み中…", MutedColor );
		}
		var language = config.OcrLanguage;
		try {
			await WithOcrAsync( () => {
				reader.Ocr.WarmUp( language );
				reader.Ocr.WarmUp( "en" );
			} );
			ready = true;
			SetStatus( running ? "準備完了" : "一時停止中", running ? OkColor : MutedColor );
		} catch( Exception exception ) {
			SetStatus( $"OCR を準備できません: {exception.Message}", ErrorColor );
		}
	}

	private async Task<bool> EnsureModelsAsync() {
		var missing = models.MissingModels( config.OcrLanguage );
		if( missing.Count == 0 ) {
			return true;
		}
		var list = string.Join( Environment.NewLine, missing.Select( model => $"・{model.FileName} ({model.ApproximateSize})" ) );
		var answer = MessageBox.Show(
			this,
			$"PaddleOCR のモデルがありません。ダウンロードしますか?{Environment.NewLine}{Environment.NewLine}{list}{Environment.NewLine}{Environment.NewLine}" +
			$"ダウンロード元: modelscope.cn (RapidOCR 配布版, Apache License 2.0){Environment.NewLine}保存先: {OcrModelStore.DownloadDirectory}{Environment.NewLine}{Environment.NewLine}" +
			"[いいえ] を選ぶと Windows 標準 OCR を使います。",
			Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question );
		if( answer != DialogResult.Yes ) {
			return false;
		}
		var progress = new Progress<string>( message => SetStatus( message, MutedColor ) );
		try {
			foreach( var model in missing ) {
				await OcrModelStore.DownloadAsync( model, progress );
			}
			return true;
		} catch( Exception exception ) {
			MessageBox.Show( this, $"ダウンロードに失敗しました。Windows 標準 OCR を使います。{Environment.NewLine}{exception.Message}", Text,
				MessageBoxButtons.OK, MessageBoxIcon.Warning );
			return false;
		}
	}

	private async Task WithOcrAsync( Action action ) {
		await ocrGate.WaitAsync();
		try {
			await Task.Run( action );
		} finally {
			ocrGate.Release();
		}
	}

	private async Task ChangeBackendAsync( string backend ) {
		if( backend == activeBackend ) {
			return;
		}
		activeBackend = config.OcrBackend = backend;
		Save();
		await WithOcrAsync( () => reader.ReplaceOcr( OcrEngineFactory.Create( backend, models, config.EffectiveOcrThreads ) ) );
		reader.Tracker.Reset();
		lastLiveReading = null;
		UpdateOcrControls();
		await PrepareOcrAsync();
		await ReadImageIfActiveAsync();
	}

	/// <summary>OCR なし (マップ名と座標を読まない) か。</summary>
	private bool NoOcr => activeBackend == NoOcrEngine.Backend;

	/// <summary>OCR なしのときは、マップを選べない (座標が分からないのでマスの種類は描けない)。</summary>
	private void UpdateOcrControls() {
		mapSelect.Enabled = !NoOcr;
		if( NoOcr ) {
			nameValue.Text = "—";
			coordinateValue.Text = "—";
		}
	}

	/// <param name="threads">null なら自動。</param>
	private async Task ChangeOcrThreadsAsync( int? threads ) {
		if( threads == config.OcrThreads ) {
			return;
		}
		config.OcrThreads = threads;
		Save();
		if( activeBackend != "paddle" ) {
			return; // Windows 標準 OCR では使わない。PaddleOCR に切り替えたときに反映される
		}
		// 推論セッションはスレッド数を後から変えられないので、エンジンごと作り直す。
		await WithOcrAsync( () => reader.ReplaceOcr( OcrEngineFactory.Create( activeBackend, models, config.EffectiveOcrThreads ) ) );
		await PrepareOcrAsync();
		await ReadImageIfActiveAsync();
	}

	private void ChangeInterval( int intervalMs ) {
		if( intervalMs == config.IntervalMs ) {
			return;
		}
		config.IntervalMs = intervalMs;
		timer.Interval = intervalMs;
		Save();
	}

	/// <summary>config のマップ名の読み取りの項目を書き換えた後に呼ぶ。保存して読み取り中の状態に反映する。</summary>
	private void NameSettingsChanged() {
		Save();
		reader.Tracker.ConfirmHits = config.NameConfirmHits;
		reader.Tracker.HoldSeconds = config.NameHoldSeconds;
		// 読み直す間隔 (NameRefreshSeconds) は MapReader が読み取りのたびに config から読む。
	}

	private async Task ChangeLanguageAsync( string language ) {
		if( language == config.OcrLanguage ) {
			return;
		}
		config.OcrLanguage = language;
		Save();
		LoadMapChoices();
		reader.Tracker.Reset();
		reader.Invalidate();
		await PrepareOcrAsync(); // 言語ごとにモデルが別なので、未配置ならダウンロードを案内する
		await ReadImageIfActiveAsync();
	}

	// ---- マップの手動選択 ----------------------------------------------------------

	private const string AutoMapChoice = "マップ: 自動 (OCR)";

	/// <summary>マップの選択肢を作り直す (データの更新・言語の変更のとき)。選んでいたマップはデータにあれば選び直す。</summary>
	private void LoadMapChoices() {
		var selected = reader.ManualMapName;
		var id = overlayData?.FindMapId( selected );
		loadingMaps = true;
		try {
			mapSelect.BeginUpdate();
			mapSelect.Items.Clear();
			mapSelect.Items.Add( AutoMapChoice );
			var names = overlayData?.MapNames( config.OcrLanguage ) ?? [];
			foreach( var name in names ) {
				mapSelect.Items.Add( name );
			}
			var index = id is null ? -1 : names.ToList().FindIndex( name => overlayData!.FindMapId( name ) == id );
			mapSelect.SelectedIndex = index + 1;
			mapSelect.EndUpdate();
		} finally {
			loadingMaps = false;
		}
		if( mapSelect.SelectedIndex == 0 && selected is not null ) {
			MapSelectionChanged(); // 選んでいたマップがなくなったので自動に戻す
		}
	}

	private void MapSelectionChanged() {
		if( loadingMaps ) {
			return;
		}
		var manual = mapSelect.SelectedIndex > 0 ? (string)mapSelect.SelectedItem! : null;
		if( manual == reader.ManualMapName ) {
			return;
		}
		reader.ManualMapName = manual;
		reader.Invalidate(); // 自動に戻したらすぐマップ名を読み直す
		lastLiveReading = null;
		nameValue.Text = manual ?? reader.Tracker.MapName ?? "—";
		_ = ReadImageIfActiveAsync();
		if( !ImageMode && running ) {
			Tick();
		}
	}

	// ---- キャプチャ対象のウィンドウ ------------------------------------------------------

	// 設定で覚えているウィンドウが今は見つからないときの選択肢。
	private sealed record MissingWindowChoice( string Title, string ProcessName ) {
		public override string ToString() => $"{Title} ({ProcessName})";
	}

	private bool loadingWindows;

	/// <summary>ウィンドウの選択肢を作り直す (起動時・一覧を開いたとき)。今の対象を選んだ状態にする。</summary>
	private void LoadWindowChoices() {
		var current = reader.Window != 0 ? reader.Window : GameWindow.Find( config.ProcessName, config.WindowTitle );
		loadingWindows = true;
		try {
			windowSelect.BeginUpdate();
			windowSelect.Items.Clear();
			var selected = -1;
			foreach( var candidate in GameWindow.Candidates() ) {
				if( candidate.Hwnd == current ) {
					selected = windowSelect.Items.Count;
				}
				windowSelect.Items.Add( candidate );
			}
			if( selected < 0 ) {
				selected = windowSelect.Items.Count;
				windowSelect.Items.Add( new MissingWindowChoice( config.WindowTitle, config.ProcessName ) );
			}
			windowSelect.SelectedIndex = selected;
			windowSelect.EndUpdate();
		} finally {
			loadingWindows = false;
		}
	}

	private void WindowSelectionChanged() {
		if( loadingWindows || windowSelect.SelectedItem is not GameWindow.Candidate candidate || candidate.Hwnd == reader.Window ) {
			return;
		}
		config.ProcessName = candidate.ProcessName;
		config.WindowTitle = candidate.Title;
		reader.SelectWindow( candidate.Hwnd );
		Save();
		AddEventLog( $"キャプチャ対象: {candidate}" );
		lastLiveReading = null;
		nameValue.Text = reader.ManualMapName ?? "—";
		coordinateValue.Text = "—";
		HideOverlay();
		UpdateMoveKeyWatcher();
		if( !ImageMode && running ) {
			Tick();
		}
	}

	// ---- 読み取りループ ----------------------------------------------------------

	private async void Tick() {
		UpdateMoveKeyWatcher(); // 前面のウィンドウの変化を取りこぼしても、ここで合わせる
		if( !ready || !running || busy || selecting || closing || ImageMode ) {
			return;
		}
		if( !NoOcr && OverlapsReadingRegions() ) {
			SetStatus( "本ツールのウィンドウが読み取り領域に重なっています。移動してください", ErrorColor );
			// 読み取らない (写り込んだ本ツールを読んでしまう) が、OCR によらないもの (グリッドなど) は座標を読めないときと同じく描く。
			if( reader.IsGameForeground() ) {
				var unread = new Reading { Status = ReadingStatus.Ok };
				ShowOverlayScene( BuildOverlayScene( unread ), null );
			} else {
				HideOverlay();
			}
			return;
		}
		busy = true;
		try {
			Reading? reading = null;
			var readWatch = System.Diagnostics.Stopwatch.StartNew();
			var readStartAt = latencyProbe.Now;
			await WithOcrAsync( () => reading = reader.Read() );
			if( !running ) {
				return; // 読み取り中に一時停止された: 結果は使わない (表示を「一時停止中」のままにする)
			}
			var recognizedAt = latencyProbe.Now;
			// 座標が変わってから描くまでの遅れの見積もり: 次の読み取りまでの待ち (平均で間隔の半分) + 読み取りにかかった時間。歩く速さを測るのに使う。
			var lagMs = config.IntervalMs / 2.0 + readWatch.Elapsed.TotalMilliseconds;
			if( reading is not null ) {
				RememberCoordinateImage( reading );
				// 座標欄の見た目が変わって読み直したときだけ、覚えた文字の画像 (キャッシュ) と OCR のどちらで読んだかを残す。
				if( reader.LastCoordinateOcrMs is { } coordinateMs ) {
					AddEventLog( $"座標読み取り: 「{reading.RawCoordinates ?? "-"}」 {( reader.LastCoordinateFromGlyphs ? "キャッシュ" : "OCR" )} {coordinateMs:0.0} ms" );
				}
			}
			if( !closing ) {
				UpdateMoveSpeed();
			}
			if( !closing && reading is not null ) {
				ShowReading( reading );
				if( reader.IsGameForeground() ) {
					UpdateOverlay( reading, lagMs );
					if( latencyProbe.Enabled && reading.Ok && reading.Coordinates is { } coordinates && coordinates != probeCoordinates ) {
						latencyProbe.OnRecognized( readStartAt, recognizedAt, latencyProbe.Now );
					}
					if( reading.Ok && reading.Coordinates is { } readCoordinates ) {
						LearnMoveKeyDirection( probeCoordinates, readCoordinates );
						probeCoordinates = readCoordinates;
					}
				} else {
					HideOverlay(); // 読み取り中に他のウィンドウへ切り替わった
				}
			}
		} catch( Exception exception ) {
			SetStatus( $"エラー: {exception.Message}", ErrorColor );
		} finally {
			busy = false;
			latencyProbe.Expire();
		}
	}

	private void ForegroundChanged() {
		UpdateMoveKeyWatcher();
		if( closing || ImageMode || !running ) {
			return;
		}
		if( !reader.IsGameForeground() ) {
			HideOverlay();
		} else {
			Tick(); // 前面に戻ったらすぐ読み直して表示する (読み取り中なら Tick が何もしない)
		}
	}

	private bool OverlapsReadingRegions() {
		if( reader.FindClientRect() is not { } client ) {
			return false;
		}
		// 設定画面も、開いたままゲームを操作できるので重なっていないか確かめる。
		var windows = settingsForm is { Visible: true } settings ? new[] { Bounds, settings.Bounds } : [Bounds];
		foreach( var kind in new[] { RegionKind.Name, RegionKind.Coordinates } ) {
			if( reader.RegionInClient( kind, client.Size, reader.LastTransform ) is { } region ) {
				region.Offset( client.Location );
				if( windows.Any( window => window.IntersectsWith( region ) ) ) {
					return true;
				}
			}
		}
		return false;
	}

	private void ShowReading( Reading reading ) {
		if( reading.Status == ReadingStatus.Background ) {
			SetStatus( reading.Message, MutedColor );
			return; // 最後の結果をそのまま表示しておく
		}
		if( !reading.Ok ) {
			SetStatus( reading.Message, ErrorColor );
		} else if( NoOcr ) {
			SetStatus( "OCR なし (グリッド・プレイヤーだけ表示)", OkColor );
			nameValue.Text = "—";
			coordinateValue.Text = "—";
			rawLabel.Text = "OCR: なし";
			return;
		} else if( reading.MapName is null ) {
			SetStatus( "マップ名を確認中…", WarnColor );
		} else if( reader.ManualMapName is not null ) {
			SetStatus( "読み取り中 (マップは手動で選択)", OkColor );
		} else if( reading.NameStale ) {
			SetStatus( "マップ名を読み取れていません (最後の結果を表示中)", WarnColor );
		} else {
			SetStatus( "読み取り中", OkColor );
		}

		nameValue.Text = reading.MapName ?? "—";
		nameValue.ForeColor = reading.NameStale ? MutedColor : ForeColor;
		coordinateValue.Text = reading.Coordinates?.ToString() ?? "—";
		coordinateValue.ForeColor = reading.CoordinatesStale ? MutedColor : ForeColor;
		var uiTransform = new UiTransform( reading.UiScale, reading.UiScaleY );
		var scale = uiTransform.IsIdentity ? "" : $"　UI の倍率 {uiTransform}";
		rawLabel.Text = $"OCR: 名前「{reading.RawName ?? "-"}」({reading.NameScore:0.00})　座標「{reading.RawCoordinates ?? "-"}」{scale}　オーバーレイ {( HasOverlayData( reading.MapName ) ? "あり" : "なし" )}";
		if( reading.NameImage is not null && reading.CoordinateImage is not null ) {
			UpdatePreview( reading.NameImage, reading.CoordinateImage );
		}
	}

	// ---- オーバーレイ ----------------------------------------------------------

	/// <summary>種類ごとのチェックボックスと色の見本を、設定に合わせる。</summary>
	private void LoadLayerSettings() {
		loadingLayers = true;
		try {
			foreach( var (layer, controls) in layerControls ) {
				controls.Box.Checked = config.IsOverlayLayerShown( layer );
				controls.Swatch.BackColor = Color.FromArgb( 255, config.GetOverlayColor( layer ) ); // 見本は不透明で見せる
			}
			gridBox.Checked = config.ShowGrid;
			gridSwatch.BackColor = Color.FromArgb( 255, config.GetGridColor() );
			playerBox.Checked = config.ShowPlayer;
			playerSwatch.BackColor = Color.FromArgb( 255, config.GetPlayerColor() );
			LoadCustomControls();
			customTiles = config.ShownCustomGroups.Select( group => group.ToCustomTiles() ).ToList();
		} finally {
			loadingLayers = false;
		}
	}

	/// <summary>カスタムのグループごとの色の見本とチェックボックスを、設定に合わせる (数が変わったら作り直す)。</summary>
	private void LoadCustomControls() {
		var groups = config.CustomGroups;
		if( customControls.Count != groups.Count ) {
			layersPanel.SuspendLayout();
			foreach( var (swatch, box) in customControls ) {
				layersPanel.Controls.Remove( swatch );
				layersPanel.Controls.Remove( box );
				swatch.Dispose();
				box.Dispose();
			}
			customControls.Clear();
			layersPanel.RowCount = Math.Max( layersPanel.RowCount, CustomFirstRow + groups.Count );
			while( layersPanel.RowStyles.Count < layersPanel.RowCount ) {
				layersPanel.RowStyles.Add( new RowStyle() );
			}
			for( var i = 0; i < groups.Count; i++ ) {
				// 大きさと余白は、DPI に合わせて拡大済みのグリッドの見本・チェックボックスに揃える。
				var swatch = new Panel { Anchor = AnchorStyles.Left, BorderStyle = BorderStyle.FixedSingle, Size = gridSwatch.Size, Margin = gridSwatch.Margin };
				var box = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = gridBox.Margin, UseVisualStyleBackColor = true };
				box.CheckedChanged += CustomBox_CheckedChanged;
				layersPanel.Controls.Add( swatch, 2, CustomFirstRow + i );
				layersPanel.Controls.Add( box, 3, CustomFirstRow + i );
				customControls.Add( (swatch, box) );
			}
			layersPanel.ResumeLayout( true );
		}
		for( var i = 0; i < groups.Count; i++ ) {
			var (swatch, box) = customControls[i];
			swatch.BackColor = Color.FromArgb( 255, groups[i].GetColor() ); // 見本は不透明で見せる
			box.Text = groups[i].Name.Length > 0 ? groups[i].Name : "(名前なし)";
			box.Checked = groups[i].Shown;
		}
	}

	/// <summary>そのマップのデータが 1 種類でもあるか (チェックボックスで表示を切っているかには関係なく)。</summary>
	private bool HasOverlayData( string? mapName ) =>
		overlayData is { } data && data.FindMapId( mapName ) is { } mapId && OverlayLayer.All.Any( layer => data.Find( layer, mapId ) is not null );

	/// <summary>config のオーバーレイの項目 (表示・色・透明度) を書き換えた後に呼ぶ。保存して表示を更新する。</summary>
	private void OverlaySettingsChanged() {
		LoadLayerSettings();
		overlay.AntiAlias = config.OverlayAntiAlias;
		overlay.ShowChunks = config.OverlayShowChunks;
		Save();
		if( ImageMode || !running ) {
			return;
		}
		if( lastLiveReading is not null ) {
			UpdateOverlay( lastLiveReading );
		} else if( !config.OverlayEnabled ) {
			HideOverlay();
		}
	}

	private void UpdateOverlay( Reading reading, double lagMs = 0 ) {
		if( reading.Ok ) {
			lastLiveReading = reading;
		} else if( reading.Status == ReadingStatus.Background && ActiveForm is not null && lastLiveReading is not null ) {
			// 本ツールのウィンドウ (設定画面など) を操作している間はゲームが前面にないので読み取りは止まるが、
			// 直前の結果で表示を続ける (設定を変えたときに見た目を確かめられるように)。
			reading = lastLiveReading;
		}
		var scene = BuildOverlayScene( reading );
		ShowOverlayScene( scene, scene is null ? null : reading.Coordinates, lagMs );
	}

	private void ShowOverlayScene( OverlayScene? scene, GameCoordinate? coordinates, double lagMs = 0 ) {
		if( !running ) {
			// 一時停止中は出さない (一時停止する前に始まった読み取りが後から表示しようとしても)。
			scene = null;
			coordinates = null;
		}
		shownCoordinates = coordinates;
		overlay.ShowScene( scene, lagMs );
	}

	private void HideOverlay() => ShowOverlayScene( null, null );

	// ゲームのウィンドウを初めて見つけてから、マップ名と座標の読み取りを待つ時間 (秒)。
	private const double StartupWaitSeconds = 1.0;
	private System.Diagnostics.Stopwatch? startupWait;

	private OverlayScene? BuildOverlayScene( Reading reading ) {
		// ウィンドウがない・裏にある・撮影できない (画面ロック中など) ときは出さない。
		// 座標やマップ名を読めないとき (読み取り領域がない・座標欄が黒い・OCR で読めない) は、OCR によらないものだけ描く。
		if( !config.OverlayEnabled || reading.Status is ReadingStatus.NoWindow or ReadingStatus.Background or ReadingStatus.CaptureFailed
			|| reader.FindClientRect() is not { } client ) {
			return null;
		}
		var coordinates = reading.Ok && !reading.CoordinatesStale ? reading.Coordinates : null;
		// 起動直後は座標とマップ名の確定を少し待つ (先にグリッドだけで描くと、読めた次のコマで地形を全部描き直すことになる)。
		// 待っても読めなければ、グリッドとプレイヤーの枠・自分で描いたマスは OCR によらないので描き始める。
		startupWait ??= System.Diagnostics.Stopwatch.StartNew();
		if( !NoOcr && ( coordinates is null || reading.MapName is null ) && startupWait.Elapsed.TotalSeconds < StartupWaitSeconds ) {
			return null;
		}
		// マスの種類は、別のマップの座標で描かないよう、マップ名と座標がどちらも読めているときだけ描く。
		// グリッドとプレイヤーの枠・自分で描いたマスはマップによらないので、マップの情報がなくても描く。
		// 座標が読めないときは、キャラクターは常に画面の中央にいるので、(0, 0) として中央に合わせて描く (OCR なしと同じ)。
		var layers = new List<(OverlayLayer Layer, TileRects Tiles, Color Color)>();
		if( coordinates is not null && overlayData is { } data && reading.MapName is { } mapName && !reading.NameStale && data.FindMapId( mapName ) is { } mapId ) {
			foreach( var layer in OverlayLayer.All.Where( config.IsOverlayLayerShown ) ) {
				if( data.Find( layer, mapId ) is { } tiles ) {
					layers.Add( (layer, tiles, config.GetOverlayColor( layer )) );
				}
			}
		}
		if( layers.Count == 0 && !config.ShowGrid && !config.ShowPlayer && customTiles.Count == 0 ) {
			return null;
		}
		var grid = IsoGrid.ForClient( client.Size, coordinates ?? new GameCoordinate( 0, 0 ) );
		return new OverlayScene( client, grid, layers,
			reader.RegionInClient( RegionKind.Name, client.Size, reader.LastTransform ),
			reader.RegionInClient( RegionKind.Coordinates, client.Size, reader.LastTransform ),
			config.ShowGrid ? config.GetGridColor() : null,
			GameUi.Rectangles( client.Size, reader.LastTransform, includeStatus: true ),
			config.ShowPlayer ? config.GetPlayerColor() : null,
			customTiles );
	}

	private void UpdatePreview( BgrImage nameImage, BgrImage coordinateImage ) {
		var combined = new Bitmap( Math.Max( nameImage.Width, coordinateImage.Width ), nameImage.Height + coordinateImage.Height + 2 );
		using( var graphics = Graphics.FromImage( combined ) )
		using( var name = nameImage.ToBitmap() )
		using( var coordinates = coordinateImage.ToBitmap() ) {
			graphics.Clear( preview.BackColor );
			graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			graphics.DrawImageUnscaled( name, 0, 0 );
			graphics.DrawImageUnscaled( coordinates, 0, nameImage.Height + 2 );
		}
		var old = preview.Image;
		preview.Image = combined;
		old?.Dispose();
	}

	// ---- 操作 -------------------------------------------------------------

	private void ToggleRunning() {
		running = !running;
		toggleButton.Text = running ? "一時停止" : "再開";
		// 一時停止中は読み取り・キーとマウスのフック・オーバーレイの表示をすべて止める。
		timer.Enabled = running;
		UpdateMoveKeyWatcher();
		if( running ) {
			SetStatus( "再開しました", MutedColor );
			Tick();
		} else {
			SetStatus( "一時停止中", MutedColor );
			HideOverlay();
		}
	}

	private void OpenSettings() {
		if( settingsForm is null || settingsForm.IsDisposed ) {
			settingsForm = new SettingsForm( this );
			settingsForm.FormClosing += ( _, e ) => {
				RememberSettingsWindow();
				// 自分で閉じたときだけ閉じた状態を覚える (メイン画面の終了で閉じるときは開いたまま)。
				if( e.CloseReason == CloseReason.UserClosing ) {
					config.SettingsWindowOpen = false;
				}
				Save();
			};
			settingsForm.Show( this );
			config.SettingsWindowOpen = true;
		} else {
			settingsForm.Activate();
		}
	}

	void ISettingsHost.ShowInfo() {
		if( infoForm is null || infoForm.IsDisposed ) {
			infoForm = new InfoForm( this );
			if( config.InfoWindowWidth is { } width && config.InfoWindowHeight is { } height ) {
				// 最小の大きさより小さくはならない (MinimumSize)。
				infoForm.Size = new Size( width, height );
			}
			if( config.InfoWindowX is { } x && config.InfoWindowY is { } y && Screen.AllScreens.Any( screen => screen.WorkingArea.Contains( x + 40, y + 20 ) ) ) {
				infoForm.Location = new Point( x, y ); // 前に閉じたときの位置 (見える画面の中にあるときだけ)
			} else {
				// 設定画面 (なければメイン画面) の右に並べる。画面からはみ出すなら作業領域に収める。
				var anchor = settingsForm is { Visible: true } settings ? (Form)settings : this;
				var area = Screen.FromControl( anchor ).WorkingArea;
				infoForm.Location = new Point(
					Math.Clamp( anchor.Right, area.Left, Math.Max( area.Left, area.Right - infoForm.Width ) ),
					Math.Clamp( anchor.Top, area.Top, Math.Max( area.Top, area.Bottom - infoForm.Height ) ) );
			}
			infoForm.FormClosing += ( _, e ) => {
				RememberInfoWindow();
				if( e.CloseReason == CloseReason.UserClosing ) {
					config.InfoWindowOpen = false;
				}
				Save();
			};
			infoForm.Show( this );
			config.InfoWindowOpen = true;
		} else {
			infoForm.Activate();
		}
	}

	/// <summary>
	/// ゲームのクライアント領域を撮る (カスタムの編集画面の背景用)。マスの幅は、その大きさでオーバーレイが使う幅。
	/// オーバーレイも画面のキャプチャに写るので、撮る間だけ隠す。
	/// </summary>
	private async Task<(Bitmap Image, double TileWidth)?> CaptureGameClientAsync() {
		if( reader.FindClientRect() is not { } client || client.Width < 2 || client.Height < 2 ) {
			return null;
		}
		overlay.Suppressed = true;
		try {
			await Task.Delay( 200 ); // 隠したオーバーレイが画面から消えるのを待つ
			var image = ScreenCapture.Capture( client ).ToBitmap();
			return (image, IsoGrid.ForClient( client.Size, new GameCoordinate( 0, 0 ) ).TileWidth);
		} finally {
			overlay.Suppressed = false;
		}
	}

	/// <summary>自分で描くマス (カスタム) の編集画面を開く。</summary>
	void ISettingsHost.ShowCustomTiles() {
		if( customForm is null || customForm.IsDisposed ) {
			customForm = new CustomTilesForm( config, OverlaySettingsChanged, CaptureGameClientAsync );
			customForm.Show( this );
		} else {
			customForm.Activate();
		}
	}

	/// <summary>設定画面の位置を config に書く (保存はしない)。</summary>
	private void RememberSettingsWindow() {
		if( settingsForm is { IsDisposed: false, Visible: true, WindowState: FormWindowState.Normal } settings ) {
			config.SettingsWindowX = settings.Location.X;
			config.SettingsWindowY = settings.Location.Y;
		}
	}

	/// <summary>情報画面の位置と大きさを config に書く (保存はしない)。</summary>
	private void RememberInfoWindow() {
		if( infoForm is { IsDisposed: false, Visible: true, WindowState: FormWindowState.Normal } info ) {
			config.InfoWindowX = info.Location.X;
			config.InfoWindowY = info.Location.Y;
			config.InfoWindowWidth = info.Width;
			config.InfoWindowHeight = info.Height;
		}
	}

	// 設定画面から呼ばれる操作 (ISettingsHost)。
	AppConfig ISettingsHost.Config => config;

	bool ISettingsHost.ImageMode => ImageMode;

	ClientRegion? ISettingsHost.RegionFor( RegionKind kind ) => ( ImageMode && imageConfig is not null ? imageConfig : config ).GetRegion( kind );

	Task ISettingsHost.SelectRegionAsync( RegionKind kind ) => SelectRegionAsync( kind );

	Task ISettingsHost.ChangeLanguageAsync( string language ) => ChangeLanguageAsync( language );

	Task ISettingsHost.ChangeBackendAsync( string backend ) => ChangeBackendAsync( backend );

	Task ISettingsHost.ChangeOcrThreadsAsync( int? threads ) => ChangeOcrThreadsAsync( threads );

	void ISettingsHost.ChangeInterval( int intervalMs ) => ChangeInterval( intervalMs );

	void ISettingsHost.NameSettingsChanged() => NameSettingsChanged();

	async Task ISettingsHost.CaptureSettingsChangedAsync() {
		Save();
		await WithOcrAsync( reader.ApplyCaptureSettings ); // 読み取り中の撮影と重ならないように
		AddEventLog( $"撮り方: {reader.CaptureName}　合成を待つ {( config.CaptureWaitComposition ? "オン" : "オフ" )}　UI の確認 {( config.CaptureUiCheck ? "オン" : "オフ" )}　座標の確定 {config.CoordinateConfirmHits} 回" );
	}

	string ISettingsHost.OverlayDataSummary => overlayData is not { } data
		? overlayError
		: data.MissingLayers.Any() ? $"{string.Join( "・", data.MissingLayers.Select( layer => layer.FileName ) )} がありません" : "";

	void ISettingsHost.OverlaySettingsChanged() => OverlaySettingsChanged();

	string IInfoSource.MotionStatus => overlay.MotionStatus + Environment.NewLine
		+ $"移動速度: {WalkSpeed.Grade( config.MoveSpeedGrade ).Name}　{( config.MoveSpeedAuto ? "自動" : "固定" )}"
		+ $"　実測 {( reader.WalkSpeed.MeasuredStepMs is { } measured ? $"{measured:0} ms ({WalkSpeed.Grade( reader.WalkSpeed.MeasuredGrade ?? 0 ).Name})" : "-" )}"
		+ ( config.OverlaySlideManual ? $"　スライド時間: 手動入力 {StepMs} ms" : "" );

	IReadOnlyList<string> IInfoSource.EventLog => [.. eventLog];

	long IInfoSource.EventLogVersion => eventLogVersion;

	void IInfoSource.ClearEventLog() {
		eventLog.Clear();
		overlay.ResetMaxGap();
		eventLogVersion++;
	}

	private void AddEventLog( string message ) => AddEventLog( message, 0 );

	/// <summary>depth はログの字下げの深さ (きっかけになった出来事の下に、その結果を 1 段下げて出す)。</summary>
	private void AddEventLog( string message, int depth ) {
		// キーを押した時刻を 0 ms として、そこからの経過時間を添える。
		var elapsed = keyDownAt is { } at ? $"{eventClock.Elapsed.TotalMilliseconds - at,4:0} ms" : "      - ms";
		eventLog.Enqueue( $"{elapsed} {new string( ' ', depth * 2 )}{message}" );
		if( eventLog.Count > MaxEventLog ) {
			eventLog.Dequeue();
		}
		eventLogVersion++;
	}

	/// <summary>
	/// 地形が動いたかを見る画面上の矩形。キャラクター (中央) の左上、クライアント領域の幅・高さの 1/5 の範囲。
	/// キャラクターの向きが変わるだけでは反応せず、画面が流れると大きく変わるところ。
	/// </summary>
	private Rectangle? TerrainScreenRect() =>
		reader.FindClientRect() is { } client
			? new Rectangle( client.X + client.Width * 3 / 20, client.Y + client.Height * 3 / 20, client.Width / 5, client.Height / 5 )
			: null;

	/// <summary>キーボードフックは、ゲームがアクティブな間だけ入れる。マウスフックは、さらに先読みをするときだけ入れる。</summary>
	private void UpdateMoveKeyWatcher() {
		moveKeys.SkillKey = !string.IsNullOrWhiteSpace( config.MoveSkillKey ) && Enum.TryParse<Keys>( config.MoveSkillKey, ignoreCase: true, out var skillKey ) ? (int)skillKey : 0;
		var active = running && !ImageMode && reader.IsGameActive();
		moveKeys.Enabled = active;
		moveMouse.Enabled = active && config.MoveKeyPrediction;
		if( !moveMouse.Enabled && mouseMoveKey != 0 ) {
			// 押している間にフックを外した: 離したことにする (離したのが見えなくなるので)。
			mouseMoveClient = null;
			SetMouseMoveKey( null );
		}
	}

	/// <summary>右クリックのカーソルの位置 (離したなら null) から、右クリックの仮想キーを決め直す。変わったら移動キーと同じように扱う。</summary>
	private void SetMouseMoveKey( Point? cursor ) {
		var key = cursor is { } point && mouseMoveClient is { } client && MouseDirection( client, point ) is { } direction
			? FirstMouseKey << Array.IndexOf( MouseDirections, direction )
			: 0;
		if( key == mouseMoveKey ) {
			return;
		}
		var previous = mouseMoveKey;
		mouseMoveKey = key;
		MoveInput_Changed( moveKeys.Held | previous, moveKeys.Held | key );
	}

	/// <summary>右クリックしたところ (画面座標) で歩く方向。キャラクターはクライアント領域の中央のマスにいる。</summary>
	private (int X, int Y)? MouseDirection( Rectangle client, Point cursor ) {
		var tile = IsoGrid.ForClient( client.Size, default ).TileAt( new PointF( cursor.X - client.X, cursor.Y - client.Y ) );
		// 斜めのときの向き: 動いている途中なら、先読みで進めている 1 歩の方向。
		return ClickDirection( tile.X, tile.Y, overlay.IsSliding && overlay.LastStepDirection is { } step ? step : facing );
	}

	/// <summary>
	/// キャラクターから (tileX, tileY) ずれたマスを右クリックしたときに歩く方向。
	/// 斜め 45° の線上のマス (X と Y のずれが同じ) なら、向いている方向 (facing) の軸で、カーソルのある側 (前か後ろ)。
	/// それより内側なら、ずれの大きい方の軸の方向 (上下左右、移動キーの方向)。キャラクターのマス、斜めで向きが分からないときは null。
	/// </summary>
	internal static (int X, int Y)? ClickDirection( int tileX, int tileY, (int X, int Y)? facing ) {
		if( Math.Abs( tileX ) > Math.Abs( tileY ) ) {
			return (Math.Sign( tileX ), 0);
		}
		if( Math.Abs( tileY ) > Math.Abs( tileX ) ) {
			return (0, Math.Sign( tileY ));
		}
		return tileX == 0 ? null
			: facing switch {
				{ X: not 0 } => (Math.Sign( tileX ), 0),
				{ Y: not 0 } => (0, Math.Sign( tileY )),
				_ => null,
			};
	}

	// ゲームの移動キーはスタック方式: 押している中で後から押したキーだけが効く (斜めには歩かない)。
	// 押した順に積み、離したら取り除く。いちばん上が今効いているキー (MoveKeyWatcher のビット 1 つ)。
	private readonly List<int> moveKeyStack = [];

	/// <summary>今効いている移動キー (押している中で最後に押したキー、ビット 1 つ)。押していなければ 0。</summary>
	private int ActiveMoveKey => moveKeyStack.Count > 0 ? moveKeyStack[^1] : 0;

	// 効いている移動キーの履歴 (eventClock の時刻、その時刻から効いていたキー)。古い順。ゲームは入力を遅れて反映するので、
	// 「少し前に効いていたキー」を引けるようにしておく。
	private const double MoveKeyHistoryMs = 3000;
	private readonly List<(double At, int Held)> moveKeyHistory = [];

	/// <summary>eventClock の時刻 at に効いていた移動キー (ビット 1 つ、なければ 0)。</summary>
	private int MoveKeysHeldAt( double at ) {
		var held = 0;
		var previousKey = 0;
		var index = -1;
		for( var i = 0; i < moveKeyHistory.Count && moveKeyHistory[i].At <= at; i++ ) {
			previousKey = held;
			held = moveKeyHistory[i].Held;
			index = i;
		}
		// 離してからすぐ (KeyGapMs 未満) 次のキーを押したときの空白は、ゲームには見えない (前のキーを押し続けていた扱いになる)。
		if( held == 0 && previousKey != 0 && index + 1 < moveKeyHistory.Count
			&& moveKeyHistory[index + 1].At - moveKeyHistory[index].At < KeyGapMs ) {
			return previousKey;
		}
		return held;
	}

	// 離してから次のキーを押すまでがこれより短ければ、離していなかったとみなす。
	private const double KeyGapMs = 50;

	/// <summary>eventClock の時刻 from から to まで、ずっと同じキーが効いていたか (効いていたらそれ、変わっていたら null)。</summary>
	private int? MoveKeysHeldThrough( double from, double to ) {
		var held = MoveKeysHeldAt( from );
		return moveKeyHistory.Any( entry => entry.At > from && entry.At <= to && entry.Held != held ) ? null : held;
	}

	/// <summary>移動キーと右クリックの仮想キーを合わせた組み合わせが変わった (前の組み合わせ, 今の組み合わせ)。</summary>
	private void MoveInput_Changed( int previous, int held ) {
		latencyProbe.OnMoveKeys( previous, held );
		var changedAt = eventClock.Elapsed.TotalMilliseconds;
		// 押した順に積む (同時に押されたものはビットの順)。離したキーは取り除く。
		var activeBefore = ActiveMoveKey;
		moveKeyStack.RemoveAll( key => ( held & key ) == 0 );
		for( var bit = 1; bit <= held; bit <<= 1 ) {
			if( ( held & bit ) != 0 && ( previous & bit ) == 0 ) {
				moveKeyStack.Add( bit );
			}
		}
		var active = ActiveMoveKey;
		if( active != activeBefore ) {
			moveKeyHistory.Add( (changedAt, active) );
			moveKeyHistory.RemoveAll( entry => entry.At < changedAt - MoveKeyHistoryMs && moveKeyHistory.Count > 1 );
		}
		if( previous == 0 && held != 0 ) {
			keyDownAt = eventClock.Elapsed.TotalMilliseconds;
			AddEventLog( $"キー押下: {MoveInputName( held )}" );
		} else if( held == 0 ) {
			AddEventLog( $"キー解放: {MoveInputName( previous )}" );
		} else {
			AddEventLog( $"キー変更: {MoveInputName( previous )} → {MoveInputName( held )}　効いているキー {MoveInputName( active )}" );
		}
		if( held != 0 ) {
			lastMoveKeys = held;
			lastMoveKeysAt = Environment.TickCount64;
		}
		if( active == activeBefore ) {
			return; // 効いているキーは変わっていない (後ろに積まれたキーを離した、または下に押した)
		}
		// 動いている途中のキー入力 (下) かどうかは、歩き始めの先読みを始める前の状態で見る。
		var wasSliding = overlay.IsSliding;
		// 歩き始め (どのキーも押していない状態からの押し下げ、または止まっている間に効いているキーが変わった) だけ先に動かす。
		// 止まっている間の切り替え: 壁で歩けないキーを押したまま別のキーを押したときなど。
		// スライド中は歩き始めの先読みをしない (着いたときに ContinueDirection で次の方向を決める)。比較元も、その歩の判定に使うので残す。
		if( !wasSliding && active != 0 && config.MoveKeyPrediction && running && !ImageMode && reader.IsGameActive()
			&& MoveDirection( active ) is { } direction && CanWalkTo( direction.X, direction.Y ) ) {
			stepBaseline = null;
			overlay.PredictMove( direction );
		} else if( !wasSliding && previous == 0 && active != 0 && config.MoveKeyPrediction ) {
			AddEventLog( MoveDirection( active ) is null ? $"先読みなし: {MoveInputName( active )} の方向未学習"
				: !CanWalkTo( MoveDirection( active )!.Value.X, MoveDirection( active )!.Value.Y ) ? "先読みなし: 行き先が壁 or 地図データなし"
				: "先読みなし: ゲーム非アクティブ等", 1 );
		}
		// 座標の変化で動いている途中に押した (方向を変えるときにキーを一度離したなど): 着いたマスから押した方向へ先に動かす。
		// 方向と壁は、着いたときに ContinueDirection で見る。
		if( wasSliding && previous == 0 && held != 0 && config.MoveKeyPrediction && config.MoveKeyRepredict && running && !ImageMode && reader.IsGameActive() ) {
			overlay.RepredictMove();
		}
	}

	/// <summary>今いるマスから (dx, dy) 隣のマスへ歩けそうか。</summary>
	private bool CanWalkTo( int dx, int dy ) =>
		lastLiveReading is { Coordinates: { } position, CoordinatesStale: false } && CanWalkTo( position.X, position.Y, dx, dy );

	/// <summary>マス (x, y) から (dx, dy) 隣のマスへ歩けそうか。オーバーレイの壁マスなら歩けない (表示を切っていても使う)。</summary>
	private bool CanWalkTo( int x, int y, int dx, int dy ) {
		if( lastLiveReading is not { MapName: { } mapName } || overlayData is not { } data || data.FindMapId( mapName ) is not { } mapId ) {
			return false;
		}
		return data.Find( OverlayLayer.ImpassableEdge, mapId ) is not { } walls || !walls.Contains( x + dx, y + dy );
	}

	/// <summary>移動キーの組み合わせで歩く方向 (覚えていなければ null)。右クリックの仮想キーなら、その方向 (覚えなくてよい)。</summary>
	private (int X, int Y)? MoveDirection( int held ) =>
		held >= FirstMouseKey ? MouseDirections[System.Numerics.BitOperations.Log2( (uint)( held / FirstMouseKey ) )]
		: held != 0 && config.MoveKeyDirections.TryGetValue( Native.MoveKeyWatcher.Name( held ), out var direction ) && direction is [var x, var y]
			? (x, y)
			: null;

	/// <summary>移動キーと右クリックの仮想キーの組み合わせの名前 (例: "W", "右クリック(0,-1)", "W+右クリック(1,0)")。</summary>
	private static string MoveInputName( int held ) {
		var names = new List<string>();
		if( Native.MoveKeyWatcher.Name( held & ( FirstMouseKey - 1 ) ) is { Length: > 0 } keys ) {
			names.Add( keys );
		}
		for( var i = 0; i < MouseDirections.Length; i++ ) {
			if( ( held & ( FirstMouseKey << i ) ) != 0 ) {
				names.Add( $"右クリック({MouseDirections[i].X},{MouseDirections[i].Y})" );
			}
		}
		return string.Join( "+", names );
	}

	// 読み取り (OCR) が撮影した座標欄の画素を、これより新しければ使い回す (撮影は 1 回 5〜20 ms かかり、同時に撮ると取り合う)。
	// OCR 1 回 (約 30 ms) の間は新しい撮影がないので、それより少し長くする。
	private const double MaxSharedPixelAgeMs = 40;

	// 直前の CaptureCoordinatePixels で、画素をどこから取ったか (ログに出す)。
	private string pixelSource = "";

	private byte[]? CaptureCoordinatePixels() {
		if( running && !ImageMode && reader.LatestCoordinatePixels is { } shared
			&& System.Diagnostics.Stopwatch.GetElapsedTime( shared.Timestamp ).TotalMilliseconds is var age and <= MaxSharedPixelAgeMs ) {
			pixelSource = $"読み取りの画像 {age:0.0} ms 前";
			return shared.Pixels;
		}
		pixelSource = $"撮影 ({reader.CaptureName})";
		return reader.CaptureCoordinatePixels();
	}

	/// <summary>
	/// マス (x, y) から歩き出したと分かったときの方向。今効いているキー、なければ最近 (1 歩の間に) 押して離したキー
	/// (ゲームは歩いている途中に押されたキーを、離されていても次の 1 歩に使うことがある)。歩けなければ null。
	/// </summary>
	private (int X, int Y)? RecentDirection( int x, int y ) {
		if( MoveDirection( ActiveMoveKey ) is { } active && CanWalkTo( x, y, active.X, active.Y ) ) {
			return active;
		}
		var now = eventClock.Elapsed.TotalMilliseconds;
		foreach( var (at, key) in Enumerable.Reverse( moveKeyHistory ) ) {
			if( at < now - overlay.SlideMs * 1.5 ) {
				break;
			}
			if( key != 0 && MoveDirection( key ) is { } direction && CanWalkTo( x, y, direction.X, direction.Y ) ) {
				return direction;
			}
		}
		return null;
	}

	/// <summary>先読みで歩いているオーバーレイがマス (x, y) に着いたとき呼ばれる。移動キーを押していて、行き先が壁でなければ、その方向。</summary>
	// 着いたときにどのキーも押していなければ、先に進めない (キーを離したのが反映遅れの時間より前でも、ゲームは止まることがある)。
	// ゲームが次の 1 歩を歩き出していれば、座標欄の画素の変化を見てから歩き出した時刻に合わせて進める (進みすぎて戻るより目立たない)。
	private (int X, int Y)? ContinueDirection( int x, int y ) =>
		config.MoveKeyPrediction && ( config.MoveKeyContinue || overlay.RepredictPending ) && reader.IsGameActive() && ActiveMoveKey != 0
			&& MoveDirection( ContinueKey() ) is { } direction
			&& CanWalkTo( x, y, direction.X, direction.Y )
			? direction
			: null;

	/// <summary>
	/// マスに着いたとき、ゲームが次の 1 歩に使うキー。入力の反映遅れの分だけ前に効いていたキー。
	/// そのときどのキーも効いていなかった (いったん全部離してから押し直した) なら、今効いているキー
	/// (押し直したキーは、歩き始めと同じくすぐ効く)。
	/// </summary>
	private int ContinueKey() =>
		MoveKeysHeldAt( NextStepStartsAt() - config.MoveInputLagMs ) is var lagged and not 0 ? lagged : ActiveMoveKey;

	/// <summary>
	/// ゲームが次の 1 歩を歩き出す (歩き出した) 時刻 (eventClock の時刻)。座標欄が最後に変わった時刻 (今の 1 歩を歩き出した時刻) + 1 歩の時間。
	/// オーバーレイが着く時刻はゲームより少し早かったり遅かったりするので、ゲームの時刻で見る。分からなければ今。
	/// </summary>
	private double NextStepStartsAt() {
		var now = eventClock.Elapsed.TotalMilliseconds;
		var changed = reader.LastCoordinateChangeTimestamp;
		if( changed == 0 ) {
			return now;
		}
		var startedAt = now - System.Diagnostics.Stopwatch.GetElapsedTime( changed ).TotalMilliseconds;
		var step = StepMs;
		if( now - startedAt > step * 1.5 ) {
			return now; // しばらく歩いていない
		}
		// 歩き出したばかり (1 歩の半分より前) なら、その 1 歩がゲームの次の 1 歩 (オーバーレイが遅れて着いた)。
		return now - startedAt < step / 2 ? startedAt : startedAt + step;
	}

	/// <summary>1 歩の時間 (ミリ秒)。移動速度の等級 (手動入力ならその時間) で決まる。</summary>
	private int StepMs => config.StepMs;

	/// <summary>
	/// 移動速度を自動で判定するとき、判定した等級が変わっていたら設定に覚えて (次に起動したときも、判定できるまでそれを使う)、スライドの時間に反映する。
	/// </summary>
	private void UpdateMoveSpeed() {
		if( !config.MoveSpeedAuto || reader.WalkSpeed.MeasuredGrade is not { } grade || grade == config.MoveSpeedGrade ) {
			return;
		}
		AddEventLog( $"移動速度: {WalkSpeed.Grade( config.MoveSpeedGrade ).Name} ({WalkSpeed.Grade( config.MoveSpeedGrade ).StepMs} ms) → {WalkSpeed.Grade( grade ).Name} ({WalkSpeed.Grade( grade ).StepMs} ms)　実測 {reader.WalkSpeed.MeasuredStepMs:0} ms" );
		config.MoveSpeedGrade = grade;
		overlay.SlideMs = config.OverlaySlide ? StepMs : 0;
		overlay.JumpDelayMs = config.OverlaySlide ? 0 : config.EffectiveOverlayJumpDelayMs;
		Save();
		settingsForm?.RefreshValues();
	}

	/// <summary>座標欄の見た目が、1 歩の途中で取っておいたときから変わったか (次の 1 歩を歩き始めたか)。</summary>
	private bool CoordinateChanged( string purpose, double? lateMs ) {
		// 確かめる予定の時刻からの遅れ (タイマーの Tick を待つ分)。予定がない Tick ごとの確認は出さない。
		var timing = lateMs is { } late ? $"予定より +{late:0} ms　" : "";
		if( !config.MovePixelCheck ) {
			AddEventLog( $"座標画素 [{purpose}]: 確認オフ (変化あり扱い)" );
			return true;
		}
		var baseline = stepBaseline;
		if( baseline is null ) {
			AddEventLog( $"座標画素 [{purpose}]: 比較元なし (変化なし扱い)" );
			return false;
		}
		var started = eventClock.Elapsed.TotalMilliseconds;
		if( CaptureCoordinatePixels() is not { } current ) {
			AddEventLog( $"座標画素 [{purpose}]: 取得失敗 (変化なし扱い)" );
			return false;
		}
		var captureMs = eventClock.Elapsed.TotalMilliseconds - started;
		if( current.Length != baseline.Length ) {
			AddEventLog( $"座標画素 [{purpose}]: サイズ不一致 {baseline.Length} → {current.Length} バイト (変化あり扱い)　取得 {captureMs:0.0} ms" );
			return true;
		}
		var diff = 0;
		for( var i = 0; i < current.Length; i++ ) {
			if( current[i] != baseline[i] ) {
				diff++;
			}
		}
		AddEventLog( $"座標画素 [{purpose}]: 変化{( diff > 0 ? "あり" : "なし" )}　{timing}差分 {diff} / {current.Length} バイト　比較元から {started - stepBaselineAt:0} ms　取得 {captureMs:0.0} ms ({pixelSource})" );
		return diff > 0;
	}

	/// <summary>座標欄の今の見た目を、次に歩き続けたかを比べる元として取っておく。</summary>
	private void CaptureStepBaseline( string purpose ) {
		if( !config.MovePixelCheck ) {
			stepBaseline = null;
			return;
		}
		stepBaseline = CaptureCoordinatePixels();
		stepBaselineAt = eventClock.Elapsed.TotalMilliseconds;
		stepBaselineText = lastLiveReading?.RawCoordinates;
		AddEventLog( stepBaseline is null ? $"座標画素 [{purpose}]: 比較元取得失敗" : $"座標画素 [{purpose}]: 比較元取得 {stepBaseline.Length} バイト ({pixelSource})",
			purpose switch { "歩き始め" or "途中キー入力" => 2, "停止直前キー継続" => 1, _ => 0 } );
	}

	/// <summary>
	/// 座標欄の画素のうち、比較元から変わったのが X と Y のどちらか ('x' / 'y')。分からなければ null。
	/// 座標欄は "X:Y" なので、コロンより左が変わっていれば X、右だけなら Y。コロンの位置は、比較元の画像の文字の並び
	/// (文字のない列で区切ったかたまり) と、そのときの座標の文字を対応させて求める。
	/// </summary>
	private char? ChangedCoordinateAxis() {
		if( stepBaseline is not { } baseline || stepBaselineText is not { } text || text.IndexOf( ':' ) is var colon && colon < 0
			|| CoordinateScreenRect() is not { } rect || rect.Width <= 0 || baseline.Length % ( rect.Width * 3 ) != 0
			|| CaptureCoordinatePixels() is not { } current || current.Length != baseline.Length ) {
			return null;
		}
		var width = rect.Width;
		var height = baseline.Length / ( width * 3 );
		bool Ink( byte[] pixels, int x ) {
			for( var y = 0; y < height; y++ ) {
				var i = ( y * width + x ) * 3;
				if( Math.Max( pixels[i], Math.Max( pixels[i + 1], pixels[i + 2] ) ) > 150 ) {
					return true;
				}
			}
			return false;
		}
		var runs = new List<int>(); // 文字のかたまりの始まりの列 (画像の端に接するかたまりは枠線なので数えない)
		for( int x = 0, previous = 0; x < width; x++ ) {
			var ink = Ink( baseline, x ) ? 1 : 0;
			if( ink == 1 && previous == 0 ) {
				runs.Add( x );
			}
			previous = ink;
		}
		if( runs.Count > 0 && runs[0] == 0 ) {
			runs.RemoveAt( 0 );
		}
		if( runs.Count > 0 && Ink( baseline, width - 1 ) ) {
			runs.RemoveAt( runs.Count - 1 );
		}
		if( runs.Count != text.Length ) {
			AddEventLog( $"座標画素の変わり方: 不明 (文字のかたまり {runs.Count} 個、文字 \"{text}\" {text.Length} 文字)", 2 );
			return null; // 文字が離れて写っていない (くっついている) など
		}
		var colonStart = runs[colon];
		for( var x = 0; x < width; x++ ) {
			for( var y = 0; y < height; y++ ) {
				var i = ( y * width + x ) * 3;
				if( Math.Abs( current[i] - baseline[i] ) > 40 || Math.Abs( current[i + 1] - baseline[i + 1] ) > 40 || Math.Abs( current[i + 2] - baseline[i + 2] ) > 40 ) {
					return x < colonStart ? 'x' : 'y';
				}
			}
		}
		return null;
	}

	/// <summary>
	/// マス (x, y) から predicted の方向へ歩き出したと見ていたが、座標欄の画素の変わり方 (X か Y か) が合わないとき、
	/// 実際に歩いた方向 (最近効いていたキーのうち、その軸の方向で歩けるもの、新しい順)。合っている・分からなければ null。
	/// </summary>
	private (int X, int Y)? ResolveStepDirection( int x, int y, (int X, int Y) predicted ) {
		if( DirectionFromImage( x, y ) is { } seen ) {
			if( seen == predicted ) {
				return null;
			}
			AddEventLog( $"座標画素の見た目: ({x + seen.X}, {y + seen.Y}) と一致　方向 ({seen.X}, {seen.Y}) と判定 (予測 ({predicted.X}, {predicted.Y}))", 1 );
			return seen;
		}
		if( ChangedCoordinateAxis() is not { } axis ) {
			return null;
		}
		bool OnAxis( (int X, int Y) d ) => axis == 'x' ? d.X != 0 && d.Y == 0 : d.Y != 0 && d.X == 0;
		if( OnAxis( predicted ) ) {
			return null;
		}
		var now = eventClock.Elapsed.TotalMilliseconds;
		foreach( var (at, key) in Enumerable.Reverse( moveKeyHistory ) ) {
			if( key != 0 && MoveDirection( key ) is { } direction && OnAxis( direction ) && CanWalkTo( x, y, direction.X, direction.Y ) ) {
				AddEventLog( $"座標画素の変わり方: {( axis == 'x' ? "X" : "Y" )} が変化　方向 ({direction.X}, {direction.Y}) と判定 (予測 ({predicted.X}, {predicted.Y}))", 1 );
				return direction;
			}
			if( at < now - 600 ) {
				break;
			}
		}
		return null;
	}

	// 座標の文字 (例: "13:76") ごとの、座標欄の見た目。歩き出した方向を、今の見た目と隣のマスの見た目を比べて見分けるのに使う。
	private readonly Dictionary<string, byte[]> coordinateImages = [];

	/// <summary>読み取った座標の文字と、そのとき撮った座標欄の画像を覚える。</summary>
	private void RememberCoordinateImage( Reading reading ) {
		if( reading.CoordinatesStale || reading.RawCoordinates is not { } text || reader.LatestCoordinatePixels is not { } pixels ) {
			return;
		}
		if( coordinateImages.Count > 2000 ) {
			coordinateImages.Clear();
		}
		coordinateImages[text] = pixels.Pixels;
	}

	/// <summary>
	/// 今の座標欄の見た目が、マス (x, y) の隣のマスのどれかの見た目と同じなら、その方向。覚えていない・どれとも違えば null。
	/// X か Y かだけでなく、同じ軸の向き (左右・上下) も見分けられる。
	/// </summary>
	private (int X, int Y)? DirectionFromImage( int x, int y ) {
		if( stepBaselineText is not { } text || text.IndexOf( ':' ) is var colon && colon < 0 || CaptureCoordinatePixels() is not { } current ) {
			return null;
		}
		(int X, int Y)? found = null;
		foreach( var d in new (int X, int Y)[] { (1, 0), (-1, 0), (0, 1), (0, -1) } ) {
			if( coordinateImages.TryGetValue( $"{x + d.X}:{y + d.Y}", out var image ) && image.Length == current.Length && SameImage( image, current ) ) {
				if( found is not null ) {
					return null;
				}
				found = d;
			}
		}
		return found;
	}

	private static bool SameImage( byte[] a, byte[] b ) {
		var differ = 0;
		for( var i = 0; i < a.Length; i++ ) {
			if( Math.Abs( a[i] - b[i] ) > 40 && ++differ > 20 ) {
				return false;
			}
		}
		return true;
	}

	/// <summary>1 マス歩いたと読み取れたら、そのとき押していたキーの組み合わせと歩いた方向を覚える。</summary>
	// 最後に 1 マス歩いた方向 (キャラクターの向き)。スキルで進む方向に使う。
	private (int X, int Y)? facing;
	/// <summary>スキルのキーを押した: 向いている方向へ、決まったマス数を先に動かす。</summary>
	private void MoveKeys_SkillPressed() {
		AddEventLog( $"キー押下: {config.MoveSkillKey} (スキル)" );
		if( facing is { } direction && config.MoveKeyPrediction && config.MoveSkillPrediction && running && !ImageMode && reader.IsGameActive() ) {
			stepBaseline = null;
			overlay.PredictSkill( direction, AppConfig.MoveSkillTiles, config.MoveSkillTileMs );
		}
	}

	private void LearnMoveKeyDirection( GameCoordinate? previous, GameCoordinate current ) {
		if( previous is not { } from || current == from || Math.Abs( current.X - from.X ) > 1 || Math.Abs( current.Y - from.Y ) > 1 ) {
			return;
		}
		if( Math.Abs( current.X - from.X ) + Math.Abs( current.Y - from.Y ) == 1 ) {
			facing = (current.X - from.X, current.Y - from.Y);
		}
		// ゲームが歩き出した時刻 (座標欄が変わった時刻) の、入力の遅れの分だけ前から歩き出すまで、ずっと同じ組み合わせを押していたときだけ覚える
		// (切り替えた直後は、ゲームはまだ前のキーの方向へ歩くので、間違って覚える)。
		var changed = reader.LastCoordinateChangeTimestamp;
		if( changed == 0 ) {
			return;
		}
		var startedAt = eventClock.Elapsed.TotalMilliseconds - System.Diagnostics.Stopwatch.GetElapsedTime( changed ).TotalMilliseconds;
		// 右クリックで歩いたときは覚えない (方向はカーソルの位置で決まっていて、キーの方向ではない)。
		if( MoveKeysHeldThrough( startedAt - config.MoveInputLagMs - 40, startedAt ) is not { } held || held == 0 || held >= FirstMouseKey ) {
			return;
		}
		var name = Native.MoveKeyWatcher.Name( held );
		int[] direction = [current.X - from.X, current.Y - from.Y];
		if( !config.MoveKeyDirections.TryGetValue( name, out var known ) || !known.SequenceEqual( direction ) ) {
			config.MoveKeyDirections[name] = direction;
			Save();
		}
	}

	/// <summary>座標欄の画面上の矩形。</summary>
	private Rectangle? CoordinateScreenRect() =>
		reader.FindClientRect() is { } client && reader.RegionInClient( RegionKind.Coordinates, client.Size, reader.LastTransform ) is { } region
			? region with { X = client.X + region.X, Y = client.Y + region.Y }
			: null;

	void ISettingsHost.SaveConfig() {
		overlay.SlideMs = config.OverlaySlide ? StepMs : 0;
		overlay.JumpDelayMs = config.OverlaySlide ? 0 : config.EffectiveOverlayJumpDelayMs;
		if( overlay.SlideMs <= 0 ) {
			overlay.CancelSlide();
		}
		overlay.KeyDelayMs = config.MoveKeyDelayMs;
		overlay.StartCheckMs = config.MoveStartCheckMs;
		overlay.SnapTiles = config.OverlaySnapTiles;
		trayIcon.Visible = config.StayInTray;
		UpdateMoveKeyWatcher();
		Save();
	}

	Task ISettingsHost.CheckUpdatesAsync() => CheckUpdatesAsync( quiet: false );

	(string App, string Data) ISettingsHost.Versions => (Updater.AppVersion, updater.DataVersion);

	private async Task SelectRegionAsync( RegionKind kind ) {
		if( selecting ) {
			return;
		}
		selecting = true;
		// 設定画面やオーバーレイがゲーム画面に重なっているとキャプチャに写り込むので、調整中は隠す
		// (オーバーレイは次の読み取りで表示し直す)。
		var settings = settingsForm is { Visible: true } visible ? visible : null;
		settings?.Hide();
		HideOverlay();
		try {
			await Task.Delay( 150 ); // 隠した後の画面が描き直されるのを待つ
			if( ImageMode ) {
				await SelectImageRegionAsync( kind );
				return;
			}
			(Rectangle ClientRect, BgrImage Frame, UiTransform Transform)? capture = null;
			await WithOcrAsync( () => capture = reader.CaptureClient() ); // 読み取り中のフレームと競合させない
			if( capture is not { } value ) {
				SetStatus( "ゲームウィンドウが見つかりません", ErrorColor );
				return;
			}
			var size = new Size( value.Frame.Width, value.Frame.Height );
			var title = RegionTitle( kind );
			var transform = value.Transform;
			using var frame = value.Frame.ToBitmap();
			using var selector = new RegionSelectorForm( frame, value.ClientRect, title, RegionAccent( kind ), reader.RegionInClient( kind, size, transform ),
				AppConfig.DefaultRegion( kind ).ClipTo( size.Width, size.Height, transform ) );
			selector.ShowDialog( this );
			if( selector.SelectedRegion is { } region ) {
				reader.StoreRegion( kind, region, transform );
				Save();
				SetStatus( $"{title}の領域を保存しました", OkColor );
			}
		} catch( Exception exception ) {
			SetStatus( $"キャプチャに失敗しました: {exception.Message}", ErrorColor );
		} finally {
			selecting = false;
			if( settings is { IsDisposed: false } ) {
				settings.Show();
				settings.Activate();
			}
		}
	}

	private static string RegionTitle( RegionKind kind ) => kind == RegionKind.Name ? "マップ名" : "座標";

	private static Color RegionAccent( RegionKind kind ) =>
		kind == RegionKind.Name ? Color.FromArgb( 79, 195, 247 ) : Color.FromArgb( 255, 183, 77 );

	// ---- 画像モード (デバッグ用) ------------------------------------------------------

	private async void OpenImage() {
		using var dialog = new OpenFileDialog {
			Title = "読み取る画像を選択 (ゲームのクライアント領域のスクリーンショット)",
			Filter = "画像 (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|すべてのファイル (*.*)|*.*",
		};
		if( dialog.ShowDialog( this ) == DialogResult.OK ) {
			await LoadImageAsync( dialog.FileName );
		}
	}

	private async Task LoadImageAsync( string path ) {
		try {
			// Alt+PrintScreen で撮った画像はタイトルバーと枠線を含むので、ゲームのクライアント領域だけにする。
			var image = BgrImage.Load( path );
			var client = WindowFrame.DetectClientArea( image );
			imageFrameRemoved = client.Size != new Size( image.Width, image.Height );
			imageFrame = imageFrameRemoved ? image.Crop( client ) : image;
			// 画像を撮ったモニターの解像度は分からないので、UI の倍率は画像から欄を探して測る (開いたときに 1 回だけ)。
			imageTransform = config.UiScale is { } fixedScale ? new UiTransform( fixedScale ) : UiLocator.Locate( imageFrame ) ?? UiTransform.Identity;
		} catch( Exception exception ) when( exception is ArgumentException or IOException or OutOfMemoryException ) {
			// GDI+ は未対応・破損した画像に ArgumentException / OutOfMemoryException を投げる。
			SetStatus( $"画像を開けません: {exception.Message}", ErrorColor );
			return;
		}
		imagePath = path;
		// 初回はライブ用の領域を引き継ぐ。以降は画像モードで設定し直した領域を使い回す。
		imageConfig ??= config.Clone();
		liveButton.Visible = true;
		toggleButton.Enabled = false;
		HideOverlay(); // 画像モード中は表示しない
		settingsForm?.RefreshValues();
		await ReadImageAsync();
	}

	private Task ReadImageIfActiveAsync() => ImageMode ? ReadImageAsync() : Task.CompletedTask;

	private async Task ReadImageAsync() {
		if( imageFrame is not { } frame || imageConfig is not { } imageSettings ) {
			return;
		}
		var label = $"画像モード: {Path.GetFileName( imagePath )} ({frame.Width}x{frame.Height}{( imageFrameRemoved ? ", ウィンドウ枠を除去" : "" )})";
		if( !ready ) {
			SetStatus( $"{label} — OCR の準備ができていません", ErrorColor );
			return;
		}
		imageSettings.OcrLanguage = config.OcrLanguage;
		Reading? reading = null;
		try {
			await WithOcrAsync( () => {
				// ライブ用の reader の状態 (確定済みのマップ名など) を汚さないよう、使い捨ての reader で読む。
				using var imageReader = new MapReader( imageSettings, reader.Ocr, ownsOcr: false ) { ManualMapName = reader.ManualMapName };
				reading = imageReader.ReadFrame( frame, force: true, imageTransform );
				for( var i = 1; i < imageSettings.NameConfirmHits && reading.Ok; i++ ) {
					reading = imageReader.ReadFrame( frame, force: true, imageTransform ); // 確定に必要な回数だけ読む
				}
			} );
		} catch( Exception exception ) {
			SetStatus( $"{label} — エラー: {exception.Message}", ErrorColor );
			return;
		}
		if( !ImageMode || reading is null ) {
			return; // 読み取り中にライブへ戻った
		}
		ShowReading( reading );
		if( !reading.Ok ) {
			SetStatus( $"{label} — {reading.Message}", ErrorColor );
		} else if( reading.MapName is null ) {
			SetStatus( $"{label} — マップ名を確定できませんでした (OCR 結果を確認してください)", WarnColor );
		} else {
			SetStatus( label, OkColor );
		}
	}

	private async Task SelectImageRegionAsync( RegionKind kind ) {
		if( imageFrame is not { } frame || imageConfig is null ) {
			return;
		}
		var size = new Size( frame.Width, frame.Height );
		var transform = imageTransform;
		using( var bitmap = frame.ToBitmap() )
		using( var selector = new RegionSelectorForm(
			bitmap, FitToScreen( size ), $"[画像] {RegionTitle( kind )}", RegionAccent( kind ), imageConfig.GetRegion( kind )?.ClipTo( size.Width, size.Height, transform ),
			AppConfig.DefaultRegion( kind ).ClipTo( size.Width, size.Height, transform ) ) ) {
			selector.ShowDialog( this );
			if( selector.SelectedRegion is not { } region ) {
				return;
			}
			imageConfig.SetRegion( kind, region, transform );
		}
		await ReadImageAsync();
	}

	/// <summary>画像を、このウィンドウがある画面の作業領域に収まるよう縮小して中央に置いた矩形。</summary>
	private Rectangle FitToScreen( Size size ) {
		var area = Screen.FromControl( this ).WorkingArea;
		var scale = Math.Min( 1.0, Math.Min( area.Width * 0.9 / size.Width, area.Height * 0.9 / size.Height ) );
		var width = (int)Math.Round( size.Width * scale );
		var height = (int)Math.Round( size.Height * scale );
		return new Rectangle( area.Left + ( area.Width - width ) / 2, area.Top + ( area.Height - height ) / 2, width, height );
	}

	private void ExitImageMode() {
		imageFrame = null;
		imagePath = null;
		liveButton.Visible = false;
		toggleButton.Enabled = true;
		nameValue.Text = reader.ManualMapName ?? reader.Tracker.MapName ?? "—";
		coordinateValue.Text = reader.Tracker.Coordinates?.ToString() ?? "—";
		SetStatus( running ? "ライブ読み取りに戻りました" : "一時停止中", MutedColor );
		settingsForm?.RefreshValues();
	}

	protected override void OnFormClosing( FormClosingEventArgs e ) {
		if( config.StayInTray && !exiting && e.CloseReason == CloseReason.UserClosing ) {
			// 常駐中は閉じずにタスクトレイに隠す (読み取りとオーバーレイは続ける)。終了はタスクトレイのメニューから。
			e.Cancel = true;
			HideToTray();
			return;
		}
		closing = true;
		timer.Stop();
		if( foregroundHook != 0 ) {
			GameWindow.UnhookWinEvent( foregroundHook );
			foregroundHook = 0;
		}
		if( WindowState == FormWindowState.Normal ) {
			config.WindowX = Location.X;
			config.WindowY = Location.Y;
		}
		RememberSettingsWindow();
		RememberInfoWindow();
		Save();
		base.OnFormClosing( e );
	}

	/// <summary>メイン画面 (と開いている設定画面・情報画面) を隠して、タスクトレイのアイコンだけにする。</summary>
	private void HideToTray() {
		if( WindowState == FormWindowState.Normal ) {
			config.WindowX = Location.X;
			config.WindowY = Location.Y;
		}
		settingsHiddenToTray = settingsForm is { IsDisposed: false, Visible: true };
		infoHiddenToTray = infoForm is { IsDisposed: false, Visible: true };
		customHiddenToTray = customForm is { IsDisposed: false, Visible: true };
		if( customHiddenToTray ) {
			customForm!.Hide();
		}
		if( settingsHiddenToTray ) {
			RememberSettingsWindow();
			settingsForm!.Hide();
		}
		if( infoHiddenToTray ) {
			RememberInfoWindow();
			infoForm!.Hide();
		}
		Save();
		Hide();
	}

	/// <summary>タスクトレイに隠したメイン画面 (と設定画面・情報画面) を表示に戻す。</summary>
	private void ShowFromTray() {
		if( !Visible ) {
			Show();
		}
		if( WindowState == FormWindowState.Minimized ) {
			WindowState = FormWindowState.Normal;
		}
		if( settingsHiddenToTray && settingsForm is { IsDisposed: false } settings ) {
			settings.Show( this );
		}
		if( infoHiddenToTray && infoForm is { IsDisposed: false } info ) {
			info.Show( this );
		}
		if( customHiddenToTray && customForm is { IsDisposed: false } custom ) {
			custom.Show( this );
		}
		settingsHiddenToTray = infoHiddenToTray = customHiddenToTray = false;
		Activate();
	}

	protected override void OnFormClosed( FormClosedEventArgs e ) {
		base.OnFormClosed( e );
		trayIcon.Visible = false; // 終了後にアイコンが残らないように
		updateStatusTip.Dispose();
		latencyProbe.Dispose();
		moveKeys.Dispose();
		moveMouse.Dispose();
		// 実行中の OCR が終わるのを少し待ってからエンジンを解放する。
		if( ocrGate.Wait( TimeSpan.FromSeconds( 3 ) ) ) {
			reader.Dispose();
		}
		// 先に外してから解放する (閉じる途中で描き直されると、解放済みの画像を描こうとして例外になる)。
		var image = preview.Image;
		preview.Image = null;
		image?.Dispose();
		overlay.Dispose();
	}
}
