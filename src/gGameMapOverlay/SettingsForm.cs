using gGameMapOverlay.Overlay;

namespace gGameMapOverlay;

/// <summary>設定画面から MainForm の操作 (領域の調整や OCR の切り替え) を呼ぶための窓口。</summary>
internal interface ISettingsHost
{
    AppConfig Config { get; }

    /// <summary>画像モード中なら、領域は画像モード専用の設定を表示・変更する。</summary>
    bool ImageMode { get; }

    ClientRegion? RegionFor(RegionKind kind);

    Task SelectRegionAsync(RegionKind kind);

    Task ChangeLanguageAsync(string language);

    Task ChangeBackendAsync(string backend);

    /// <summary>PaddleOCR の推論スレッド数を変える。null なら自動。</summary>
    Task ChangeOcrThreadsAsync(int? threads);

    /// <summary>読み取り間隔 (ミリ秒) を変える。</summary>
    void ChangeInterval(int intervalMs);

    /// <summary>Config のマップ名の読み取りの項目を書き換えた後に呼ぶ (保存して反映する)。</summary>
    void NameSettingsChanged();

    /// <summary>オーバーレイのデータの問題 (読み込めない・ファイルがない)。問題がなければ空。</summary>
    string OverlayDataSummary { get; }

    /// <summary>Config のオーバーレイの色・不透明度を書き換えた後に呼ぶ (保存して表示を更新する)。</summary>
    void OverlaySettingsChanged();

    /// <summary>Config の画面の表示に関する項目を書き換えた後に呼ぶ (保存する)。</summary>
    void SaveConfig();

    /// <summary>本体とマップ情報の更新を確認し、あれば確認してから更新する。</summary>
    Task CheckUpdatesAsync();

    /// <summary>今の版 (本体, マップ情報)。</summary>
    (string App, string Data) Versions { get; }

    /// <summary>情報画面 (移動の状態・歩く速さ・ログ) を開く。</summary>
    void ShowInfo();

}

/// <summary>
/// 読み取り領域・言語・OCR エンジン・読み取りの速さ・マップ名の確定・オーバーレイの移動の設定画面。変更はその場で反映・保存する。
/// (オーバーレイの種類ごとの表示・色・不透明度はメイン画面で設定する。)
/// 開いたままゲームを操作できるよう、モードレスで表示する。
/// </summary>
internal sealed partial class SettingsForm : Form {
	// コンボボックスの項目 (デザイナーで設定) と同じ順序。
	private static readonly string[] LanguageKeys = ["ja", "ko", "en"];
	private static readonly string[] BackendKeys = ["paddle", "windows", "none"];

	private readonly ISettingsHost host;
	private bool refreshing;

	public SettingsForm( ISettingsHost host ) {
		this.host = host;
		InitializeComponent();
		// 選べるスレッド数は CPU のコア数で変わるので、項目はここで作る (先頭は自動)。
		threadsBox.Items.Add( $"自動 ({AppConfig.AutoOcrThreads})" );
		for( var threads = 1; threads <= AppConfig.MaxOcrThreads; threads++ ) {
			threadsBox.Items.Add( threads.ToString() );
		}
		// 移動速度: 先頭は自動 (表示は RefreshValues で今の等級に合わせる)、続けて等級 0〜4。
		moveSpeedBox.Items.Add( "自動" );
		moveSpeedBox.Items.AddRange( WalkSpeed.Grades.Select( g => g.Name ).ToArray() );
		RefreshValues();
	}

