using Microsoft.Xna.Framework;

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

    public Point AddPostionDeltaFromMargin(Point margin)
    {
        var xFactor = 1;
        var yFactor = 1;
        if (Horizontal == HorizontalAlignment.Right)
        {
            xFactor = -1;
        }

        if (Horizontal == HorizontalAlignment.Center)
        {
            xFactor = 0;
        }

        if (Vertical == VerticalAlignment.Bottom)
        {
            yFactor = -1;
        }

        if (Vertical == VerticalAlignment.Center)
        {
            yFactor = 0;
        }

        return new Point(margin.X * xFactor, margin.Y * yFactor);
    }

    public Vector2 GetRelativePositionOfElement(Vector2 availableSpace, Vector2 totalUsedSpace)
    {
        var result = Vector2.Zero;

        if (Horizontal == HorizontalAlignment.Center)
        {
            result.X = availableSpace.X / 2 - totalUsedSpace.X / 2;
        }

        if (Horizontal == HorizontalAlignment.Right)
        {
            result.X = availableSpace.X - totalUsedSpace.X;
        }

        if (Vertical == VerticalAlignment.Center)
        {
            result.Y = availableSpace.Y / 2 - totalUsedSpace.Y / 2;
        }

        if (Vertical == VerticalAlignment.Bottom)
        {
            result.Y = availableSpace.Y - totalUsedSpace.Y;
        }

        // assumes top left is default
        return result;
    }
}