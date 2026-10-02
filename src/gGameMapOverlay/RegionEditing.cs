namespace gGameMapOverlay;

/// <summary>四角形のどこを掴んだか。角は 2 つの辺の組み合わせ。</summary>
[Flags]
internal enum Grip
{
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8,
    Move = 16,
}

/// <summary>領域の四角形を移動・サイズ変更する計算 (RegionSelectorForm から切り出したもの)。</summary>
internal static class RegionEditing
{
    public const int MinWidth = 8;
    public const int MinHeight = 6;

    /// <summary>表示上の四角形に対して、point が辺・角・内側のどれに当たるか。</summary>
    public static Grip HitTest(Rectangle display, Point point, int tolerance)
    {
        if (!Rectangle.Inflate(display, tolerance, tolerance).Contains(point))
        {
            return Grip.None;
        }
        var grip = Grip.None;
        if (Math.Abs(point.X - display.Left) <= tolerance)
        {
            grip |= Grip.Left;
        }
        else if (Math.Abs(point.X - display.Right) <= tolerance)
        {
            grip |= Grip.Right;
        }
        if (Math.Abs(point.Y - display.Top) <= tolerance)
        {
            grip |= Grip.Top;
        }
        else if (Math.Abs(point.Y - display.Bottom) <= tolerance)
        {
            grip |= Grip.Bottom;
        }
        return grip == Grip.None ? Grip.Move : grip;
    }

    /// <summary>移動量を、移動または掴んだ辺のサイズ変更として反映する。結果は bounds の中に収める。</summary>
    public static Rectangle Apply(Rectangle start, Grip grip, int dx, int dy, Size bounds)
    {
        if (grip == Grip.Move)
        {
            return Clamp(new Rectangle(start.X + dx, start.Y + dy, start.Width, start.Height), bounds);
        }
        int left = start.Left, top = start.Top, right = start.Right, bottom = start.Bottom;
        if (grip.HasFlag(Grip.Left))
        {
            left = Math.Clamp(left + dx, 0, right - MinWidth);
        }
        if (grip.HasFlag(Grip.Right))
        {
            right = Math.Clamp(right + dx, left + MinWidth, bounds.Width);
        }
        if (grip.HasFlag(Grip.Top))
        {
            top = Math.Clamp(top + dy, 0, bottom - MinHeight);
        }
        if (grip.HasFlag(Grip.Bottom))
        {
            bottom = Math.Clamp(bottom + dy, top + MinHeight, bounds.Height);
        }
        return Rectangle.FromLTRB(left, top, right, bottom);
    }

    /// <summary>サイズを保ったまま bounds の中に収める (大きすぎる場合は縮め、小さすぎる場合は最小サイズにする)。</summary>
    public static Rectangle Clamp(Rectangle value, Size bounds)
    {
        var width = Math.Clamp(value.Width, MinWidth, bounds.Width);
        var height = Math.Clamp(value.Height, MinHeight, bounds.Height);
        var x = Math.Clamp(value.X, 0, bounds.Width - width);
        var y = Math.Clamp(value.Y, 0, bounds.Height - height);
        return new Rectangle(x, y, width, height);
    }
}
