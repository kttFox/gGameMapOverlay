namespace gGameMapOverlay;

partial class InfoForm
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
		this.infoPanel = new TableLayoutPanel();
		this.motionCaption = new Label();
		this.motionValue = new Label();
		this.eventLogCaption = new Label();
		this.eventLogBox = new TextBox();
		this.buttonPanel = new FlowLayoutPanel();
		this.closeButton = new Button();
		this.clearLogButton = new Button();
		this.copyLogButton = new Button();
		this.pinLogCheck = new CheckBox();
		this.infoTimer = new System.Windows.Forms.Timer( this.components );
		this.infoPanel.SuspendLayout();
		this.buttonPanel.SuspendLayout();
		this.SuspendLayout();
		// 
		// infoPanel
		// 
		this.infoPanel.ColumnCount = 1;
		this.infoPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
		this.infoPanel.Controls.Add( this.motionCaption, 0, 0 );
		this.infoPanel.Controls.Add( this.motionValue, 0, 1 );
		this.infoPanel.Controls.Add( this.eventLogCaption, 0, 4 );
		this.infoPanel.Controls.Add( this.eventLogBox, 0, 5 );
		this.infoPanel.Controls.Add( this.buttonPanel, 0, 6 );
		this.infoPanel.Dock = DockStyle.Fill;
		this.infoPanel.Location = new Point( 0, 0 );
		this.infoPanel.Margin = new Padding( 0 );
		this.infoPanel.Name = "infoPanel";
		this.infoPanel.RowCount = 7;
		this.infoPanel.RowStyles.Add( new RowStyle() );
		this.infoPanel.RowStyles.Add( new RowStyle() );
		this.infoPanel.RowStyles.Add( new RowStyle() );
		this.infoPanel.RowStyles.Add( new RowStyle() );
		this.infoPanel.RowStyles.Add( new RowStyle() );
		this.infoPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) );
		this.infoPanel.RowStyles.Add( new RowStyle() );
		this.infoPanel.Size = new Size( 560, 540 );
		this.infoPanel.TabIndex = 0;
		// 
		// motionCaption
		// 
		this.motionCaption.AutoSize = true;
		this.motionCaption.Font = new Font( "Yu Gothic UI", 9F, FontStyle.Bold );
		this.motionCaption.Location = new Point( 3, 0 );
		this.motionCaption.Name = "motionCaption";
		this.motionCaption.Size = new Size( 65, 15 );
		this.motionCaption.TabIndex = 0;
		this.motionCaption.Text = "移動の状態";
		// 
		// motionValue
		// 
		this.motionValue.AutoSize = true;
		this.motionValue.Location = new Point( 3, 15 );
		this.motionValue.Margin = new Padding( 3, 0, 3, 8 );
		this.motionValue.Name = "motionValue";
		this.motionValue.Size = new Size( 12, 15 );
		this.motionValue.TabIndex = 1;
		this.motionValue.Text = "-";
		// 
		// eventLogCaption
		// 
		this.eventLogCaption.AutoSize = true;
		this.eventLogCaption.Font = new Font( "Yu Gothic UI", 9F, FontStyle.Bold );
		this.eventLogCaption.Location = new Point( 3, 38 );
		this.eventLogCaption.Name = "eventLogCaption";
		this.eventLogCaption.Size = new Size( 62, 15 );
		this.eventLogCaption.TabIndex = 4;
		this.eventLogCaption.Text = "イベントログ";
		// 
		// eventLogBox
		// 
		this.eventLogBox.Dock = DockStyle.Fill;
		this.eventLogBox.Location = new Point( 0, 53 );
		this.eventLogBox.Margin = new Padding( 0 );
		this.eventLogBox.Multiline = true;
		this.eventLogBox.Name = "eventLogBox";
		this.eventLogBox.ReadOnly = true;
		this.eventLogBox.ScrollBars = ScrollBars.Both;
		this.eventLogBox.Size = new Size( 560, 456 );
		this.eventLogBox.TabIndex = 5;
		this.eventLogBox.WordWrap = false;
		// 
		// buttonPanel
		// 
		this.buttonPanel.AutoSize = true;
		this.buttonPanel.Controls.Add( this.closeButton );
		this.buttonPanel.Controls.Add( this.clearLogButton );
		this.buttonPanel.Controls.Add( this.copyLogButton );
		this.buttonPanel.Controls.Add( this.pinLogCheck );
		this.buttonPanel.Dock = DockStyle.Fill;
		this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
		this.buttonPanel.Location = new Point( 0, 509 );
		this.buttonPanel.Margin = new Padding( 0 );
		this.buttonPanel.Name = "buttonPanel";
		this.buttonPanel.Size = new Size( 560, 31 );
		this.buttonPanel.TabIndex = 6;
		// 
		// closeButton
		// 
		this.closeButton.AutoSize = true;
		this.closeButton.Location = new Point( 482, 3 );
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new Size( 75, 25 );
		this.closeButton.TabIndex = 2;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click +=  this.CloseButton_Click ;
		// 
		// clearLogButton
		// 
		this.clearLogButton.AutoSize = true;
		this.clearLogButton.Location = new Point( 401, 3 );
		this.clearLogButton.Name = "clearLogButton";
		this.clearLogButton.Size = new Size( 75, 25 );
		this.clearLogButton.TabIndex = 1;
		this.clearLogButton.Text = "ログを消す";
		this.clearLogButton.UseVisualStyleBackColor = true;
		this.clearLogButton.Click +=  this.ClearLogButton_Click ;
		// 
		// copyLogButton
		// 
		this.copyLogButton.AutoSize = true;
		this.copyLogButton.Location = new Point( 320, 3 );
		this.copyLogButton.Name = "copyLogButton";
		this.copyLogButton.Size = new Size( 75, 25 );
		this.copyLogButton.TabIndex = 0;
		this.copyLogButton.Text = "コピー";
		this.copyLogButton.UseVisualStyleBackColor = true;
		this.copyLogButton.Click +=  this.CopyLogButton_Click ;
		// 
		// pinLogCheck
		// 
		this.pinLogCheck.Anchor = AnchorStyles.Left;
		this.pinLogCheck.AutoSize = true;
		this.pinLogCheck.Checked = true;
		this.pinLogCheck.CheckState = CheckState.Checked;
		this.pinLogCheck.Location = new Point( 214, 6 );
		this.pinLogCheck.Margin = new Padding( 3, 3, 12, 3 );
		this.pinLogCheck.Name = "pinLogCheck";
		this.pinLogCheck.Size = new Size( 91, 19 );
		this.pinLogCheck.TabIndex = 3;
		this.pinLogCheck.Text = "一番下に固定";
		this.pinLogCheck.UseVisualStyleBackColor = true;
		this.pinLogCheck.CheckedChanged +=  this.PinLogCheck_CheckedChanged ;
		// 
		// infoTimer
		// 
		this.infoTimer.Enabled = true;
		this.infoTimer.Interval = 10;
		this.infoTimer.Tick +=  this.InfoTimer_Tick ;
		// 
		// InfoForm
		// 
		this.AutoScaleDimensions = new SizeF( 96F, 96F );
		this.AutoScaleMode = AutoScaleMode.Dpi;
		this.CancelButton = this.closeButton;
		this.ClientSize = new Size( 560, 540 );
		this.Controls.Add( this.infoPanel );
		this.Font = new Font( "Yu Gothic UI", 9F );
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.MinimumSize = new Size( 360, 320 );
		this.Name = "InfoForm";
		this.StartPosition = FormStartPosition.Manual;
		this.Text = "情報";
		this.infoPanel.ResumeLayout( false );
		this.infoPanel.PerformLayout();
		this.buttonPanel.ResumeLayout( false );
		this.buttonPanel.PerformLayout();
		this.ResumeLayout( false );
	}

	#endregion

	private TableLayoutPanel infoPanel;
	private Label motionCaption;
	private Label motionValue;
	private Label eventLogCaption;
	private TextBox eventLogBox;
	private FlowLayoutPanel buttonPanel;
	private Button closeButton;
	private Button clearLogButton;
	private Button copyLogButton;
	private CheckBox pinLogCheck;
	private System.Windows.Forms.Timer infoTimer;
}
