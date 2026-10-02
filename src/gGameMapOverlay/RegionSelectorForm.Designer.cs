namespace gGameMapOverlay;

partial class RegionSelectorForm
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
    private void InitializeComponent()
    {
        toolPanel = new FlowLayoutPanel();
        guideLabel = new Label();
        zoomPreview = new PictureBox();
        buttonsPanel = new FlowLayoutPanel();
        okButton = new Button();
        resetButton = new Button();
        cancelButton = new Button();
        positionLabel = new Label();
        toolPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)zoomPreview).BeginInit();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        //
        // toolPanel
        //
        toolPanel.AutoSize = true;
        toolPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        toolPanel.BackColor = Color.FromArgb(24, 24, 24);
        toolPanel.Controls.Add(guideLabel);
        toolPanel.Controls.Add(zoomPreview);
        toolPanel.Controls.Add(buttonsPanel);
        toolPanel.FlowDirection = FlowDirection.TopDown;
        toolPanel.ForeColor = Color.White;
        toolPanel.Location = new Point(160, 440);
        toolPanel.Name = "toolPanel";
        toolPanel.Padding = new Padding(8);
        toolPanel.Size = new Size(496, 150);
        toolPanel.TabIndex = 0;
        toolPanel.WrapContents = false;
        //
        // guideLabel
        //
        guideLabel.AutoSize = true;
        guideLabel.Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold);
        guideLabel.Location = new Point(11, 8);
        guideLabel.MaximumSize = new Size(480, 0);
        guideLabel.Name = "guideLabel";
        guideLabel.Size = new Size(120, 19);
        guideLabel.TabIndex = 0;
        guideLabel.Text = "領域を合わせてください";
        //
        // zoomPreview
        //
        zoomPreview.BackColor = Color.Black;
        zoomPreview.Location = new Point(11, 30);
        zoomPreview.Name = "zoomPreview";
        zoomPreview.Size = new Size(480, 60);
        zoomPreview.SizeMode = PictureBoxSizeMode.CenterImage;
        zoomPreview.TabIndex = 1;
        zoomPreview.TabStop = false;
        //
        // buttonsPanel
        //
        buttonsPanel.AutoSize = true;
        buttonsPanel.Controls.Add(okButton);
        buttonsPanel.Controls.Add(resetButton);
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(positionLabel);
        buttonsPanel.Location = new Point(8, 93);
        buttonsPanel.Margin = new Padding(0);
        buttonsPanel.Name = "buttonsPanel";
        buttonsPanel.Size = new Size(470, 31);
        buttonsPanel.TabIndex = 2;
        buttonsPanel.WrapContents = false;
        //
        // okButton
        //
        okButton.AutoSize = true;
        okButton.BackColor = SystemColors.Control;
        okButton.ForeColor = SystemColors.ControlText;
        okButton.Location = new Point(3, 3);
        okButton.Name = "okButton";
        okButton.Size = new Size(75, 25);
        okButton.TabIndex = 0;
        okButton.Text = "決定";
        okButton.UseVisualStyleBackColor = true;
        okButton.Click += OkButton_Click;
        //
        // resetButton
        //
        resetButton.AutoSize = true;
        resetButton.BackColor = SystemColors.Control;
        resetButton.ForeColor = SystemColors.ControlText;
        resetButton.Location = new Point(84, 3);
        resetButton.Name = "resetButton";
        resetButton.Size = new Size(90, 25);
        resetButton.TabIndex = 1;
        resetButton.Text = "初期値に戻す";
        resetButton.UseVisualStyleBackColor = true;
        resetButton.Click += ResetButton_Click;
        //
        // cancelButton
        //
        cancelButton.AutoSize = true;
        cancelButton.BackColor = SystemColors.Control;
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.ForeColor = SystemColors.ControlText;
        cancelButton.Location = new Point(180, 3);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 25);
        cancelButton.TabIndex = 2;
        cancelButton.Text = "キャンセル";
        cancelButton.UseVisualStyleBackColor = true;
        //
        // positionLabel
        //
        positionLabel.Anchor = AnchorStyles.Left;
        positionLabel.AutoSize = true;
        positionLabel.ForeColor = Color.Silver;
        positionLabel.Location = new Point(261, 8);
        positionLabel.Margin = new Padding(6, 0, 3, 0);
        positionLabel.Name = "positionLabel";
        positionLabel.Size = new Size(100, 15);
        positionLabel.TabIndex = 3;
        positionLabel.Text = "X 0  Y 0  幅 0  高さ 0";
        //
        // RegionSelectorForm
        //
        AcceptButton = okButton;
        AutoScaleMode = AutoScaleMode.None;
        CancelButton = cancelButton;
        ClientSize = new Size(800, 600);
        Controls.Add(toolPanel);
        DoubleBuffered = true;
        Font = new Font("Yu Gothic UI", 9F);
        FormBorderStyle = FormBorderStyle.None;
        KeyPreview = true;
        Name = "RegionSelectorForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Text = "領域の選択";
        TopMost = true;
        toolPanel.ResumeLayout(false);
        toolPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)zoomPreview).EndInit();
        buttonsPanel.ResumeLayout(false);
        buttonsPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel toolPanel;
    private Label guideLabel;
    private PictureBox zoomPreview;
    private FlowLayoutPanel buttonsPanel;
    private Button okButton;
    private Button resetButton;
    private Button cancelButton;
    private Label positionLabel;
}
