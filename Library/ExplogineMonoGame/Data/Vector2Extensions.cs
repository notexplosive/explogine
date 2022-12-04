using System;
using System.Diagnostics.Contracts;
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

    public static Vector2 FromAspectRatio(float aspectRatio)
    {
        return new Vector2(aspectRatio, 1);
    }
}
