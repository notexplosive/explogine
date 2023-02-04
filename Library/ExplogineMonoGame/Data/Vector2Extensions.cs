using System;
using System.Diagnostics.Contracts;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public static class Vector2Extensions
{
    public static Vector2 Normalized(this Vector2 vec)
    {
        var copy = vec;
        copy.Normalize();
        return copy;
    }

    public static Vector2 StraightMultiply(this Vector2 vec, Vector2 other)
    {
        return new Vector2(vec.X * other.X, vec.Y * other.Y);
    }

    public static Vector2 StraightMultiply(this Vector2 vec, float otherX, float otherY)
    {
        return new Vector2(vec.X * otherX, vec.Y * otherY);
    }

    public static Vector2 StraightMultiply(this Vector2 vec, Point other)
    {
        return vec.StraightMultiply(other.ToVector2());
    }

    public static Vector2 StraightDivide(this Vector2 vec, Vector2 other)
    {
        return new Vector2(vec.X / other.X, vec.Y / other.Y);
    }

    public static Vector2 StraightDivide(this Vector2 vec, float otherX, float otherY)
    {
        return new Vector2(vec.X / otherX, vec.Y / otherY);
    }

    public static Vector2 StraightDivide(this Vector2 vec, Point other)
    {
        return vec.StraightMultiply(other.ToVector2());
    }

    /// <summary>
    ///     Rotates the vector clockwise around an origin point
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="radians"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static Vector2 Rotated(this Vector2 vec, float radians, Vector2 origin)
    {
        return Vector2.Transform(vec,
            Matrix.CreateTranslation(new Vector3(-origin, 0)) * Matrix.CreateRotationZ(radians) *
            Matrix.CreateTranslation(new Vector3(origin, 0)));
    }

    public static float CalculateScalarDifference(Vector2 outerSize, Vector2 innerSize)
    {
        var enclosingSizeIsTooWide = Vector2Extensions.IsEnclosingSizeTooWide(outerSize, innerSize);

        if (enclosingSizeIsTooWide)
        {
            return outerSize.Y / innerSize.Y;
        }

        return outerSize.X / innerSize.X;
    }

    public static bool IsEnclosingSizeTooWide(Vector2 enclosingSize, Vector2 sizeToEnclose)
    {
        return enclosingSize.AspectRatio() > sizeToEnclose.AspectRatio();
    }

    [Pure]
    public static float AspectRatio(this Vector2 vec)
    {
        // AspectRatio MUST be X / Y. Other things depend on this
        return vec.X / vec.Y;
    }

    public static Vector2 AspectSize(this Vector2 vec)
    {
        return FromAspectRatio(vec.AspectRatio());
    }

    public static float MaxXy(this Vector2 vec)
    {
        return MathF.Max(vec.X, vec.Y);
    }

    public static Vector2 JustX(this Vector2 vec)
    {
        return new Vector2(vec.X, 0);
    }
    
    public static Vector2 JustY(this Vector2 vec)
    {
        return new Vector2(0, vec.Y);
    }

    public static Vector2 FromAspectRatio(float aspectRatio)
    {
        return new Vector2(aspectRatio, 1);
    }

    public static Vector2 Truncate(this Vector2 vector)
    {
        return new Vector2((int)vector.X, (int)vector.Y);
    }
    
    public static void SetAxis(this ref Vector2 vec, Axis axis, float value)
    {
        if (axis == Axis.X)
        {
            vec.X = value;
        }
        else if (axis == Axis.Y)
        {
            vec.Y = value;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }
    }

    public static float GetAxis(this Vector2 vec, Axis axis)
    {
        if (axis == Axis.X)
        {
            return vec.X;
        }

        if (axis == Axis.Y)
        {
            return vec.Y;
        }

        throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
    }
    
    /// <summary>
    /// If given Axis.X, returns JustX(), if given Axis.Y, returns JustY()
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Vector2 JustAxis(this Vector2 vec, Axis axis)
    {
        if (axis == Axis.X)
        {
            return vec.JustX();
        }

        if (axis == Axis.Y)
        {
            return vec.JustY();
        }

        throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
    }

    public static Vector2 FromAxisFirst(Axis axis, float first, float second)
    {
        if (axis == Axis.X)
        {
            return new Vector2(first, second);
        }

        if (axis == Axis.Y)
        {
            return new Vector2(second, first);
        }

        throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
    }

    public static Vector2 Polar(float radius, float theta)
    {
        return new Vector2(MathF.Cos(theta), MathF.Sin(theta)) * radius;
    }

    public static float GetAngleFromUnitX(Vector2 vector)
    {
        var unitX = Vector2.UnitX;

        
        var dot = Vector2.Dot(unitX.Normalized(), vector.Normalized());

        if (float.IsNaN(dot))
        {
            return 0;
        }

        var angle = MathF.Acos((unitX.X * vector.X + unitX.Y * vector.Y) / (vector.Length() * unitX.Length()));

        if (vector.Y < 0)
        {
            angle = -angle;
        }
        return angle;
    }

    public static Vector2 Floored(this Vector2 vector2)
    {
        var copy = vector2;
        copy.Floor();
        return copy;
    }
    
    public static Vector2 Ceilinged(this Vector2 vector2)
    {
        var copy = vector2;
        copy.Ceiling();
        return copy;
    }
}
