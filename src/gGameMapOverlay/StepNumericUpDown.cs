namespace gGameMapOverlay;

/// <summary>
/// ホイール 1 ノッチで Increment 1 回分だけ動く NumericUpDown。
/// 標準ではホイールを回したときにスクロールする行数 (既定 3) の回数だけ動く。
/// </summary>
internal sealed class StepNumericUpDown : NumericUpDown
{
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if( e is HandledMouseEventArgs handled ) handled.Handled = true;
        if( e.Delta > 0 ) UpButton();
        else if( e.Delta < 0 ) DownButton();
    }
}
