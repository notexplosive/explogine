using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly record struct DrawOrigin
{
    private readonly Vector2 _constantValue;
    private readonly Style _style = Style.None;

    public DrawOrigin(Vector2 vector2)
    {
        _constantValue = vector2;
        _style = Style.Constant;
    }

    private DrawOrigin(Style style)
    {
        _style = style;
        _constantValue = Vector2.Zero;
    }

    public static DrawOrigin Zero => new(Vector2.Zero);

    public static DrawOrigin Center => new(Style.Centered);

    [Pure]
    public Vector2 Calculate(Point size)
    {
        return Calculate(size.ToVector2());
    }

    [Pure]
    public Vector2 Calculate(Vector2 size)
    {
        if (_style == Style.Constant)
        {
            return _constantValue;
        }

        if (_style == Style.Centered)
        {
            return size / 2;
        }

        return Vector2.Zero;
    }

    public override string ToString()
    {
        switch (_style)
        {
            case Style.Centered:
                return "Centered";
            case Style.Constant:
                return $"Constant: {_constantValue.X} {_constantValue.Y}";
        }

        return $"Uninitialized ({nameof(_style)} has not been set)";
    }

    public static DrawOrigin operator -(DrawOrigin origin)
    {
        if (origin._style == Style.Constant)
        {
            return new DrawOrigin(-origin._constantValue);
        }

        return origin;
    }

    private enum Style
    {
        None,
        Constant,
        Centered
    }
}
