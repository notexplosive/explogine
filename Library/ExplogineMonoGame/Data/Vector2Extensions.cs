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
}
