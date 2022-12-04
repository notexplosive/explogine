using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public static class PointExtensions
{
    public static float AspectRatio(this Point point)
    {
        return (float) point.X / point.Y;
    }

    public static int MaxXy(this Point point)
    {
        return Math.Max(point.X, point.Y);
    }
    
    public static Point Multiplied(this Point point, float scalar)
    {
        return new Point((int) (point.X * scalar), (int) (point.Y * scalar));
    }

    /// <summary>
    ///     Calculate the scalar sizeToEnclose needs to multiply by to fit within enclosingSize
    /// </summary>
    /// <param name="outerSize">The "Window" size that encloses the other size</param>
    /// <param name="innerSize">The "Canvas" size that will be scaled by the scalar</param>
    /// <returns>Scalar to multiply sizeToEnclose by</returns>
    public static float CalculateScalarDifference(Point outerSize, Point innerSize)
    {
        var enclosingSizeIsTooWide = PointExtensions.IsEnclosingSizeTooWide(outerSize, innerSize);

        if (enclosingSizeIsTooWide)
        {
            return (float) outerSize.Y / innerSize.Y;
        }

        return (float) outerSize.X / innerSize.X;
    }

    public static bool IsEnclosingSizeTooWide(Point enclosingSize, Point sizeToEnclose)
    {
        return enclosingSize.AspectRatio() > sizeToEnclose.AspectRatio();
    }
}
