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
		this.colorCaption = new Label();
		this.colorButton = new Button();
		this.opacityCaption = new Label();
		this.opacityBox = new NumericUpDown();
		this.opacityUnit = new Label();
		this.shapeCaption = new Label();
		this.shapePanel = new FlowLayoutPanel();
		this.thicknessPanel = new FlowLayoutPanel();
		this.thicknessCaption = new Label();
		this.thicknessBox = new ComboBox();
		this.clearButton = new Button();
		this.helpLabel = new Label();
		this.canvas = new CustomTilesCanvas();
		this.bottomPanel = new TableLayoutPanel();
		this.gameCheck = new CheckBox();
		this.recaptureButton = new Button();
		this.statusLabel = new Label();
		this.closeButton = new Button();
		this.shapeTip = new ToolTip( this.components );
		this.mainPanel.SuspendLayout();
		this.sidePanel.SuspendLayout();
		this.groupButtons.SuspendLayout();
		this.propertyPanel.SuspendLayout();
		this.thicknessPanel.SuspendLayout();
		( (System.ComponentModel.ISupportInitialize)this.opacityBox ).BeginInit();
		this.bottomPanel.SuspendLayout();
		this.SuspendLayout();
		// 
		// mainPanel
		// 
		this.mainPanel.ColumnCount = 2;
		this.mainPanel.ColumnStyles.Add( new ColumnStyle() );
		this.mainPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.mainPanel.Controls.Add( this.sidePanel, 0, 0 );
		this.mainPanel.Controls.Add( this.canvas, 1, 0 );
		this.mainPanel.Controls.Add( this.bottomPanel, 0, 1 );
		this.mainPanel.Dock = DockStyle.Fill;
		this.mainPanel.Location = new Point( 0, 0 );
		this.mainPanel.Name = "mainPanel";
		this.mainPanel.Padding = new Padding( 6 );
		this.mainPanel.RowCount = 2;
		this.mainPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.mainPanel.RowStyles.Add( new RowStyle() );
		this.mainPanel.Size = new Size( 900, 660 );
		this.mainPanel.TabIndex = 0;
		// 
		// sidePanel
		// 
		this.sidePanel.AutoSize = true;
		this.sidePanel.ColumnCount = 1;
		this.sidePanel.ColumnStyles.Add( new ColumnStyle() );
		this.sidePanel.Controls.Add( this.groupCaption, 0, 0 );
		this.sidePanel.Controls.Add( this.groupList, 0, 1 );
		this.sidePanel.Controls.Add( this.groupButtons, 0, 2 );
		this.sidePanel.Controls.Add( this.propertyPanel, 0, 3 );
		this.sidePanel.Controls.Add( this.shapeCaption, 0, 4 );
		this.sidePanel.Controls.Add( this.shapePanel, 0, 5 );
		this.sidePanel.Controls.Add( this.thicknessPanel, 0, 6 );
		this.sidePanel.Controls.Add( this.clearButton, 0, 7 );
		this.sidePanel.Controls.Add( this.helpLabel, 0, 8 );
		this.sidePanel.Dock = DockStyle.Fill;
		this.sidePanel.Location = new Point( 9, 9 );
		this.sidePanel.Margin = new Padding( 3, 3, 9, 3 );
		this.sidePanel.Name = "sidePanel";
		this.sidePanel.RowCount = 9;
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.RowStyles.Add( new RowStyle() );
		this.sidePanel.Size = new Size( 220, 606 );
		this.sidePanel.TabIndex = 0;
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
		this.groupList.MinimumSize = new Size( 214, 90 );
		this.groupList.MultiSelect = false;
		this.groupList.Name = "groupList";
		this.groupList.Size = new Size( 214, 292 );
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
		this.groupButtons.Location = new Point( 0, 316 );
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
		this.propertyPanel.Controls.Add( this.colorCaption, 0, 1 );
		this.propertyPanel.Controls.Add( this.colorButton, 1, 1 );
		this.propertyPanel.Controls.Add( this.opacityCaption, 0, 2 );
		this.propertyPanel.Controls.Add( this.opacityBox, 1, 2 );
		this.propertyPanel.Controls.Add( this.opacityUnit, 2, 2 );
		this.propertyPanel.Location = new Point( 0, 353 );
		this.propertyPanel.Margin = new Padding( 0, 6, 0, 0 );
		this.propertyPanel.Name = "propertyPanel";
		this.propertyPanel.RowCount = 3;
		this.propertyPanel.RowStyles.Add( new RowStyle() );
		this.propertyPanel.RowStyles.Add( new RowStyle() );
		this.propertyPanel.RowStyles.Add( new RowStyle() );
		this.propertyPanel.Size = new Size( 220, 87 );
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
		// colorCaption
		// 
		this.colorCaption.Anchor = AnchorStyles.Left;
		this.colorCaption.AutoSize = true;
		this.colorCaption.Location = new Point( 3, 36 );
		this.colorCaption.Name = "colorCaption";
		this.colorCaption.Size = new Size( 19, 15 );
		this.colorCaption.TabIndex = 2;
		this.colorCaption.Text = "色";
		// 
		// colorButton
		// 
		this.colorButton.Anchor = AnchorStyles.Left;
		this.colorButton.FlatStyle = FlatStyle.Flat;
		this.colorButton.Location = new Point( 64, 32 );
		this.colorButton.Name = "colorButton";
		this.colorButton.Size = new Size( 40, 23 );
		this.colorButton.TabIndex = 3;
		this.colorButton.UseVisualStyleBackColor = false;
		this.colorButton.Click +=  this.ColorButton_Click ;
		// 
		// opacityCaption
		// 
		this.opacityCaption.Anchor = AnchorStyles.Left;
		this.opacityCaption.AutoSize = true;
		this.opacityCaption.Location = new Point( 3, 65 );
		this.opacityCaption.Name = "opacityCaption";
		this.opacityCaption.Size = new Size( 55, 15 );
		this.opacityCaption.TabIndex = 4;
		this.opacityCaption.Text = "不透明度";
		// 
		// opacityBox
		// 
		this.opacityBox.Anchor = AnchorStyles.Left;
		this.opacityBox.Location = new Point( 64, 61 );
		this.opacityBox.Name = "opacityBox";
		this.opacityBox.Size = new Size( 60, 23 );
		this.opacityBox.TabIndex = 5;
		this.opacityBox.ValueChanged +=  this.OpacityBox_ValueChanged ;
		// 
		// opacityUnit
		// 
		this.opacityUnit.Anchor = AnchorStyles.Left;
		this.opacityUnit.AutoSize = true;
		this.opacityUnit.Location = new Point( 130, 65 );
		this.opacityUnit.Name = "opacityUnit";
		this.opacityUnit.Size = new Size( 17, 15 );
		this.opacityUnit.TabIndex = 6;
		this.opacityUnit.Text = "%";
		// 
		// shapeCaption
		// 
		this.shapeCaption.AutoSize = true;
		this.shapeCaption.Location = new Point( 3, 446 );
		this.shapeCaption.Margin = new Padding( 3, 6, 3, 3 );
		this.shapeCaption.Name = "shapeCaption";
		this.shapeCaption.Size = new Size( 38, 15 );
		this.shapeCaption.TabIndex = 6;
		this.shapeCaption.Text = "描く形";
		// 
		// shapePanel
		// 
		this.shapePanel.AutoSize = true;
		this.shapePanel.Location = new Point( 0, 464 );
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
		this.thicknessPanel.Location = new Point( 0, 467 );
		this.thicknessPanel.Margin = new Padding( 0, 3, 0, 0 );
		this.thicknessPanel.Name = "thicknessPanel";
		this.thicknessPanel.Size = new Size( 146, 29 );
		this.thicknessPanel.TabIndex = 8;
		// 
		// thicknessCaption
		// 
		this.thicknessCaption.Anchor = AnchorStyles.Left;
		this.thicknessCaption.AutoSize = true;
		this.thicknessCaption.Location = new Point( 3, 7 );
		this.thicknessCaption.Name = "thicknessCaption";
		this.thicknessCaption.Size = new Size( 55, 15 );
		this.thicknessCaption.TabIndex = 0;
		this.thicknessCaption.Text = "線の太さ";
		// 
		// thicknessBox
		// 
		this.thicknessBox.DropDownStyle = ComboBoxStyle.DropDownList;
		this.thicknessBox.Location = new Point( 64, 3 );
		this.thicknessBox.Name = "thicknessBox";
		this.thicknessBox.Size = new Size( 79, 23 );
		this.thicknessBox.TabIndex = 1;
		this.thicknessBox.SelectedIndexChanged +=  this.ThicknessBox_SelectedIndexChanged ;
		// 
		// clearButton
		// 
		this.clearButton.AutoSize = true;
		this.clearButton.Location = new Point( 3, 467 );
		this.clearButton.Name = "clearButton";
		this.clearButton.Size = new Size( 120, 25 );
		this.clearButton.TabIndex = 4;
		this.clearButton.Text = "マスを全部消す...";
		this.clearButton.UseVisualStyleBackColor = true;
		this.clearButton.Click +=  this.ClearButton_Click ;
		// 
		// helpLabel
		// 
		this.helpLabel.AutoSize = true;
		this.helpLabel.ForeColor = Color.FromArgb( 117, 117, 117 );
		this.helpLabel.Location = new Point( 3, 501 );
		this.helpLabel.Margin = new Padding( 3, 6, 3, 0 );
		this.helpLabel.MaximumSize = new Size( 214, 0 );
		this.helpLabel.Name = "helpLabel";
		this.helpLabel.Size = new Size( 207, 105 );
		this.helpLabel.TabIndex = 5;
		this.helpLabel.Text = "左クリック: 描く\r\n右クリック: 消す\r\nホイール: 拡大・縮小\r\nホイールドラッグ: 動かす\r\n\r\n黄色の枠がキャラクターのいるマスです。\r\n描いたマスはキャラクターと一緒に動きます。";
		// 
		// canvas
		// 
		this.canvas.BackColor = Color.FromArgb( 32, 32, 32 );
		this.canvas.Dock = DockStyle.Fill;
		this.canvas.Location = new Point( 241, 9 );
		this.canvas.MinimumSize = new Size( 320, 200 );
		this.canvas.Name = "canvas";
		this.canvas.Size = new Size( 650, 606 );
		this.canvas.TabIndex = 1;
		this.canvas.CellsChanged +=  this.Canvas_CellsChanged ;
		this.canvas.HoverChanged +=  this.Canvas_HoverChanged ;
		// 
		// bottomPanel
		// 
		this.bottomPanel.AutoSize = true;
		this.bottomPanel.ColumnCount = 4;
		this.mainPanel.SetColumnSpan( this.bottomPanel, 2 );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.bottomPanel.ColumnStyles.Add( new ColumnStyle() );
		this.bottomPanel.Controls.Add( this.gameCheck, 0, 0 );
		this.bottomPanel.Controls.Add( this.recaptureButton, 1, 0 );
		this.bottomPanel.Controls.Add( this.statusLabel, 2, 0 );
		this.bottomPanel.Controls.Add( this.closeButton, 3, 0 );
		this.bottomPanel.Dock = DockStyle.Fill;
		this.bottomPanel.Location = new Point( 6, 618 );
		this.bottomPanel.Margin = new Padding( 0 );
		this.bottomPanel.Name = "bottomPanel";
		this.bottomPanel.RowCount = 1;
		this.bottomPanel.RowStyles.Add( new RowStyle() );
		this.bottomPanel.Size = new Size( 888, 36 );
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
		// closeButton
		// 
		this.closeButton.AutoSize = true;
		this.closeButton.DialogResult = DialogResult.Cancel;
		this.closeButton.Location = new Point( 810, 3 );
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new Size( 75, 30 );
		this.closeButton.TabIndex = 3;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click +=  this.CloseButton_Click ;
		// 
		// CustomTilesForm
		// 
		this.AutoScaleDimensions = new SizeF( 96F, 96F );
		this.AutoScaleMode = AutoScaleMode.Dpi;
		this.CancelButton = this.closeButton;
		this.ClientSize = new Size( 900, 660 );
		this.Controls.Add( this.mainPanel );
		this.Font = new Font( "Yu Gothic UI", 9F );
		this.MinimizeBox = false;
		this.MinimumSize = new Size( 620, 660 );
		this.Name = "CustomTilesForm";
		this.ShowInTaskbar = false;
		this.StartPosition = FormStartPosition.CenterScreen;
		this.Text = "カスタムのマス";
		this.mainPanel.ResumeLayout( false );
		this.mainPanel.PerformLayout();
		this.sidePanel.ResumeLayout( false );
		this.sidePanel.PerformLayout();
		this.groupButtons.ResumeLayout( false );
		this.groupButtons.PerformLayout();
		this.propertyPanel.ResumeLayout( false );
		this.propertyPanel.PerformLayout();
		this.thicknessPanel.ResumeLayout( false );
		this.thicknessPanel.PerformLayout();
		( (System.ComponentModel.ISupportInitialize)this.opacityBox ).EndInit();
		this.bottomPanel.ResumeLayout( false );
		this.bottomPanel.PerformLayout();
		this.ResumeLayout( false );
	}

	#endregion

	private TableLayoutPanel mainPanel;
	private TableLayoutPanel sidePanel;
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
	private Label nameCaption;
	private TextBox nameBox;
	private Label colorCaption;
	private Button colorButton;
	private Label opacityCaption;
	private NumericUpDown opacityBox;
	private Label opacityUnit;
	private Label shapeCaption;
	private FlowLayoutPanel shapePanel;
	private FlowLayoutPanel thicknessPanel;
	private Label thicknessCaption;
	private ComboBox thicknessBox;
	private ToolTip shapeTip;
	private Button clearButton;
	private Label helpLabel;
	private CustomTilesCanvas canvas;
	private TableLayoutPanel bottomPanel;
	private Label statusLabel;
	private CheckBox gameCheck;
	private Button recaptureButton;
	private Button closeButton;
}
