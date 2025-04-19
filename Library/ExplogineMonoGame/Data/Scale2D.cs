using System;
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

    public override string ToString()
    {
        if (Math.Abs(Value.X - Value.Y) < float.Epsilon)
        {
            return $"{Value.X}";
        }

        return $"{Value.X}, {Value.Y}";
    }

    public Scale2D(float scale)
    {
        Value = new Vector2(scale);
    }

    public static Scale2D One => new(1);

    public static Scale2D operator *(Scale2D a, float b)
    {
        return new Scale2D(a.Value * b);
    }

    public static Scale2D operator /(Scale2D a, float b)
    {
        return new Scale2D(a.Value / b);
    }

    public static Scale2D operator +(Scale2D a, Scale2D b)
    {
        return new Scale2D(a.Value + b.Value);
    }

    public static Scale2D operator -(Scale2D a, Scale2D b)
    {
        return new Scale2D(a.Value - b.Value);
    }
}