	private void ShowChunksCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.OverlayShowChunks = showChunksCheck.Checked;
			host.OverlaySettingsChanged();
		}
	}

	private async void NameRegionButton_Click( object? sender, EventArgs e ) => await RunAsync( () => host.SelectRegionAsync( RegionKind.Name ) );

	private async void CoordinateRegionButton_Click( object? sender, EventArgs e ) => await RunAsync( () => host.SelectRegionAsync( RegionKind.Coordinates ) );

	private async void LanguageBox_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( !refreshing && languageBox.SelectedIndex >= 0 ) {
			await RunAsync( () => host.ChangeLanguageAsync( LanguageKeys[languageBox.SelectedIndex] ) );
		}
	}

	private async void BackendBox_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( !refreshing && backendBox.SelectedIndex >= 0 ) {
			await RunAsync( () => host.ChangeBackendAsync( BackendKeys[backendBox.SelectedIndex] ) );
		}
	}

	private void IntervalBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.ChangeInterval( (int)intervalBox.Value );
		}
	}

	private void MoveSpeedBox_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( !refreshing && moveSpeedBox.SelectedIndex >= 0 ) {
			// 自動にしたときは、判定できるまで今の等級を使う。
			host.Config.MoveSpeedAuto = moveSpeedBox.SelectedIndex == 0;
			if( moveSpeedBox.SelectedIndex > 0 ) {
				host.Config.MoveSpeedGrade = moveSpeedBox.SelectedIndex - 1;
			}
			host.SaveConfig();
			RefreshValues();
		}
	}

	private void OverlaySlideManualCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			// 手動入力にしたときは、前に入れた時間を使う。
			host.Config.OverlaySlideManual = overlaySlideManualCheck.Checked;
			host.SaveConfig();
			RefreshValues();
		}
	}

	private void OverlaySlideBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing && host.Config.OverlaySlideManual ) {
			host.Config.OverlaySlideMs = (int)overlaySlideBox.Value;
			host.SaveConfig();
		}
	}

	private void SnapTilesBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.OverlaySnapTiles = (double)snapTilesBox.Value;
			host.SaveConfig();
		}
	}

	private void KeyPredictionCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveKeyPrediction = keyPredictionCheck.Checked;
			host.SaveConfig();
		}
		UpdateSlideControls();
	}

	private void KeyRepredictCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveKeyRepredict = keyRepredictCheck.Checked;
			host.SaveConfig();
		}
	}

	private void SkillPredictionCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveSkillPrediction = skillPredictionCheck.Checked;
			host.SaveConfig();
		}
		UpdateSlideControls();
	}

	/// <summary>押したキーをスキルのキーにする (外すときは右クリックの「クリア」)。</summary>
	private void SkillKeyBox_KeyDown( object? sender, KeyEventArgs e ) {
		e.SuppressKeyPress = true;
		var key = e.KeyCode;
		if( key is Keys.ShiftKey or Keys.ControlKey or Keys.Menu or Keys.Tab or Keys.Escape ) {
			return;
		}
		SetSkillKey( key.ToString() );
	}

	private void SkillKeyClearItem_Click( object? sender, EventArgs e ) => SetSkillKey( "" );

	private void SetSkillKey( string key ) {
		host.Config.MoveSkillKey = key;
		skillKeyBox.Text = SkillKeyText( key );
		host.SaveConfig();
	}

	private static string SkillKeyText( string key ) => string.IsNullOrEmpty( key ) ? "なし" : key;

	private void SkillTileMsBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveSkillTileMs = (int)skillTileMsBox.Value;
			host.SaveConfig();
		}
	}

	private void JumpDelayBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.OverlayJumpDelayMs = (int)jumpDelayBox.Value;
			host.SaveConfig();
		}
	}

	private void JumpDelayExtraBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.OverlayJumpDelayExtraMs = (int)jumpDelayExtraBox.Value;
			host.SaveConfig();
			UpdateJumpDelayTotal();
		}
	}

	// 2 つのラジオボタンは別々のパネルにあるので、片方を選んだらもう片方を外す。
	private void JumpDelayManualRadio_CheckedChanged( object? sender, EventArgs e ) => SetJumpDelayManual( jumpDelayValueCaption.Checked );

	private void JumpDelayAutoRadio_CheckedChanged( object? sender, EventArgs e ) => SetJumpDelayManual( !jumpDelayExtraCaption.Checked );

	private void SetJumpDelayManual( bool manual ) {
		jumpDelayValueCaption.Checked = manual;
		jumpDelayExtraCaption.Checked = !manual;
		if( !refreshing && host.Config.OverlayJumpDelayManual != manual ) {
			host.Config.OverlayJumpDelayManual = manual;
			host.SaveConfig();
		}
		UpdateSlideControls();
	}

	/// <summary>自動+手動の行に、自動の値と合計を見せる (例: 自動 170 → 190)。</summary>
	private void UpdateJumpDelayTotal() {
		var config = host.Config;
		jumpDelayTotal.Text = $"自動{config.AutoOverlayJumpDelayMs} → {config.AutoOverlayJumpDelayMs + config.OverlayJumpDelayExtraMs}";
	}

	private void SlideCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.OverlaySlide = slideRadio.Checked;
			host.SaveConfig();
		}
		UpdateSlideControls();
	}

	private void KeyContinueCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveKeyContinue = keyContinueCheck.Checked;
			host.SaveConfig();
		}
	}

	private void GlyphCacheCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.CoordinateGlyphCache = glyphCacheCheck.Checked;
			host.SaveConfig();
		}
	}

	private void PixelCheckCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MovePixelCheck = pixelCheckCheck.Checked;
			host.SaveConfig();
		}
	}

	private void InputLagBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveInputLagMs = (int)inputLagBox.Value;
			host.SaveConfig();
		}
	}

	private void StartCheckBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveStartCheckMs = (int)startCheckBox.Value;
			host.SaveConfig();
		}
	}

	private void KeyDelayBox_ValueChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.MoveKeyDelayMs = (int)keyDelayBox.Value;
			host.SaveConfig();
		}
	}

	private async void ThreadsBox_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( !refreshing && threadsBox.SelectedIndex >= 0 ) {
			await RunAsync( () => host.ChangeOcrThreadsAsync( threadsBox.SelectedIndex == 0 ? null : threadsBox.SelectedIndex ) );
		}
	}

	private void NameSettingBox_ValueChanged( object? sender, EventArgs e ) {
		if( refreshing ) {
			return;
		}
		var config = host.Config;
		config.NameRefreshSeconds = (double)nameRefreshBox.Value;
		config.NameConfirmHits = (int)nameConfirmBox.Value;
		config.NameHoldSeconds = (double)nameHoldBox.Value;
		host.NameSettingsChanged();
	}

	private void AdvancedCheck_CheckedChanged( object? sender, EventArgs e ) {
		advancedPanel.Visible = advancedCheck.Checked;
		infoButton.Visible = advancedCheck.Checked; // 情報画面 (移動の状態・ログ) は詳細設定のときだけ
		FitToScreen();
		if( !refreshing ) {
			host.Config.ShowAdvancedSettings = advancedCheck.Checked;
			host.SaveConfig();
		}
	}

	private void StayInTrayCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.StayInTray = stayInTrayCheck.Checked;
			host.SaveConfig();
		}
	}

	private void AutoUpdateCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			host.Config.CheckUpdatesOnStartup = autoUpdateCheck.Checked;
			host.SaveConfig();
		}
	}

	// クリックでもツールチップの説明を表示する (キーボード操作やホバーに気づかない場合のため)。
	private void HelpLink_LinkClicked( object? sender, LinkLabelLinkClickedEventArgs e ) {
		if( sender is LinkLabel link ) {
			toolTip.Show( toolTip.GetToolTip( link ), link, 0, link.Height, 10000 );
		}
	}

	// クリックで出したツールチップは、ホバーが外れたら消す。
	private void HelpLink_MouseLeave( object? sender, EventArgs e ) {
		if( sender is LinkLabel link ) {
			toolTip.Hide( link );
		}
	}

	private void CloseButton_Click( object? sender, EventArgs e ) => Close();

	private async void UpdateButton_Click( object? sender, EventArgs e ) => await RunAsync( host.CheckUpdatesAsync );

	/// <summary>
	/// 反映が終わるまで操作できないようにし、終わったら表示を実際の設定に合わせ直す
	/// (モデルのダウンロードを断ると Windows 標準 OCR に切り替わるなど、選んだ値と変わることがある)。
	/// </summary>
	private async Task RunAsync( Func<Task> action ) {
		SetInputsEnabled( false );
		try {
			await action();
		} finally {
			if( !IsDisposed ) {
				SetInputsEnabled( true );
				RefreshValues();
			}
		}
	}

	/// <summary>
	/// スライドでの追従がオフの間は、スライドと先読みの設定を使えなくする。先読みの細かい設定は、キー入力での先読みもオンのときだけ。
	/// </summary>
	private void UpdateSlideControls() {
		var slide = slideRadio.Checked;
		overlaySlideManualCheck.Enabled = slide;
		overlaySlideBox.Enabled = slide && overlaySlideManualCheck.Checked;
		// オフのときの遅延: 選んだ方式の行だけ入力できる。
		jumpDelayValueCaption.Enabled = !slide;
		jumpDelayExtraCaption.Enabled = !slide;
		jumpDelayBox.Enabled = !slide && jumpDelayValueCaption.Checked;
		jumpDelayExtraBox.Enabled = !slide && jumpDelayExtraCaption.Checked;
		snapTilesBox.Enabled = slide;
		keyPredictionCheck.Enabled = slide;
		var prediction = slide && keyPredictionCheck.Checked;
		foreach( Control control in new Control[] { keyDelayBox, startCheckBox, inputLagBox, keyRepredictCheck, keyContinueCheck, skillPredictionCheck, pixelCheckCheck } ) {
			control.Enabled = prediction;
		}
		skillKeyBox.Enabled = prediction && skillPredictionCheck.Checked;
		skillTileMsBox.Enabled = skillKeyBox.Enabled;
	}

	private void SetInputsEnabled( bool enabled ) {
		foreach( Control control in Controls ) {
			control.Enabled = enabled;
		}
	}

	/// <summary>表示を現在の設定に合わせる。画像モードの切り替え時などに MainForm からも呼ぶ。</summary>
	public void RefreshValues() {
		refreshing = true;
		try {
			var config = host.Config;
			appVersionValue.Text = "：" + host.Versions.App;
			dataVersionValue.Text = "：" + host.Versions.Data;
			languageBox.SelectedIndex = Math.Max( 0, Array.IndexOf( LanguageKeys, config.OcrLanguage ) );
			backendBox.SelectedIndex = Math.Max( 0, Array.IndexOf( BackendKeys, config.OcrBackend ) );
			glyphCacheCheck.Checked = config.CoordinateGlyphCache;
			intervalBox.Value = Math.Clamp( config.IntervalMs, intervalBox.Minimum, intervalBox.Maximum );
			moveSpeedBox.Items[0] = $"自動 (現在: {WalkSpeed.Grade( config.MoveSpeedGrade ).Name} )";
			moveSpeedBox.SelectedIndex = config.MoveSpeedAuto ? 0 : config.MoveSpeedGrade + 1;
			overlaySlideManualCheck.Checked = config.OverlaySlideManual;
			// 手動入力でないときは、今使っている時間 (等級の 1 歩の時間) を見せる。
			overlaySlideBox.Value = Math.Clamp( config.StepMs, overlaySlideBox.Minimum, overlaySlideBox.Maximum );
			slideRadio.Checked = config.OverlaySlide;
			jumpRadio.Checked = !config.OverlaySlide;
			SetJumpDelayManual( config.OverlayJumpDelayManual );
			jumpDelayBox.Value = Math.Clamp( config.OverlayJumpDelayMs, jumpDelayBox.Minimum, jumpDelayBox.Maximum );
			jumpDelayExtraBox.Value = Math.Clamp( config.OverlayJumpDelayExtraMs, jumpDelayExtraBox.Minimum, jumpDelayExtraBox.Maximum );
			UpdateJumpDelayTotal();
			snapTilesBox.Value = Math.Clamp( (decimal)config.OverlaySnapTiles, snapTilesBox.Minimum, snapTilesBox.Maximum );
			keyPredictionCheck.Checked = config.MoveKeyPrediction;
			keyDelayBox.Value = Math.Clamp( config.MoveKeyDelayMs, keyDelayBox.Minimum, keyDelayBox.Maximum );
			startCheckBox.Value = Math.Clamp( config.MoveStartCheckMs, startCheckBox.Minimum, startCheckBox.Maximum );
			inputLagBox.Value = Math.Clamp( config.MoveInputLagMs, inputLagBox.Minimum, inputLagBox.Maximum );
			keyRepredictCheck.Checked = config.MoveKeyRepredict;
			keyContinueCheck.Checked = config.MoveKeyContinue;
			skillPredictionCheck.Checked = config.MoveSkillPrediction;
			skillKeyBox.Text = SkillKeyText( config.MoveSkillKey );
			skillTileMsBox.Value = Math.Clamp( config.MoveSkillTileMs, skillTileMsBox.Minimum, skillTileMsBox.Maximum );
			pixelCheckCheck.Checked = config.MovePixelCheck;
			UpdateSlideControls();
			threadsBox.SelectedIndex = config.OcrThreads is { } threads ? Math.Clamp( threads, 1, threadsBox.Items.Count - 1 ) : 0;
			// スレッド数は PaddleOCR だけの設定。
			threadsBox.Enabled = config.OcrBackend == "paddle";
			nameRefreshBox.Value = Math.Clamp( (decimal)config.NameRefreshSeconds, nameRefreshBox.Minimum, nameRefreshBox.Maximum );
			nameConfirmBox.Value = Math.Clamp( config.NameConfirmHits, nameConfirmBox.Minimum, nameConfirmBox.Maximum );
			nameHoldBox.Value = Math.Clamp( (decimal)config.NameHoldSeconds, nameHoldBox.Minimum, nameHoldBox.Maximum );
			advancedCheck.Checked = config.ShowAdvancedSettings;
			autoUpdateCheck.Checked = config.CheckUpdatesOnStartup;
			stayInTrayCheck.Checked = config.StayInTray;
			advancedPanel.Visible = config.ShowAdvancedSettings;
			infoButton.Visible = config.ShowAdvancedSettings;
			nameRegionValue.Text = DescribeRegion( host.RegionFor( RegionKind.Name ) );
			coordinateRegionValue.Text = DescribeRegion( host.RegionFor( RegionKind.Coordinates ) );
			showChunksCheck.Checked = config.OverlayShowChunks;
		} finally {
			refreshing = false;
		}
	}

	/// <summary>領域の表示用の文字列 (UI の倍率を掛ける前の、等倍のゲーム座標)。</summary>
	internal static string DescribeRegion( ClientRegion? region ) =>
		region is null ? "未設定" : $"位置 ({region.Left}, {region.Top})　サイズ {region.Right - region.Left}×{region.Bottom - region.Top}";

	private void InfoButton_Click( object? sender, EventArgs e ) => host.ShowInfo();

	/// <summary>
	/// 設定画面が画面の高さより大きくなったら、設定の部分 (scrollPanel) を画面に収まる高さに抑えてスクロールできるようにする。
	/// 下のボタン (詳細設定を表示・情報・閉じる) はスクロールの外に置き、いつも見えるようにする。
	/// </summary>
	private void FitToScreen() {
		var area = Screen.FromControl( this ).WorkingArea;
		var content = contentPanel.GetPreferredSize( Size.Empty );
		// 設定の部分以外 (枠・下のボタン・余白) の高さ。
		var others = Height - scrollPanel.Height;
		var available = area.Height - others;
		if( content.Height > available ) {
			scrollPanel.AutoSize = false;
			scrollPanel.AutoScroll = true;
			// 縦のスクロールバーの幅だけ広げて、中身が隠れて横のスクロールバーが出ないようにする。
			scrollPanel.Size = new Size( content.Width + SystemInformation.VerticalScrollBarWidth, Math.Max( 100, available ) );
		} else {
			scrollPanel.AutoScroll = false;
			scrollPanel.AutoSize = true;
		}
		if( Bottom > area.Bottom ) {
			Top = Math.Max( area.Top, area.Bottom - Height );
		}
	}

	protected override void OnResizeEnd( EventArgs e ) {
		base.OnResizeEnd( e );
		FitToScreen(); // 別の画面へ動かしたとき
	}

	protected override void OnLoad( EventArgs e ) {
		base.OnLoad( e );
		FitToScreen();
		if( host.Config.SettingsWindowX is { } savedX && host.Config.SettingsWindowY is { } savedY
			&& Screen.AllScreens.Any( screen => screen.WorkingArea.Contains( savedX + 40, savedY + 20 ) ) ) {
			Location = new Point( savedX, savedY ); // 前に閉じたときの位置 (見える画面の中にあるときだけ)
			FitToScreen();
			return;
		}
		// モードレスでは StartPosition = CenterParent が効かないので、自分で親ウィンドウの中央に置く。
		if( Owner is { } owner ) {
			var area = Screen.FromControl( owner ).WorkingArea;
			var x = owner.Left + ( owner.Width - Width ) / 2;
			var y = owner.Top + ( owner.Height - Height ) / 2;
			Location = new Point( Math.Clamp( x, area.Left, Math.Max( area.Left, area.Right - Width ) ), Math.Clamp( y, area.Top, Math.Max( area.Top, area.Bottom - Height ) ) );
		}
	}
}
