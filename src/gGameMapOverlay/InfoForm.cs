namespace gGameMapOverlay;

/// <summary>情報画面から MainForm の状態を読むための窓口。</summary>
internal interface IInfoSource
{
    /// <summary>移動の状態 (読み取った座標・描いている位置・先読みなど)。</summary>
    string MotionStatus { get; }

    /// <summary>スライドの出来事のログ (古い順、時刻つき)。</summary>
    IReadOnlyList<string> EventLog { get; }

    /// <summary>ログが変わるたびに増える数 (変わったら表示し直す)。</summary>
    long EventLogVersion { get; }

    void ClearEventLog();
}

/// <summary>
/// 移動の状態・歩く速さと遅れ・スライドの出来事のログを表示する画面。開いている間、定期的に表示し直す。
/// 開いたままゲームを操作できるよう、モードレスで表示する。
/// </summary>
internal sealed partial class InfoForm : Form
{
    private readonly IInfoSource source;
    private long shownLogVersion = -1;
    private const int LogRefreshMs = 200;
    private long logShownAt;

    public InfoForm(IInfoSource source)
    {
        this.source = source;
        InitializeComponent();
        RefreshInfo();
    }

    private void InfoTimer_Tick(object? sender, EventArgs e) => RefreshInfo();

    private void RefreshInfo()
    {
        SetText(motionValue, source.MotionStatus);
        // ログは書き換えると全体が描き直されてちらつくので、間引いて (LogRefreshMs ごと)、描き直しを止めてから書き換える。
        if (source.EventLogVersion != shownLogVersion && Environment.TickCount64 - logShownAt >= LogRefreshMs)
        {
            shownLogVersion = source.EventLogVersion;
            logShownAt = Environment.TickCount64;
            Native.Redraw.Batch(eventLogBox, () =>
            {
                // 一番下に固定しないときは、見ていた位置 (いちばん上の行) と選択を保つ。
                var firstLine = Native.Redraw.FirstVisibleLine(eventLogBox);
                var (selectionStart, selectionLength) = (eventLogBox.SelectionStart, eventLogBox.SelectionLength);
                eventLogBox.Text = string.Join(Environment.NewLine, source.EventLog);
                if (pinLogCheck.Checked)
                {
                    eventLogBox.SelectionStart = eventLogBox.TextLength;
                    Native.Redraw.ScrollToBottom(eventLogBox);
                }
                else
                {
                    eventLogBox.Select(Math.Min(selectionStart, eventLogBox.TextLength), selectionLength);
                    Native.Redraw.ScrollToLine(eventLogBox, firstLine);
                }
            });
        }

        static void SetText(Control control, string text)
        {
            if (control.Text != text)
            {
                control.Text = text;
            }
        }
    }

    private void ClearLogButton_Click(object? sender, EventArgs e)
    {
        source.ClearEventLog();
        RefreshInfo();
    }

    private void CopyLogButton_Click(object? sender, EventArgs e)
    {
        Clipboard.SetText(string.Join(Environment.NewLine, [motionValue.Text, .. source.EventLog]));
    }

    private void PinLogCheck_CheckedChanged(object? sender, EventArgs e)
    {
        if (pinLogCheck.Checked)
        {
            eventLogBox.SelectionStart = eventLogBox.TextLength;
            Native.Redraw.ScrollToBottom(eventLogBox);
        }
    }

    private void CloseButton_Click(object? sender, EventArgs e) => Close();
}
