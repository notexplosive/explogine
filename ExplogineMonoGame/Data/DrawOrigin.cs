using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly struct DrawOrigin
{
    private readonly Vector2 _constantValue;
    private readonly Style _style = Style.None;

    public Vector2 Value(Point size)
    {
        if (_style == Style.Constant)
        {
            return _constantValue;
        }

        if (_style == Style.Centered)
        {
            return size.ToVector2() / 2;
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

    private enum Style
    {
        None,
        Constant,
        Centered
    }
}
