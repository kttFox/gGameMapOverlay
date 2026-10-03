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
		this.rawLabel = new Label();
		this.headerPanel = new TableLayoutPanel();
		this.statusLabel = new Label();
		this.settingsButton = new Button();
		this.targetCaption = new Label();
		this.windowSelect = new ComboBox();
		this.preview = new PictureBox();
		this.namePanel = new FlowLayoutPanel();
		this.nameValue = new Label();
		this.mapSelect = new ComboBox();
		this.overlayPanel = new TableLayoutPanel();
		this.layersPanel = new TableLayoutPanel();
		this.overallOpacityLabel = new Label();
		this.overallOpacityBox = new StepNumericUpDown();
		this.overallOpacityUnit = new Label();
		this.impassableEdgeBox = new CheckBox();
		this.impassableEdgeSwatch = new Button();
		this.colorMenu = new ContextMenuStrip( this.components );
		this.colorDefaultItem = new ToolStripMenuItem();
		this.impassableEdgeOpacityCheck = new CheckBox();
		this.impassableEdgeOpacityBox = new StepNumericUpDown();
		this.impassableEdgeOpacityUnit = new Label();
		this.monsterBlockBox = new CheckBox();
		this.monsterBlockSwatch = new Button();
		this.monsterBlockOpacityCheck = new CheckBox();
		this.monsterBlockOpacityBox = new StepNumericUpDown();
		this.monsterBlockOpacityUnit = new Label();
		this.mapMoveBox = new CheckBox();
		this.mapMoveSwatch = new Button();
		this.mapMoveOpacityCheck = new CheckBox();
		this.mapMoveOpacityBox = new StepNumericUpDown();
		this.mapMoveOpacityUnit = new Label();
		this.specialBox = new CheckBox();
		this.specialSwatch = new Button();
		this.specialOpacityCheck = new CheckBox();
		this.specialOpacityBox = new StepNumericUpDown();
		this.specialOpacityUnit = new Label();
		this.gridBox = new CheckBox();
		this.gridSwatch = new Button();
		this.gridOpacityCheck = new CheckBox();
		this.gridOpacityBox = new StepNumericUpDown();
		this.gridOpacityUnit = new Label();
		this.playerBox = new CheckBox();
		this.playerSwatch = new Button();
		this.playerOpacityCheck = new CheckBox();
		this.playerOpacityBox = new StepNumericUpDown();
		this.playerOpacityUnit = new Label();
		this.flowLayoutPanel1 = new FlowLayoutPanel();
		this.customTilesButton = new Button();
		this.overlayOptionPanel = new FlowLayoutPanel();
		this.antiAliasCheck = new CheckBox();
		this.controlsPanel = new TableLayoutPanel();
		this.toggleButton = new Button();
		this.updateStatusLabel = new Label();
		this.closeButton = new Button();
		this.debugPanel = new FlowLayoutPanel();
		this.imageButton = new Button();
		this.liveButton = new Button();
		this.toolTip = new ToolTip( this.components );
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
		this.overlayPanel.SuspendLayout();
		this.layersPanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.overallOpacityBox ).BeginInit();
		this.colorMenu.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.impassableEdgeOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.monsterBlockOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.mapMoveOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.specialOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.gridOpacityBox ).BeginInit();
		( (System.ComponentModel.ISupportInitialize)this.playerOpacityBox ).BeginInit();
		this.flowLayoutPanel1.SuspendLayout();
		this.overlayOptionPanel.SuspendLayout();
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
		this.bodyPanel.Controls.Add( this.rawLabel, 0, 3 );
		this.bodyPanel.Controls.Add( this.headerPanel, 0, 0 );
		this.bodyPanel.Controls.Add( this.targetCaption, 0, 1 );
		this.bodyPanel.Controls.Add( this.windowSelect, 1, 1 );
		this.bodyPanel.Controls.Add( this.preview, 0, 2 );
		this.bodyPanel.Controls.Add( this.namePanel, 0, 5 );
		this.bodyPanel.Controls.Add( this.overlayPanel, 0, 6 );
		this.bodyPanel.Controls.Add( this.controlsPanel, 0, 8 );
		this.bodyPanel.Controls.Add( this.debugPanel, 0, 9 );
		this.bodyPanel.Dock = DockStyle.Fill;
		this.bodyPanel.Location = new Point( 0, 0 );
		this.bodyPanel.Name = "bodyPanel";
		this.bodyPanel.Padding = new Padding( 12 );
		this.bodyPanel.RowCount = 10;
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.RowStyles.Add( new RowStyle() );
		this.bodyPanel.Size = new Size( 451, 496 );
		this.bodyPanel.TabIndex = 0;
		// 
		// rawLabel
		// 
		this.rawLabel.Anchor = AnchorStyles.Left;
		this.rawLabel.AutoEllipsis = true;
		this.rawLabel.AutoSize = true;
		this.bodyPanel.SetColumnSpan( this.rawLabel, 2 );
		this.rawLabel.ForeColor = Color.FromArgb( 117, 117, 117 );
		this.rawLabel.Location = new Point( 15, 167 );
		this.rawLabel.Margin = new Padding( 3, 0, 0, 0 );
		this.rawLabel.Name = "rawLabel";
		this.rawLabel.Size = new Size( 41, 15 );
		this.rawLabel.TabIndex = 7;
		this.rawLabel.Text = "OCR: -";
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
		this.windowSelect.Location = new Point( 52, 50 );
		this.windowSelect.MaxDropDownItems = 20;
		this.windowSelect.Name = "windowSelect";
		this.windowSelect.Size = new Size( 384, 23 );
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
		// namePanel
		// 
		this.bodyPanel.SetColumnSpan( this.namePanel, 2 );
		this.namePanel.Controls.Add( this.nameValue );
		this.namePanel.Controls.Add( this.mapSelect );
		this.namePanel.Location = new Point( 12, 182 );
		this.namePanel.Margin = new Padding( 0 );
		this.namePanel.Name = "namePanel";
		this.namePanel.Size = new Size( 427, 34 );
		this.namePanel.TabIndex = 2;
		this.namePanel.WrapContents = false;
		// 
		// nameValue
		// 
		this.nameValue.Font = new Font( "Yu Gothic UI", 16F, FontStyle.Bold );
		this.nameValue.Location = new Point( 3, 0 );
		this.nameValue.Name = "nameValue";
		this.nameValue.Size = new Size( 294, 34 );
		this.nameValue.TabIndex = 0;
		this.nameValue.Text = "—";
		this.nameValue.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// mapSelect
		// 
		this.mapSelect.Anchor = AnchorStyles.Left;
		this.mapSelect.DropDownStyle = ComboBoxStyle.DropDownList;
		this.mapSelect.DropDownWidth = 200;
		this.mapSelect.Location = new Point( 303, 5 );
		this.mapSelect.Margin = new Padding( 3, 0, 3, 0 );
		this.mapSelect.MaxDropDownItems = 20;
		this.mapSelect.Name = "mapSelect";
		this.mapSelect.Size = new Size( 120, 23 );
		this.mapSelect.TabIndex = 1;
		this.mapSelect.SelectedIndexChanged +=  this.MapSelect_SelectedIndexChanged ;
		// 
		// overlayPanel
		// 
		this.overlayPanel.AutoSize = true;
		this.overlayPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.overlayPanel.ColumnCount = 2;
		this.bodyPanel.SetColumnSpan( this.overlayPanel, 2 );
		this.overlayPanel.ColumnStyles.Add( new ColumnStyle() );
		this.overlayPanel.ColumnStyles.Add( new ColumnStyle() );
		this.overlayPanel.Controls.Add( this.layersPanel, 0, 0 );
		this.overlayPanel.Controls.Add( this.flowLayoutPanel1, 1, 1 );
		this.overlayPanel.Controls.Add( this.overlayOptionPanel, 1, 0 );
		this.overlayPanel.Dock = DockStyle.Fill;
		this.overlayPanel.Location = new Point( 12, 216 );
		this.overlayPanel.Margin = new Padding( 0 );
		this.overlayPanel.Name = "overlayPanel";
		this.overlayPanel.RowCount = 2;
		this.overlayPanel.RowStyles.Add( new RowStyle() );
		this.overlayPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.overlayPanel.Size = new Size( 427, 181 );
		this.overlayPanel.TabIndex = 12;
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
		this.layersPanel.Controls.Add( this.impassableEdgeBox, 0, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeSwatch, 1, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeOpacityCheck, 2, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeOpacityBox, 3, 1 );
		this.layersPanel.Controls.Add( this.impassableEdgeOpacityUnit, 4, 1 );
		this.layersPanel.Controls.Add( this.monsterBlockBox, 0, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockSwatch, 1, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockOpacityCheck, 2, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockOpacityBox, 3, 2 );
		this.layersPanel.Controls.Add( this.monsterBlockOpacityUnit, 4, 2 );
		this.layersPanel.Controls.Add( this.mapMoveBox, 0, 3 );
		this.layersPanel.Controls.Add( this.mapMoveSwatch, 1, 3 );
		this.layersPanel.Controls.Add( this.mapMoveOpacityCheck, 2, 3 );
		this.layersPanel.Controls.Add( this.mapMoveOpacityBox, 3, 3 );
		this.layersPanel.Controls.Add( this.mapMoveOpacityUnit, 4, 3 );
		this.layersPanel.Controls.Add( this.specialBox, 0, 4 );
		this.layersPanel.Controls.Add( this.specialSwatch, 1, 4 );
		this.layersPanel.Controls.Add( this.specialOpacityCheck, 2, 4 );
		this.layersPanel.Controls.Add( this.specialOpacityBox, 3, 4 );
		this.layersPanel.Controls.Add( this.specialOpacityUnit, 4, 4 );
		this.layersPanel.Controls.Add( this.gridBox, 0, 5 );
		this.layersPanel.Controls.Add( this.gridSwatch, 1, 5 );
		this.layersPanel.Controls.Add( this.gridOpacityCheck, 2, 5 );
		this.layersPanel.Controls.Add( this.gridOpacityBox, 3, 5 );
		this.layersPanel.Controls.Add( this.gridOpacityUnit, 4, 5 );
		this.layersPanel.Controls.Add( this.playerBox, 0, 6 );
		this.layersPanel.Controls.Add( this.playerSwatch, 1, 6 );
		this.layersPanel.Controls.Add( this.playerOpacityCheck, 2, 6 );
		this.layersPanel.Controls.Add( this.playerOpacityBox, 3, 6 );
		this.layersPanel.Controls.Add( this.playerOpacityUnit, 4, 6 );
		this.layersPanel.Location = new Point( 0, 4 );
		this.layersPanel.Margin = new Padding( 0, 4, 0, 2 );
		this.layersPanel.Name = "layersPanel";
		this.layersPanel.RowCount = 7;
		this.overlayPanel.SetRowSpan( this.layersPanel, 2 );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle() );
		this.layersPanel.RowStyles.Add( new RowStyle( SizeType.Absolute, 20F ) );
		this.layersPanel.Size = new Size( 220, 175 );
		this.layersPanel.TabIndex = 12;
		// 
		// overallOpacityLabel
		// 
		this.overallOpacityLabel.Anchor = AnchorStyles.Right;
		this.overallOpacityLabel.AutoSize = true;
		this.layersPanel.SetColumnSpan( this.overallOpacityLabel, 3 );
		this.overallOpacityLabel.Location = new Point( 52, 5 );
		this.overallOpacityLabel.Name = "overallOpacityLabel";
		this.overallOpacityLabel.Size = new Size( 89, 15 );
		this.overallOpacityLabel.TabIndex = 0;
		this.overallOpacityLabel.Text = "全体の不透明度";
		// 
		// overallOpacityBox
		// 
		this.overallOpacityBox.Anchor = AnchorStyles.Left;
		this.overallOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.overallOpacityBox.Location = new Point( 147, 1 );
		this.overallOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.overallOpacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.overallOpacityBox.Name = "overallOpacityBox";
		this.overallOpacityBox.Size = new Size( 50, 23 );
		this.overallOpacityBox.TabIndex = 0;
		this.overallOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.overallOpacityBox, "全体の不透明度。100% で不透明。小さくするほどゲーム画面が透けて見えます\n個別にチェックを入れていない色に使います" );
		this.overallOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.overallOpacityBox.ValueChanged +=  this.OverallOpacityBox_ValueChanged ;
		// 
		// overallOpacityUnit
		// 
		this.overallOpacityUnit.Anchor = AnchorStyles.Left;
		this.overallOpacityUnit.AutoSize = true;
		this.overallOpacityUnit.Location = new Point( 197, 5 );
		this.overallOpacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.overallOpacityUnit.Name = "overallOpacityUnit";
		this.overallOpacityUnit.Size = new Size( 17, 15 );
		this.overallOpacityUnit.TabIndex = 1;
		this.overallOpacityUnit.Text = "%";
		// 
		// impassableEdgeBox
		// 
		this.impassableEdgeBox.Dock = DockStyle.Fill;
		this.impassableEdgeBox.Location = new Point( 3, 28 );
		this.impassableEdgeBox.Margin = new Padding( 3, 3, 0, 3 );
		this.impassableEdgeBox.Name = "impassableEdgeBox";
		this.impassableEdgeBox.Size = new Size( 94, 19 );
		this.impassableEdgeBox.TabIndex = 2;
		this.impassableEdgeBox.Text = "壁";
		this.toolTip.SetToolTip( this.impassableEdgeBox, "オーバーレイに表示します" );
		this.impassableEdgeBox.UseVisualStyleBackColor = true;
		this.impassableEdgeBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// impassableEdgeSwatch
		// 
		this.impassableEdgeSwatch.Anchor = AnchorStyles.Left;
		this.impassableEdgeSwatch.ContextMenuStrip = this.colorMenu;
		this.impassableEdgeSwatch.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.impassableEdgeSwatch.FlatStyle = FlatStyle.Flat;
		this.impassableEdgeSwatch.Location = new Point( 97, 27 );
		this.impassableEdgeSwatch.Margin = new Padding( 0 );
		this.impassableEdgeSwatch.Name = "impassableEdgeSwatch";
		this.impassableEdgeSwatch.Size = new Size( 20, 20 );
		this.impassableEdgeSwatch.TabIndex = 1;
		this.toolTip.SetToolTip( this.impassableEdgeSwatch, "クリックで色を変更します" );
		this.impassableEdgeSwatch.Click +=  this.ColorButton_Click ;
		// 
		// colorMenu
		// 
		this.colorMenu.Items.AddRange( new ToolStripItem[] { this.colorDefaultItem } );
		this.colorMenu.Name = "colorMenu";
		this.colorMenu.Size = new Size( 142, 26 );
		this.colorMenu.Opening +=  this.ColorMenu_Opening ;
		// 
		// colorDefaultItem
		// 
		this.colorDefaultItem.Name = "colorDefaultItem";
		this.colorDefaultItem.Size = new Size( 141, 22 );
		this.colorDefaultItem.Text = "初期値に戻す";
		this.colorDefaultItem.Click +=  this.ColorDefaultItem_Click ;
		// 
		// impassableEdgeOpacityCheck
		// 
		this.impassableEdgeOpacityCheck.Anchor = AnchorStyles.Left;
		this.impassableEdgeOpacityCheck.AutoSize = true;
		this.impassableEdgeOpacityCheck.Location = new Point( 129, 30 );
		this.impassableEdgeOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.impassableEdgeOpacityCheck.Name = "impassableEdgeOpacityCheck";
		this.impassableEdgeOpacityCheck.Size = new Size( 15, 14 );
		this.impassableEdgeOpacityCheck.TabIndex = 3;
		this.toolTip.SetToolTip( this.impassableEdgeOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.impassableEdgeOpacityCheck.UseVisualStyleBackColor = true;
		this.impassableEdgeOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// impassableEdgeOpacityBox
		// 
		this.impassableEdgeOpacityBox.Anchor = AnchorStyles.Left;
		this.impassableEdgeOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.impassableEdgeOpacityBox.Location = new Point( 147, 26 );
		this.impassableEdgeOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.impassableEdgeOpacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.impassableEdgeOpacityBox.Name = "impassableEdgeOpacityBox";
		this.impassableEdgeOpacityBox.Size = new Size( 50, 23 );
		this.impassableEdgeOpacityBox.TabIndex = 4;
		this.impassableEdgeOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.impassableEdgeOpacityBox, "この色の不透明度" );
		this.impassableEdgeOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.impassableEdgeOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// impassableEdgeOpacityUnit
		// 
		this.impassableEdgeOpacityUnit.Anchor = AnchorStyles.Left;
		this.impassableEdgeOpacityUnit.AutoSize = true;
		this.impassableEdgeOpacityUnit.Location = new Point( 197, 30 );
		this.impassableEdgeOpacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.impassableEdgeOpacityUnit.Name = "impassableEdgeOpacityUnit";
		this.impassableEdgeOpacityUnit.Size = new Size( 17, 15 );
		this.impassableEdgeOpacityUnit.TabIndex = 5;
		this.impassableEdgeOpacityUnit.Text = "%";
		// 
		// monsterBlockBox
		// 
		this.monsterBlockBox.Dock = DockStyle.Fill;
		this.monsterBlockBox.Location = new Point( 3, 53 );
		this.monsterBlockBox.Margin = new Padding( 3, 3, 0, 3 );
		this.monsterBlockBox.Name = "monsterBlockBox";
		this.monsterBlockBox.Size = new Size( 94, 19 );
		this.monsterBlockBox.TabIndex = 6;
		this.monsterBlockBox.Text = "モンスター境界";
		this.toolTip.SetToolTip( this.monsterBlockBox, "オーバーレイに表示します" );
		this.monsterBlockBox.UseVisualStyleBackColor = true;
		this.monsterBlockBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// monsterBlockSwatch
		// 
		this.monsterBlockSwatch.Anchor = AnchorStyles.Left;
		this.monsterBlockSwatch.ContextMenuStrip = this.colorMenu;
		this.monsterBlockSwatch.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.monsterBlockSwatch.FlatStyle = FlatStyle.Flat;
		this.monsterBlockSwatch.Location = new Point( 97, 52 );
		this.monsterBlockSwatch.Margin = new Padding( 0 );
		this.monsterBlockSwatch.Name = "monsterBlockSwatch";
		this.monsterBlockSwatch.Size = new Size( 20, 20 );
		this.monsterBlockSwatch.TabIndex = 5;
		this.toolTip.SetToolTip( this.monsterBlockSwatch, "クリックで色を変更します" );
		this.monsterBlockSwatch.Click +=  this.ColorButton_Click ;
		// 
		// monsterBlockOpacityCheck
		// 
		this.monsterBlockOpacityCheck.Anchor = AnchorStyles.Left;
		this.monsterBlockOpacityCheck.AutoSize = true;
		this.monsterBlockOpacityCheck.Location = new Point( 129, 55 );
		this.monsterBlockOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.monsterBlockOpacityCheck.Name = "monsterBlockOpacityCheck";
		this.monsterBlockOpacityCheck.Size = new Size( 15, 14 );
		this.monsterBlockOpacityCheck.TabIndex = 7;
		this.toolTip.SetToolTip( this.monsterBlockOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.monsterBlockOpacityCheck.UseVisualStyleBackColor = true;
		this.monsterBlockOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// monsterBlockOpacityBox
		// 
		this.monsterBlockOpacityBox.Anchor = AnchorStyles.Left;
		this.monsterBlockOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.monsterBlockOpacityBox.Location = new Point( 147, 51 );
		this.monsterBlockOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.monsterBlockOpacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.monsterBlockOpacityBox.Name = "monsterBlockOpacityBox";
		this.monsterBlockOpacityBox.Size = new Size( 50, 23 );
		this.monsterBlockOpacityBox.TabIndex = 8;
		this.monsterBlockOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.monsterBlockOpacityBox, "この色の不透明度" );
		this.monsterBlockOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.monsterBlockOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// monsterBlockOpacityUnit
		// 
		this.monsterBlockOpacityUnit.Anchor = AnchorStyles.Left;
		this.monsterBlockOpacityUnit.AutoSize = true;
		this.monsterBlockOpacityUnit.Location = new Point( 197, 55 );
		this.monsterBlockOpacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.monsterBlockOpacityUnit.Name = "monsterBlockOpacityUnit";
		this.monsterBlockOpacityUnit.Size = new Size( 17, 15 );
		this.monsterBlockOpacityUnit.TabIndex = 9;
		this.monsterBlockOpacityUnit.Text = "%";
		// 
		// mapMoveBox
		// 
		this.mapMoveBox.Dock = DockStyle.Fill;
		this.mapMoveBox.Location = new Point( 3, 78 );
		this.mapMoveBox.Margin = new Padding( 3, 3, 0, 3 );
		this.mapMoveBox.Name = "mapMoveBox";
		this.mapMoveBox.Size = new Size( 94, 19 );
		this.mapMoveBox.TabIndex = 10;
		this.mapMoveBox.Text = "出入口";
		this.toolTip.SetToolTip( this.mapMoveBox, "オーバーレイに表示します" );
		this.mapMoveBox.UseVisualStyleBackColor = true;
		this.mapMoveBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// mapMoveSwatch
		// 
		this.mapMoveSwatch.Anchor = AnchorStyles.Left;
		this.mapMoveSwatch.ContextMenuStrip = this.colorMenu;
		this.mapMoveSwatch.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.mapMoveSwatch.FlatStyle = FlatStyle.Flat;
		this.mapMoveSwatch.Location = new Point( 97, 77 );
		this.mapMoveSwatch.Margin = new Padding( 0 );
		this.mapMoveSwatch.Name = "mapMoveSwatch";
		this.mapMoveSwatch.Size = new Size( 20, 20 );
		this.mapMoveSwatch.TabIndex = 9;
		this.toolTip.SetToolTip( this.mapMoveSwatch, "クリックで色を変更します" );
		this.mapMoveSwatch.Click +=  this.ColorButton_Click ;
		// 
		// mapMoveOpacityCheck
		// 
		this.mapMoveOpacityCheck.Anchor = AnchorStyles.Left;
		this.mapMoveOpacityCheck.AutoSize = true;
		this.mapMoveOpacityCheck.Location = new Point( 129, 80 );
		this.mapMoveOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.mapMoveOpacityCheck.Name = "mapMoveOpacityCheck";
		this.mapMoveOpacityCheck.Size = new Size( 15, 14 );
		this.mapMoveOpacityCheck.TabIndex = 11;
		this.toolTip.SetToolTip( this.mapMoveOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.mapMoveOpacityCheck.UseVisualStyleBackColor = true;
		this.mapMoveOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// mapMoveOpacityBox
		// 
		this.mapMoveOpacityBox.Anchor = AnchorStyles.Left;
		this.mapMoveOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.mapMoveOpacityBox.Location = new Point( 147, 76 );
		this.mapMoveOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.mapMoveOpacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.mapMoveOpacityBox.Name = "mapMoveOpacityBox";
		this.mapMoveOpacityBox.Size = new Size( 50, 23 );
		this.mapMoveOpacityBox.TabIndex = 12;
		this.mapMoveOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.mapMoveOpacityBox, "この色の不透明度" );
		this.mapMoveOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.mapMoveOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// mapMoveOpacityUnit
		// 
		this.mapMoveOpacityUnit.Anchor = AnchorStyles.Left;
		this.mapMoveOpacityUnit.AutoSize = true;
		this.mapMoveOpacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.mapMoveOpacityUnit.Location = new Point( 200, 80 );
		this.mapMoveOpacityUnit.Name = "mapMoveOpacityUnit";
		this.mapMoveOpacityUnit.Size = new Size( 17, 15 );
		this.mapMoveOpacityUnit.TabIndex = 13;
		this.mapMoveOpacityUnit.Text = "%";
		// 
		// specialBox
		// 
		this.specialBox.Dock = DockStyle.Fill;
		this.specialBox.Location = new Point( 3, 103 );
		this.specialBox.Margin = new Padding( 3, 3, 0, 3 );
		this.specialBox.Name = "specialBox";
		this.specialBox.Size = new Size( 94, 19 );
		this.specialBox.TabIndex = 14;
		this.specialBox.Text = "その他";
		this.toolTip.SetToolTip( this.specialBox, "オーバーレイに表示します" );
		this.specialBox.UseVisualStyleBackColor = true;
		this.specialBox.CheckedChanged +=  this.LayerBox_CheckedChanged ;
		// 
		// specialSwatch
		// 
		this.specialSwatch.Anchor = AnchorStyles.Left;
		this.specialSwatch.ContextMenuStrip = this.colorMenu;
		this.specialSwatch.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.specialSwatch.FlatStyle = FlatStyle.Flat;
		this.specialSwatch.Location = new Point( 97, 102 );
		this.specialSwatch.Margin = new Padding( 0 );
		this.specialSwatch.Name = "specialSwatch";
		this.specialSwatch.Size = new Size( 20, 20 );
		this.specialSwatch.TabIndex = 13;
		this.toolTip.SetToolTip( this.specialSwatch, "クリックで色を変更します" );
		this.specialSwatch.Click +=  this.ColorButton_Click ;
		// 
		// specialOpacityCheck
		// 
		this.specialOpacityCheck.Anchor = AnchorStyles.Left;
		this.specialOpacityCheck.AutoSize = true;
		this.specialOpacityCheck.Location = new Point( 129, 105 );
		this.specialOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.specialOpacityCheck.Name = "specialOpacityCheck";
		this.specialOpacityCheck.Size = new Size( 15, 14 );
		this.specialOpacityCheck.TabIndex = 15;
		this.toolTip.SetToolTip( this.specialOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.specialOpacityCheck.UseVisualStyleBackColor = true;
		this.specialOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// specialOpacityBox
		// 
		this.specialOpacityBox.Anchor = AnchorStyles.Left;
		this.specialOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.specialOpacityBox.Location = new Point( 147, 101 );
		this.specialOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.specialOpacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.specialOpacityBox.Name = "specialOpacityBox";
		this.specialOpacityBox.Size = new Size( 50, 23 );
		this.specialOpacityBox.TabIndex = 16;
		this.specialOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.specialOpacityBox, "この色の不透明度" );
		this.specialOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.specialOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// specialOpacityUnit
		// 
		this.specialOpacityUnit.Anchor = AnchorStyles.Left;
		this.specialOpacityUnit.AutoSize = true;
		this.specialOpacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.specialOpacityUnit.Location = new Point( 200, 105 );
		this.specialOpacityUnit.Name = "specialOpacityUnit";
		this.specialOpacityUnit.Size = new Size( 17, 15 );
		this.specialOpacityUnit.TabIndex = 17;
		this.specialOpacityUnit.Text = "%";
		// 
		// gridBox
		// 
		this.gridBox.Dock = DockStyle.Fill;
		this.gridBox.Location = new Point( 3, 128 );
		this.gridBox.Margin = new Padding( 3, 3, 0, 3 );
		this.gridBox.Name = "gridBox";
		this.gridBox.Size = new Size( 94, 19 );
		this.gridBox.TabIndex = 18;
		this.gridBox.Text = "グリッド";
		this.toolTip.SetToolTip( this.gridBox, "オーバーレイに表示します" );
		this.gridBox.UseVisualStyleBackColor = true;
		this.gridBox.CheckedChanged +=  this.GridBox_CheckedChanged ;
		// 
		// gridSwatch
		// 
		this.gridSwatch.Anchor = AnchorStyles.Left;
		this.gridSwatch.ContextMenuStrip = this.colorMenu;
		this.gridSwatch.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.gridSwatch.FlatStyle = FlatStyle.Flat;
		this.gridSwatch.Location = new Point( 97, 127 );
		this.gridSwatch.Margin = new Padding( 0 );
		this.gridSwatch.Name = "gridSwatch";
		this.gridSwatch.Size = new Size( 20, 20 );
		this.gridSwatch.TabIndex = 17;
		this.toolTip.SetToolTip( this.gridSwatch, "クリックで色を変更します" );
		this.gridSwatch.Click +=  this.GridColorButton_Click ;
		// 
		// gridOpacityCheck
		// 
		this.gridOpacityCheck.Anchor = AnchorStyles.Left;
		this.gridOpacityCheck.AutoSize = true;
		this.gridOpacityCheck.Location = new Point( 129, 130 );
		this.gridOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.gridOpacityCheck.Name = "gridOpacityCheck";
		this.gridOpacityCheck.Size = new Size( 15, 14 );
		this.gridOpacityCheck.TabIndex = 19;
		this.toolTip.SetToolTip( this.gridOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.gridOpacityCheck.UseVisualStyleBackColor = true;
		this.gridOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// gridOpacityBox
		// 
		this.gridOpacityBox.Anchor = AnchorStyles.Left;
		this.gridOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.gridOpacityBox.Location = new Point( 147, 126 );
		this.gridOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.gridOpacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.gridOpacityBox.Name = "gridOpacityBox";
		this.gridOpacityBox.Size = new Size( 50, 23 );
		this.gridOpacityBox.TabIndex = 20;
		this.gridOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.gridOpacityBox, "この色の不透明度" );
		this.gridOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.gridOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// gridOpacityUnit
		// 
		this.gridOpacityUnit.Anchor = AnchorStyles.Left;
		this.gridOpacityUnit.AutoSize = true;
		this.gridOpacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.gridOpacityUnit.Location = new Point( 200, 130 );
		this.gridOpacityUnit.Name = "gridOpacityUnit";
		this.gridOpacityUnit.Size = new Size( 17, 15 );
		this.gridOpacityUnit.TabIndex = 21;
		this.gridOpacityUnit.Text = "%";
		// 
		// playerBox
		// 
		this.playerBox.Dock = DockStyle.Fill;
		this.playerBox.Location = new Point( 3, 153 );
		this.playerBox.Margin = new Padding( 3, 3, 0, 3 );
		this.playerBox.Name = "playerBox";
		this.playerBox.Size = new Size( 94, 19 );
		this.playerBox.TabIndex = 22;
		this.playerBox.Text = "プレイヤー";
		this.toolTip.SetToolTip( this.playerBox, "オーバーレイに表示します" );
		this.playerBox.UseVisualStyleBackColor = true;
		this.playerBox.CheckedChanged +=  this.PlayerBox_CheckedChanged ;
		// 
		// playerSwatch
		// 
		this.playerSwatch.Anchor = AnchorStyles.Left;
		this.playerSwatch.ContextMenuStrip = this.colorMenu;
		this.playerSwatch.FlatAppearance.BorderColor = Color.FromArgb( 117, 117, 117 );
		this.playerSwatch.FlatStyle = FlatStyle.Flat;
		this.playerSwatch.Location = new Point( 97, 152 );
		this.playerSwatch.Margin = new Padding( 0 );
		this.playerSwatch.Name = "playerSwatch";
		this.playerSwatch.Size = new Size( 20, 20 );
		this.playerSwatch.TabIndex = 21;
		this.toolTip.SetToolTip( this.playerSwatch, "クリックで色を変更します" );
		this.playerSwatch.Click +=  this.PlayerColorButton_Click ;
		// 
		// playerOpacityCheck
		// 
		this.playerOpacityCheck.Anchor = AnchorStyles.Left;
		this.playerOpacityCheck.AutoSize = true;
		this.playerOpacityCheck.Location = new Point( 129, 155 );
		this.playerOpacityCheck.Margin = new Padding( 12, 3, 0, 3 );
		this.playerOpacityCheck.Name = "playerOpacityCheck";
		this.playerOpacityCheck.Size = new Size( 15, 14 );
		this.playerOpacityCheck.TabIndex = 23;
		this.toolTip.SetToolTip( this.playerOpacityCheck, "チェックを入れると、この色だけ個別の不透明度にします" );
		this.playerOpacityCheck.UseVisualStyleBackColor = true;
		this.playerOpacityCheck.CheckedChanged +=  this.OwnOpacityCheck_CheckedChanged ;
		// 
		// playerOpacityBox
		// 
		this.playerOpacityBox.Anchor = AnchorStyles.Left;
		this.playerOpacityBox.Increment = new decimal( new int[] { 5, 0, 0, 0 } );
		this.playerOpacityBox.Location = new Point( 147, 151 );
		this.playerOpacityBox.Margin = new Padding( 3, 1, 0, 1 );
		this.playerOpacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.playerOpacityBox.Name = "playerOpacityBox";
		this.playerOpacityBox.Size = new Size( 50, 23 );
		this.playerOpacityBox.TabIndex = 24;
		this.playerOpacityBox.TextAlign = HorizontalAlignment.Right;
		this.toolTip.SetToolTip( this.playerOpacityBox, "この色の不透明度" );
		this.playerOpacityBox.Value = new decimal( new int[] { 50, 0, 0, 0 } );
		this.playerOpacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// playerOpacityUnit
		// 
		this.playerOpacityUnit.Anchor = AnchorStyles.Left;
		this.playerOpacityUnit.AutoSize = true;
		this.playerOpacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.playerOpacityUnit.Location = new Point( 200, 155 );
		this.playerOpacityUnit.Name = "playerOpacityUnit";
		this.playerOpacityUnit.Size = new Size( 17, 15 );
		this.playerOpacityUnit.TabIndex = 25;
		this.playerOpacityUnit.Text = "%";
		// 
		// flowLayoutPanel1
		// 
		this.flowLayoutPanel1.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
		this.flowLayoutPanel1.AutoSize = true;
		this.flowLayoutPanel1.Controls.Add( this.customTilesButton );
		this.flowLayoutPanel1.Location = new Point( 235, 151 );
		this.flowLayoutPanel1.Margin = new Padding( 15, 3, 3, 0 );
		this.flowLayoutPanel1.Name = "flowLayoutPanel1";
		this.flowLayoutPanel1.Size = new Size( 106, 30 );
		this.flowLayoutPanel1.TabIndex = 2;
		// 
		// customTilesButton
		// 
		this.customTilesButton.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
		this.customTilesButton.AutoSize = true;
		this.customTilesButton.Location = new Point( 3, 3 );
		this.customTilesButton.Margin = new Padding( 3, 3, 3, 2 );
		this.customTilesButton.Name = "customTilesButton";
		this.customTilesButton.Size = new Size( 100, 25 );
		this.customTilesButton.TabIndex = 2;
		this.customTilesButton.Text = "カスタムマス...";
		this.toolTip.SetToolTip( this.customTilesButton, "キャラクターの周りに自分でマスを描きます" );
		this.customTilesButton.UseVisualStyleBackColor = true;
		this.customTilesButton.Visible = false;
		this.customTilesButton.Click +=  this.CustomTilesButton_Click ;
		// 
		// overlayOptionPanel
		// 
		this.overlayOptionPanel.Anchor =   AnchorStyles.Top  |  AnchorStyles.Bottom   |  AnchorStyles.Left ;
		this.overlayOptionPanel.AutoSize = true;
		this.overlayOptionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.overlayOptionPanel.Controls.Add( this.antiAliasCheck );
		this.overlayOptionPanel.FlowDirection = FlowDirection.TopDown;
		this.overlayOptionPanel.Location = new Point( 232, 4 );
		this.overlayOptionPanel.Margin = new Padding( 12, 4, 0, 2 );
		this.overlayOptionPanel.Name = "overlayOptionPanel";
		this.overlayOptionPanel.Size = new Size( 103, 25 );
		this.overlayOptionPanel.TabIndex = 13;
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
		this.controlsPanel.Location = new Point( 12, 399 );
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
		this.toggleButton.Size = new Size( 217, 30 );
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
		this.debugPanel.Location = new Point( 12, 439 );
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
		this.ClientSize = new Size( 451, 496 );
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
		this.overlayPanel.ResumeLayout( false );
		this.overlayPanel.PerformLayout();
		this.layersPanel.ResumeLayout( false );
		this.layersPanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.overallOpacityBox ).EndInit();
		this.colorMenu.ResumeLayout( false );
		( (System.ComponentModel.ISupportInitialize)this.impassableEdgeOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.monsterBlockOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.mapMoveOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.specialOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.gridOpacityBox ).EndInit();
		( (System.ComponentModel.ISupportInitialize)this.playerOpacityBox ).EndInit();
		this.flowLayoutPanel1.ResumeLayout( false );
		this.flowLayoutPanel1.PerformLayout();
		this.overlayOptionPanel.ResumeLayout( false );
		this.overlayOptionPanel.PerformLayout();
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
    private Label overallOpacityLabel;
    private StepNumericUpDown overallOpacityBox;
    private Label overallOpacityUnit;
    private CheckBox impassableEdgeOpacityCheck;
    private StepNumericUpDown impassableEdgeOpacityBox;
    private Label impassableEdgeOpacityUnit;
    private CheckBox monsterBlockOpacityCheck;
    private StepNumericUpDown monsterBlockOpacityBox;
    private Label monsterBlockOpacityUnit;
    private CheckBox mapMoveOpacityCheck;
    private StepNumericUpDown mapMoveOpacityBox;
    private Label mapMoveOpacityUnit;
    private CheckBox specialOpacityCheck;
    private StepNumericUpDown specialOpacityBox;
    private Label specialOpacityUnit;
    private CheckBox gridOpacityCheck;
    private StepNumericUpDown gridOpacityBox;
    private Label gridOpacityUnit;
    private CheckBox playerOpacityCheck;
    private StepNumericUpDown playerOpacityBox;
    private Label playerOpacityUnit;
    private FlowLayoutPanel overlayOptionPanel;
    private CheckBox antiAliasCheck;
    private Button customTilesButton;
    private ToolTip toolTip;
    private TableLayoutPanel headerPanel;
    private Label statusLabel;
    private Button settingsButton;
    private Label targetCaption;
    private ComboBox windowSelect;
    private Label nameValue;
    private Label rawLabel;
    private TableLayoutPanel overlayPanel;
    private TableLayoutPanel layersPanel;
    private Button monsterBlockSwatch;
    private CheckBox monsterBlockBox;
    private Button specialSwatch;
    private CheckBox specialBox;
    private Button impassableEdgeSwatch;
    private CheckBox impassableEdgeBox;
    private Button mapMoveSwatch;
    private CheckBox mapMoveBox;
    private Button gridSwatch;
    private CheckBox gridBox;
    private Button playerSwatch;
    private CheckBox playerBox;
    private PictureBox preview;
    private TableLayoutPanel controlsPanel;
    private Button toggleButton;
    private Button closeButton;
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private ContextMenuStrip colorMenu;
    private ToolStripMenuItem colorDefaultItem;
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
	private FlowLayoutPanel flowLayoutPanel1;
}
