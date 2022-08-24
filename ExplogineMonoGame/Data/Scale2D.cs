using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly struct Scale2D
{
    public Vector2 Value { get; }

    public Scale2D()
    {
        Value = Vector2.One;
    }

    public Scale2D(Vector2 scale)
    {
        Value = scale;
    }

    public Scale2D(float scale)
    {
        Value = new Vector2(scale);
    }

    public static Scale2D One => new Scale2D(1);
    
    public static Scale2D operator *(Scale2D a, float b)
    {
        return new Scale2D(a.Value * b);
    }

    public static Scale2D operator /(Scale2D a, float b)
    {
        return new Scale2D(a.Value / b);
    }
}
