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
}
