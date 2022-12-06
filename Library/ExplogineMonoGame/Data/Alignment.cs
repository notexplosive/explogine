namespace ExplogineMonoGame.Data;

public enum VerticalAlignment
{
    Top,
    Center,
    Bottom
}

public enum HorizontalAlignment
{
    Left,
    Center,
    Right
}

public readonly struct Alignment
{
    public HorizontalAlignment Horizontal { get; }
    public VerticalAlignment Vertical { get; }

    public Alignment(HorizontalAlignment horizontal = HorizontalAlignment.Left,
        VerticalAlignment vertical = VerticalAlignment.Top)
    {
        Horizontal = horizontal;
        Vertical = vertical;
    }

    public static Alignment TopLeft { get; } = new();
    public static Alignment TopRight { get; } = new(HorizontalAlignment.Right);
    public static Alignment TopCenter { get; } = new(HorizontalAlignment.Center, VerticalAlignment.Top);
    public static Alignment BottomCenter { get; } = new(HorizontalAlignment.Center, VerticalAlignment.Bottom);
    public static Alignment BottomRight { get; } = new(HorizontalAlignment.Right, VerticalAlignment.Bottom);
    public static Alignment BottomLeft { get; } = new(HorizontalAlignment.Left, VerticalAlignment.Bottom);
    public static Alignment Center { get; } = new(HorizontalAlignment.Center, VerticalAlignment.Center);
    public static Alignment CenterRight { get; } = new(HorizontalAlignment.Right, VerticalAlignment.Center);
    public static Alignment CenterLeft { get; } = new(HorizontalAlignment.Left, VerticalAlignment.Center);

    public override string ToString()
    {
        return $"{Horizontal} {Vertical}";
    }
}