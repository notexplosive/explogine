using System;
using System.Diagnostics.Contracts;
using ExplogineCore.Data;
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
    
    public static int MinXy(this Point point)
    {
        return Math.Min(point.X, point.Y);
    }

    public static Point Multiplied(this Point point, float scalar)
    {
        return new Point((int) (point.X * scalar), (int) (point.Y * scalar));
    }

    public static void AddToAxis(this Point point, Axis axis, int amountToAdd)
    {
        point.SetAxis(axis, point.GetAxis(axis) + amountToAdd);
    }

    public static void SetAxis(this Point point, Axis axis, int value)
    {
        if (axis == Axis.X)
        {
            point.X = value;
        }
        else if (axis == Axis.Y)
        {
            point.Y = value;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }
    }

    public static int GetAxis(this Point point, Axis axis)
    {
        if (axis == Axis.X)
        {
            return point.X;
        }

        if (axis == Axis.Y)
        {
            return point.Y;
        }

        throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
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

    [Pure]
    public static RectangleF ToRectangleF(this Point point)
    {
        return new RectangleF(Vector2.Zero, point.ToVector2());
    }
    
    [Pure]
    public static Rectangle ToRectangle(this Point point)
    {
        return new Rectangle(Point.Zero, point);
    }

    [Pure]
    public static Point Min(Point a, Point b)
    {
        var min = new Point
        {
            X = Math.Min(a.X, b.X),
            Y = Math.Min(a.Y, b.Y)
        };
        return min;
    }
    
    [Pure]
    public static Point Max(Point a, Point b)
    {
        var min = new Point
        {
            X = Math.Max(a.X, b.X),
            Y = Math.Max(a.Y, b.Y)
        };
        return min;
    }
    
    
    [Pure]
    public static Point JustX(this Point vec)
    {
        return new Point(vec.X, 0);
    }
    
    [Pure]
    public static Point JustY(this Point vec)
    {
        return new Point(0, vec.Y);
    }
    
    [Pure]
    public static Point Abs(this Point vec)
    {
        return new Point(Math.Abs(vec.X), Math.Abs(vec.Y));
    }
}
