using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly record struct DrawOrigin
{
    private readonly Vector2 _constantValue;
    private readonly Style _style = Style.None;

    [Pure]
    public Vector2 Value(Point size)
    {
        return Value(size.ToVector2());
    }
    
    [Pure]
    public Vector2 Value(Vector2 size)
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

    public DrawOrigin(Vector2 vector2)
    {
        _constantValue = vector2;
        _style = Style.Constant;
    }

    public static DrawOrigin Zero => new(Vector2.Zero);

    private DrawOrigin(Style style)
    {
        _style = style;
        _constantValue = Vector2.Zero;
    }

    public static DrawOrigin Center => new(Style.Centered);

    public override string ToString()
    {
        switch (_style)
        {
            case Style.Centered:
                return "Centered";
            case Style.Constant:
                return $"Constant: {_constantValue.X} {_constantValue.Y}";
        }
        return $"Uninitialized ({nameof(DrawOrigin._style)} has not been set)";
    }

    private enum Style
    {
        None,
        Constant,
        Centered
    }
}
