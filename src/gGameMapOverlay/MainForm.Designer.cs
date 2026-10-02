namespace gGameMapOverlay;

partial class MainForm
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
		this.bodyPanel = new TableLayoutPanel();
		this.headerPanel = new TableLayoutPanel();
		this.statusLabel = new Label();
		this.settingsButton = new Button();
		this.targetCaption = new Label();
		this.windowSelect = new ComboBox();
		this.preview = new PictureBox();
		this.nameCaption = new Label();
		this.namePanel = new FlowLayoutPanel();
		this.nameValue = new Label();
		this.mapSelect = new ComboBox();
		this.coordinateCaption = new Label();
		this.coordinateValue = new Label();
		this.rawLabel = new Label();
		this.layersPanel = new TableLayoutPanel();
		this.impassableEdgeSwatch = new Panel();
		this.impassableEdgeBox = new CheckBox();
		this.monsterBlockSwatch = new Panel();
		this.monsterBlockBox = new CheckBox();
		this.mapMoveSwatch = new Panel();
		this.mapMoveBox = new CheckBox();
		this.specialSwatch = new Panel();
		this.specialBox = new CheckBox();
		this.gridSwatch = new Panel();
		this.gridBox = new CheckBox();
		this.playerSwatch = new Panel();
		this.playerBox = new CheckBox();
		this.controlsPanel = new TableLayoutPanel();
		this.toggleButton = new Button();
		this.updateStatusLabel = new Label();
		this.closeButton = new Button();
		this.debugPanel = new FlowLayoutPanel();
		this.imageButton = new Button();
		this.liveButton = new Button();
		this.trayIcon = new NotifyIcon( this.components );
		this.trayMenu = new ContextMenuStrip( this.components );
		this.trayShowItem = new ToolStripMenuItem();
		this.trayMenuSeparator = new ToolStripSeparator();
		this.trayExitItem = new ToolStripMenuItem();
		this.timer = new System.Windows.Forms.Timer( this.components );
		this.bodyPanel.SuspendLayout();
		this.headerPanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.preview ).BeginInit();
		this.namePanel.SuspendLayout();
		this.layersPanel.SuspendLayout();
		this.controlsPanel.SuspendLayout();
		this.debugPanel.SuspendLayout();
		this.trayMenu.SuspendLayout();
		this.SuspendLayout();
		// 
		// bodyPanel
		// 
		this.bodyPanel.AutoSize = true;
		this.bodyPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.bodyPanel.ColumnCount = 2;
		this.bodyPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bodyPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bodyPanel.Controls.Add( this.headerPanel, 0, 0 );
		this.bodyPanel.Controls.Add( this.targetCaption, 0, 1 );
		this.bodyPanel.Controls.Add( this.windowSelect, 1, 1 );
		this.bodyPanel.Controls.Add( this.preview, 0, 2 );
		this.bodyPanel.Controls.Add( this.nameCaption, 0, 3 );
		this.bodyPanel.Controls.Add( this.namePanel, 1, 3 );
		this.bodyPanel.Controls.Add( this.coordinateCaption, 0, 4 );
		this.bodyPanel.Controls.Add( this.coordinateValue, 1, 4 );
		this.bodyPanel.Controls.Add( this.rawLabel, 0, 5 );
		this.bodyPanel.Controls.Add( this.layersPanel, 0, 6 );
		this.bodyPanel.Controls.Add( this.controlsPanel, 0, 7 );
		this.bodyPanel.Controls.Add( this.debugPanel, 0, 8 );
		this.bodyPanel.Dock = DockStyle.Fill;
		this.bodyPanel.Location = new Point( 0, 0 );
		this.bodyPanel.Name = "bodyPanel";
		this.bodyPanel.Padding = new Padding( 12 );
		this.bodyPanel.RowCount = 9;
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.Size = new Size( 451, 454 );
		this.bodyPanel.TabIndex = 0;
		// 
		// headerPanel
		// 
		this.headerPanel.AutoSize = true;
		this.headerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.headerPanel.ColumnCount = 2;
		this.bodyPanel.SetColumnSpan( this.headerPanel, 2 );
		this.headerPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.headerPanel.ColumnStyles.Add( new ColumnStyle() );
		this.headerPanel.Controls.Add( this.statusLabel, 0, 0 );
		this.headerPanel.Controls.Add( this.settingsButton, 1, 0 );
		this.headerPanel.Dock = DockStyle.Fill;
		this.headerPanel.Location = new Point( 12, 12 );
		this.headerPanel.Margin = new Padding( 0, 0, 0, 4 );
		this.headerPanel.Name = "headerPanel";
		this.headerPanel.RowCount = 1;
		this.headerPanel.RowStyles.Add( new RowStyle() );
		this.headerPanel.Size = new Size( 427, 31 );
		this.headerPanel.TabIndex = 0;
		// 
		// statusLabel
		// 
		this.statusLabel.Anchor = AnchorStyles.Left;
		this.statusLabel.AutoSize = true;
		this.statusLabel.ForeColor = Color.FromArgb( 117, 117, 117 );
		this.statusLabel.Location = new Point( 3, 8 );
		this.statusLabel.Name = "statusLabel";
		this.statusLabel.Size = new Size( 87, 15 );
		this.statusLabel.TabIndex = 0;
		this.statusLabel.Text = "OCR を準備中…";
		// 
		// settingsButton
		// 
		this.settingsButton.Anchor =  AnchorStyles.Top  |  AnchorStyles.Right ;
		this.settingsButton.AutoSize = true;
		this.settingsButton.Location = new Point( 349, 3 );
		this.settingsButton.Name = "settingsButton";
		this.settingsButton.Size = new Size( 75, 25 );
		this.settingsButton.TabIndex = 1;
		this.settingsButton.Text = "設定…";
		this.settingsButton.UseVisualStyleBackColor = true;
		this.settingsButton.Click +=  this.SettingsButton_Click ;
		// 
		// targetCaption
		// 
		this.targetCaption.Anchor = AnchorStyles.Left;
		this.targetCaption.AutoSize = true;
		this.targetCaption.Location = new Point( 15, 54 );
		this.targetCaption.Name = "targetCaption";
		this.targetCaption.Size = new Size( 31, 15 );
		this.targetCaption.TabIndex = 9;
		this.targetCaption.Text = "対象";
		// 
		// windowSelect
		// 
		this.windowSelect.Anchor =   AnchorStyles.Top  |  AnchorStyles.Left   |  AnchorStyles.Right ;
		this.windowSelect.DropDownStyle = ComboBoxStyle.DropDownList;
		this.windowSelect.DropDownWidth = 420;
		this.windowSelect.Location = new Point( 66, 50 );
		this.windowSelect.MaxDropDownItems = 20;
		this.windowSelect.Name = "windowSelect";
		this.windowSelect.Size = new Size( 370, 23 );
		this.windowSelect.TabIndex = 11;
		this.windowSelect.DropDown +=  this.WindowSelect_DropDown ;
		this.windowSelect.SelectedIndexChanged +=  this.WindowSelect_SelectedIndexChanged ;
		// 
		// preview
		// 
		this.preview.BackColor = Color.FromArgb( 32, 32, 32 );
		this.bodyPanel.SetColumnSpan( this.preview, 2 );
		this.preview.Location = new Point( 15, 79 );
		this.preview.Margin = new Padding( 3, 3, 3, 8 );
		this.preview.Name = "preview";
		this.preview.Size = new Size( 420, 80 );
		this.preview.SizeMode = PictureBoxSizeMode.Zoom;
		this.preview.TabIndex = 8;
		this.preview.TabStop = false;
		// 
		// nameCaption
		// 
		this.nameCaption.Anchor = AnchorStyles.Left;
		this.nameCaption.AutoSize = true;
		this.nameCaption.Location = new Point( 15, 176 );
		this.nameCaption.Name = "nameCaption";
		this.nameCaption.Size = new Size( 45, 15 );
		this.nameCaption.TabIndex = 1;
		this.nameCaption.Text = "マップ名";
		// 
		// namePanel
		// 
		this.namePanel.AutoSize = true;
		this.namePanel.Controls.Add( this.nameValue );
		this.namePanel.Controls.Add( this.mapSelect );
		this.namePanel.Location = new Point( 63, 167 );
		this.namePanel.Margin = new Padding( 0 );
		this.namePanel.Name = "namePanel";
		this.namePanel.Size = new Size( 373, 34 );
		this.namePanel.TabIndex = 2;
		this.namePanel.WrapContents = false;
		// 
		// nameValue
		// 
		this.nameValue.Font = new Font( "Yu Gothic UI", 16F, FontStyle.Bold );
		this.nameValue.Location = new Point( 3, 0 );
		this.nameValue.Name = "nameValue";
		this.nameValue.Size = new Size( 244, 34 );
		this.nameValue.TabIndex = 0;
		this.nameValue.Text = "—";
		this.nameValue.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// mapSelect
		// 
		this.mapSelect.Anchor = AnchorStyles.Left;
		this.mapSelect.DropDownStyle = ComboBoxStyle.DropDownList;
		this.mapSelect.DropDownWidth = 200;
		this.mapSelect.Location = new Point( 253, 5 );
		this.mapSelect.Margin = new Padding( 3, 0, 0, 0 );
		this.mapSelect.MaxDropDownItems = 20;
		this.mapSelect.Name = "mapSelect";
		this.mapSelect.Size = new Size( 120, 23 );
		this.mapSelect.TabIndex = 1;
		this.mapSelect.SelectedIndexChanged +=  this.MapSelect_SelectedIndexChanged ;
		// 
		// coordinateCaption
		// 
		this.coordinateCaption.Anchor = AnchorStyles.Left;
		this.coordinateCaption.AutoSize = true;
		this.coordinateCaption.Location = new Point( 15, 210 );
		this.coordinateCaption.Name = "coordinateCaption";
		this.coordinateCaption.Size = new Size( 31, 15 );
		this.coordinateCaption.TabIndex = 4;
		this.coordinateCaption.Text = "座標";
		// 
		// coordinateValue
		// 
		this.coordinateValue.Font = new Font( "Yu Gothic UI", 16F, FontStyle.Bold );
		this.coordinateValue.Location = new Point( 66, 201 );
		this.coordinateValue.Name = "coordinateValue";
		this.coordinateValue.Size = new Size( 244, 34 );
		this.coordinateValue.TabIndex = 5;
		this.coordinateValue.Text = "—";
		this.coordinateValue.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// rawLabel
		// 
		this.rawLabel.AutoSize = true;
		this.bodyPanel.SetColumnSpan( this.rawLabel, 2 );
		this.rawLabel.ForeColor = Color.FromArgb( 117, 117, 117 );
		this.rawLabel.Location = new Point( 15, 241 );
		this.rawLabel.Margin = new Padding( 3, 6, 3, 2 );
		this.rawLabel.Name = "rawLabel";
		this.rawLabel.Size = new Size( 41, 15 );
		this.rawLabel.TabIndex = 7;
		this.rawLabel.Text = "OCR: -";
		// 
		// layersPanel
		// 
		this.layersPanel.AutoSize = true;
		this.layersPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.layersPanel.ColumnCount = 4;
		this.bodyPanel.SetColumnSpan( this.layersPanel, 2 );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.ColumnStyles.Add( new ColumnStyle() );
		this.layersPanel.Controls.Add( this.impassableEdgeSwatch, 0, 0 );
		this.layersPanel.Controls.Add( this.impassableEdgeBox, 1, 0 );
		this.layersPanel.Controls.Add( this.monsterBlockSwatch, 0, 1 );
		this.layersPanel.Controls.Add( this.monsterBlockBox, 1, 1 );
		this.layersPanel.Controls.Add( this.mapMoveSwatch, 0, 2 );
		this.layersPanel.Controls.Add( this.mapMoveBox, 1, 2 );
		this.layersPanel.Controls.Add( this.specialSwatch, 0, 3 );
		this.layersPanel.Controls.Add( this.specialBox, 1, 3 );
		this.layersPanel.Controls.Add( this.gridSwatch, 2, 0 );
		this.layersPanel.Controls.Add( this.gridBox, 3, 0 );
		this.layersPanel.Controls.Add( this.playerSwatch, 2, 1 );
		this.layersPanel.Controls.Add( this.playerBox, 3, 1 );
		this.layersPanel.Location = new Point( 12, 258 );
		this.layersPanel.Margin = new Padding( 0, 0, 0, 2 );
		this.layersPanel.Name = "layersPanel";
		this.layersPanel.RowCount = 4;
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.Size = new Size( 220, 100 );
		this.layersPanel.TabIndex = 12;
		// 
		// impassableEdgeSwatch
		// 
		this.impassableEdgeSwatch.Anchor = AnchorStyles.Left;
		this.impassableEdgeSwatch.BorderStyle = BorderStyle.FixedSingle;
		this.impassableEdgeSwatch.Location = new Point( 3, 5 );
		this.impassableEdgeSwatch.Margin = new Padding( 3, 3, 0, 3 );
		this.impassableEdgeSwatch.Name = "impassableEdgeSwatch";
		this.impassableEdgeSwatch.Size = new Size( 14, 14 );
		this.impassableEdgeSwatch.TabIndex = 0;
		// 
		// impassableEdgeBox
		// 
		this.impassableEdgeBox.Anchor = AnchorStyles.Left;
		this.impassableEdgeBox.AutoSize = true;
		this.impassableEdgeBox.Location = new Point( 20, 3 );
		this.impassableEdgeBox.Name = "impassableEdgeBox";
		this.impassableEdgeBox.Size = new Size( 38, 19 );
		this.impassableEdgeBox.TabIndex = 1;
		this.impassableEdgeBox.Text = "壁";
		this.impassableEdgeBox.UseVisualStyleBackColor = true;
		this.impassableEdgeBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// monsterBlockSwatch
		// 
		this.monsterBlockSwatch.Anchor = AnchorStyles.Left;
		this.monsterBlockSwatch.BorderStyle = BorderStyle.FixedSingle;
		this.monsterBlockSwatch.Location = new Point( 3, 30 );
		this.monsterBlockSwatch.Margin = new Padding( 3, 3, 0, 3 );
		this.monsterBlockSwatch.Name = "monsterBlockSwatch";
		this.monsterBlockSwatch.Size = new Size( 14, 14 );
		this.monsterBlockSwatch.TabIndex = 3;
		// 
		// monsterBlockBox
		// 
		this.monsterBlockBox.Anchor = AnchorStyles.Left;
		this.monsterBlockBox.AutoSize = true;
		this.monsterBlockBox.Location = new Point( 20, 28 );
		this.monsterBlockBox.Name = "monsterBlockBox";
		this.monsterBlockBox.Size = new Size( 94, 19 );
		this.monsterBlockBox.TabIndex = 4;
		this.monsterBlockBox.Text = "モンスター境界";
		this.monsterBlockBox.UseVisualStyleBackColor = true;
		this.monsterBlockBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// mapMoveSwatch
		// 
		this.mapMoveSwatch.Anchor = AnchorStyles.Left;
		this.mapMoveSwatch.BorderStyle = BorderStyle.FixedSingle;
		this.mapMoveSwatch.Location = new Point( 3, 55 );
		this.mapMoveSwatch.Margin = new Padding( 3, 3, 0, 3 );
		this.mapMoveSwatch.Name = "mapMoveSwatch";
		this.mapMoveSwatch.Size = new Size( 14, 14 );
		this.mapMoveSwatch.TabIndex = 6;
		// 
		// mapMoveBox
		// 
		this.mapMoveBox.Anchor = AnchorStyles.Left;
		this.mapMoveBox.AutoSize = true;
		this.mapMoveBox.Location = new Point( 20, 53 );
		this.mapMoveBox.Name = "mapMoveBox";
		this.mapMoveBox.Size = new Size( 62, 19 );
		this.mapMoveBox.TabIndex = 7;
		this.mapMoveBox.Text = "出入口";
		this.mapMoveBox.UseVisualStyleBackColor = true;
		this.mapMoveBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// specialSwatch
		// 
		this.specialSwatch.Anchor = AnchorStyles.Left;
		this.specialSwatch.BorderStyle = BorderStyle.FixedSingle;
		this.specialSwatch.Location = new Point( 3, 80 );
		this.specialSwatch.Margin = new Padding( 3, 3, 0, 3 );
		this.specialSwatch.Name = "specialSwatch";
		this.specialSwatch.Size = new Size( 14, 14 );
		this.specialSwatch.TabIndex = 9;
		// 
		// specialBox
		// 
		this.specialBox.Anchor = AnchorStyles.Left;
		this.specialBox.AutoSize = true;
		this.specialBox.Location = new Point( 20, 78 );
		this.specialBox.Name = "specialBox";
		this.specialBox.Size = new Size( 57, 19 );
		this.specialBox.TabIndex = 10;
		this.specialBox.Text = "その他";
		this.specialBox.UseVisualStyleBackColor = true;
		this.specialBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// gridSwatch
		// 
		this.gridSwatch.Anchor = AnchorStyles.Left;
		this.gridSwatch.BorderStyle = BorderStyle.FixedSingle;
		this.gridSwatch.Location = new Point( 129, 5 );
		this.gridSwatch.Margin = new Padding( 12, 3, 0, 3 );
		this.gridSwatch.Name = "gridSwatch";
		this.gridSwatch.Size = new Size( 14, 14 );
		this.gridSwatch.TabIndex = 11;
		// 
		// gridBox
		// 
		this.gridBox.Anchor = AnchorStyles.Left;
		this.gridBox.AutoSize = true;
		this.gridBox.Location = new Point( 146, 3 );
		this.gridBox.Name = "gridBox";
		this.gridBox.Size = new Size( 59, 19 );
		this.gridBox.TabIndex = 12;
		this.gridBox.Text = "グリッド";
		this.gridBox.UseVisualStyleBackColor = true;
		this.gridBox.CheckedChanged +=  this.GridBox_CheckedChanged ;
		// 
		// playerSwatch
		// 
		this.playerSwatch.Anchor = AnchorStyles.Left;
		this.playerSwatch.BorderStyle = BorderStyle.FixedSingle;
		this.playerSwatch.Location = new Point( 129, 30 );
		this.playerSwatch.Margin = new Padding( 12, 3, 0, 3 );
		this.playerSwatch.Name = "playerSwatch";
		this.playerSwatch.Size = new Size( 14, 14 );
		this.playerSwatch.TabIndex = 13;
		// 
		// playerBox
		// 
		this.playerBox.Anchor = AnchorStyles.Left;
		this.playerBox.AutoSize = true;
		this.playerBox.Location = new Point( 146, 28 );
		this.playerBox.Name = "playerBox";
		this.playerBox.Size = new Size( 71, 19 );
		this.playerBox.TabIndex = 14;
		this.playerBox.Text = "プレイヤー";
		this.playerBox.UseVisualStyleBackColor = true;
		this.playerBox.CheckedChanged +=  this.PlayerBox_CheckedChanged ;
		// 
		// controlsPanel
		// 
		this.controlsPanel.Anchor =   AnchorStyles.Top  |  AnchorStyles.Left   |  AnchorStyles.Right ;
		this.controlsPanel.AutoSize = true;
		this.controlsPanel.ColumnCount = 3;
		this.bodyPanel.SetColumnSpan( this.controlsPanel, 2 );
		this.controlsPanel.ColumnStyles.Add( new ColumnStyle() );
		this.controlsPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.controlsPanel.ColumnStyles.Add( new ColumnStyle() );
		this.controlsPanel.Controls.Add( this.toggleButton, 0, 0 );
		this.controlsPanel.Controls.Add( this.updateStatusLabel, 1, 0 );
		this.controlsPanel.Controls.Add( this.closeButton, 2, 0 );
		this.controlsPanel.Location = new Point( 12, 362 );
		this.controlsPanel.Margin = new Padding( 0, 2, 0, 2 );
		this.controlsPanel.Name = "controlsPanel";
		this.controlsPanel.RowCount = 1;
		this.controlsPanel.RowStyles.Add( new RowStyle() );
		this.controlsPanel.Size = new Size( 427, 36 );
		this.controlsPanel.TabIndex = 9;
		// 
		// toggleButton
		// 
		this.toggleButton.AutoSize = true;
		this.toggleButton.Location = new Point( 3, 3 );
		this.toggleButton.Name = "toggleButton";
		this.toggleButton.Size = new Size( 100, 30 );
		this.toggleButton.TabIndex = 0;
		this.toggleButton.Text = "一時停止";
		this.toggleButton.UseVisualStyleBackColor = true;
		this.toggleButton.Click +=  this.ToggleButton_Click ;
		// 
		// updateStatusLabel
		// 
		this.updateStatusLabel.Anchor = AnchorStyles.Right;
		this.updateStatusLabel.AutoSize = true;
		this.updateStatusLabel.ForeColor = SystemColors.GrayText;
		this.updateStatusLabel.Location = new Point( 343, 10 );
		this.updateStatusLabel.Margin = new Padding( 9, 0, 3, 0 );
		this.updateStatusLabel.Name = "updateStatusLabel";
		this.updateStatusLabel.Size = new Size( 0, 15 );
		this.updateStatusLabel.TabIndex = 1;
		// 
		// closeButton
		// 
		this.closeButton.Anchor = AnchorStyles.Right;
		this.closeButton.AutoSize = true;
		this.closeButton.Location = new Point( 349, 3 );
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new Size( 75, 30 );
		this.closeButton.TabIndex = 2;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click +=  this.CloseButton_Click ;
		// 
		// debugPanel
		// 
		this.debugPanel.AutoSize = true;
		this.bodyPanel.SetColumnSpan( this.debugPanel, 2 );
		this.debugPanel.Controls.Add( this.imageButton );
		this.debugPanel.Controls.Add( this.liveButton );
		this.debugPanel.Location = new Point( 12, 402 );
		this.debugPanel.Margin = new Padding( 0, 2, 0, 2 );
		this.debugPanel.Name = "debugPanel";
		this.debugPanel.Size = new Size( 216, 31 );
		this.debugPanel.TabIndex = 10;
		this.debugPanel.Visible = false;
		this.debugPanel.WrapContents = false;
		// 
		// imageButton
		// 
		this.imageButton.AutoSize = true;
		this.imageButton.Location = new Point( 3, 3 );
		this.imageButton.Name = "imageButton";
		this.imageButton.Size = new Size( 112, 25 );
		this.imageButton.TabIndex = 0;
		this.imageButton.Text = "画像から読み取り…";
		this.imageButton.UseVisualStyleBackColor = true;
		this.imageButton.Click +=  this.ImageButton_Click ;
		// 
		// liveButton
		// 
		this.liveButton.AutoSize = true;
		this.liveButton.Location = new Point( 121, 3 );
		this.liveButton.Name = "liveButton";
		this.liveButton.Size = new Size( 92, 25 );
		this.liveButton.TabIndex = 1;
		this.liveButton.Text = "ライブに戻る";
		this.liveButton.UseVisualStyleBackColor = true;
		this.liveButton.Visible = false;
		this.liveButton.Click +=  this.LiveButton_Click ;
		// 
		// trayIcon
		// 
		this.trayIcon.ContextMenuStrip = this.trayMenu;
		this.trayIcon.Text = "gGame Map Overlay";
		this.trayIcon.DoubleClick +=  this.TrayIcon_DoubleClick ;
		// 
		// trayMenu
		// 
		this.trayMenu.Items.AddRange( new ToolStripItem[] { this.trayShowItem, this.trayMenuSeparator, this.trayExitItem } );
		this.trayMenu.Name = "trayMenu";
		this.trayMenu.Size = new Size( 99, 54 );
		// 
		// trayShowItem
		// 
		this.trayShowItem.Font = new Font( "Yu Gothic UI", 9F, FontStyle.Bold );
		this.trayShowItem.Name = "trayShowItem";
		this.trayShowItem.Size = new Size( 98, 22 );
		this.trayShowItem.Text = "表示";
		this.trayShowItem.Click +=  this.TrayShowItem_Click ;
		// 
		// trayMenuSeparator
		// 
		this.trayMenuSeparator.Name = "trayMenuSeparator";
		this.trayMenuSeparator.Size = new Size( 95, 6 );
		// 
		// trayExitItem
		// 
		this.trayExitItem.Name = "trayExitItem";
		this.trayExitItem.Size = new Size( 98, 22 );
		this.trayExitItem.Text = "終了";
		this.trayExitItem.Click +=  this.TrayExitItem_Click ;
		// 
		// timer
		// 
		this.timer.Interval = 250;
		this.timer.Tick +=  this.Timer_Tick ;
		// 
		// MainForm
		// 
		this.AutoScaleDimensions = new SizeF( 96F, 96F );
		this.AutoScaleMode = AutoScaleMode.Dpi;
		this.AutoSize = true;
		this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.ClientSize = new Size( 451, 454 );
		this.Controls.Add( this.bodyPanel );
		this.Font = new Font( "Yu Gothic UI", 9F );
		this.FormBorderStyle = FormBorderStyle.FixedSingle;
		this.MaximizeBox = false;
		this.Name = "MainForm";
		this.Text = "gGame Map Overlay";
		this.bodyPanel.ResumeLayout( false );
		this.bodyPanel.PerformLayout();
		this.headerPanel.ResumeLayout( false );
		this.headerPanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.preview ).EndInit();
		this.namePanel.ResumeLayout( false );
		this.layersPanel.ResumeLayout( false );
		this.layersPanel.PerformLayout();
		this.controlsPanel.ResumeLayout( false );
		this.controlsPanel.PerformLayout();
		this.debugPanel.ResumeLayout( false );
		this.debugPanel.PerformLayout();
		this.trayMenu.ResumeLayout( false );
		this.ResumeLayout( false );
		this.PerformLayout();
	}

	#endregion

	private TableLayoutPanel bodyPanel;
    private TableLayoutPanel headerPanel;
    private Label statusLabel;
    private Button settingsButton;
    private Label targetCaption;
    private ComboBox windowSelect;
    private Label nameCaption;
    private Label nameValue;
    private Label coordinateCaption;
    private Label coordinateValue;
    private Label rawLabel;
    private TableLayoutPanel layersPanel;
    private Panel monsterBlockSwatch;
    private CheckBox monsterBlockBox;
    private Panel specialSwatch;
    private CheckBox specialBox;
    private Panel impassableEdgeSwatch;
    private CheckBox impassableEdgeBox;
    private Panel mapMoveSwatch;
    private CheckBox mapMoveBox;
    private Panel gridSwatch;
    private CheckBox gridBox;
    private Panel playerSwatch;
    private CheckBox playerBox;
    private PictureBox preview;
    private TableLayoutPanel controlsPanel;
    private Button toggleButton;
    private Button closeButton;
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private ToolStripMenuItem trayShowItem;
    private ToolStripSeparator trayMenuSeparator;
    private ToolStripMenuItem trayExitItem;
    private Label updateStatusLabel;
    private ComboBox mapSelect;
    private FlowLayoutPanel namePanel;
    private FlowLayoutPanel debugPanel;
    private Button imageButton;
    private Button liveButton;
    private System.Windows.Forms.Timer timer;
}
