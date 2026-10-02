namespace gGameMapOverlay;

partial class SettingsForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

	#region Windows Form Designer generated code

	/// <summary>
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {
		this.components = new System.ComponentModel.Container();
		this.regionGroup = new GroupBox();
		this.regionPanel = new TableLayoutPanel();
		this.nameRegionCaption = new Label();
		this.nameRegionValue = new Label();
		this.nameRegionButton = new Button();
		this.coordinateRegionCaption = new Label();
		this.coordinateRegionValue = new Label();
		this.coordinateRegionButton = new Button();
		this.ocrGroup = new GroupBox();
		this.ocrPanel = new TableLayoutPanel();
		this.languageCaption = new Label();
		this.languageBox = new ComboBox();
		this.backendCaption = new Label();
		this.backendBox = new ComboBox();
		this.glyphCacheCheck = new CheckBox();
		this.overlayGroup = new GroupBox();
		this.overlayPanel = new TableLayoutPanel();
		this.resetColorsButton = new Button();
		this.overlayOptionPanel = new FlowLayoutPanel();
		this.antiAliasCheck = new CheckBox();
		this.customTilesButton = new Button();
		this.layersPanel = new TableLayoutPanel();
		this.overallOpacityLabel = new Label();
		this.overallOpacityBox = new NumericUpDown();
		this.overallOpacityUnit = new Label();
		this.impassableEdgeColorButton = new Button();
		this.impassableEdgeColorLabel = new Label();
		this.impassableEdgeOpacityCheck = new CheckBox();
		this.impassableEdgeOpacityBox = new NumericUpDown();
		this.impassableEdgeOpacityUnit = new Label();
		this.monsterBlockColorButton = new Button();
		this.monsterBlockColorLabel = new Label();
		this.monsterBlockOpacityCheck = new CheckBox();
		this.monsterBlockOpacityBox = new NumericUpDown();
		this.monsterBlockOpacityUnit = new Label();
		this.mapMoveColorButton = new Button();
		this.mapMoveColorLabel = new Label();
		this.mapMoveOpacityCheck = new CheckBox();
		this.mapMoveOpacityBox = new NumericUpDown();
		this.mapMoveOpacityUnit = new Label();
		this.specialColorButton = new Button();
		this.specialColorLabel = new Label();
		this.specialOpacityCheck = new CheckBox();
		this.specialOpacityBox = new NumericUpDown();
		this.specialOpacityUnit = new Label();
		this.gridColorButton = new Button();
		this.gridColorLabel = new Label();
		this.gridOpacityCheck = new CheckBox();
		this.gridOpacityBox = new NumericUpDown();
		this.gridOpacityUnit = new Label();
		this.playerColorButton = new Button();
		this.playerColorLabel = new Label();
		this.playerOpacityCheck = new CheckBox();
		this.playerOpacityBox = new NumericUpDown();
		this.playerOpacityUnit = new Label();
		this.showChunksCheck = new CheckBox();
		this.motionGroup = new GroupBox();
		this.motionPanel = new TableLayoutPanel();
		this.jumpRadio = new RadioButton();
		this.moveSpeedPanel = new FlowLayoutPanel();
		this.moveSpeedCaption = new Label();
		this.moveSpeedBox = new ComboBox();
		this.jumpDelayPanel = new FlowLayoutPanel();
		this.jumpDelayCaption = new Label();
		this.jumpDelayExtraPanel = new FlowLayoutPanel();
		this.jumpDelayExtraCaption = new RadioButton();
		this.jumpDelayExtraBox = new NumericUpDown();
		this.jumpDelayExtraUnit = new Label();
		this.jumpDelayTotal = new Label();
		this.jumpDelayValuePanel = new FlowLayoutPanel();
		this.jumpDelayValueCaption = new RadioButton();
		this.jumpDelayBox = new NumericUpDown();
		this.jumpDelayUnit = new Label();
		this.slideRadio = new RadioButton();
		this.advancedPanel = new TableLayoutPanel();
		this.speedGroup = new GroupBox();
		this.speedPanel = new TableLayoutPanel();
		this.intervalCaption = new Label();
		this.intervalBox = new NumericUpDown();
		this.intervalUnit = new Label();
		this.threadsCaption = new Label();
		this.threadsBox = new ComboBox();
		this.slideGroup = new GroupBox();
		this.slidePanel = new TableLayoutPanel();
		this.overlaySlideCaption = new Label();
		this.overlaySlideBox = new NumericUpDown();
		this.overlaySlideOptionPanel = new FlowLayoutPanel();
		this.overlaySlideUnit = new Label();
		this.overlaySlideManualCheck = new CheckBox();
		this.snapTilesCaption = new Label();
		this.snapTilesBox = new NumericUpDown();
		this.snapTilesUnit = new Label();
		this.keyPredictionCheck = new CheckBox();
		this.keyDelayCaption = new Label();
		this.keyDelayBox = new NumericUpDown();
		this.keyDelayUnit = new Label();
		this.keyContinueCheck = new CheckBox();
		this.inputLagCaption = new Label();
		this.inputLagBox = new NumericUpDown();
		this.inputLagUnit = new Label();
		this.keyRepredictCheck = new CheckBox();
		this.pixelCheckCheck = new CheckBox();
		this.startCheckCaption = new Label();
		this.startCheckBox = new NumericUpDown();
		this.startCheckUnit = new Label();
		this.skillPredictionCheck = new CheckBox();
		this.skillKeyCaption = new Label();
		this.skillKeyBox = new TextBox();
		this.skillKeyMenu = new ContextMenuStrip( this.components );
		this.skillKeyClearItem = new ToolStripMenuItem();
		this.skillTileMsCaption = new Label();
		this.skillTileMsBox = new NumericUpDown();
		this.skillTileMsUnit = new Label();
		this.nameGroup = new GroupBox();
		this.namePanel = new TableLayoutPanel();
		this.nameRefreshCaption = new Label();
		this.nameRefreshBox = new NumericUpDown();
		this.nameRefreshUnit = new Label();
		this.nameConfirmCaption = new Label();
		this.nameConfirmBox = new NumericUpDown();
		this.nameConfirmUnit = new Label();
		this.nameHoldCaption = new Label();
		this.nameHoldBox = new NumericUpDown();
		this.nameHoldUnit = new Label();
		this.drawingGroup = new GroupBox();
		this.drawingPanel = new FlowLayoutPanel();
		this.bottomPanel = new TableLayoutPanel();
		this.advancedCheck = new CheckBox();
		this.infoButton = new Button();
		this.closeButton = new Button();
		this.updateButton = new Button();
		this.appVersionCaption = new Label();
		this.appVersionValue = new Label();
		this.dataVersionCaption = new Label();
		this.dataVersionValue = new Label();
		this.autoUpdateCheck = new CheckBox();
		this.updateGroup = new GroupBox();
		this.updatePanel = new TableLayoutPanel();
		this.generalGroup = new GroupBox();
		this.generalPanel = new FlowLayoutPanel();
		this.stayInTrayCheck = new CheckBox();
		this.toolTip = new ToolTip( this.components );
		this.basicPanel = new TableLayoutPanel();
		this.rootPanel = new TableLayoutPanel();
		this.scrollPanel = new Panel();
		this.contentPanel = new TableLayoutPanel();
		this.regionGroup.SuspendLayout();
		this.regionPanel.SuspendLayout();
		this.ocrGroup.SuspendLayout();
		this.ocrPanel.SuspendLayout();
		this.overlayGroup.SuspendLayout();
		this.overlayPanel.SuspendLayout();
		this.overlayOptionPanel.SuspendLayout();
		this.layersPanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.overallOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.impassableEdgeOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.monsterBlockOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.mapMoveOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.specialOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.gridOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.playerOpacityBox ).BeginInit();
		this.motionGroup.SuspendLayout();
		this.motionPanel.SuspendLayout();
		this.moveSpeedPanel.SuspendLayout();
		this.jumpDelayPanel.SuspendLayout();
		this.jumpDelayExtraPanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.jumpDelayExtraBox ).BeginInit();
		this.jumpDelayValuePanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.jumpDelayBox ).BeginInit();
		this.advancedPanel.SuspendLayout();
		this.speedGroup.SuspendLayout();
		this.speedPanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.intervalBox ).BeginInit();
		this.slideGroup.SuspendLayout();
		this.slidePanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.overlaySlideBox ).BeginInit();
		this.overlaySlideOptionPanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.snapTilesBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.keyDelayBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.inputLagBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.startCheckBox ).BeginInit();
		this.skillKeyMenu.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.skillTileMsBox ).BeginInit();
		this.nameGroup.SuspendLayout();
		this.namePanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.nameRefreshBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.nameConfirmBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.nameHoldBox ).BeginInit();
		this.drawingGroup.SuspendLayout();
		this.drawingPanel.SuspendLayout();
		this.bottomPanel.SuspendLayout();
		this.updateGroup.SuspendLayout();
		this.updatePanel.SuspendLayout();
		this.generalGroup.SuspendLayout();
		this.generalPanel.SuspendLayout();
		this.basicPanel.SuspendLayout();
		this.rootPanel.SuspendLayout();
		this.scrollPanel.SuspendLayout();
		this.contentPanel.SuspendLayout();
		this.SuspendLayout();
		// 
		// regionGroup
		// 
		this.regionGroup.AutoSize = true;
		this.regionGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.regionGroup.Controls.Add( this.regionPanel );
		this.regionGroup.Dock = DockStyle.Fill;
		this.regionGroup.Location = new Point( 3, 3 );
		this.regionGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.regionGroup.Name = "regionGroup";
		this.regionGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.regionGroup.Size = new Size( 323, 80 );
		this.regionGroup.TabIndex = 0;
		this.regionGroup.TabStop = false;
		this.regionGroup.Text = "読み取り領域";
		// 
		// regionPanel
		// 
		this.regionPanel.AutoSize = true;
		this.regionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.regionPanel.ColumnCount = 3;
		this.regionPanel.ColumnStyles.Add( new ColumnStyle() );
		this.regionPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.regionPanel.ColumnStyles.Add( new ColumnStyle() );
		this.regionPanel.Controls.Add( this.nameRegionCaption, 0, 0 );
		this.regionPanel.Controls.Add( this.nameRegionValue, 1, 0 );
		this.regionPanel.Controls.Add( this.nameRegionButton, 2, 0 );
		this.regionPanel.Controls.Add( this.coordinateRegionCaption, 0, 1 );
		this.regionPanel.Controls.Add( this.coordinateRegionValue, 1, 1 );
		this.regionPanel.Controls.Add( this.coordinateRegionButton, 2, 1 );
		this.regionPanel.Dock = DockStyle.Fill;
		this.regionPanel.Location = new Point( 8, 20 );
		this.regionPanel.Name = "regionPanel";
		this.regionPanel.RowCount = 2;
		this.regionPanel.RowStyles.Add( new RowStyle() );
		this.regionPanel.RowStyles.Add( new RowStyle() );
		this.regionPanel.Size = new Size( 307, 54 );
		this.regionPanel.TabIndex = 0;
		// 
		// nameRegionCaption
		// 
		this.nameRegionCaption.Anchor = AnchorStyles.Left;
		this.nameRegionCaption.AutoSize = true;
		this.nameRegionCaption.Location = new Point( 3, 6 );
		this.nameRegionCaption.Name = "nameRegionCaption";
		this.nameRegionCaption.Size = new Size( 45, 15 );
		this.nameRegionCaption.TabIndex = 0;
		this.nameRegionCaption.Text = "マップ名";
		// 
		// nameRegionValue
		// 
		this.nameRegionValue.AutoSize = true;
		this.nameRegionValue.Dock = DockStyle.Fill;
		this.nameRegionValue.Location = new Point( 54, 0 );
		this.nameRegionValue.Name = "nameRegionValue";
		this.nameRegionValue.Size = new Size( 169, 27 );
		this.nameRegionValue.TabIndex = 1;
		this.nameRegionValue.Text = "未設定";
		this.nameRegionValue.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// nameRegionButton
		// 
		this.nameRegionButton.AutoSize = true;
		this.nameRegionButton.Location = new Point( 229, 1 );
		this.nameRegionButton.Margin = new Padding( 3, 1, 3, 1 );
		this.nameRegionButton.Name = "nameRegionButton";
		this.nameRegionButton.Size = new Size( 75, 25 );
		this.nameRegionButton.TabIndex = 2;
		this.nameRegionButton.Text = "調整…";
		this.nameRegionButton.UseVisualStyleBackColor = true;
		this.nameRegionButton.Click +=  this.NameRegionButton_Click ;
		// 
		// coordinateRegionCaption
		// 
		this.coordinateRegionCaption.Anchor = AnchorStyles.Left;
		this.coordinateRegionCaption.AutoSize = true;
		this.coordinateRegionCaption.Location = new Point( 3, 33 );
		this.coordinateRegionCaption.Name = "coordinateRegionCaption";
		this.coordinateRegionCaption.Size = new Size( 31, 15 );
		this.coordinateRegionCaption.TabIndex = 3;
		this.coordinateRegionCaption.Text = "座標";
		// 
		// coordinateRegionValue
		// 
		this.coordinateRegionValue.AutoSize = true;
		this.coordinateRegionValue.Dock = DockStyle.Fill;
		this.coordinateRegionValue.Location = new Point( 54, 27 );
		this.coordinateRegionValue.Name = "coordinateRegionValue";
		this.coordinateRegionValue.Size = new Size( 169, 27 );
		this.coordinateRegionValue.TabIndex = 4;
		this.coordinateRegionValue.Text = "未設定";
		this.coordinateRegionValue.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// coordinateRegionButton
		// 
		this.coordinateRegionButton.AutoSize = true;
		this.coordinateRegionButton.Location = new Point( 229, 28 );
		this.coordinateRegionButton.Margin = new Padding( 3, 1, 3, 1 );
		this.coordinateRegionButton.Name = "coordinateRegionButton";
		this.coordinateRegionButton.Size = new Size( 75, 25 );
		this.coordinateRegionButton.TabIndex = 5;
		this.coordinateRegionButton.Text = "調整…";
		this.coordinateRegionButton.UseVisualStyleBackColor = true;
		this.coordinateRegionButton.Click +=  this.CoordinateRegionButton_Click ;
		// 
		// ocrGroup
		// 
		this.ocrGroup.AutoSize = true;
		this.ocrGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.ocrGroup.Controls.Add( this.ocrPanel );
		this.ocrGroup.Dock = DockStyle.Fill;
		this.ocrGroup.Location = new Point( 3, 3 );
		this.ocrGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.ocrGroup.Name = "ocrGroup";
		this.ocrGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.ocrGroup.Size = new Size( 308, 109 );
		this.ocrGroup.TabIndex = 0;
		this.ocrGroup.TabStop = false;
		this.ocrGroup.Text = "OCR";
		// 
		// ocrPanel
		// 
		this.ocrPanel.AutoSize = true;
		this.ocrPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.ocrPanel.ColumnCount = 2;
		this.ocrPanel.ColumnStyles.Add( new ColumnStyle() );
		this.ocrPanel.ColumnStyles.Add( new ColumnStyle() );
		this.ocrPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Absolute, 20F ) );
		this.ocrPanel.Controls.Add( this.languageCaption, 0, 0 );
		this.ocrPanel.Controls.Add( this.languageBox, 1, 0 );
		this.ocrPanel.Controls.Add( this.backendCaption, 0, 1 );
		this.ocrPanel.Controls.Add( this.backendBox, 1, 1 );
		this.ocrPanel.Controls.Add( this.glyphCacheCheck, 0, 2 );
		this.ocrPanel.Dock = DockStyle.Fill;
		this.ocrPanel.Location = new Point( 8, 20 );
		this.ocrPanel.Name = "ocrPanel";
		this.ocrPanel.RowCount = 3;
		this.ocrPanel.RowStyles.Add( new RowStyle() );
		this.ocrPanel.RowStyles.Add( new RowStyle() );
		this.ocrPanel.RowStyles.Add( new RowStyle() );
		this.ocrPanel.Size = new Size( 292, 83 );
		this.ocrPanel.TabIndex = 0;
		// 
		// languageCaption
		// 
		this.languageCaption.Anchor = AnchorStyles.Left;
		this.languageCaption.AutoSize = true;
		this.languageCaption.Location = new Point( 3, 7 );
		this.languageCaption.Name = "languageCaption";
		this.languageCaption.Size = new Size( 79, 15 );
		this.languageCaption.TabIndex = 0;
		this.languageCaption.Text = "マップ名の言語";
		// 
		// languageBox
		// 
		this.languageBox.DropDownStyle = ComboBoxStyle.DropDownList;
		this.languageBox.Items.AddRange( new object[] { "日本語", "한국어", "English" } );
		this.languageBox.Location = new Point( 88, 3 );
		this.languageBox.Name = "languageBox";
		this.languageBox.Size = new Size( 150, 23 );
		this.languageBox.TabIndex = 1;
		this.languageBox.SelectedIndexChanged +=  this.LanguageBox_SelectedIndexChanged ;
		// 
		// backendCaption
		// 
		this.backendCaption.Anchor = AnchorStyles.Left;
		this.backendCaption.AutoSize = true;
		this.backendCaption.Location = new Point( 3, 36 );
		this.backendCaption.Name = "backendCaption";
		this.backendCaption.Size = new Size( 44, 15 );
		this.backendCaption.TabIndex = 3;
		this.backendCaption.Text = "エンジン";
		this.toolTip.SetToolTip( this.backendCaption, "座標は言語に関係なく英語モデルで読み取ります" );
		// 
		// backendBox
		// 
		this.backendBox.DropDownStyle = ComboBoxStyle.DropDownList;
		this.backendBox.Items.AddRange( new object[] { "PaddleOCR (推奨)", "Windows 標準 OCR", "なし" } );
		this.backendBox.Location = new Point( 88, 32 );
		this.backendBox.Name = "backendBox";
		this.backendBox.Size = new Size( 150, 23 );
		this.toolTip.SetToolTip( this.backendBox, "「なし」にすると OCR を使わず、グリッド・プレイヤーだけを表示します (マスの種類は表示しません)" );
		this.backendBox.TabIndex = 4;
		this.backendBox.SelectedIndexChanged +=  this.BackendBox_SelectedIndexChanged ;
		// 
		// glyphCacheCheck
		// 
		this.glyphCacheCheck.AutoSize = true;
		this.ocrPanel.SetColumnSpan( this.glyphCacheCheck, 2 );
		this.glyphCacheCheck.Location = new Point( 3, 61 );
		this.glyphCacheCheck.Name = "glyphCacheCheck";
		this.glyphCacheCheck.Size = new Size( 160, 19 );
		this.glyphCacheCheck.TabIndex = 5;
		this.glyphCacheCheck.Text = "座標は文字画像を学習する";
		this.toolTip.SetToolTip( this.glyphCacheCheck, "OCR で読めた座標から数字の画像を覚え、次からは画像の一致で読みます。OCR より速く、覚えていない文字があるときは OCR で読みます" );
		this.glyphCacheCheck.UseVisualStyleBackColor = true;
		this.glyphCacheCheck.CheckedChanged +=  this.GlyphCacheCheck_CheckedChanged ;
		// 
		// overlayGroup
		// 
		this.overlayGroup.AutoSize = true;
		this.overlayGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.overlayGroup.Controls.Add( this.overlayPanel );
		this.overlayGroup.Dock = DockStyle.Fill;
		this.overlayGroup.Location = new Point( 3, 123 );
		this.overlayGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.overlayGroup.Name = "overlayGroup";
		this.overlayGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.overlayGroup.Size = new Size( 308, 232 );
		this.overlayGroup.TabIndex = 1;
		this.overlayGroup.TabStop = false;
		this.overlayGroup.Text = "オーバーレイの表示";
		// 
		// overlayPanel
		// 
		this.overlayPanel.AutoSize = true;
		this.overlayPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.overlayPanel.ColumnCount = 2;
		this.overlayPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.overlayPanel.ColumnStyles.Add( new ColumnStyle() );
		this.overlayPanel.Controls.Add( this.resetColorsButton, 1, 0 );
		this.overlayPanel.Controls.Add( this.overlayOptionPanel, 0, 1 );
		this.overlayPanel.Controls.Add( this.layersPanel, 0, 0 );
		this.overlayPanel.Dock = DockStyle.Fill;
		this.overlayPanel.Location = new Point( 8, 20 );
		this.overlayPanel.Name = "overlayPanel";
		this.overlayPanel.RowCount = 2;
		this.overlayPanel.RowStyles.Add( new RowStyle() );
		this.overlayPanel.RowStyles.Add( new RowStyle() );
		this.overlayPanel.Size = new Size( 292, 206 );
		this.overlayPanel.TabIndex = 0;
		this.overlayPanel.Paint +=  this.overlayPanel_Paint ;
		// 
		// resetColorsButton
		// 
		this.resetColorsButton.Anchor =  AnchorStyles.Top  |  AnchorStyles.Right ;
		this.resetColorsButton.AutoSize = true;
		this.resetColorsButton.Location = new Point( 229, 3 );
		this.resetColorsButton.Name = "resetColorsButton";
		this.resetColorsButton.Size = new Size( 60, 25 );
		this.resetColorsButton.TabIndex = 8;
		this.resetColorsButton.Text = "リセット...";
		this.toolTip.SetToolTip( this.resetColorsButton, "色を初期値に戻し、個別の不透明度をやめます" );
		this.resetColorsButton.UseVisualStyleBackColor = true;
		this.resetColorsButton.Click +=  this.ResetColorsButton_Click ;
		// 
		// overlayOptionPanel
		// 
		this.overlayOptionPanel.AutoSize = true;
		this.overlayOptionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.overlayPanel.SetColumnSpan( this.overlayOptionPanel, 2 );
		this.overlayOptionPanel.Controls.Add( this.antiAliasCheck );
		this.overlayOptionPanel.Controls.Add( this.customTilesButton );
		this.overlayOptionPanel.Location = new Point( 0, 181 );
		this.overlayOptionPanel.Margin = new Padding( 0, 6, 0, 0 );
		this.overlayOptionPanel.Name = "overlayOptionPanel";
		this.overlayOptionPanel.Size = new Size( 218, 25 );
		this.overlayOptionPanel.TabIndex = 1;
		this.overlayOptionPanel.WrapContents = false;
		// 
		// antiAliasCheck
		// 
		this.antiAliasCheck.Anchor = AnchorStyles.Left;
		this.antiAliasCheck.AutoSize = true;
		this.antiAliasCheck.Location = new Point( 3, 3 );
		this.antiAliasCheck.Name = "antiAliasCheck";
		this.antiAliasCheck.Size = new Size( 97, 19 );
		this.antiAliasCheck.TabIndex = 0;
		this.antiAliasCheck.Text = "アンチエイリアス";
		this.toolTip.SetToolTip( this.antiAliasCheck, "線の縁をなめらかにします" );
		this.antiAliasCheck.UseVisualStyleBackColor = true;
		this.antiAliasCheck.CheckedChanged +=  this.AntiAliasCheck_CheckedChanged ;
		// 
		// customTilesButton
		// 
		this.customTilesButton.Anchor = AnchorStyles.Left;
		this.customTilesButton.AutoSize = true;
		this.customTilesButton.Location = new Point( 115, 0 );
		this.customTilesButton.Margin = new Padding( 12, 0, 3, 0 );
		this.customTilesButton.Name = "customTilesButton";
		this.customTilesButton.Size = new Size( 100, 25 );
		this.customTilesButton.TabIndex = 1;
		this.customTilesButton.Text = "カスタムのマス...";
		this.toolTip.SetToolTip( this.customTilesButton, "キャラクターの周りに自分でマスを描きます (メイン画面でグループごとに表示を切り替えます)" );
		this.customTilesButton.UseVisualStyleBackColor = true;
		this.customTilesButton.Click +=  this.CustomTilesButton_Click ;
		// 
		// layersPanel
		// 
		this.layersPanel.AutoSize = true;
		this.layersPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.layersPanel.ColumnCount = 5;
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.Controls.Add( this.overallOpacityLabel, 0, 0 );
		this.layersPanel.Controls.Add( this.overallOpacityBox, 3, 0 );
		this.layersPanel.Controls.Add( this.overallOpacityUnit, 4, 0 );
		this.layersPanel.Controls.Add( this.impassableEdgeColorButton, 0, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeColorLabel, 1, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeOpacityCheck, 2, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeOpacityBox, 3, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeOpacityUnit, 4, 1 );
		this.layersPanel.Controls.Add( this.monsterBlockColorButton, 0, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockColorLabel, 1, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockOpacityCheck, 2, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockOpacityBox, 3, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockOpacityUnit, 4, 2 );
		this.layersPanel.Controls.Add( this.mapMoveColorButton, 0, 3 );
		this.layersPanel.Controls.Add( this.mapMoveColorLabel, 1, 3 );
		this.layersPanel.Controls.Add( this.mapMoveOpacityCheck, 2, 3 );
		this.layersPanel.Controls.Add( this.mapMoveOpacityBox, 3, 3 );
		this.layersPanel.Controls.Add( this.mapMoveOpacityUnit, 4, 3 );
		this.layersPanel.Controls.Add( this.specialColorButton, 0, 4 );
		this.layersPanel.Controls.Add( this.specialColorLabel, 1, 4 );
		this.layersPanel.Controls.Add( this.specialOpacityCheck, 2, 4 );
		this.layersPanel.Controls.Add( this.specialOpacityBox, 3, 4 );
		this.layersPanel.Controls.Add( this.specialOpacityUnit, 4, 4 );
		this.layersPanel.Controls.Add( this.gridColorButton, 0, 5 );
		this.layersPanel.Controls.Add( this.gridColorLabel, 1, 5 );
		this.layersPanel.Controls.Add( this.gridOpacityCheck, 2, 5 );
		this.layersPanel.Controls.Add( this.gridOpacityBox, 3, 5 );
		this.layersPanel.Controls.Add( this.gridOpacityUnit, 4, 5 );
		this.layersPanel.Controls.Add( this.playerColorButton, 0, 6 );
		this.layersPanel.Controls.Add( this.playerColorLabel, 1, 6 );
		this.layersPanel.Controls.Add( this.playerOpacityCheck, 2, 6 );
		this.layersPanel.Controls.Add( this.playerOpacityBox, 3, 6 );
		this.layersPanel.Controls.Add( this.playerOpacityUnit, 4, 6 );
		this.layersPanel.Location = new Point( 0, 0 );
		this.layersPanel.Margin = new Padding( 0 );
		this.layersPanel.Name = "layersPanel";
		this.layersPanel.RowCount = 7;
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.Size = new Size( 205, 175 );
		this.layersPanel.TabIndex = 0;
		// 
		// overallOpacityLabel
		// 
		this.overallOpacityLabel.Anchor = AnchorStyles.Left;
		this.overallOpacityLabel.AutoSize = true;
		this.layersPanel.SetColumnSpan( this.overallOpacityLabel, 3 );
		this.overallOpacityLabel.Location = new Point( 3, 5 );
		this.overallOpacityLabel.Name = "overallOpacityLabel";
		this.overallOpacityLabel.Size = new Size( 89, 15 );
		this.overallOpacityLabel.TabIndex = 21;
		this.overallOpacityLabel.Text = "全体の不透明度";
		// 
		// overallOpacityBox
		// 
		this.overallOpacityBox.Anchor = AnchorStyles.Left;
		this.overallOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.overallOpacityBox.Location = new Point( 132, 1 );
		this.overallOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.overallOpacityBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.overallOpacityBox.Name = "overallOpacityBox";
		this.overallOpacityBox.Size = new Size( 50, 23 );
		this.overallOpacityBox.TabIndex = 22;
		this.overallOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.overallOpacityBox, "全体の不透明度。100% で不透明。小さくするほどゲーム画面が透けて見えます\n個別にチェックを入れていない色に使います" );
		this.overallOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.overallOpacityBox.ValueChanged +=  this.OverallOpacityBox_ValueChanged ;
		// 
		// overallOpacityUnit
		// 
		this.overallOpacityUnit.Anchor = AnchorStyles.Left;
		this.overallOpacityUnit.AutoSize = true;
		this.overallOpacityUnit.Location = new Point( 185, 5 );
		this.overallOpacityUnit.Name = "overallOpacityUnit";
		this.overallOpacityUnit.Size = new Size( 17, 15 );
		this.overallOpacityUnit.TabIndex = 23;
		this.overallOpacityUnit.Text = "%";
		// 
		// impassableEdgeColorButton
		// 
		this.impassableEdgeColorButton.Anchor = AnchorStyles.Left;
		this.impassableEdgeColorButton.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.impassableEdgeColorButton.FlatStyle = FlatStyle.Flat;
		this.impassableEdgeColorButton.Location = new Point( 3, 28 );
		this.impassableEdgeColorButton.Margin = new Padding( 3, 3, 0, 3 );
		this.impassableEdgeColorButton.Name = "impassableEdgeColorButton";
		this.impassableEdgeColorButton.Size = new Size( 18, 18 );
		this.impassableEdgeColorButton.TabIndex = 0;
		this.toolTip.SetToolTip( this.impassableEdgeColorButton, "クリックで色を変更します" );
		this.impassableEdgeColorButton.Click +=  this.ColorButton_Click ;
		// 
		// impassableEdgeColorLabel
		// 
		this.impassableEdgeColorLabel.Anchor = AnchorStyles.Left;
		this.impassableEdgeColorLabel.AutoSize = true;
		this.impassableEdgeColorLabel.Location = new Point( 24, 30 );
		this.impassableEdgeColorLabel.Name = "impassableEdgeColorLabel";
		this.impassableEdgeColorLabel.Size = new Size( 19, 15 );
		this.impassableEdgeColorLabel.TabIndex = 1;
		this.impassableEdgeColorLabel.Text = "壁";
		// 
		// impassableEdgeOpacityCheck
		// 
		this.impassableEdgeOpacityCheck.Anchor = AnchorStyles.Left;
		this.impassableEdgeOpacityCheck.AutoSize = true;
		this.impassableEdgeOpacityCheck.Location = new Point( 114, 30 );
		this.impassableEdgeOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.impassableEdgeOpacityCheck.Name = "impassableEdgeOpacityCheck";
		this.impassableEdgeOpacityCheck.Size = new Size( 15, 14 );
		this.impassableEdgeOpacityCheck.TabIndex = 24;
		this.toolTip.SetToolTip( this.impassableEdgeOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.impassableEdgeOpacityCheck.UseVisualStyleBackColor = true;
		this.impassableEdgeOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// impassableEdgeOpacityBox
		// 
		this.impassableEdgeOpacityBox.Anchor = AnchorStyles.Left;
		this.impassableEdgeOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.impassableEdgeOpacityBox.Location = new Point( 132, 26 );
		this.impassableEdgeOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.impassableEdgeOpacityBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.impassableEdgeOpacityBox.Name = "impassableEdgeOpacityBox";
		this.impassableEdgeOpacityBox.Size = new Size( 50, 23 );
		this.impassableEdgeOpacityBox.TabIndex = 25;
		this.impassableEdgeOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.impassableEdgeOpacityBox, "この色の不透明度" );
		this.impassableEdgeOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.impassableEdgeOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// impassableEdgeOpacityUnit
		// 
		this.impassableEdgeOpacityUnit.Anchor = AnchorStyles.Left;
		this.impassableEdgeOpacityUnit.AutoSize = true;
		this.impassableEdgeOpacityUnit.Location = new Point( 185, 30 );
		this.impassableEdgeOpacityUnit.Name = "impassableEdgeOpacityUnit";
		this.impassableEdgeOpacityUnit.Size = new Size( 17, 15 );
		this.impassableEdgeOpacityUnit.TabIndex = 26;
		this.impassableEdgeOpacityUnit.Text = "%";
		// 
		// monsterBlockColorButton
		// 
		this.monsterBlockColorButton.Anchor = AnchorStyles.Left;
		this.monsterBlockColorButton.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.monsterBlockColorButton.FlatStyle = FlatStyle.Flat;
		this.monsterBlockColorButton.Location = new Point( 3, 53 );
		this.monsterBlockColorButton.Margin = new Padding( 3, 3, 0, 3 );
		this.monsterBlockColorButton.Name = "monsterBlockColorButton";
		this.monsterBlockColorButton.Size = new Size( 18, 18 );
		this.monsterBlockColorButton.TabIndex = 2;
		this.toolTip.SetToolTip( this.monsterBlockColorButton, "クリックで色を変更します" );
		this.monsterBlockColorButton.Click +=  this.ColorButton_Click ;
		// 
		// monsterBlockColorLabel
		// 
		this.monsterBlockColorLabel.Anchor = AnchorStyles.Left;
		this.monsterBlockColorLabel.AutoSize = true;
		this.monsterBlockColorLabel.Location = new Point( 24, 55 );
		this.monsterBlockColorLabel.Name = "monsterBlockColorLabel";
		this.monsterBlockColorLabel.Size = new Size( 75, 15 );
		this.monsterBlockColorLabel.TabIndex = 3;
		this.monsterBlockColorLabel.Text = "モンスター境界";
		// 
		// monsterBlockOpacityCheck
		// 
		this.monsterBlockOpacityCheck.Anchor = AnchorStyles.Left;
		this.monsterBlockOpacityCheck.AutoSize = true;
		this.monsterBlockOpacityCheck.Location = new Point( 114, 55 );
		this.monsterBlockOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.monsterBlockOpacityCheck.Name = "monsterBlockOpacityCheck";
		this.monsterBlockOpacityCheck.Size = new Size( 15, 14 );
		this.monsterBlockOpacityCheck.TabIndex = 27;
		this.toolTip.SetToolTip( this.monsterBlockOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.monsterBlockOpacityCheck.UseVisualStyleBackColor = true;
		this.monsterBlockOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// monsterBlockOpacityBox
		// 
		this.monsterBlockOpacityBox.Anchor = AnchorStyles.Left;
		this.monsterBlockOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.monsterBlockOpacityBox.Location = new Point( 132, 51 );
		this.monsterBlockOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.monsterBlockOpacityBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.monsterBlockOpacityBox.Name = "monsterBlockOpacityBox";
		this.monsterBlockOpacityBox.Size = new Size( 50, 23 );
		this.monsterBlockOpacityBox.TabIndex = 28;
		this.monsterBlockOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.monsterBlockOpacityBox, "この色の不透明度" );
		this.monsterBlockOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.monsterBlockOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// monsterBlockOpacityUnit
		// 
		this.monsterBlockOpacityUnit.Anchor = AnchorStyles.Left;
		this.monsterBlockOpacityUnit.AutoSize = true;
		this.monsterBlockOpacityUnit.Location = new Point( 185, 55 );
		this.monsterBlockOpacityUnit.Name = "monsterBlockOpacityUnit";
		this.monsterBlockOpacityUnit.Size = new Size( 17, 15 );
		this.monsterBlockOpacityUnit.TabIndex = 29;
		this.monsterBlockOpacityUnit.Text = "%";
		// 
		// mapMoveColorButton
		// 
		this.mapMoveColorButton.Anchor = AnchorStyles.Left;
		this.mapMoveColorButton.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.mapMoveColorButton.FlatStyle = FlatStyle.Flat;
		this.mapMoveColorButton.Location = new Point( 3, 78 );
		this.mapMoveColorButton.Margin = new Padding( 3, 3, 0, 3 );
		this.mapMoveColorButton.Name = "mapMoveColorButton";
		this.mapMoveColorButton.Size = new Size( 18, 18 );
		this.mapMoveColorButton.TabIndex = 4;
		this.toolTip.SetToolTip( this.mapMoveColorButton, "クリックで色を変更します" );
		this.mapMoveColorButton.Click +=  this.ColorButton_Click ;
		// 
		// mapMoveColorLabel
		// 
		this.mapMoveColorLabel.Anchor = AnchorStyles.Left;
		this.mapMoveColorLabel.AutoSize = true;
		this.mapMoveColorLabel.Location = new Point( 24, 80 );
		this.mapMoveColorLabel.Name = "mapMoveColorLabel";
		this.mapMoveColorLabel.Size = new Size( 43, 15 );
		this.mapMoveColorLabel.TabIndex = 5;
		this.mapMoveColorLabel.Text = "出入口";
		// 
		// mapMoveOpacityCheck
		// 
		this.mapMoveOpacityCheck.Anchor = AnchorStyles.Left;
		this.mapMoveOpacityCheck.AutoSize = true;
		this.mapMoveOpacityCheck.Location = new Point( 114, 80 );
		this.mapMoveOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.mapMoveOpacityCheck.Name = "mapMoveOpacityCheck";
		this.mapMoveOpacityCheck.Size = new Size( 15, 14 );
		this.mapMoveOpacityCheck.TabIndex = 30;
		this.toolTip.SetToolTip( this.mapMoveOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.mapMoveOpacityCheck.UseVisualStyleBackColor = true;
		this.mapMoveOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// mapMoveOpacityBox
		// 
		this.mapMoveOpacityBox.Anchor = AnchorStyles.Left;
		this.mapMoveOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.mapMoveOpacityBox.Location = new Point( 132, 76 );
		this.mapMoveOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.mapMoveOpacityBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.mapMoveOpacityBox.Name = "mapMoveOpacityBox";
		this.mapMoveOpacityBox.Size = new Size( 50, 23 );
		this.mapMoveOpacityBox.TabIndex = 31;
		this.mapMoveOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.mapMoveOpacityBox, "この色の不透明度" );
		this.mapMoveOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.mapMoveOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// mapMoveOpacityUnit
		// 
		this.mapMoveOpacityUnit.Anchor = AnchorStyles.Left;
		this.mapMoveOpacityUnit.AutoSize = true;
		this.mapMoveOpacityUnit.Location = new Point( 185, 80 );
		this.mapMoveOpacityUnit.Name = "mapMoveOpacityUnit";
		this.mapMoveOpacityUnit.Size = new Size( 17, 15 );
		this.mapMoveOpacityUnit.TabIndex = 32;
		this.mapMoveOpacityUnit.Text = "%";
		// 
		// specialColorButton
		// 
		this.specialColorButton.Anchor = AnchorStyles.Left;
		this.specialColorButton.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.specialColorButton.FlatStyle = FlatStyle.Flat;
		this.specialColorButton.Location = new Point( 3, 103 );
		this.specialColorButton.Margin = new Padding( 3, 3, 0, 3 );
		this.specialColorButton.Name = "specialColorButton";
		this.specialColorButton.Size = new Size( 18, 18 );
		this.specialColorButton.TabIndex = 6;
		this.toolTip.SetToolTip( this.specialColorButton, "クリックで色を変更します" );
		this.specialColorButton.Click +=  this.ColorButton_Click ;
		// 
		// specialColorLabel
		// 
		this.specialColorLabel.Anchor = AnchorStyles.Left;
		this.specialColorLabel.AutoSize = true;
		this.specialColorLabel.Location = new Point( 24, 105 );
		this.specialColorLabel.Name = "specialColorLabel";
		this.specialColorLabel.Size = new Size( 38, 15 );
		this.specialColorLabel.TabIndex = 7;
		this.specialColorLabel.Text = "その他";
		// 
		// specialOpacityCheck
		// 
		this.specialOpacityCheck.Anchor = AnchorStyles.Left;
		this.specialOpacityCheck.AutoSize = true;
		this.specialOpacityCheck.Location = new Point( 114, 105 );
		this.specialOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.specialOpacityCheck.Name = "specialOpacityCheck";
		this.specialOpacityCheck.Size = new Size( 15, 14 );
		this.specialOpacityCheck.TabIndex = 33;
		this.toolTip.SetToolTip( this.specialOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.specialOpacityCheck.UseVisualStyleBackColor = true;
		this.specialOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// specialOpacityBox
		// 
		this.specialOpacityBox.Anchor = AnchorStyles.Left;
		this.specialOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.specialOpacityBox.Location = new Point( 132, 101 );
		this.specialOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.specialOpacityBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.specialOpacityBox.Name = "specialOpacityBox";
		this.specialOpacityBox.Size = new Size( 50, 23 );
		this.specialOpacityBox.TabIndex = 34;
		this.specialOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.specialOpacityBox, "この色の不透明度" );
		this.specialOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.specialOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// specialOpacityUnit
		// 
		this.specialOpacityUnit.Anchor = AnchorStyles.Left;
		this.specialOpacityUnit.AutoSize = true;
		this.specialOpacityUnit.Location = new Point( 185, 105 );
		this.specialOpacityUnit.Name = "specialOpacityUnit";
		this.specialOpacityUnit.Size = new Size( 17, 15 );
		this.specialOpacityUnit.TabIndex = 35;
		this.specialOpacityUnit.Text = "%";
		// 
		// gridColorButton
		// 
		this.gridColorButton.Anchor = AnchorStyles.Left;
		this.gridColorButton.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.gridColorButton.FlatStyle = FlatStyle.Flat;
		this.gridColorButton.Location = new Point( 3, 128 );
		this.gridColorButton.Margin = new Padding( 3, 3, 0, 3 );
		this.gridColorButton.Name = "gridColorButton";
		this.gridColorButton.Size = new Size( 18, 18 );
		this.gridColorButton.TabIndex = 8;
		this.toolTip.SetToolTip( this.gridColorButton, "クリックで色を変更します" );
		this.gridColorButton.Click +=  this.GridColorButton_Click ;
		// 
		// gridColorLabel
		// 
		this.gridColorLabel.Anchor = AnchorStyles.Left;
		this.gridColorLabel.AutoSize = true;
		this.gridColorLabel.Location = new Point( 24, 130 );
		this.gridColorLabel.Name = "gridColorLabel";
		this.gridColorLabel.Size = new Size( 40, 15 );
		this.gridColorLabel.TabIndex = 9;
		this.gridColorLabel.Text = "グリッド";
		// 
		// gridOpacityCheck
		// 
		this.gridOpacityCheck.Anchor = AnchorStyles.Left;
		this.gridOpacityCheck.AutoSize = true;
		this.gridOpacityCheck.Location = new Point( 114, 130 );
		this.gridOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.gridOpacityCheck.Name = "gridOpacityCheck";
		this.gridOpacityCheck.Size = new Size( 15, 14 );
		this.gridOpacityCheck.TabIndex = 36;
		this.toolTip.SetToolTip( this.gridOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.gridOpacityCheck.UseVisualStyleBackColor = true;
		this.gridOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// gridOpacityBox
		// 
		this.gridOpacityBox.Anchor = AnchorStyles.Left;
		this.gridOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.gridOpacityBox.Location = new Point( 132, 126 );
		this.gridOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.gridOpacityBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.gridOpacityBox.Name = "gridOpacityBox";
		this.gridOpacityBox.Size = new Size( 50, 23 );
		this.gridOpacityBox.TabIndex = 37;
		this.gridOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.gridOpacityBox, "この色の不透明度" );
		this.gridOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.gridOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// gridOpacityUnit
		// 
		this.gridOpacityUnit.Anchor = AnchorStyles.Left;
		this.gridOpacityUnit.AutoSize = true;
		this.gridOpacityUnit.Location = new Point( 185, 130 );
		this.gridOpacityUnit.Name = "gridOpacityUnit";
		this.gridOpacityUnit.Size = new Size( 17, 15 );
		this.gridOpacityUnit.TabIndex = 38;
		this.gridOpacityUnit.Text = "%";
		// 
		// playerColorButton
		// 
		this.playerColorButton.Anchor = AnchorStyles.Left;
		this.playerColorButton.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.playerColorButton.FlatStyle = FlatStyle.Flat;
		this.playerColorButton.Location = new Point( 3, 153 );
		this.playerColorButton.Margin = new Padding( 3, 3, 0, 3 );
		this.playerColorButton.Name = "playerColorButton";
		this.playerColorButton.Size = new Size( 18, 18 );
		this.playerColorButton.TabIndex = 10;
		this.toolTip.SetToolTip( this.playerColorButton, "クリックで色を変更します" );
		this.playerColorButton.Click +=  this.PlayerColorButton_Click ;
		// 
		// playerColorLabel
		// 
		this.playerColorLabel.Anchor = AnchorStyles.Left;
		this.playerColorLabel.AutoSize = true;
		this.playerColorLabel.Location = new Point( 24, 155 );
		this.playerColorLabel.Name = "playerColorLabel";
		this.playerColorLabel.Size = new Size( 52, 15 );
		this.playerColorLabel.TabIndex = 11;
		this.playerColorLabel.Text = "プレイヤー";
		// 
		// playerOpacityCheck
		// 
		this.playerOpacityCheck.Anchor = AnchorStyles.Left;
		this.playerOpacityCheck.AutoSize = true;
		this.playerOpacityCheck.Location = new Point( 114, 155 );
		this.playerOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.playerOpacityCheck.Name = "playerOpacityCheck";
		this.playerOpacityCheck.Size = new Size( 15, 14 );
		this.playerOpacityCheck.TabIndex = 39;
		this.toolTip.SetToolTip( this.playerOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.playerOpacityCheck.UseVisualStyleBackColor = true;
		this.playerOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// playerOpacityBox
		// 
		this.playerOpacityBox.Anchor = AnchorStyles.Left;
		this.playerOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.playerOpacityBox.Location = new Point( 132, 151 );
		this.playerOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.playerOpacityBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.playerOpacityBox.Name = "playerOpacityBox";
		this.playerOpacityBox.Size = new Size( 50, 23 );
		this.playerOpacityBox.TabIndex = 40;
		this.playerOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.playerOpacityBox, "この色の不透明度" );
		this.playerOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.playerOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// playerOpacityUnit
		// 
		this.playerOpacityUnit.Anchor = AnchorStyles.Left;
		this.playerOpacityUnit.AutoSize = true;
		this.playerOpacityUnit.Location = new Point( 185, 155 );
		this.playerOpacityUnit.Name = "playerOpacityUnit";
		this.playerOpacityUnit.Size = new Size( 17, 15 );
		this.playerOpacityUnit.TabIndex = 41;
		this.playerOpacityUnit.Text = "%";
		// 
		// showChunksCheck
		// 
		this.showChunksCheck.Anchor = AnchorStyles.Left;
		this.showChunksCheck.AutoSize = true;
		this.showChunksCheck.Location = new Point( 3, 0 );
		this.showChunksCheck.Margin = new Padding( 3, 0, 3, 0 );
		this.showChunksCheck.Name = "showChunksCheck";
		this.showChunksCheck.Size = new Size( 94, 19 );
		this.showChunksCheck.TabIndex = 0;
		this.showChunksCheck.Text = "チャンクを表示";
		this.toolTip.SetToolTip( this.showChunksCheck, "地形の画像のチャンクの境目と番号を描きます。描いたばかりのチャンクは黄色くなります" );
		this.showChunksCheck.UseVisualStyleBackColor = true;
		this.showChunksCheck.CheckedChanged +=  this.ShowChunksCheck_CheckedChanged ;
		// 
		// motionGroup
		// 
		this.motionGroup.AutoSize = true;
		this.motionGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.motionGroup.Controls.Add( this.motionPanel );
		this.motionGroup.Dock = DockStyle.Fill;
		this.motionGroup.Location = new Point( 3, 366 );
		this.motionGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.motionGroup.Name = "motionGroup";
		this.motionGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.motionGroup.Size = new Size( 308, 170 );
		this.motionGroup.TabIndex = 2;
		this.motionGroup.TabStop = false;
		this.motionGroup.Text = "オーバーレイの移動";
		// 
		// motionPanel
		// 
		this.motionPanel.AutoSize = true;
		this.motionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.motionPanel.ColumnCount = 2;
		this.motionPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.motionPanel.ColumnStyles.Add( new ColumnStyle() );
		this.motionPanel.Controls.Add( this.jumpRadio, 0, 1 );
		this.motionPanel.Controls.Add( this.moveSpeedPanel, 0, 0 );
		this.motionPanel.Controls.Add( this.jumpDelayPanel, 0, 2 );
		this.motionPanel.Controls.Add( this.jumpDelayExtraPanel, 0, 3 );
		this.motionPanel.Controls.Add( this.jumpDelayValuePanel, 0, 4 );
		this.motionPanel.Controls.Add( this.slideRadio, 0, 5 );
		this.motionPanel.Dock = DockStyle.Fill;
		this.motionPanel.Location = new Point( 8, 20 );
		this.motionPanel.Name = "motionPanel";
		this.motionPanel.RowCount = 6;
		this.motionPanel.RowStyles.Add( new RowStyle() );
		this.motionPanel.RowStyles.Add( new RowStyle() );
		this.motionPanel.RowStyles.Add( new RowStyle() );
		this.motionPanel.RowStyles.Add( new RowStyle() );
		this.motionPanel.RowStyles.Add( new RowStyle() );
		this.motionPanel.RowStyles.Add( new RowStyle() );
		this.motionPanel.Size = new Size( 292, 144 );
		this.motionPanel.TabIndex = 0;
		// 
		// jumpRadio
		// 
		this.jumpRadio.AutoSize = true;
		this.motionPanel.SetColumnSpan( this.jumpRadio, 2 );
		this.jumpRadio.Location = new Point( 3, 32 );
		this.jumpRadio.Name = "jumpRadio";
		this.jumpRadio.Size = new Size( 101, 19 );
		this.jumpRadio.TabIndex = 2;
		this.jumpRadio.TabStop = true;
		this.jumpRadio.Text = "マス単位で移動";
		this.toolTip.SetToolTip( this.jumpRadio, "座標が変わるたびに、移動遅延だけ待って表示マスを更新します" );
		this.jumpRadio.UseVisualStyleBackColor = true;
		// 
		// moveSpeedPanel
		// 
		this.moveSpeedPanel.AutoSize = true;
		this.moveSpeedPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.motionPanel.SetColumnSpan( this.moveSpeedPanel, 2 );
		this.moveSpeedPanel.Controls.Add( this.moveSpeedCaption );
		this.moveSpeedPanel.Controls.Add( this.moveSpeedBox );
		this.moveSpeedPanel.Location = new Point( 0, 0 );
		this.moveSpeedPanel.Margin = new Padding( 0 );
		this.moveSpeedPanel.Name = "moveSpeedPanel";
		this.moveSpeedPanel.Size = new Size( 245, 29 );
		this.moveSpeedPanel.TabIndex = 3;
		this.moveSpeedPanel.WrapContents = false;
		// 
		// moveSpeedCaption
		// 
		this.moveSpeedCaption.Anchor = AnchorStyles.Left;
		this.moveSpeedCaption.AutoSize = true;
		this.moveSpeedCaption.Location = new Point( 3, 7 );
		this.moveSpeedCaption.Margin = new Padding( 3, 0, 12, 0 );
		this.moveSpeedCaption.Name = "moveSpeedCaption";
		this.moveSpeedCaption.Size = new Size( 104, 15 );
		this.moveSpeedCaption.TabIndex = 0;
		this.moveSpeedCaption.Text = "キャラ移動速度検出";
		this.toolTip.SetToolTip( this.moveSpeedCaption, "キャラクターの移動速度です。5 段階のうち、自動なら歩いたときの座標の変わる間隔から判定し、判定できるまでは前回の速度を使います" );
		// 
		// moveSpeedBox
		// 
		this.moveSpeedBox.DropDownStyle = ComboBoxStyle.DropDownList;
		this.moveSpeedBox.Location = new Point( 122, 3 );
		this.moveSpeedBox.Name = "moveSpeedBox";
		this.moveSpeedBox.Size = new Size( 120, 23 );
		this.moveSpeedBox.TabIndex = 1;
		this.moveSpeedBox.SelectedIndexChanged +=  this.MoveSpeedBox_SelectedIndexChanged ;
		// 
		// jumpDelayPanel
		// 
		this.jumpDelayPanel.AutoSize = true;
		this.jumpDelayPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.motionPanel.SetColumnSpan( this.jumpDelayPanel, 2 );
		this.jumpDelayPanel.Controls.Add( this.jumpDelayCaption );
		this.jumpDelayPanel.Location = new Point( 0, 54 );
		this.jumpDelayPanel.Margin = new Padding( 0 );
		this.jumpDelayPanel.Name = "jumpDelayPanel";
		this.jumpDelayPanel.Size = new Size( 101, 15 );
		this.jumpDelayPanel.TabIndex = 4;
		this.jumpDelayPanel.WrapContents = false;
		// 
		// jumpDelayCaption
		// 
		this.jumpDelayCaption.Anchor = AnchorStyles.Left;
		this.jumpDelayCaption.AutoSize = true;
		this.jumpDelayCaption.Location = new Point( 15, 0 );
		this.jumpDelayCaption.Margin = new Padding( 15, 0, 3, 0 );
		this.jumpDelayCaption.Name = "jumpDelayCaption";
		this.jumpDelayCaption.Size = new Size( 83, 15 );
		this.jumpDelayCaption.TabIndex = 0;
		this.jumpDelayCaption.Text = "マスの移動遅延";
		this.toolTip.SetToolTip( this.jumpDelayCaption, "座標が変わってからこの時間だけ待ってマスを移します。座標は歩き出した瞬間に変わるので、1 歩の半分くらい待つとキャラクターの絵とずれにくくなります" );
		// 
		// jumpDelayExtraPanel
		// 
		this.jumpDelayExtraPanel.AutoSize = true;
		this.jumpDelayExtraPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.motionPanel.SetColumnSpan( this.jumpDelayExtraPanel, 2 );
		this.jumpDelayExtraPanel.Controls.Add( this.jumpDelayExtraCaption );
		this.jumpDelayExtraPanel.Controls.Add( this.jumpDelayExtraBox );
		this.jumpDelayExtraPanel.Controls.Add( this.jumpDelayExtraUnit );
		this.jumpDelayExtraPanel.Controls.Add( this.jumpDelayTotal );
		this.jumpDelayExtraPanel.Location = new Point( 20, 69 );
		this.jumpDelayExtraPanel.Margin = new Padding( 20, 0, 0, 0 );
		this.jumpDelayExtraPanel.Name = "jumpDelayExtraPanel";
		this.jumpDelayExtraPanel.Size = new Size( 272, 25 );
		this.jumpDelayExtraPanel.TabIndex = 6;
		this.jumpDelayExtraPanel.WrapContents = false;
		// 
		// jumpDelayExtraCaption
		// 
		this.jumpDelayExtraCaption.Anchor = AnchorStyles.Left;
		this.jumpDelayExtraCaption.AutoSize = true;
		this.jumpDelayExtraCaption.Location = new Point( 3, 3 );
		this.jumpDelayExtraCaption.Name = "jumpDelayExtraCaption";
		this.jumpDelayExtraCaption.Size = new Size( 81, 19 );
		this.jumpDelayExtraCaption.TabIndex = 0;
		this.jumpDelayExtraCaption.Text = "自動+手動";
		this.toolTip.SetToolTip( this.jumpDelayExtraCaption, "移動速度の半分を 10 ミリ秒単位に丸めた時間（自動）に、入れた時間を足して待ちます" );
		this.jumpDelayExtraCaption.UseVisualStyleBackColor = true;
		this.jumpDelayExtraCaption.CheckedChanged +=  this.JumpDelayAutoRadio_CheckedChanged ;
		// 
		// jumpDelayExtraBox
		// 
		this.jumpDelayExtraBox.Increment = new decimal( new int[] { 10, 0, 0, 0 } );
		this.jumpDelayExtraBox.Location = new Point( 90, 0 );
		this.jumpDelayExtraBox.Margin = new Padding( 3, 0, 0, 0 );
		this.jumpDelayExtraBox.Maximum = new decimal( new int[] { 500, 0, 0, 0 } );
		this.jumpDelayExtraBox.Minimum = new decimal( new int[] { 500, 0, 0, int.MinValue } );
		this.jumpDelayExtraBox.Name = "jumpDelayExtraBox";
		this.jumpDelayExtraBox.Size = new Size( 60, 23 );
		this.jumpDelayExtraBox.TabIndex = 1;
		this.jumpDelayExtraBox.TextAlign = HorizontalAlignment.Right;
		this.jumpDelayExtraBox.Value = new decimal( new int[] { 40, 0, 0, 0 } );
		this.jumpDelayExtraBox.ValueChanged +=  this.JumpDelayExtraBox_ValueChanged ;
		// 
		// jumpDelayExtraUnit
		// 
		this.jumpDelayExtraUnit.Anchor = AnchorStyles.Left;
		this.jumpDelayExtraUnit.AutoSize = true;
		this.jumpDelayExtraUnit.Location = new Point( 150, 5 );
		this.jumpDelayExtraUnit.Margin = new Padding( 0 );
		this.jumpDelayExtraUnit.Name = "jumpDelayExtraUnit";
		this.jumpDelayExtraUnit.Size = new Size( 34, 15 );
		this.jumpDelayExtraUnit.TabIndex = 2;
		this.jumpDelayExtraUnit.Text = "ミリ秒";
		// 
		// jumpDelayTotal
		// 
		this.jumpDelayTotal.Anchor = AnchorStyles.Left;
		this.jumpDelayTotal.AutoSize = true;
		this.jumpDelayTotal.Location = new Point( 184, 5 );
		this.jumpDelayTotal.Margin = new Padding( 0, 0, 3, 0 );
		this.jumpDelayTotal.Name = "jumpDelayTotal";
		this.jumpDelayTotal.Size = new Size( 85, 15 );
		this.jumpDelayTotal.TabIndex = 3;
		this.jumpDelayTotal.Text = "自動150 → 190";
		// 
		// jumpDelayValuePanel
		// 
		this.jumpDelayValuePanel.AutoSize = true;
		this.jumpDelayValuePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.motionPanel.SetColumnSpan( this.jumpDelayValuePanel, 2 );
		this.jumpDelayValuePanel.Controls.Add( this.jumpDelayValueCaption );
		this.jumpDelayValuePanel.Controls.Add( this.jumpDelayBox );
		this.jumpDelayValuePanel.Controls.Add( this.jumpDelayUnit );
		this.jumpDelayValuePanel.Location = new Point( 20, 94 );
		this.jumpDelayValuePanel.Margin = new Padding( 20, 0, 0, 0 );
		this.jumpDelayValuePanel.Name = "jumpDelayValuePanel";
		this.jumpDelayValuePanel.Size = new Size( 187, 25 );
		this.jumpDelayValuePanel.TabIndex = 5;
		this.jumpDelayValuePanel.WrapContents = false;
		// 
		// jumpDelayValueCaption
		// 
		this.jumpDelayValueCaption.Anchor = AnchorStyles.Left;
		this.jumpDelayValueCaption.Location = new Point( 3, 3 );
		this.jumpDelayValueCaption.Name = "jumpDelayValueCaption";
		this.jumpDelayValueCaption.Size = new Size( 81, 19 );
		this.jumpDelayValueCaption.TabIndex = 0;
		this.jumpDelayValueCaption.Text = "手動";
		this.toolTip.SetToolTip( this.jumpDelayValueCaption, "入れた時間を待ちます" );
		this.jumpDelayValueCaption.UseVisualStyleBackColor = true;
		this.jumpDelayValueCaption.CheckedChanged +=  this.JumpDelayManualRadio_CheckedChanged ;
		// 
		// jumpDelayBox
		// 
		this.jumpDelayBox.Increment = new decimal( new int[] { 10, 0, 0, 0 } );
		this.jumpDelayBox.Location = new Point( 90, 0 );
		this.jumpDelayBox.Margin = new Padding( 3, 0, 0, 0 );
		this.jumpDelayBox.Maximum = new decimal( new int[] { 1000, 0, 0, 0 } );
		this.jumpDelayBox.Name = "jumpDelayBox";
		this.jumpDelayBox.Size = new Size( 60, 23 );
		this.jumpDelayBox.TabIndex = 1;
		this.jumpDelayBox.TextAlign = HorizontalAlignment.Right;
		this.jumpDelayBox.ValueChanged +=  this.JumpDelayBox_ValueChanged ;
		// 
		// jumpDelayUnit
		// 
		this.jumpDelayUnit.Anchor = AnchorStyles.Left;
		this.jumpDelayUnit.AutoSize = true;
		this.jumpDelayUnit.Location = new Point( 150, 5 );
		this.jumpDelayUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.jumpDelayUnit.Name = "jumpDelayUnit";
		this.jumpDelayUnit.Size = new Size( 34, 15 );
		this.jumpDelayUnit.TabIndex = 2;
		this.jumpDelayUnit.Text = "ミリ秒";
		// 
		// slideRadio
		// 
		this.slideRadio.AutoSize = true;
		this.motionPanel.SetColumnSpan( this.slideRadio, 2 );
		this.slideRadio.Location = new Point( 3, 122 );
		this.slideRadio.Name = "slideRadio";
		this.slideRadio.Size = new Size( 143, 19 );
		this.slideRadio.TabIndex = 7;
		this.slideRadio.Text = "スライドで移動 (ベータ版)";
		this.toolTip.SetToolTip( this.slideRadio, "キャラクターの移動に合わせてマスを追従させます" );
		this.slideRadio.UseVisualStyleBackColor = true;
		this.slideRadio.CheckedChanged +=  this.SlideCheck_CheckedChanged ;
		// 
		// advancedPanel
		// 
		this.advancedPanel.AutoSize = true;
		this.advancedPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.advancedPanel.ColumnCount = 1;
		this.advancedPanel.ColumnStyles.Add( new ColumnStyle() );
		this.advancedPanel.Controls.Add( this.regionGroup, 0, 0 );
		this.advancedPanel.Controls.Add( this.speedGroup, 0, 1 );
		this.advancedPanel.Controls.Add( this.slideGroup, 0, 4 );
		this.advancedPanel.Controls.Add( this.nameGroup, 0, 2 );
		this.advancedPanel.Controls.Add( this.drawingGroup, 0, 3 );
		this.advancedPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		this.advancedPanel.Location = new Point( 320, 0 );
		this.advancedPanel.Margin = new Padding( 6, 0, 0, 0 );
		this.advancedPanel.Name = "advancedPanel";
		this.advancedPanel.RowCount = 5;
		this.advancedPanel.RowStyles.Add( new RowStyle() );
		this.advancedPanel.RowStyles.Add( new RowStyle() );
		this.advancedPanel.RowStyles.Add( new RowStyle() );
		this.advancedPanel.RowStyles.Add( new RowStyle() );
		this.advancedPanel.RowStyles.Add( new RowStyle() );
		this.advancedPanel.Size = new Size( 329, 709 );
		this.advancedPanel.TabIndex = 1;
		this.advancedPanel.Visible = false;
		// 
		// speedGroup
		// 
		this.speedGroup.AutoSize = true;
		this.speedGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.speedGroup.Controls.Add( this.speedPanel );
		this.speedGroup.Dock = DockStyle.Fill;
		this.speedGroup.Location = new Point( 3, 94 );
		this.speedGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.speedGroup.Name = "speedGroup";
		this.speedGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.speedGroup.Size = new Size( 323, 76 );
		this.speedGroup.TabIndex = 1;
		this.speedGroup.TabStop = false;
		this.speedGroup.Text = "読み取り";
		// 
		// speedPanel
		// 
		this.speedPanel.AutoSize = true;
		this.speedPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.speedPanel.ColumnCount = 3;
		this.speedPanel.ColumnStyles.Add( new ColumnStyle() );
		this.speedPanel.ColumnStyles.Add( new ColumnStyle() );
		this.speedPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.speedPanel.Controls.Add( this.intervalCaption, 0, 0 );
		this.speedPanel.Controls.Add( this.intervalBox, 1, 0 );
		this.speedPanel.Controls.Add( this.intervalUnit, 2, 0 );
		this.speedPanel.Controls.Add( this.threadsCaption, 0, 1 );
		this.speedPanel.Controls.Add( this.threadsBox, 1, 1 );
		this.speedPanel.Dock = DockStyle.Fill;
		this.speedPanel.Location = new Point( 8, 20 );
		this.speedPanel.Name = "speedPanel";
		this.speedPanel.RowCount = 3;
		this.speedPanel.RowStyles.Add( new RowStyle() );
		this.speedPanel.RowStyles.Add( new RowStyle() );
		this.speedPanel.RowStyles.Add( new RowStyle() );
		this.speedPanel.Size = new Size( 307, 50 );
		this.speedPanel.TabIndex = 0;
		// 
		// intervalCaption
		// 
		this.intervalCaption.Anchor = AnchorStyles.Left;
		this.intervalCaption.AutoSize = true;
		this.intervalCaption.Location = new Point( 3, 5 );
		this.intervalCaption.Name = "intervalCaption";
		this.intervalCaption.Size = new Size( 74, 15 );
		this.intervalCaption.TabIndex = 0;
		this.intervalCaption.Text = "読み取り間隔";
		this.toolTip.SetToolTip( this.intervalCaption, "Windowsの最小タイマーが約15.6msなので、それ以下の値は入力できません" );
		// 
		// intervalBox
		// 
		this.intervalBox.Increment = new decimal( new int[] { 10, 0, 0, 0 } );
		this.intervalBox.Location = new Point( 98, 1 );
		this.intervalBox.Margin = new Padding( 3, 1, 3, 1 );
		this.intervalBox.Maximum = new decimal( new int[] { 5000, 0, 0, 0 } );
		this.intervalBox.Minimum = new decimal( new int[] { 15, 0, 0, 0 } );
		this.intervalBox.Name = "intervalBox";
		this.intervalBox.Size = new Size( 80, 23 );
		this.intervalBox.TabIndex = 1;
		this.intervalBox.TextAlign = HorizontalAlignment.Right;
		this.intervalBox.Value = new decimal( new int[] { 15, 0, 0, 0 } );
		this.intervalBox.ValueChanged +=  this.IntervalBox_ValueChanged ;
		// 
		// intervalUnit
		// 
		this.intervalUnit.Anchor = AnchorStyles.Left;
		this.intervalUnit.AutoSize = true;
		this.intervalUnit.Location = new Point( 181, 5 );
		this.intervalUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.intervalUnit.Name = "intervalUnit";
		this.intervalUnit.Size = new Size( 34, 15 );
		this.intervalUnit.TabIndex = 2;
		this.intervalUnit.Text = "ミリ秒";
		// 
		// threadsCaption
		// 
		this.threadsCaption.Anchor = AnchorStyles.Left;
		this.threadsCaption.AutoSize = true;
		this.threadsCaption.Location = new Point( 3, 30 );
		this.threadsCaption.Name = "threadsCaption";
		this.threadsCaption.Size = new Size( 89, 15 );
		this.threadsCaption.TabIndex = 3;
		this.threadsCaption.Text = "OCR のスレッド数";
		this.toolTip.SetToolTip( this.threadsCaption, "PaddleOCR のときだけ使います" );
		// 
		// threadsBox
		// 
		this.threadsBox.DropDownStyle = ComboBoxStyle.DropDownList;
		this.threadsBox.Location = new Point( 98, 26 );
		this.threadsBox.Margin = new Padding( 3, 1, 3, 1 );
		this.threadsBox.Name = "threadsBox";
		this.threadsBox.Size = new Size( 80, 23 );
		this.threadsBox.TabIndex = 4;
		this.toolTip.SetToolTip( this.threadsBox, "PaddleOCR のときだけ使います" );
		this.threadsBox.SelectedIndexChanged +=  this.ThreadsBox_SelectedIndexChanged ;
		// 
		// slideGroup
		// 
		this.slideGroup.AutoSize = true;
		this.slideGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.slideGroup.Controls.Add( this.slidePanel );
		this.slideGroup.Dock = DockStyle.Fill;
		this.slideGroup.Location = new Point( 3, 344 );
		this.slideGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.slideGroup.Name = "slideGroup";
		this.slideGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.slideGroup.Size = new Size( 323, 357 );
		this.slideGroup.TabIndex = 2;
		this.slideGroup.TabStop = false;
		this.slideGroup.Text = "スライド (ベータ版)";
		// 
		// slidePanel
		// 
		this.slidePanel.AutoSize = true;
		this.slidePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.slidePanel.ColumnCount = 3;
		this.slidePanel.ColumnStyles.Add( new ColumnStyle() );
		this.slidePanel.ColumnStyles.Add( new ColumnStyle() );
		this.slidePanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.slidePanel.Controls.Add( this.overlaySlideCaption, 0, 0 );
		this.slidePanel.Controls.Add( this.overlaySlideBox, 1, 0 );
		this.slidePanel.Controls.Add( this.overlaySlideOptionPanel, 2, 0 );
		this.slidePanel.Controls.Add( this.snapTilesCaption, 0, 1 );
		this.slidePanel.Controls.Add( this.snapTilesBox, 1, 1 );
		this.slidePanel.Controls.Add( this.snapTilesUnit, 2, 1 );
		this.slidePanel.Controls.Add( this.keyPredictionCheck, 0, 2 );
		this.slidePanel.Controls.Add( this.keyDelayCaption, 0, 3 );
		this.slidePanel.Controls.Add( this.keyDelayBox, 1, 3 );
		this.slidePanel.Controls.Add( this.keyDelayUnit, 2, 3 );
		this.slidePanel.Controls.Add( this.keyContinueCheck, 0, 4 );
		this.slidePanel.Controls.Add( this.inputLagCaption, 0, 5 );
		this.slidePanel.Controls.Add( this.inputLagBox, 1, 5 );
		this.slidePanel.Controls.Add( this.inputLagUnit, 2, 5 );
		this.slidePanel.Controls.Add( this.keyRepredictCheck, 0, 6 );
		this.slidePanel.Controls.Add( this.pixelCheckCheck, 0, 7 );
		this.slidePanel.Controls.Add( this.startCheckCaption, 0, 8 );
		this.slidePanel.Controls.Add( this.startCheckBox, 1, 8 );
		this.slidePanel.Controls.Add( this.startCheckUnit, 2, 8 );
		this.slidePanel.Controls.Add( this.skillPredictionCheck, 0, 9 );
		this.slidePanel.Controls.Add( this.skillKeyCaption, 0, 10 );
		this.slidePanel.Controls.Add( this.skillKeyBox, 1, 10 );
		this.slidePanel.Controls.Add( this.skillTileMsCaption, 0, 11 );
		this.slidePanel.Controls.Add( this.skillTileMsBox, 1, 11 );
		this.slidePanel.Controls.Add( this.skillTileMsUnit, 2, 11 );
		this.slidePanel.Dock = DockStyle.Fill;
		this.slidePanel.Location = new Point( 8, 20 );
		this.slidePanel.Name = "slidePanel";
		this.slidePanel.RowCount = 13;
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.RowStyles.Add( new RowStyle() );
		this.slidePanel.Size = new Size( 307, 331 );
		this.slidePanel.TabIndex = 0;
		// 
		// overlaySlideCaption
		// 
		this.overlaySlideCaption.Anchor = AnchorStyles.Left;
		this.overlaySlideCaption.AutoSize = true;
		this.overlaySlideCaption.Location = new Point( 3, 5 );
		this.overlaySlideCaption.Name = "overlaySlideCaption";
		this.overlaySlideCaption.Size = new Size( 65, 15 );
		this.overlaySlideCaption.TabIndex = 8;
		this.overlaySlideCaption.Text = "スライド時間";
		this.toolTip.SetToolTip( this.overlaySlideCaption, "1 マスのスライドにかける時間です" );
		// 
		// overlaySlideBox
		// 
		this.overlaySlideBox.Location = new Point( 131, 1 );
		this.overlaySlideBox.Margin = new Padding( 3, 1, 3, 1 );
		this.overlaySlideBox.Maximum = new decimal( new int[] { 2000, 0, 0, 0 } );
		this.overlaySlideBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.overlaySlideBox.Name = "overlaySlideBox";
		this.overlaySlideBox.Size = new Size( 80, 23 );
		this.overlaySlideBox.TabIndex = 9;
		this.overlaySlideBox.TextAlign = HorizontalAlignment.Right;
		this.overlaySlideBox.Value = new decimal( new int[] { 263, 0, 0, 0 } );
		this.overlaySlideBox.ValueChanged +=  this.OverlaySlideBox_ValueChanged ;
		// 
		// overlaySlideOptionPanel
		// 
		this.overlaySlideOptionPanel.Anchor = AnchorStyles.Left;
		this.overlaySlideOptionPanel.AutoSize = true;
		this.overlaySlideOptionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.overlaySlideOptionPanel.Controls.Add( this.overlaySlideUnit );
		this.overlaySlideOptionPanel.Controls.Add( this.overlaySlideManualCheck );
		this.overlaySlideOptionPanel.Location = new Point( 214, 3 );
		this.overlaySlideOptionPanel.Margin = new Padding( 0 );
		this.overlaySlideOptionPanel.Name = "overlaySlideOptionPanel";
		this.overlaySlideOptionPanel.Size = new Size( 93, 19 );
		this.overlaySlideOptionPanel.TabIndex = 10;
		this.overlaySlideOptionPanel.WrapContents = false;
		// 
		// overlaySlideUnit
		// 
		this.overlaySlideUnit.Anchor = AnchorStyles.Left;
		this.overlaySlideUnit.AutoSize = true;
		this.overlaySlideUnit.Location = new Point( 0, 2 );
		this.overlaySlideUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.overlaySlideUnit.Name = "overlaySlideUnit";
		this.overlaySlideUnit.Size = new Size( 34, 15 );
		this.overlaySlideUnit.TabIndex = 0;
		this.overlaySlideUnit.Text = "ミリ秒";
		// 
		// overlaySlideManualCheck
		// 
		this.overlaySlideManualCheck.Anchor = AnchorStyles.Left;
		this.overlaySlideManualCheck.AutoSize = true;
		this.overlaySlideManualCheck.Location = new Point( 40, 0 );
		this.overlaySlideManualCheck.Margin = new Padding( 3, 0, 3, 0 );
		this.overlaySlideManualCheck.Name = "overlaySlideManualCheck";
		this.overlaySlideManualCheck.Size = new Size( 50, 19 );
		this.overlaySlideManualCheck.TabIndex = 1;
		this.overlaySlideManualCheck.Text = "手動";
		this.toolTip.SetToolTip( this.overlaySlideManualCheck, "スライド時間を手動で設定します" );
		this.overlaySlideManualCheck.UseVisualStyleBackColor = true;
		this.overlaySlideManualCheck.CheckedChanged +=  this.OverlaySlideManualCheck_CheckedChanged ;
		// 
		// snapTilesCaption
		// 
		this.snapTilesCaption.Anchor = AnchorStyles.Left;
		this.snapTilesCaption.AutoSize = true;
		this.snapTilesCaption.Location = new Point( 3, 30 );
		this.snapTilesCaption.Name = "snapTilesCaption";
		this.snapTilesCaption.Size = new Size( 65, 15 );
		this.snapTilesCaption.TabIndex = 27;
		this.snapTilesCaption.Text = "スライド上限";
		this.toolTip.SetToolTip( this.snapTilesCaption, "描画位置が読み取った座標からこのマス数以上離れたら、スライドせずにすぐ移します" );
		// 
		// snapTilesBox
		// 
		this.snapTilesBox.DecimalPlaces = 1;
		this.snapTilesBox.Increment = new decimal( new int[] { 1, 0, 0, 65536 } );
		this.snapTilesBox.Location = new Point( 131, 26 );
		this.snapTilesBox.Margin = new Padding( 3, 1, 3, 1 );
		this.snapTilesBox.Maximum = new decimal( new int[] { 5, 0, 0, 0 } );
		this.snapTilesBox.Name = "snapTilesBox";
		this.snapTilesBox.Size = new Size( 80, 23 );
		this.snapTilesBox.TabIndex = 28;
		this.snapTilesBox.TextAlign = HorizontalAlignment.Right;
		this.snapTilesBox.Value = new decimal( new int[] { 15, 0, 0, 65536 } );
		this.snapTilesBox.ValueChanged +=  this.SnapTilesBox_ValueChanged ;
		// 
		// snapTilesUnit
		// 
		this.snapTilesUnit.Anchor = AnchorStyles.Left;
		this.snapTilesUnit.AutoSize = true;
		this.snapTilesUnit.Location = new Point( 214, 30 );
		this.snapTilesUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.snapTilesUnit.Name = "snapTilesUnit";
		this.snapTilesUnit.Size = new Size( 25, 15 );
		this.snapTilesUnit.TabIndex = 29;
		this.snapTilesUnit.Text = "マス";
		// 
		// keyPredictionCheck
		// 
		this.keyPredictionCheck.AutoSize = true;
		this.slidePanel.SetColumnSpan( this.keyPredictionCheck, 3 );
		this.keyPredictionCheck.Location = new Point( 3, 50 );
		this.keyPredictionCheck.Margin = new Padding( 3, 0, 3, 0 );
		this.keyPredictionCheck.Name = "keyPredictionCheck";
		this.keyPredictionCheck.Size = new Size( 180, 19 );
		this.keyPredictionCheck.TabIndex = 19;
		this.keyPredictionCheck.Text = "1 歩目は移動入力で先に動かす";
		this.toolTip.SetToolTip( this.keyPredictionCheck, "1 歩目の移動キーを押したら、座標が変わるのを待たずに押した方向へ 1 マス動かし始めます" );
		this.keyPredictionCheck.CheckedChanged +=  this.KeyPredictionCheck_CheckedChanged ;
		// 
		// keyDelayCaption
		// 
		this.keyDelayCaption.Anchor = AnchorStyles.Left;
		this.keyDelayCaption.AutoSize = true;
		this.keyDelayCaption.Location = new Point( 20, 74 );
		this.keyDelayCaption.Margin = new Padding( 20, 3, 3, 3 );
		this.keyDelayCaption.Name = "keyDelayCaption";
		this.keyDelayCaption.Size = new Size( 93, 15 );
		this.keyDelayCaption.TabIndex = 20;
		this.keyDelayCaption.Text = "1 歩目までの遅延";
		this.toolTip.SetToolTip( this.keyDelayCaption, "移動キーを押してからキャラクターが歩き出すまでの時間です" );
		// 
		// keyDelayBox
		// 
		this.keyDelayBox.Increment = new decimal( new int[] { 10, 0, 0, 0 } );
		this.keyDelayBox.Location = new Point( 131, 70 );
		this.keyDelayBox.Margin = new Padding( 3, 1, 3, 1 );
		this.keyDelayBox.Maximum = new decimal( new int[] { 500, 0, 0, 0 } );
		this.keyDelayBox.Name = "keyDelayBox";
		this.keyDelayBox.Size = new Size( 80, 23 );
		this.keyDelayBox.TabIndex = 21;
		this.keyDelayBox.TextAlign = HorizontalAlignment.Right;
		this.keyDelayBox.Value = new decimal( new int[] { 40, 0, 0, 0 } );
		this.keyDelayBox.ValueChanged +=  this.KeyDelayBox_ValueChanged ;
		// 
		// keyDelayUnit
		// 
		this.keyDelayUnit.Anchor = AnchorStyles.Left;
		this.keyDelayUnit.AutoSize = true;
		this.keyDelayUnit.Location = new Point( 214, 74 );
		this.keyDelayUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.keyDelayUnit.Name = "keyDelayUnit";
		this.keyDelayUnit.Size = new Size( 34, 15 );
		this.keyDelayUnit.TabIndex = 22;
		this.keyDelayUnit.Text = "ミリ秒";
		// 
		// keyContinueCheck
		// 
		this.keyContinueCheck.AutoSize = true;
		this.slidePanel.SetColumnSpan( this.keyContinueCheck, 3 );
		this.keyContinueCheck.Location = new Point( 20, 94 );
		this.keyContinueCheck.Margin = new Padding( 20, 0, 3, 0 );
		this.keyContinueCheck.Name = "keyContinueCheck";
		this.keyContinueCheck.Size = new Size( 203, 19 );
		this.keyContinueCheck.TabIndex = 31;
		this.keyContinueCheck.Text = "2 歩目以降も移動入力で先に動かす";
		this.toolTip.SetToolTip( this.keyContinueCheck, "2 歩目から、マスに着いたときに移動キーを押していれば、止めずにその方向の次のマスへ進み続けます" );
		this.keyContinueCheck.CheckedChanged +=  this.KeyContinueCheck_CheckedChanged ;
		// 
		// inputLagCaption
		// 
		this.inputLagCaption.Anchor = AnchorStyles.Left;
		this.inputLagCaption.AutoSize = true;
		this.inputLagCaption.Location = new Point( 37, 118 );
		this.inputLagCaption.Margin = new Padding( 37, 3, 3, 3 );
		this.inputLagCaption.Name = "inputLagCaption";
		this.inputLagCaption.Size = new Size( 88, 15 );
		this.inputLagCaption.TabIndex = 36;
		this.inputLagCaption.Text = "入力の反映遅れ";
		this.toolTip.SetToolTip( this.inputLagCaption, "マスに着いたとき、この時間だけ前に押していたキーの方向へ次の 1 歩を歩くとみなします。外れたときは座標の読み取りで進路を直します" );
		// 
		// inputLagBox
		// 
		this.inputLagBox.Location = new Point( 131, 114 );
		this.inputLagBox.Margin = new Padding( 3, 1, 3, 1 );
		this.inputLagBox.Maximum = new decimal( new int[] { 300, 0, 0, 0 } );
		this.inputLagBox.Name = "inputLagBox";
		this.inputLagBox.Size = new Size( 80, 23 );
		this.inputLagBox.TabIndex = 37;
		this.inputLagBox.TextAlign = HorizontalAlignment.Right;
		this.inputLagBox.Value = new decimal( new int[] { 120, 0, 0, 0 } );
		this.inputLagBox.ValueChanged +=  this.InputLagBox_ValueChanged ;
		// 
		// inputLagUnit
		// 
		this.inputLagUnit.Anchor = AnchorStyles.Left;
		this.inputLagUnit.AutoSize = true;
		this.inputLagUnit.Location = new Point( 214, 118 );
		this.inputLagUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.inputLagUnit.Name = "inputLagUnit";
		this.inputLagUnit.Size = new Size( 34, 15 );
		this.inputLagUnit.TabIndex = 38;
		this.inputLagUnit.Text = "ミリ秒";
		// 
		// keyRepredictCheck
		// 
		this.keyRepredictCheck.AutoSize = true;
		this.slidePanel.SetColumnSpan( this.keyRepredictCheck, 3 );
		this.keyRepredictCheck.Location = new Point( 20, 138 );
		this.keyRepredictCheck.Margin = new Padding( 20, 0, 3, 0 );
		this.keyRepredictCheck.Name = "keyRepredictCheck";
		this.keyRepredictCheck.Size = new Size( 223, 19 );
		this.keyRepredictCheck.TabIndex = 30;
		this.keyRepredictCheck.Text = "動いている途中のキー入力でも先に動かす";
		this.toolTip.SetToolTip( this.keyRepredictCheck, "座標の変化で動いている途中に移動キーを押したら、着いたマスから押した方向へ座標の変化を待たずに動かします" );
		this.keyRepredictCheck.CheckedChanged +=  this.KeyRepredictCheck_CheckedChanged ;
		// 
		// pixelCheckCheck
		// 
		this.pixelCheckCheck.AutoSize = true;
		this.slidePanel.SetColumnSpan( this.pixelCheckCheck, 3 );
		this.pixelCheckCheck.Location = new Point( 20, 157 );
		this.pixelCheckCheck.Margin = new Padding( 20, 0, 3, 0 );
		this.pixelCheckCheck.Name = "pixelCheckCheck";
		this.pixelCheckCheck.Size = new Size( 231, 19 );
		this.pixelCheckCheck.TabIndex = 32;
		this.pixelCheckCheck.Text = "座標欄の画素の変化で歩いたかを確かめる";
		this.toolTip.SetToolTip( this.pixelCheckCheck, "キー入力で先に動かしたあと、座標欄の画像が変わったかで本当に歩いたかを確かめ、変わらなければ戻します。オフなら歩いたとみなし、座標の認識で外れていたら戻します" );
		this.pixelCheckCheck.CheckedChanged +=  this.PixelCheckCheck_CheckedChanged ;
		// 
		// startCheckCaption
		// 
		this.startCheckCaption.Anchor = AnchorStyles.Left;
		this.startCheckCaption.AutoSize = true;
		this.startCheckCaption.Location = new Point( 37, 181 );
		this.startCheckCaption.Margin = new Padding( 37, 3, 3, 3 );
		this.startCheckCaption.Name = "startCheckCaption";
		this.startCheckCaption.Size = new Size( 74, 15 );
		this.startCheckCaption.TabIndex = 33;
		this.startCheckCaption.Text = "1 歩目の判定";
		this.toolTip.SetToolTip( this.startCheckCaption, "歩き始めに、キーを押してから座標の文字が変わったかを確かめるまでの時間です。変わっていなければ歩いていないとみなして戻します。短すぎると歩いているのに止まったと判定します" );
		// 
		// startCheckBox
		// 
		this.startCheckBox.Location = new Point( 131, 177 );
		this.startCheckBox.Margin = new Padding( 3, 1, 3, 1 );
		this.startCheckBox.Maximum = new decimal( new int[] { 500, 0, 0, 0 } );
		this.startCheckBox.Name = "startCheckBox";
		this.startCheckBox.Size = new Size( 80, 23 );
		this.startCheckBox.TabIndex = 34;
		this.startCheckBox.TextAlign = HorizontalAlignment.Right;
		this.startCheckBox.Value = new decimal( new int[] { 40, 0, 0, 0 } );
		this.startCheckBox.ValueChanged +=  this.StartCheckBox_ValueChanged ;
		// 
		// startCheckUnit
		// 
		this.startCheckUnit.Anchor = AnchorStyles.Left;
		this.startCheckUnit.AutoSize = true;
		this.startCheckUnit.Location = new Point( 214, 181 );
		this.startCheckUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.startCheckUnit.Name = "startCheckUnit";
		this.startCheckUnit.Size = new Size( 34, 15 );
		this.startCheckUnit.TabIndex = 35;
		this.startCheckUnit.Text = "ミリ秒";
		// 
		// skillPredictionCheck
		// 
		this.skillPredictionCheck.AutoSize = true;
		this.slidePanel.SetColumnSpan( this.skillPredictionCheck, 3 );
		this.skillPredictionCheck.Location = new Point( 20, 201 );
		this.skillPredictionCheck.Margin = new Padding( 20, 0, 3, 0 );
		this.skillPredictionCheck.Name = "skillPredictionCheck";
		this.skillPredictionCheck.Size = new Size( 140, 19 );
		this.skillPredictionCheck.TabIndex = 39;
		this.skillPredictionCheck.Text = "縮地のキーで先に動かす";
		this.toolTip.SetToolTip( this.skillPredictionCheck, "縮地(タイタンチャージ)のキーを押したら、最後に歩いた方向へ決まったマス数を、座標の変化を待たずに動かします" );
		this.skillPredictionCheck.CheckedChanged +=  this.SkillPredictionCheck_CheckedChanged ;
		// 
		// skillKeyCaption
		// 
		this.skillKeyCaption.Anchor = AnchorStyles.Left;
		this.skillKeyCaption.AutoSize = true;
		this.skillKeyCaption.Location = new Point( 37, 225 );
		this.skillKeyCaption.Margin = new Padding( 37, 3, 3, 3 );
		this.skillKeyCaption.Name = "skillKeyCaption";
		this.skillKeyCaption.Size = new Size( 58, 15 );
		this.skillKeyCaption.TabIndex = 0;
		this.skillKeyCaption.Text = "縮地のキー";
		this.toolTip.SetToolTip( this.skillKeyCaption, "入力欄を選んでキーを押すと、そのキーを縮地のキーにします。右クリックの「クリア」で外します" );
		// 
		// skillKeyBox
		// 
		this.skillKeyBox.ContextMenuStrip = this.skillKeyMenu;
		this.skillKeyBox.Location = new Point( 131, 221 );
		this.skillKeyBox.Margin = new Padding( 3, 1, 3, 1 );
		this.skillKeyBox.Name = "skillKeyBox";
		this.skillKeyBox.ReadOnly = true;
		this.skillKeyBox.ShortcutsEnabled = false;
		this.skillKeyBox.Size = new Size( 80, 23 );
		this.skillKeyBox.TabIndex = 1;
		this.skillKeyBox.TextAlign = HorizontalAlignment.Center;
		this.toolTip.SetToolTip( this.skillKeyBox, "入力欄を選んでキーを押すと、そのキーをスキルのキーにします。右クリックの「クリア」で外します" );
		this.skillKeyBox.KeyDown +=  this.SkillKeyBox_KeyDown ;
		// 
		// skillKeyMenu
		// 
		this.skillKeyMenu.Items.AddRange( new ToolStripItem[] { this.skillKeyClearItem } );
		this.skillKeyMenu.Name = "skillKeyMenu";
		this.skillKeyMenu.Size = new Size( 101, 26 );
		// 
		// skillKeyClearItem
		// 
		this.skillKeyClearItem.Name = "skillKeyClearItem";
		this.skillKeyClearItem.Size = new Size( 100, 22 );
		this.skillKeyClearItem.Text = "クリア";
		this.skillKeyClearItem.Click +=  this.SkillKeyClearItem_Click ;
		// 
		// skillTileMsCaption
		// 
		this.skillTileMsCaption.Anchor = AnchorStyles.Left;
		this.skillTileMsCaption.AutoSize = true;
		this.skillTileMsCaption.Location = new Point( 37, 250 );
		this.skillTileMsCaption.Margin = new Padding( 37, 3, 3, 3 );
		this.skillTileMsCaption.Name = "skillTileMsCaption";
		this.skillTileMsCaption.Size = new Size( 68, 15 );
		this.skillTileMsCaption.TabIndex = 5;
		this.skillTileMsCaption.Text = "1 マスの時間";
		this.toolTip.SetToolTip( this.skillTileMsCaption, "スキルで 1 マス進むのにかかる時間です" );
		// 
		// skillTileMsBox
		// 
		this.skillTileMsBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.skillTileMsBox.Location = new Point( 131, 246 );
		this.skillTileMsBox.Margin = new Padding( 3, 1, 3, 1 );
		this.skillTileMsBox.Maximum = new decimal( new int[] { 1000, 0, 0, 0 } );
		this.skillTileMsBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.skillTileMsBox.Name = "skillTileMsBox";
		this.skillTileMsBox.Size = new Size( 80, 23 );
		this.skillTileMsBox.TabIndex = 6;
		this.skillTileMsBox.TextAlign = HorizontalAlignment.Right;
		this.skillTileMsBox.Value = new decimal( new int[] { 65, 0, 0, 0 } );
		this.skillTileMsBox.ValueChanged +=  this.SkillTileMsBox_ValueChanged ;
		// 
		// skillTileMsUnit
		// 
		this.skillTileMsUnit.Anchor = AnchorStyles.Left;
		this.skillTileMsUnit.AutoSize = true;
		this.skillTileMsUnit.Location = new Point( 214, 250 );
		this.skillTileMsUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.skillTileMsUnit.Name = "skillTileMsUnit";
		this.skillTileMsUnit.Size = new Size( 34, 15 );
		this.skillTileMsUnit.TabIndex = 7;
		this.skillTileMsUnit.Text = "ミリ秒";
		// 
		// nameGroup
		// 
		this.nameGroup.AutoSize = true;
		this.nameGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.nameGroup.Controls.Add( this.namePanel );
		this.nameGroup.Dock = DockStyle.Fill;
		this.nameGroup.Location = new Point( 3, 181 );
		this.nameGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.nameGroup.Name = "nameGroup";
		this.nameGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.nameGroup.Size = new Size( 323, 101 );
		this.nameGroup.TabIndex = 3;
		this.nameGroup.TabStop = false;
		this.nameGroup.Text = "マップ名";
		// 
		// namePanel
		// 
		this.namePanel.AutoSize = true;
		this.namePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.namePanel.ColumnCount = 3;
		this.namePanel.ColumnStyles.Add( new ColumnStyle() );
		this.namePanel.ColumnStyles.Add( new ColumnStyle() );
		this.namePanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.namePanel.Controls.Add( this.nameRefreshCaption, 0, 0 );
		this.namePanel.Controls.Add( this.nameRefreshBox, 1, 0 );
		this.namePanel.Controls.Add( this.nameRefreshUnit, 2, 0 );
		this.namePanel.Controls.Add( this.nameConfirmCaption, 0, 1 );
		this.namePanel.Controls.Add( this.nameConfirmBox, 1, 1 );
		this.namePanel.Controls.Add( this.nameConfirmUnit, 2, 1 );
		this.namePanel.Controls.Add( this.nameHoldCaption, 0, 2 );
		this.namePanel.Controls.Add( this.nameHoldBox, 1, 2 );
		this.namePanel.Controls.Add( this.nameHoldUnit, 2, 2 );
		this.namePanel.Dock = DockStyle.Fill;
		this.namePanel.Location = new Point( 8, 20 );
		this.namePanel.Name = "namePanel";
		this.namePanel.RowCount = 4;
		this.namePanel.RowStyles.Add( new RowStyle() );
		this.namePanel.RowStyles.Add( new RowStyle() );
		this.namePanel.RowStyles.Add( new RowStyle() );
		this.namePanel.RowStyles.Add( new RowStyle() );
		this.namePanel.Size = new Size( 307, 75 );
		this.namePanel.TabIndex = 0;
		// 
		// nameRefreshCaption
		// 
		this.nameRefreshCaption.Anchor = AnchorStyles.Left;
		this.nameRefreshCaption.AutoSize = true;
		this.nameRefreshCaption.Location = new Point( 3, 5 );
		this.nameRefreshCaption.Name = "nameRefreshCaption";
		this.nameRefreshCaption.Size = new Size( 76, 15 );
		this.nameRefreshCaption.TabIndex = 0;
		this.nameRefreshCaption.Text = "読み直す間隔";
		this.toolTip.SetToolTip( this.nameRefreshCaption, "マップ名が確定した後、この間隔で読み直します。確定前は毎回読みます" );
		// 
		// nameRefreshBox
		// 
		this.nameRefreshBox.DecimalPlaces = 1;
		this.nameRefreshBox.Increment = new decimal( new int[] { 1, 0, 0, 65536 } );
		this.nameRefreshBox.Location = new Point( 110, 1 );
		this.nameRefreshBox.Margin = new Padding( 3, 1, 3, 1 );
		this.nameRefreshBox.Maximum = new decimal( new int[] { 60, 0, 0, 0 } );
		this.nameRefreshBox.Minimum = new decimal( new int[] { 1, 0, 0, 65536 } );
		this.nameRefreshBox.Name = "nameRefreshBox";
		this.nameRefreshBox.Size = new Size( 80, 23 );
		this.nameRefreshBox.TabIndex = 1;
		this.nameRefreshBox.TextAlign = HorizontalAlignment.Right;
		this.nameRefreshBox.Value = new decimal( new int[] { 1, 0, 0, 0 } );
		this.nameRefreshBox.ValueChanged +=  this.NameSettingBox_ValueChanged ;
		// 
		// nameRefreshUnit
		// 
		this.nameRefreshUnit.Anchor = AnchorStyles.Left;
		this.nameRefreshUnit.AutoSize = true;
		this.nameRefreshUnit.Location = new Point( 193, 5 );
		this.nameRefreshUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.nameRefreshUnit.Name = "nameRefreshUnit";
		this.nameRefreshUnit.Size = new Size( 19, 15 );
		this.nameRefreshUnit.TabIndex = 2;
		this.nameRefreshUnit.Text = "秒";
		// 
		// nameConfirmCaption
		// 
		this.nameConfirmCaption.Anchor = AnchorStyles.Left;
		this.nameConfirmCaption.AutoSize = true;
		this.nameConfirmCaption.Location = new Point( 3, 30 );
		this.nameConfirmCaption.Name = "nameConfirmCaption";
		this.nameConfirmCaption.Size = new Size( 98, 15 );
		this.nameConfirmCaption.TabIndex = 3;
		this.nameConfirmCaption.Text = "確定に必要な回数";
		this.toolTip.SetToolTip( this.nameConfirmCaption, "同じ結果がこの回数続いたらマップ名を確定します" );
		// 
		// nameConfirmBox
		// 
		this.nameConfirmBox.Location = new Point( 110, 26 );
		this.nameConfirmBox.Margin = new Padding( 3, 1, 3, 1 );
		this.nameConfirmBox.Maximum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.nameConfirmBox.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
		this.nameConfirmBox.Name = "nameConfirmBox";
		this.nameConfirmBox.Size = new Size( 80, 23 );
		this.nameConfirmBox.TabIndex = 4;
		this.nameConfirmBox.TextAlign = HorizontalAlignment.Right;
		this.nameConfirmBox.Value = new decimal( new int[] { 2, 0, 0, 0 } );
		this.nameConfirmBox.ValueChanged +=  this.NameSettingBox_ValueChanged ;
		// 
		// nameConfirmUnit
		// 
		this.nameConfirmUnit.Anchor = AnchorStyles.Left;
		this.nameConfirmUnit.AutoSize = true;
		this.nameConfirmUnit.Location = new Point( 193, 30 );
		this.nameConfirmUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.nameConfirmUnit.Name = "nameConfirmUnit";
		this.nameConfirmUnit.Size = new Size( 19, 15 );
		this.nameConfirmUnit.TabIndex = 5;
		this.nameConfirmUnit.Text = "回";
		// 
		// nameHoldCaption
		// 
		this.nameHoldCaption.Anchor = AnchorStyles.Left;
		this.nameHoldCaption.AutoSize = true;
		this.nameHoldCaption.Location = new Point( 3, 55 );
		this.nameHoldCaption.Name = "nameHoldCaption";
		this.nameHoldCaption.Size = new Size( 101, 15 );
		this.nameHoldCaption.TabIndex = 6;
		this.nameHoldCaption.Text = "読めないときの保持";
		this.toolTip.SetToolTip( this.nameHoldCaption, "読めない状態がこれを超えると灰色表示にし、オーバーレイを消します" );
		// 
		// nameHoldBox
		// 
		this.nameHoldBox.DecimalPlaces = 1;
		this.nameHoldBox.Increment = new decimal( new int[] { 5, 0, 0, 65536 } );
		this.nameHoldBox.Location = new Point( 110, 51 );
		this.nameHoldBox.Margin = new Padding( 3, 1, 3, 1 );
		this.nameHoldBox.Maximum = new decimal( new int[] { 60, 0, 0, 0 } );
		this.nameHoldBox.Minimum = new decimal( new int[] { 5, 0, 0, 65536 } );
		this.nameHoldBox.Name = "nameHoldBox";
		this.nameHoldBox.Size = new Size( 80, 23 );
		this.nameHoldBox.TabIndex = 7;
		this.nameHoldBox.TextAlign = HorizontalAlignment.Right;
		this.nameHoldBox.Value = new decimal( new int[] { 3, 0, 0, 0 } );
		this.nameHoldBox.ValueChanged +=  this.NameSettingBox_ValueChanged ;
		// 
		// nameHoldUnit
		// 
		this.nameHoldUnit.Anchor = AnchorStyles.Left;
		this.nameHoldUnit.AutoSize = true;
		this.nameHoldUnit.Location = new Point( 193, 55 );
		this.nameHoldUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.nameHoldUnit.Name = "nameHoldUnit";
		this.nameHoldUnit.Size = new Size( 19, 15 );
		this.nameHoldUnit.TabIndex = 8;
		this.nameHoldUnit.Text = "秒";
		// 
		// drawingGroup
		// 
		this.drawingGroup.AutoSize = true;
		this.drawingGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.drawingGroup.Controls.Add( this.drawingPanel );
		this.drawingGroup.Dock = DockStyle.Fill;
		this.drawingGroup.Location = new Point( 3, 293 );
		this.drawingGroup.Name = "drawingGroup";
		this.drawingGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.drawingGroup.Size = new Size( 323, 45 );
		this.drawingGroup.TabIndex = 4;
		this.drawingGroup.TabStop = false;
		this.drawingGroup.Text = "オーバーレイの描画";
		// 
		// drawingPanel
		// 
		this.drawingPanel.AutoSize = true;
		this.drawingPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.drawingPanel.Controls.Add( this.showChunksCheck );
		this.drawingPanel.Dock = DockStyle.Fill;
		this.drawingPanel.Location = new Point( 8, 20 );
		this.drawingPanel.Name = "drawingPanel";
		this.drawingPanel.Size = new Size( 307, 19 );
		this.drawingPanel.TabIndex = 0;
		this.drawingPanel.WrapContents = false;
		// 
		// bottomPanel
		// 
		this.bottomPanel.AutoSize = true;
		this.bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.bottomPanel.ColumnCount = 3;
		this.rootPanel.SetColumnSpan( this.bottomPanel, 2 );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.Controls.Add( this.advancedCheck, 0, 0 );
		this.bottomPanel.Controls.Add( this.infoButton, 1, 0 );
		this.bottomPanel.Controls.Add( this.closeButton, 2, 0 );
		this.bottomPanel.Dock = DockStyle.Fill;
		this.bottomPanel.Location = new Point( 0, 709 );
		this.bottomPanel.Margin = new Padding( 0 );
		this.bottomPanel.Name = "bottomPanel";
		this.bottomPanel.RowCount = 1;
		this.bottomPanel.RowStyles.Add( new RowStyle() );
		this.bottomPanel.Size = new Size( 649, 31 );
		this.bottomPanel.TabIndex = 2;
		// 
		// advancedCheck
		// 
		this.advancedCheck.Anchor = AnchorStyles.Left;
		this.advancedCheck.AutoSize = true;
		this.advancedCheck.Location = new Point( 3, 6 );
		this.advancedCheck.Name = "advancedCheck";
		this.advancedCheck.Size = new Size( 107, 19 );
		this.advancedCheck.TabIndex = 0;
		this.advancedCheck.Text = "詳細設定を表示";
		this.advancedCheck.UseVisualStyleBackColor = true;
		this.advancedCheck.CheckedChanged +=  this.AdvancedCheck_CheckedChanged ;
		// 
		// infoButton
		// 
		this.infoButton.Anchor = AnchorStyles.Right;
		this.infoButton.AutoSize = true;
		this.infoButton.Location = new Point( 490, 3 );
		this.infoButton.Name = "infoButton";
		this.infoButton.Size = new Size( 75, 25 );
		this.infoButton.TabIndex = 2;
		this.infoButton.Text = "情報...";
		this.infoButton.UseVisualStyleBackColor = true;
		this.infoButton.Click +=  this.InfoButton_Click ;
		// 
		// closeButton
		// 
		this.closeButton.Anchor = AnchorStyles.Right;
		this.closeButton.AutoSize = true;
		this.closeButton.Location = new Point( 571, 3 );
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new Size( 75, 25 );
		this.closeButton.TabIndex = 1;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click +=  this.CloseButton_Click ;
		// 
		// updateButton
		// 
		this.updateButton.Anchor = AnchorStyles.Right;
		this.updateButton.AutoSize = true;
		this.updateButton.Location = new Point( 153, 33 );
		this.updateButton.Name = "updateButton";
		this.updateButton.Size = new Size( 85, 25 );
		this.updateButton.TabIndex = 1;
		this.updateButton.Text = "更新を確認...";
		this.updateButton.UseVisualStyleBackColor = true;
		this.updateButton.Click +=  this.UpdateButton_Click ;
		// 
		// appVersionCaption
		// 
		this.appVersionCaption.Anchor = AnchorStyles.Left;
		this.appVersionCaption.AutoSize = true;
		this.appVersionCaption.Location = new Point( 3, 0 );
		this.appVersionCaption.Name = "appVersionCaption";
		this.appVersionCaption.Size = new Size( 31, 15 );
		this.appVersionCaption.TabIndex = 2;
		this.appVersionCaption.Text = "本体";
		// 
		// appVersionValue
		// 
		this.appVersionValue.Anchor = AnchorStyles.Left;
		this.appVersionValue.AutoSize = true;
		this.updatePanel.SetColumnSpan( this.appVersionValue, 2 );
		this.appVersionValue.Location = new Point( 66, 0 );
		this.appVersionValue.Name = "appVersionValue";
		this.appVersionValue.Size = new Size( 24, 15 );
		this.appVersionValue.TabIndex = 3;
		this.appVersionValue.Text = "：-";
		// 
		// dataVersionCaption
		// 
		this.dataVersionCaption.Anchor = AnchorStyles.Left;
		this.dataVersionCaption.AutoSize = true;
		this.dataVersionCaption.Location = new Point( 3, 15 );
		this.dataVersionCaption.Name = "dataVersionCaption";
		this.dataVersionCaption.Size = new Size( 57, 15 );
		this.dataVersionCaption.TabIndex = 4;
		this.dataVersionCaption.Text = "マップ情報";
		// 
		// dataVersionValue
		// 
		this.dataVersionValue.Anchor = AnchorStyles.Left;
		this.dataVersionValue.AutoSize = true;
		this.updatePanel.SetColumnSpan( this.dataVersionValue, 2 );
		this.dataVersionValue.Location = new Point( 66, 15 );
		this.dataVersionValue.Name = "dataVersionValue";
		this.dataVersionValue.Size = new Size( 24, 15 );
		this.dataVersionValue.TabIndex = 5;
		this.dataVersionValue.Text = "：-";
		// 
		// autoUpdateCheck
		// 
		this.autoUpdateCheck.AutoSize = true;
		this.updatePanel.SetColumnSpan( this.autoUpdateCheck, 2 );
		this.autoUpdateCheck.Dock = DockStyle.Fill;
		this.autoUpdateCheck.Location = new Point( 3, 33 );
		this.autoUpdateCheck.Name = "autoUpdateCheck";
		this.autoUpdateCheck.Size = new Size( 144, 25 );
		this.autoUpdateCheck.TabIndex = 0;
		this.autoUpdateCheck.Text = "起動時に更新を確認";
		this.toolTip.SetToolTip( this.autoUpdateCheck, "起動時に本体とマップ情報の新しい版を確認します" );
		this.autoUpdateCheck.UseVisualStyleBackColor = true;
		this.autoUpdateCheck.CheckedChanged +=  this.AutoUpdateCheck_CheckedChanged ;
		// 
		// updateGroup
		// 
		this.updateGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.updateGroup.Controls.Add( this.updatePanel );
		this.updateGroup.Location = new Point( 3, 547 );
		this.updateGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.updateGroup.Name = "updateGroup";
		this.updateGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.updateGroup.Size = new Size( 257, 92 );
		this.updateGroup.TabIndex = 3;
		this.updateGroup.TabStop = false;
		this.updateGroup.Text = "バージョンと更新";
		// 
		// updatePanel
		// 
		this.updatePanel.AutoSize = true;
		this.updatePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.updatePanel.ColumnCount = 3;
		this.updatePanel.ColumnStyles.Add( new ColumnStyle() );
		this.updatePanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.updatePanel.ColumnStyles.Add( new ColumnStyle() );
		this.updatePanel.Controls.Add( this.appVersionCaption, 0, 0 );
		this.updatePanel.Controls.Add( this.appVersionValue, 1, 0 );
		this.updatePanel.Controls.Add( this.dataVersionCaption, 0, 1 );
		this.updatePanel.Controls.Add( this.dataVersionValue, 1, 1 );
		this.updatePanel.Controls.Add( this.autoUpdateCheck, 0, 2 );
		this.updatePanel.Controls.Add( this.updateButton, 2, 2 );
		this.updatePanel.Dock = DockStyle.Top;
		this.updatePanel.Location = new Point( 8, 20 );
		this.updatePanel.Name = "updatePanel";
		this.updatePanel.RowCount = 3;
		this.updatePanel.RowStyles.Add( new RowStyle() );
		this.updatePanel.RowStyles.Add( new RowStyle() );
		this.updatePanel.RowStyles.Add( new RowStyle() );
		this.updatePanel.Size = new Size( 241, 61 );
		this.updatePanel.TabIndex = 0;
		// 
		// generalGroup
		// 
		this.generalGroup.AutoSize = true;
		this.generalGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.generalGroup.Controls.Add( this.generalPanel );
		this.generalGroup.Dock = DockStyle.Top;
		this.generalGroup.Location = new Point( 3, 650 );
		this.generalGroup.Margin = new Padding( 3, 3, 3, 8 );
		this.generalGroup.Name = "generalGroup";
		this.generalGroup.Padding = new Padding( 8, 4, 8, 6 );
		this.generalGroup.Size = new Size( 308, 51 );
		this.generalGroup.TabIndex = 4;
		this.generalGroup.TabStop = false;
		this.generalGroup.Text = "全般";
		// 
		// generalPanel
		// 
		this.generalPanel.AutoSize = true;
		this.generalPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.generalPanel.Controls.Add( this.stayInTrayCheck );
		this.generalPanel.Dock = DockStyle.Fill;
		this.generalPanel.Location = new Point( 8, 20 );
		this.generalPanel.Name = "generalPanel";
		this.generalPanel.Size = new Size( 292, 25 );
		this.generalPanel.TabIndex = 0;
		this.generalPanel.WrapContents = false;
		// 
		// stayInTrayCheck
		// 
		this.stayInTrayCheck.AutoSize = true;
		this.stayInTrayCheck.Location = new Point( 3, 3 );
		this.stayInTrayCheck.Name = "stayInTrayCheck";
		this.stayInTrayCheck.Size = new Size( 112, 19 );
		this.stayInTrayCheck.TabIndex = 0;
		this.stayInTrayCheck.Text = "タスクトレイに常駐";
		this.toolTip.SetToolTip( this.stayInTrayCheck, "メイン画面を閉じてもタスクトレイに残します\n終了はタスクトレイのアイコンの右クリックメニューから行います" );
		this.stayInTrayCheck.UseVisualStyleBackColor = true;
		this.stayInTrayCheck.CheckedChanged +=  this.StayInTrayCheck_CheckedChanged ;
		// 
		// basicPanel
		// 
		this.basicPanel.AutoSize = true;
		this.basicPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.basicPanel.ColumnCount = 1;
		this.basicPanel.ColumnStyles.Add( new ColumnStyle() );
		this.basicPanel.Controls.Add( this.ocrGroup, 0, 0 );
		this.basicPanel.Controls.Add( this.overlayGroup, 0, 1 );
		this.basicPanel.Controls.Add( this.motionGroup, 0, 2 );
		this.basicPanel.Controls.Add( this.updateGroup, 0, 3 );
		this.basicPanel.Controls.Add( this.generalGroup, 0, 4 );
		this.basicPanel.Dock = DockStyle.Fill;
		this.basicPanel.Location = new Point( 0, 0 );
		this.basicPanel.Margin = new Padding( 0 );
		this.basicPanel.Name = "basicPanel";
		this.basicPanel.RowCount = 5;
		this.basicPanel.RowStyles.Add( new RowStyle() );
		this.basicPanel.RowStyles.Add( new RowStyle() );
		this.basicPanel.RowStyles.Add( new RowStyle() );
		this.basicPanel.RowStyles.Add( new RowStyle() );
		this.basicPanel.RowStyles.Add( new RowStyle() );
		this.basicPanel.Size = new Size( 314, 709 );
		this.basicPanel.TabIndex = 0;
		// 
		// rootPanel
		// 
		this.rootPanel.AutoSize = true;
		this.rootPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.rootPanel.ColumnCount = 2;
		this.rootPanel.ColumnStyles.Add( new ColumnStyle() );
		this.rootPanel.ColumnStyles.Add( new ColumnStyle() );
		this.rootPanel.Controls.Add( this.scrollPanel, 0, 0 );
		this.rootPanel.Controls.Add( this.bottomPanel, 0, 1 );
		this.rootPanel.Location = new Point( 12, 12 );
		this.rootPanel.Margin = new Padding( 0 );
		this.rootPanel.Name = "rootPanel";
		this.rootPanel.RowCount = 2;
		this.rootPanel.RowStyles.Add( new RowStyle() );
		this.rootPanel.RowStyles.Add( new RowStyle() );
		this.rootPanel.Size = new Size( 649, 740 );
		this.rootPanel.TabIndex = 0;
		// 
		// scrollPanel
		// 
		this.scrollPanel.AutoSize = true;
		this.scrollPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.rootPanel.SetColumnSpan( this.scrollPanel, 2 );
		this.scrollPanel.Controls.Add( this.contentPanel );
		this.scrollPanel.Location = new Point( 0, 0 );
		this.scrollPanel.Margin = new Padding( 0 );
		this.scrollPanel.Name = "scrollPanel";
		this.scrollPanel.Size = new Size( 649, 709 );
		this.scrollPanel.TabIndex = 0;
		// 
		// contentPanel
		// 
		this.contentPanel.AutoSize = true;
		this.contentPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.contentPanel.ColumnCount = 2;
		this.contentPanel.ColumnStyles.Add( new ColumnStyle() );
		this.contentPanel.ColumnStyles.Add( new ColumnStyle() );
		this.contentPanel.Controls.Add( this.basicPanel, 0, 0 );
		this.contentPanel.Controls.Add( this.advancedPanel, 1, 0 );
		this.contentPanel.Location = new Point( 0, 0 );
		this.contentPanel.Margin = new Padding( 0 );
		this.contentPanel.Name = "contentPanel";
		this.contentPanel.RowCount = 1;
		this.contentPanel.RowStyles.Add( new RowStyle() );
		this.contentPanel.Size = new Size( 649, 709 );
		this.contentPanel.TabIndex = 0;
		// 
		// SettingsForm
		// 
		this.AutoScaleDimensions = new SizeF( 96F, 96F );
		this.AutoScaleMode = AutoScaleMode.Dpi;
		this.AutoSize = true;
		this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.CancelButton = this.closeButton;
		this.ClientSize = new Size( 678, 804 );
		this.Controls.Add( this.rootPanel );
		this.Font = new Font( "Yu Gothic UI", 9F );
		this.FormBorderStyle = FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "SettingsForm";
		this.Padding = new Padding( 0, 0, 12, 12 );
		this.ShowInTaskbar = false;
		this.StartPosition = FormStartPosition.Manual;
		this.Text = "設定";
		this.regionGroup.ResumeLayout( false );
		this.regionGroup.PerformLayout();
		this.regionPanel.ResumeLayout( false );
		this.regionPanel.PerformLayout();
		this.ocrGroup.ResumeLayout( false );
		this.ocrGroup.PerformLayout();
		this.ocrPanel.ResumeLayout( false );
		this.ocrPanel.PerformLayout();
		this.overlayGroup.ResumeLayout( false );
		this.overlayGroup.PerformLayout();
		this.overlayPanel.ResumeLayout( false );
		this.overlayPanel.PerformLayout();
		this.overlayOptionPanel.ResumeLayout( false );
		this.overlayOptionPanel.PerformLayout();
		this.layersPanel.ResumeLayout( false );
		this.layersPanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.overallOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.impassableEdgeOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.monsterBlockOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.mapMoveOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.specialOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.gridOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.playerOpacityBox ).EndInit();
		this.motionGroup.ResumeLayout( false );
		this.motionGroup.PerformLayout();
		this.motionPanel.ResumeLayout( false );
		this.motionPanel.PerformLayout();
		this.moveSpeedPanel.ResumeLayout( false );
		this.moveSpeedPanel.PerformLayout();
		this.jumpDelayPanel.ResumeLayout( false );
		this.jumpDelayPanel.PerformLayout();
		this.jumpDelayExtraPanel.ResumeLayout( false );
		this.jumpDelayExtraPanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.jumpDelayExtraBox ).EndInit();
		this.jumpDelayValuePanel.ResumeLayout( false );
		this.jumpDelayValuePanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.jumpDelayBox ).EndInit();
		this.advancedPanel.ResumeLayout( false );
		this.advancedPanel.PerformLayout();
		this.speedGroup.ResumeLayout( false );
		this.speedGroup.PerformLayout();
		this.speedPanel.ResumeLayout( false );
		this.speedPanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.intervalBox ).EndInit();
		this.slideGroup.ResumeLayout( false );
		this.slideGroup.PerformLayout();
		this.slidePanel.ResumeLayout( false );
		this.slidePanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.overlaySlideBox ).EndInit();
		this.overlaySlideOptionPanel.ResumeLayout( false );
		this.overlaySlideOptionPanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.snapTilesBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.keyDelayBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.inputLagBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.startCheckBox ).EndInit();
		this.skillKeyMenu.ResumeLayout( false );
		( (System.ComponentModel.ISupportInitialize)this.skillTileMsBox ).EndInit();
		this.nameGroup.ResumeLayout( false );
		this.nameGroup.PerformLayout();
		this.namePanel.ResumeLayout( false );
		this.namePanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.nameRefreshBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.nameConfirmBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.nameHoldBox ).EndInit();
		this.drawingGroup.ResumeLayout( false );
		this.drawingGroup.PerformLayout();
		this.drawingPanel.ResumeLayout( false );
		this.drawingPanel.PerformLayout();
		this.bottomPanel.ResumeLayout( false );
		this.bottomPanel.PerformLayout();
		this.updateGroup.ResumeLayout( false );
		this.updateGroup.PerformLayout();
		this.updatePanel.ResumeLayout( false );
		this.updatePanel.PerformLayout();
		this.generalGroup.ResumeLayout( false );
		this.generalGroup.PerformLayout();
		this.generalPanel.ResumeLayout( false );
		this.generalPanel.PerformLayout();
		this.basicPanel.ResumeLayout( false );
		this.basicPanel.PerformLayout();
		this.rootPanel.ResumeLayout( false );
		this.rootPanel.PerformLayout();
		this.scrollPanel.ResumeLayout( false );
		this.scrollPanel.PerformLayout();
		this.contentPanel.ResumeLayout( false );
		this.contentPanel.PerformLayout();
		this.ResumeLayout( false );
		this.PerformLayout();
	}

	#endregion

	private GroupBox regionGroup;
    private TableLayoutPanel regionPanel;
    private Label nameRegionCaption;
    private Label nameRegionValue;
    private Button nameRegionButton;
    private Label coordinateRegionCaption;
    private Label coordinateRegionValue;
    private Button coordinateRegionButton;
    private GroupBox ocrGroup;
    private TableLayoutPanel ocrPanel;
    private Label languageCaption;
    private ComboBox languageBox;
    private Label backendCaption;
    private ComboBox backendBox;
    private CheckBox glyphCacheCheck;
    private GroupBox speedGroup;
    private TableLayoutPanel speedPanel;
    private GroupBox slideGroup;
    private TableLayoutPanel slidePanel;
    private Label skillKeyCaption;
    private TextBox skillKeyBox;
    private ContextMenuStrip skillKeyMenu;
    private ToolStripMenuItem skillKeyClearItem;
    private Label skillTileMsCaption;
    private NumericUpDown skillTileMsBox;
    private Label skillTileMsUnit;
    private Label intervalCaption;
    private NumericUpDown intervalBox;
    private FlowLayoutPanel overlayOptionPanel;
    private CheckBox antiAliasCheck;
    private Button customTilesButton;
    private CheckBox showChunksCheck;
    private GroupBox drawingGroup;
    private FlowLayoutPanel drawingPanel;
    private Label intervalUnit;
    private FlowLayoutPanel moveSpeedPanel;
    private Label moveSpeedCaption;
    private ComboBox moveSpeedBox;
    private Label overlaySlideCaption;
    private NumericUpDown overlaySlideBox;
    private Label overlaySlideUnit;
    private FlowLayoutPanel overlaySlideOptionPanel;
    private CheckBox overlaySlideManualCheck;
    private CheckBox keyPredictionCheck;
    private CheckBox keyRepredictCheck;
    private CheckBox keyContinueCheck;
    private RadioButton jumpRadio;
    private RadioButton slideRadio;
    private FlowLayoutPanel jumpDelayPanel;
    private Label jumpDelayCaption;
    private NumericUpDown jumpDelayBox;
    private Label jumpDelayUnit;
    private FlowLayoutPanel jumpDelayValuePanel;
    private RadioButton jumpDelayValueCaption;
    private Label jumpDelayTotal;
    private FlowLayoutPanel jumpDelayExtraPanel;
    private RadioButton jumpDelayExtraCaption;
    private NumericUpDown jumpDelayExtraBox;
    private Label jumpDelayExtraUnit;
    private CheckBox skillPredictionCheck;
    private CheckBox pixelCheckCheck;
    private Label keyDelayCaption;
    private NumericUpDown keyDelayBox;
    private Label keyDelayUnit;
    private Label startCheckCaption;
    private NumericUpDown startCheckBox;
    private Label startCheckUnit;
    private Label inputLagCaption;
    private NumericUpDown inputLagBox;
    private Label inputLagUnit;
    private Label snapTilesCaption;
    private NumericUpDown snapTilesBox;
    private Label snapTilesUnit;
    private Label threadsCaption;
    private ComboBox threadsBox;
    private GroupBox nameGroup;
    private TableLayoutPanel namePanel;
    private Label nameRefreshCaption;
    private NumericUpDown nameRefreshBox;
    private Label nameRefreshUnit;
    private Label nameConfirmCaption;
    private NumericUpDown nameConfirmBox;
    private Label nameConfirmUnit;
    private Label nameHoldCaption;
    private NumericUpDown nameHoldBox;
    private Label nameHoldUnit;
    private GroupBox overlayGroup;
    private GroupBox motionGroup;
    private TableLayoutPanel motionPanel;
    private TableLayoutPanel overlayPanel;
    private TableLayoutPanel layersPanel;
    private Button impassableEdgeColorButton;
    private Label impassableEdgeColorLabel;
    private Button monsterBlockColorButton;
    private Label monsterBlockColorLabel;
    private Button mapMoveColorButton;
    private Label mapMoveColorLabel;
    private Button specialColorButton;
    private Label specialColorLabel;
    private Button gridColorButton;
    private Label gridColorLabel;
    private Label overallOpacityLabel;
    private NumericUpDown overallOpacityBox;
    private Label overallOpacityUnit;
    private Button playerColorButton;
    private Label playerColorLabel;
    private CheckBox impassableEdgeOpacityCheck;
    private NumericUpDown impassableEdgeOpacityBox;
    private Label impassableEdgeOpacityUnit;
    private CheckBox monsterBlockOpacityCheck;
    private NumericUpDown monsterBlockOpacityBox;
    private Label monsterBlockOpacityUnit;
    private CheckBox mapMoveOpacityCheck;
    private NumericUpDown mapMoveOpacityBox;
    private Label mapMoveOpacityUnit;
    private CheckBox specialOpacityCheck;
    private NumericUpDown specialOpacityBox;
    private Label specialOpacityUnit;
    private CheckBox gridOpacityCheck;
    private NumericUpDown gridOpacityBox;
    private Label gridOpacityUnit;
    private CheckBox playerOpacityCheck;
    private NumericUpDown playerOpacityBox;
    private Label playerOpacityUnit;
    private Button resetColorsButton;
    private ToolTip toolTip;
    private TableLayoutPanel advancedPanel;
    private TableLayoutPanel bottomPanel;
    private CheckBox advancedCheck;
    private Button closeButton;
    private Button updateButton;
    private Label appVersionCaption;
    private Label appVersionValue;
    private Label dataVersionCaption;
    private Label dataVersionValue;
    private CheckBox autoUpdateCheck;
    private GroupBox updateGroup;
    private GroupBox generalGroup;
    private FlowLayoutPanel generalPanel;
    private CheckBox stayInTrayCheck;
    private TableLayoutPanel updatePanel;
	private TableLayoutPanel basicPanel;
	private TableLayoutPanel rootPanel;
	private Panel scrollPanel;
	private TableLayoutPanel contentPanel;
	private Button infoButton;
}
