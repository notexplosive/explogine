using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public static class PointExtensions
{
    public static float AspectRatio(this Point point)
    {
        return (float) point.X / point.Y;
    }

    public static int MaxXY(this Point point)
    {
        return Math.Max(point.X, point.Y);
    }

    /// <summary>
    ///     Calculate the scalar sizeToEnclose needs to multiply by to fit within enclosingSize
    /// </summary>
    /// <param name="enclosingSize">The "Window" rect that encloses the other rect</param>
    /// <param name="sizeToEnclose">The "Canvas" rect that will be scaled by the scalar</param>
    /// <returns>Scalar to multiply sizeToEnclose by</returns>
    public static float CalculateScalarDifference(Point enclosingSize, Point sizeToEnclose)
    {
        var enclosingSizeIsTooWide = PointExtensions.IsEnclosingSizeTooWide(enclosingSize, sizeToEnclose);

        if (enclosingSizeIsTooWide)
        {
            return (float) enclosingSize.Y / sizeToEnclose.Y;
        }

        return (float) enclosingSize.X / sizeToEnclose.X;
    }

    public static bool IsEnclosingSizeTooWide(Point enclosingSize, Point sizeToEnclose)
    {
        return enclosingSize.AspectRatio() > sizeToEnclose.AspectRatio();
    }
}
