namespace gGameMapOverlay;

partial class CustomTilesForm
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
		this.mainPanel = new TableLayoutPanel();
		this.sidePanel = new TableLayoutPanel();
		this.listSplit = new SplitContainer();
		this.groupPanel = new TableLayoutPanel();
		this.groupCaption = new Label();
		this.groupList = new ListView();
		this.groupColumn = new ColumnHeader();
		this.swatches = new ImageList( this.components );
		this.groupButtons = new FlowLayoutPanel();
		this.addButton = new Button();
		this.removeButton = new Button();
		this.upButton = new Button();
		this.downButton = new Button();
		this.propertyPanel = new TableLayoutPanel();
		this.nameCaption = new Label();
		this.nameBox = new TextBox();
		this.opacityCaption = new Label();
		this.opacityPanel = new FlowLayoutPanel();
		this.opacityCheck = new CheckBox();
		this.opacityBox = new StepNumericUpDown();
		this.opacityUnit = new Label();
		this.layerPanel = new TableLayoutPanel();
		this.layerCaption = new Label();
		this.layerList = new ListView();
		this.layerColumn = new ColumnHeader();
		this.layerSwatches = new ImageList( this.components );
		this.layerButtons = new FlowLayoutPanel();
		this.layerAddButton = new Button();
		this.layerRemoveButton = new Button();
		this.layerUpButton = new Button();
		this.layerDownButton = new Button();
		this.colorPanel = new FlowLayoutPanel();
		this.colorCaption = new Label();
		this.colorButton = new Button();
		this.shapeCaption = new Label();
		this.shapePanel = new FlowLayoutPanel();
		this.thicknessPanel = new FlowLayoutPanel();
		this.thicknessCaption = new Label();
		this.thicknessBox = new ComboBox();
		this.helpLabel = new Label();
		this.toolPanel = new TableLayoutPanel();
		this.undoPanel = new FlowLayoutPanel();
		this.undoButton = new Button();
		this.redoButton = new Button();
		this.zoomPanel = new FlowLayoutPanel();
		this.zoomOutButton = new Button();
		this.zoomResetButton = new Button();
		this.zoomInButton = new Button();
		this.canvas = new CustomTilesCanvas();
		this.bottomPanel = new TableLayoutPanel();
		this.gameCheck = new CheckBox();
		this.recaptureButton = new Button();
		this.statusLabel = new Label();
		this.okButton = new Button();
		this.cancelButton = new Button();
		this.shapeTip = new ToolTip( this.components );
		this.mainPanel.SuspendLayout();
		this.sidePanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.listSplit ).BeginInit();
		this.listSplit.Panel1.SuspendLayout();
		this.listSplit.Panel2.SuspendLayout();
		this.listSplit.SuspendLayout();
		this.groupPanel.SuspendLayout();
		this.groupButtons.SuspendLayout();
		this.propertyPanel.SuspendLayout();
		this.opacityPanel.SuspendLayout();
		( this.opacityBox ).BeginInit();
		this.layerPanel.SuspendLayout();
		this.layerButtons.SuspendLayout();
		this.colorPanel.SuspendLayout();
		this.thicknessPanel.SuspendLayout();
		this.toolPanel.SuspendLayout();
		this.undoPanel.SuspendLayout();
		this.zoomPanel.SuspendLayout();
		this.bottomPanel.SuspendLayout();
		this.SuspendLayout();
		// 
		// mainPanel
		// 
		this.mainPanel.ColumnCount = 2;
		this.mainPanel.ColumnStyles.Add( new ColumnStyle() );
		this.mainPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.mainPanel.Controls.Add( this.sidePanel, 0, 0 );
		this.mainPanel.Controls.Add( this.toolPanel, 1, 0 );
		this.mainPanel.Controls.Add( this.canvas, 1, 1 );
		this.mainPanel.Controls.Add( this.bottomPanel, 1, 2 );
		this.mainPanel.Dock = DockStyle.Fill;
		this.mainPanel.Location = new Point( 0, 0 );
		this.mainPanel.Name = "mainPanel";
		this.mainPanel.Padding = new Padding( 6 );
		this.mainPanel.RowCount = 3;
		this.mainPanel.RowStyles.Add( new RowStyle() );
		this.mainPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.mainPanel.RowStyles.Add( new RowStyle() );
		this.mainPanel.RowStyles.Add( new RowStyle( SizeType.Absolute, 20F ) );
		this.mainPanel.Size = new Size( 900, 619 );
		this.mainPanel.TabIndex = 0;
		// 
		// sidePanel
		// 
		this.sidePanel.AutoSize = true;
		this.sidePanel.ColumnCount = 1;
		this.sidePanel.ColumnStyles.Add( new ColumnStyle() );
		this.sidePanel.Controls.Add( this.listSplit, 0, 0 );
		this.sidePanel.Controls.Add( this.shapeCaption, 0, 1 );
		this.sidePanel.Controls.Add( this.shapePanel, 0, 2 );
		this.sidePanel.Controls.Add( this.thicknessPanel, 0, 3 );
		this.sidePanel.Controls.Add( this.helpLabel, 0, 4 );
		this.sidePanel.Dock = DockStyle.Fill;
		this.sidePanel.Location = new Point( 9, 9 );
		this.sidePanel.Margin = new Padding( 3, 3, 9, 3 );
		this.sidePanel.Name = "sidePanel";
		this.sidePanel.RowCount = 5;
		this.mainPanel.SetRowSpan( this.sidePanel, 3 );
		this.sidePanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.Size = new Size( 220, 601 );
		this.sidePanel.TabIndex = 0;
		// 
		// listSplit
		// 
		this.listSplit.Dock = DockStyle.Fill;
		this.listSplit.Location = new Point( 0, 0 );
		this.listSplit.Margin = new Padding( 0 );
		this.listSplit.IsSplitterFixed = true;
		this.listSplit.Name = "listSplit";
		this.listSplit.Orientation = Orientation.Horizontal;
		// 
		// listSplit.Panel1
		// 
		this.listSplit.Panel1.Controls.Add( this.groupPanel );
		this.listSplit.Panel1.Cursor = Cursors.Default;
		this.listSplit.Panel1MinSize = 170;
		// 
		// listSplit.Panel2
		// 
		this.listSplit.Panel2.Controls.Add( this.layerPanel );
		this.listSplit.Panel2.Cursor = Cursors.Default;
		this.listSplit.Panel2MinSize = 130;
		this.listSplit.Size = new Size( 220, 479 );
		this.listSplit.SplitterDistance = 285;
		this.listSplit.SplitterWidth = 8;
		this.listSplit.TabIndex = 0;
		this.listSplit.Paint +=  this.ListSplit_Paint ;
		this.listSplit.MouseDown +=  this.ListSplit_MouseDown ;
		this.listSplit.MouseMove +=  this.ListSplit_MouseMove ;
		this.listSplit.MouseUp +=  this.ListSplit_MouseUp ;
		this.listSplit.MouseLeave +=  this.ListSplit_MouseLeave ;
		this.listSplit.Resize +=  this.ListSplit_Resize ;
		// 
		// groupPanel
		// 
		this.groupPanel.ColumnCount = 1;
		this.groupPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.groupPanel.Controls.Add( this.groupCaption, 0, 0 );
		this.groupPanel.Controls.Add( this.groupList, 0, 1 );
		this.groupPanel.Controls.Add( this.groupButtons, 0, 2 );
		this.groupPanel.Controls.Add( this.propertyPanel, 0, 3 );
		this.groupPanel.Dock = DockStyle.Fill;
		this.groupPanel.Location = new Point( 0, 0 );
		this.groupPanel.Margin = new Padding( 0 );
		this.groupPanel.Name = "groupPanel";
		this.groupPanel.RowCount = 4;
		this.groupPanel.RowStyles.Add( new RowStyle() );
		this.groupPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.groupPanel.RowStyles.Add( new RowStyle() );
		this.groupPanel.RowStyles.Add( new RowStyle() );
		this.groupPanel.Size = new Size( 220, 285 );
		this.groupPanel.TabIndex = 0;
		// 
		// groupCaption
		// 
		this.groupCaption.AutoSize = true;
		this.groupCaption.Location = new Point( 3, 0 );
		this.groupCaption.Margin = new Padding( 3, 0, 3, 3 );
		this.groupCaption.Name = "groupCaption";
		this.groupCaption.Size = new Size( 121, 15 );
		this.groupCaption.TabIndex = 0;
		this.groupCaption.Text = "グループ (チェックで表示)";
		// 
		// groupList
		// 
		this.groupList.CheckBoxes = true;
		this.groupList.Columns.AddRange( new ColumnHeader[] { this.groupColumn } );
		this.groupList.Dock = DockStyle.Fill;
		this.groupList.FullRowSelect = true;
		this.groupList.HeaderStyle = ColumnHeaderStyle.None;
		this.groupList.Location = new Point( 3, 21 );
		this.groupList.MinimumSize = new Size( 214, 50 );
		this.groupList.MultiSelect = false;
		this.groupList.Name = "groupList";
		this.groupList.Size = new Size( 214, 166 );
		this.groupList.SmallImageList = this.swatches;
		this.groupList.TabIndex = 1;
		this.groupList.UseCompatibleStateImageBehavior = false;
		this.groupList.View = View.Details;
		this.groupList.ItemChecked +=  this.GroupList_ItemChecked ;
		this.groupList.SelectedIndexChanged +=  this.GroupList_SelectedIndexChanged ;
		this.groupList.Resize +=  this.GroupList_Resize ;
		// 
		// groupColumn
		// 
		this.groupColumn.Width = 190;
		// 
		// swatches
		// 
		this.swatches.ColorDepth = ColorDepth.Depth32Bit;
		this.swatches.ImageSize = new Size( 14, 14 );
		this.swatches.TransparentColor = Color.Transparent;
		// 
		// groupButtons
		// 
		this.groupButtons.AutoSize = true;
		this.groupButtons.Controls.Add( this.addButton );
		this.groupButtons.Controls.Add( this.removeButton );
		this.groupButtons.Controls.Add( this.upButton );
		this.groupButtons.Controls.Add( this.downButton );
		this.groupButtons.Location = new Point( 0, 190 );
		this.groupButtons.Margin = new Padding( 0 );
		this.groupButtons.Name = "groupButtons";
		this.groupButtons.Size = new Size( 220, 31 );
		this.groupButtons.TabIndex = 2;
		this.groupButtons.WrapContents = false;
		// 
		// addButton
		// 
		this.addButton.AutoSize = true;
		this.addButton.Location = new Point( 3, 3 );
		this.addButton.Name = "addButton";
		this.addButton.Size = new Size( 60, 25 );
		this.addButton.TabIndex = 0;
		this.addButton.Text = "追加";
		this.addButton.UseVisualStyleBackColor = true;
		this.addButton.Click +=  this.AddButton_Click ;
		// 
		// removeButton
		// 
		this.removeButton.AutoSize = true;
		this.removeButton.Location = new Point( 69, 3 );
		this.removeButton.Name = "removeButton";
		this.removeButton.Size = new Size( 60, 25 );
		this.removeButton.TabIndex = 1;
		this.removeButton.Text = "削除";
		this.removeButton.UseVisualStyleBackColor = true;
		this.removeButton.Click +=  this.RemoveButton_Click ;
		// 
		// upButton
		// 
		this.upButton.Location = new Point( 135, 3 );
		this.upButton.Name = "upButton";
		this.upButton.Size = new Size( 38, 25 );
		this.upButton.TabIndex = 2;
		this.upButton.Text = "↑";
		this.upButton.UseVisualStyleBackColor = true;
		this.upButton.Click +=  this.UpButton_Click ;
		// 
		// downButton
		// 
		this.downButton.Location = new Point( 179, 3 );
		this.downButton.Name = "downButton";
		this.downButton.Size = new Size( 38, 25 );
		this.downButton.TabIndex = 3;
		this.downButton.Text = "↓";
		this.downButton.UseVisualStyleBackColor = true;
		this.downButton.Click +=  this.DownButton_Click ;
		// 
		// propertyPanel
		// 
		this.propertyPanel.Anchor =   AnchorStyles.Top  |  AnchorStyles.Left   |  AnchorStyles.Right ;
		this.propertyPanel.AutoSize = true;
		this.propertyPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.propertyPanel.ColumnCount = 3;
		this.propertyPanel.ColumnStyles.Add( new ColumnStyle() );
		this.propertyPanel.ColumnStyles.Add( new ColumnStyle() );
		this.propertyPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.propertyPanel.Controls.Add( this.nameCaption, 0, 0 );
		this.propertyPanel.Controls.Add( this.nameBox, 1, 0 );
		this.propertyPanel.Controls.Add( this.opacityCaption, 0, 1 );
		this.propertyPanel.Controls.Add( this.opacityPanel, 1, 1 );
		this.propertyPanel.Location = new Point( 0, 227 );
		this.propertyPanel.Margin = new Padding( 0, 6, 0, 0 );
		this.propertyPanel.Name = "propertyPanel";
		this.propertyPanel.RowCount = 2;
		this.propertyPanel.RowStyles.Add( new RowStyle() );
		this.propertyPanel.RowStyles.Add( new RowStyle() );
		this.propertyPanel.Size = new Size( 220, 58 );
		this.propertyPanel.TabIndex = 3;
		// 
		// nameCaption
		// 
		this.nameCaption.Anchor = AnchorStyles.Left;
		this.nameCaption.AutoSize = true;
		this.nameCaption.Location = new Point( 3, 7 );
		this.nameCaption.Name = "nameCaption";
		this.nameCaption.Size = new Size( 31, 15 );
		this.nameCaption.TabIndex = 0;
		this.nameCaption.Text = "名前";
		// 
		// nameBox
		// 
		this.nameBox.Anchor =  AnchorStyles.Left  |  AnchorStyles.Right ;
		this.propertyPanel.SetColumnSpan( this.nameBox, 2 );
		this.nameBox.Location = new Point( 64, 3 );
		this.nameBox.Name = "nameBox";
		this.nameBox.Size = new Size( 153, 23 );
		this.nameBox.TabIndex = 1;
		this.nameBox.TextChanged +=  this.NameBox_TextChanged ;
		// 
		// opacityCaption
		// 
		this.opacityCaption.Anchor = AnchorStyles.Left;
		this.opacityCaption.AutoSize = true;
		this.opacityCaption.Location = new Point( 3, 36 );
		this.opacityCaption.Name = "opacityCaption";
		this.opacityCaption.Size = new Size( 55, 15 );
		this.opacityCaption.TabIndex = 4;
		this.opacityCaption.Text = "不透明度";
		// 
		// opacityPanel
		// 
		this.opacityPanel.Anchor = AnchorStyles.Left;
		this.opacityPanel.AutoSize = true;
		this.opacityPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.propertyPanel.SetColumnSpan( this.opacityPanel, 2 );
		this.opacityPanel.Controls.Add( this.opacityCheck );
		this.opacityPanel.Controls.Add( this.opacityBox );
		this.opacityPanel.Controls.Add( this.opacityUnit );
		this.opacityPanel.Location = new Point( 61, 29 );
		this.opacityPanel.Margin = new Padding( 0 );
		this.opacityPanel.Name = "opacityPanel";
		this.opacityPanel.Size = new Size( 107, 29 );
		this.opacityPanel.TabIndex = 5;
		this.opacityPanel.WrapContents = false;
		// 
		// opacityCheck
		// 
		this.opacityCheck.Anchor = AnchorStyles.Left;
		this.opacityCheck.AutoSize = true;
		this.opacityCheck.Location = new Point( 3, 7 );
		this.opacityCheck.Name = "opacityCheck";
		this.opacityCheck.Size = new Size( 15, 14 );
		this.opacityCheck.TabIndex = 0;
		this.opacityCheck.UseVisualStyleBackColor = true;
		this.opacityCheck.CheckedChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// opacityBox
		// 
		this.opacityBox.Anchor = AnchorStyles.Left;
		this.opacityBox.Location = new Point( 24, 3 );
		this.opacityBox.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
		this.opacityBox.Name = "opacityBox";
		this.opacityBox.Size = new Size( 60, 23 );
		this.opacityBox.TabIndex = 1;
		this.opacityBox.Value = new decimal( new int[] { 10, 0, 0, 0 } );
		this.opacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// opacityUnit
		// 
		this.opacityUnit.Anchor = AnchorStyles.Left;
		this.opacityUnit.AutoSize = true;
		this.opacityUnit.Location = new Point( 87, 7 );
		this.opacityUnit.Margin = new Padding( 0, 0, 3, 0 );
		this.opacityUnit.Name = "opacityUnit";
		this.opacityUnit.Size = new Size( 17, 15 );
		this.opacityUnit.TabIndex = 2;
		this.opacityUnit.Text = "%";
		// 
		// layerPanel
		// 
		this.layerPanel.ColumnCount = 1;
		this.layerPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.layerPanel.Controls.Add( this.layerCaption, 0, 0 );
		this.layerPanel.Controls.Add( this.layerList, 0, 1 );
		this.layerPanel.Controls.Add( this.layerButtons, 0, 2 );
		this.layerPanel.Controls.Add( this.colorPanel, 0, 3 );
		this.layerPanel.Dock = DockStyle.Fill;
		this.layerPanel.Location = new Point( 0, 0 );
		this.layerPanel.Margin = new Padding( 0 );
		this.layerPanel.Name = "layerPanel";
		this.layerPanel.RowCount = 4;
		this.layerPanel.RowStyles.Add( new RowStyle() );
		this.layerPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.layerPanel.RowStyles.Add( new RowStyle() );
		this.layerPanel.RowStyles.Add( new RowStyle() );
		this.layerPanel.Size = new Size( 220, 186 );
		this.layerPanel.TabIndex = 0;
		// 
		// layerCaption
		// 
		this.layerCaption.AutoSize = true;
		this.layerCaption.Location = new Point( 3, 0 );
		this.layerCaption.Margin = new Padding( 3, 0, 3, 3 );
		this.layerCaption.Name = "layerCaption";
		this.layerCaption.Size = new Size( 43, 15 );
		this.layerCaption.TabIndex = 9;
		this.layerCaption.Text = "レイヤー";
		// 
		// layerList
		// 
		this.layerList.Columns.AddRange( new ColumnHeader[] { this.layerColumn } );
		this.layerList.Dock = DockStyle.Fill;
		this.layerList.FullRowSelect = true;
		this.layerList.HeaderStyle = ColumnHeaderStyle.None;
		this.layerList.Location = new Point( 3, 21 );
		this.layerList.MultiSelect = false;
		this.layerList.Name = "layerList";
		this.layerList.Size = new Size( 214, 102 );
		this.layerList.SmallImageList = this.layerSwatches;
		this.layerList.TabIndex = 10;
		this.layerList.UseCompatibleStateImageBehavior = false;
		this.layerList.View = View.Details;
		this.layerList.SelectedIndexChanged +=  this.LayerList_SelectedIndexChanged ;
		this.layerList.DoubleClick +=  this.ColorButton_Click ;
		this.layerList.Resize +=  this.LayerList_Resize ;
		// 
		// layerColumn
		// 
		this.layerColumn.Width = 190;
		// 
		// layerSwatches
		// 
		this.layerSwatches.ColorDepth = ColorDepth.Depth32Bit;
		this.layerSwatches.ImageSize = new Size( 14, 14 );
		this.layerSwatches.TransparentColor = Color.Transparent;
		// 
		// layerButtons
		// 
		this.layerButtons.AutoSize = true;
		this.layerButtons.Controls.Add( this.layerAddButton );
		this.layerButtons.Controls.Add( this.layerRemoveButton );
		this.layerButtons.Controls.Add( this.layerUpButton );
		this.layerButtons.Controls.Add( this.layerDownButton );
		this.layerButtons.Location = new Point( 0, 126 );
		this.layerButtons.Margin = new Padding( 0 );
		this.layerButtons.Name = "layerButtons";
		this.layerButtons.Size = new Size( 220, 31 );
		this.layerButtons.TabIndex = 11;
		this.layerButtons.WrapContents = false;
		// 
		// layerAddButton
		// 
		this.layerAddButton.AutoSize = true;
		this.layerAddButton.Location = new Point( 3, 3 );
		this.layerAddButton.Name = "layerAddButton";
		this.layerAddButton.Size = new Size( 60, 25 );
		this.layerAddButton.TabIndex = 0;
		this.layerAddButton.Text = "追加";
		this.shapeTip.SetToolTip( this.layerAddButton, "別の色のレイヤーを追加" );
		this.layerAddButton.UseVisualStyleBackColor = true;
		this.layerAddButton.Click +=  this.LayerAddButton_Click ;
		// 
		// layerRemoveButton
		// 
		this.layerRemoveButton.AutoSize = true;
		this.layerRemoveButton.Location = new Point( 69, 3 );
		this.layerRemoveButton.Name = "layerRemoveButton";
		this.layerRemoveButton.Size = new Size( 60, 25 );
		this.layerRemoveButton.TabIndex = 1;
		this.layerRemoveButton.Text = "削除";
		this.layerRemoveButton.UseVisualStyleBackColor = true;
		this.layerRemoveButton.Click +=  this.LayerRemoveButton_Click ;
		// 
		// layerUpButton
		// 
		this.layerUpButton.Location = new Point( 135, 3 );
		this.layerUpButton.Name = "layerUpButton";
		this.layerUpButton.Size = new Size( 38, 25 );
		this.layerUpButton.TabIndex = 2;
		this.layerUpButton.Text = "↑";
		this.layerUpButton.UseVisualStyleBackColor = true;
		this.layerUpButton.Click +=  this.LayerUpButton_Click ;
		// 
		// layerDownButton
		// 
		this.layerDownButton.Location = new Point( 179, 3 );
		this.layerDownButton.Name = "layerDownButton";
		this.layerDownButton.Size = new Size( 38, 25 );
		this.layerDownButton.TabIndex = 3;
		this.layerDownButton.Text = "↓";
		this.layerDownButton.UseVisualStyleBackColor = true;
		this.layerDownButton.Click +=  this.LayerDownButton_Click ;
		// 
		// colorPanel
		// 
		this.colorPanel.AutoSize = true;
		this.colorPanel.Controls.Add( this.colorCaption );
		this.colorPanel.Controls.Add( this.colorButton );
		this.colorPanel.Location = new Point( 0, 157 );
		this.colorPanel.Margin = new Padding( 0 );
		this.colorPanel.Name = "colorPanel";
		this.colorPanel.Size = new Size( 117, 29 );
		this.colorPanel.TabIndex = 12;
		this.colorPanel.WrapContents = false;
		// 
		// colorCaption
		// 
		this.colorCaption.Anchor = AnchorStyles.Left;
		this.colorCaption.AutoSize = true;
		this.colorCaption.Location = new Point( 3, 7 );
		this.colorCaption.Name = "colorCaption";
		this.colorCaption.Size = new Size( 65, 15 );
		this.colorCaption.TabIndex = 0;
		this.colorCaption.Text = "レイヤーの色";
		// 
		// colorButton
		// 
		this.colorButton.Anchor = AnchorStyles.Left;
		this.colorButton.FlatStyle = FlatStyle.Flat;
		this.colorButton.Location = new Point( 74, 3 );
		this.colorButton.Name = "colorButton";
		this.colorButton.Size = new Size( 40, 23 );
		this.colorButton.TabIndex = 1;
		this.colorButton.UseVisualStyleBackColor = false;
		this.colorButton.Click +=  this.ColorButton_Click ;
		// 
		// shapeCaption
		// 
		this.shapeCaption.AutoSize = true;
		this.shapeCaption.Location = new Point( 3, 485 );
		this.shapeCaption.Margin = new Padding( 3, 6, 3, 3 );
		this.shapeCaption.Name = "shapeCaption";
		this.shapeCaption.Size = new Size( 38, 15 );
		this.shapeCaption.TabIndex = 6;
		this.shapeCaption.Text = "描く形";
		// 
		// shapePanel
		// 
		this.shapePanel.AutoSize = true;
		this.shapePanel.Location = new Point( 0, 503 );
		this.shapePanel.Margin = new Padding( 0 );
		this.shapePanel.MaximumSize = new Size( 220, 0 );
		this.shapePanel.Name = "shapePanel";
		this.shapePanel.Size = new Size( 0, 0 );
		this.shapePanel.TabIndex = 7;
		// 
		// thicknessPanel
		// 
		this.thicknessPanel.AutoSize = true;
		this.thicknessPanel.Controls.Add( this.thicknessCaption );
		this.thicknessPanel.Controls.Add( this.thicknessBox );
		this.thicknessPanel.Location = new Point( 0, 506 );
		this.thicknessPanel.Margin = new Padding( 0, 3, 0, 0 );
		this.thicknessPanel.Name = "thicknessPanel";
		this.thicknessPanel.Size = new Size( 140, 29 );
		this.thicknessPanel.TabIndex = 8;
		// 
		// thicknessCaption
		// 
		this.thicknessCaption.Anchor = AnchorStyles.Left;
		this.thicknessCaption.AutoSize = true;
		this.thicknessCaption.Location = new Point( 3, 7 );
		this.thicknessCaption.Name = "thicknessCaption";
		this.thicknessCaption.Size = new Size( 49, 15 );
		this.thicknessCaption.TabIndex = 0;
		this.thicknessCaption.Text = "線の太さ";
		// 
		// thicknessBox
		// 
		this.thicknessBox.DropDownStyle = ComboBoxStyle.DropDownList;
		this.thicknessBox.Location = new Point( 58, 3 );
		this.thicknessBox.Name = "thicknessBox";
		this.thicknessBox.Size = new Size( 79, 23 );
		this.thicknessBox.TabIndex = 1;
		this.thicknessBox.SelectedIndexChanged +=  this.ThicknessBox_SelectedIndexChanged ;
		// 
		// helpLabel
		// 
		this.helpLabel.AutoSize = true;
		this.helpLabel.ForeColor = Color.FromArgb( 117, 117, 117 );
		this.helpLabel.Location = new Point( 3, 541 );
		this.helpLabel.Margin = new Padding( 3, 6, 3, 0 );
		this.helpLabel.MaximumSize = new Size( 214, 0 );
		this.helpLabel.Name = "helpLabel";
		this.helpLabel.Size = new Size( 181, 60 );
		this.helpLabel.TabIndex = 5;
		this.helpLabel.Text = "左クリック: 描く　右クリック: 消す\r\nホイール: 拡大・縮小\r\nホイールドラッグ / Space: 移動\r\nCtrl+Z / Ctrl+Y: 元に戻す / やり直し";
		// 
		// toolPanel
		// 
		this.toolPanel.AutoSize = true;
		this.toolPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.toolPanel.ColumnCount = 3;
		this.toolPanel.ColumnStyles.Add( new ColumnStyle() );
		this.toolPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.toolPanel.ColumnStyles.Add( new ColumnStyle() );
		this.toolPanel.Controls.Add( this.undoPanel, 0, 0 );
		this.toolPanel.Controls.Add( this.zoomPanel, 2, 0 );
		this.toolPanel.Dock = DockStyle.Fill;
		this.toolPanel.Location = new Point( 238, 6 );
		this.toolPanel.Margin = new Padding( 0 );
		this.toolPanel.Name = "toolPanel";
		this.toolPanel.RowCount = 1;
		this.toolPanel.RowStyles.Add( new RowStyle() );
		this.toolPanel.Size = new Size( 656, 32 );
		this.toolPanel.TabIndex = 3;
		// 
		// undoPanel
		// 
		this.undoPanel.AutoSize = true;
		this.undoPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.undoPanel.Controls.Add( this.undoButton );
		this.undoPanel.Controls.Add( this.redoButton );
		this.undoPanel.Location = new Point( 0, 0 );
		this.undoPanel.Margin = new Padding( 0 );
		this.undoPanel.Name = "undoPanel";
		this.undoPanel.Size = new Size( 162, 31 );
		this.undoPanel.TabIndex = 1;
		this.undoPanel.WrapContents = false;
		// 
		// undoButton
		// 
		this.undoButton.AutoSize = true;
		this.undoButton.Enabled = false;
		this.undoButton.Location = new Point( 3, 3 );
		this.undoButton.Name = "undoButton";
		this.undoButton.Size = new Size( 75, 25 );
		this.undoButton.TabIndex = 0;
		this.undoButton.Text = "元に戻す";
		this.shapeTip.SetToolTip( this.undoButton, "元に戻す (Ctrl+Z)" );
		this.undoButton.UseVisualStyleBackColor = true;
		this.undoButton.Click +=  this.UndoButton_Click ;
		// 
		// redoButton
		// 
		this.redoButton.AutoSize = true;
		this.redoButton.Enabled = false;
		this.redoButton.Location = new Point( 84, 3 );
		this.redoButton.Name = "redoButton";
		this.redoButton.Size = new Size( 75, 25 );
		this.redoButton.TabIndex = 1;
		this.redoButton.Text = "やり直し";
		this.shapeTip.SetToolTip( this.redoButton, "やり直し (Ctrl+Y)" );
		this.redoButton.UseVisualStyleBackColor = true;
		this.redoButton.Click +=  this.RedoButton_Click ;
		// 
		// zoomPanel
		// 
		this.zoomPanel.Anchor = AnchorStyles.Right;
		this.zoomPanel.AutoSize = true;
		this.zoomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		this.zoomPanel.Controls.Add( this.zoomOutButton );
		this.zoomPanel.Controls.Add( this.zoomResetButton );
		this.zoomPanel.Controls.Add( this.zoomInButton );
		this.zoomPanel.Location = new Point( 524, 0 );
		this.zoomPanel.Margin = new Padding( 0 );
		this.zoomPanel.Name = "zoomPanel";
		this.zoomPanel.Size = new Size( 132, 32 );
		this.zoomPanel.TabIndex = 0;
		this.zoomPanel.WrapContents = false;
		// 
		// zoomOutButton
		// 
		this.zoomOutButton.Location = new Point( 3, 3 );
		this.zoomOutButton.Name = "zoomOutButton";
		this.zoomOutButton.Size = new Size( 30, 26 );
		this.zoomOutButton.TabIndex = 0;
		this.zoomOutButton.Text = "-";
		this.zoomOutButton.UseVisualStyleBackColor = true;
		this.zoomOutButton.Click +=  this.ZoomOutButton_Click ;
		// 
		// zoomResetButton
		// 
		this.zoomResetButton.Location = new Point( 39, 3 );
		this.zoomResetButton.Name = "zoomResetButton";
		this.zoomResetButton.Size = new Size( 54, 26 );
		this.zoomResetButton.TabIndex = 1;
		this.zoomResetButton.Text = "100%";
		this.shapeTip.SetToolTip( this.zoomResetButton, "100% に戻す" );
		this.zoomResetButton.UseVisualStyleBackColor = true;
		this.zoomResetButton.Click +=  this.ZoomResetButton_Click ;
		// 
		// zoomInButton
		// 
		this.zoomInButton.Location = new Point( 99, 3 );
		this.zoomInButton.Name = "zoomInButton";
		this.zoomInButton.Size = new Size( 30, 26 );
		this.zoomInButton.TabIndex = 2;
		this.zoomInButton.Text = "+";
		this.zoomInButton.UseVisualStyleBackColor = true;
		this.zoomInButton.Click +=  this.ZoomInButton_Click ;
		// 
		// canvas
		// 
		this.canvas.BackColor = Color.FromArgb( 32, 32, 32 );
		this.canvas.Dock = DockStyle.Fill;
		this.canvas.Location = new Point( 241, 41 );
		this.canvas.MinimumSize = new Size( 320, 200 );
		this.canvas.Name = "canvas";
		this.canvas.Size = new Size( 650, 533 );
		this.canvas.TabIndex = 1;
		this.canvas.CellsChanged +=  this.Canvas_CellsChanged ;
		this.canvas.StrokeStarting +=  this.Canvas_StrokeStarting ;
		this.canvas.HoverChanged +=  this.Canvas_HoverChanged ;
		this.canvas.ZoomChanged +=  this.Canvas_ZoomChanged ;
		// 
		// bottomPanel
		// 
		this.bottomPanel.AutoSize = true;
		this.bottomPanel.ColumnCount = 5;
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.Controls.Add( this.gameCheck, 0, 0 );
		this.bottomPanel.Controls.Add( this.recaptureButton, 1, 0 );
		this.bottomPanel.Controls.Add( this.statusLabel, 2, 0 );
		this.bottomPanel.Controls.Add( this.okButton, 3, 0 );
		this.bottomPanel.Controls.Add( this.cancelButton, 4, 0 );
		this.bottomPanel.Dock = DockStyle.Fill;
		this.bottomPanel.Location = new Point( 238, 577 );
		this.bottomPanel.Margin = new Padding( 0 );
		this.bottomPanel.Name = "bottomPanel";
		this.bottomPanel.RowCount = 1;
		this.bottomPanel.RowStyles.Add( new RowStyle() );
		this.bottomPanel.RowStyles.Add( new RowStyle( SizeType.Absolute, 20F ) );
		this.bottomPanel.Size = new Size( 656, 36 );
		this.bottomPanel.TabIndex = 2;
		// 
		// gameCheck
		// 
		this.gameCheck.Anchor = AnchorStyles.Left;
		this.gameCheck.AutoSize = true;
		this.gameCheck.Location = new Point( 3, 8 );
		this.gameCheck.Name = "gameCheck";
		this.gameCheck.Size = new Size( 110, 19 );
		this.gameCheck.TabIndex = 0;
		this.gameCheck.Text = "背景にゲーム画面";
		this.gameCheck.UseVisualStyleBackColor = true;
		this.gameCheck.CheckedChanged +=  this.GameCheck_CheckedChanged ;
		// 
		// recaptureButton
		// 
		this.recaptureButton.Anchor = AnchorStyles.Left;
		this.recaptureButton.AutoSize = true;
		this.recaptureButton.Location = new Point( 119, 5 );
		this.recaptureButton.Margin = new Padding( 3, 3, 12, 3 );
		this.recaptureButton.Name = "recaptureButton";
		this.recaptureButton.Size = new Size( 75, 25 );
		this.recaptureButton.TabIndex = 1;
		this.recaptureButton.Text = "撮り直す";
		this.recaptureButton.UseVisualStyleBackColor = true;
		this.recaptureButton.Click +=  this.RecaptureButton_Click ;
		// 
		// statusLabel
		// 
		this.statusLabel.Anchor = AnchorStyles.Left;
		this.statusLabel.AutoSize = true;
		this.statusLabel.Location = new Point( 209, 10 );
		this.statusLabel.Name = "statusLabel";
		this.statusLabel.Size = new Size( 12, 15 );
		this.statusLabel.TabIndex = 2;
		this.statusLabel.Text = "-";
		// 
		// okButton
		// 
		this.okButton.AutoSize = true;
		this.okButton.DialogResult = DialogResult.OK;
		this.okButton.Location = new Point( 497, 3 );
		this.okButton.Name = "okButton";
		this.okButton.Size = new Size( 75, 30 );
		this.okButton.TabIndex = 3;
		this.okButton.Text = "OK";
		this.okButton.UseVisualStyleBackColor = true;
		// 
		// cancelButton
		// 
		this.cancelButton.AutoSize = true;
		this.cancelButton.DialogResult = DialogResult.Cancel;
		this.cancelButton.Location = new Point( 578, 3 );
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new Size( 75, 30 );
		this.cancelButton.TabIndex = 4;
		this.cancelButton.Text = "キャンセル";
		this.cancelButton.UseVisualStyleBackColor = true;
		// 
		// CustomTilesForm
		// 
		this.AutoScaleDimensions = new SizeF( 96F, 96F );
		this.AutoScaleMode = AutoScaleMode.Dpi;
		this.CancelButton = this.cancelButton;
		this.ClientSize = new Size( 900, 619 );
		this.Controls.Add( this.mainPanel );
		this.Font = new Font( "Yu Gothic UI", 9F );
		this.MinimizeBox = false;
		this.MinimumSize = new Size( 660, 600 );
		this.Name = "CustomTilesForm";
		this.ShowInTaskbar = false;
		this.StartPosition = FormStartPosition.CenterScreen;
		this.Text = "カスタムのマス";
		this.mainPanel.ResumeLayout( false );
		this.mainPanel.PerformLayout();
		this.sidePanel.ResumeLayout( false );
		this.sidePanel.PerformLayout();
		this.listSplit.Panel1.ResumeLayout( false );
		this.listSplit.Panel2.ResumeLayout( false );
		( (System.ComponentModel.ISupportInitialize)this.listSplit ).EndInit();
		this.listSplit.ResumeLayout( false );
		this.groupPanel.ResumeLayout( false );
		this.groupPanel.PerformLayout();
		this.groupButtons.ResumeLayout( false );
		this.groupButtons.PerformLayout();
		this.propertyPanel.ResumeLayout( false );
		this.propertyPanel.PerformLayout();
		this.opacityPanel.ResumeLayout( false );
		this.opacityPanel.PerformLayout();
		( this.opacityBox ).EndInit();
		this.layerPanel.ResumeLayout( false );
		this.layerPanel.PerformLayout();
		this.layerButtons.ResumeLayout( false );
		this.layerButtons.PerformLayout();
		this.colorPanel.ResumeLayout( false );
		this.colorPanel.PerformLayout();
		this.thicknessPanel.ResumeLayout( false );
		this.thicknessPanel.PerformLayout();
		this.toolPanel.ResumeLayout( false );
		this.toolPanel.PerformLayout();
		this.undoPanel.ResumeLayout( false );
		this.undoPanel.PerformLayout();
		this.zoomPanel.ResumeLayout( false );
		this.bottomPanel.ResumeLayout( false );
		this.bottomPanel.PerformLayout();
		this.ResumeLayout( false );
	}

	#endregion

	private TableLayoutPanel mainPanel;
	private TableLayoutPanel sidePanel;
	private SplitContainer listSplit;
	private TableLayoutPanel groupPanel;
	private TableLayoutPanel layerPanel;
	private Label groupCaption;
	private ListView groupList;
	private ColumnHeader groupColumn;
	private ImageList swatches;
	private FlowLayoutPanel groupButtons;
	private Button addButton;
	private Button removeButton;
	private Button upButton;
	private Button downButton;
	private TableLayoutPanel propertyPanel;
	private Label layerCaption;
	private ListView layerList;
	private ColumnHeader layerColumn;
	private ImageList layerSwatches;
	private FlowLayoutPanel layerButtons;
	private Button layerAddButton;
	private Button layerRemoveButton;
	private Button layerUpButton;
	private Button layerDownButton;
	private FlowLayoutPanel colorPanel;
	private Label nameCaption;
	private TextBox nameBox;
	private Label colorCaption;
	private Button colorButton;
	private Label opacityCaption;
	private FlowLayoutPanel opacityPanel;
	private CheckBox opacityCheck;
	private StepNumericUpDown opacityBox;
	private Label opacityUnit;
	private Label shapeCaption;
	private FlowLayoutPanel shapePanel;
	private FlowLayoutPanel thicknessPanel;
	private Label thicknessCaption;
	private ComboBox thicknessBox;
	private ToolTip shapeTip;
	private Label helpLabel;
	private CustomTilesCanvas canvas;
	private TableLayoutPanel toolPanel;
	private FlowLayoutPanel zoomPanel;
	private Button zoomOutButton;
	private Button zoomResetButton;
	private Button zoomInButton;
	private TableLayoutPanel bottomPanel;
	private Label statusLabel;
	private CheckBox gameCheck;
	private Button recaptureButton;
	private FlowLayoutPanel undoPanel;
	private Button undoButton;
	private Button redoButton;
	private Button okButton;
	private Button cancelButton;
}
