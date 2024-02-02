using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public static class RectangleExtensions
{
    public static RectangleF ToRectangleF(this Rectangle rectangle)
    {
        return new RectangleF(rectangle);
    }
    
    [Pure]
    public static Rectangle FromCorners(Point cornerA, Point cornerB)
    {
        var x = Math.Min(cornerA.X, cornerB.X);
        var y = Math.Min(cornerA.Y, cornerB.Y);
        var width = Math.Abs(cornerA.X - cornerB.X);
        var height = Math.Abs(cornerA.Y - cornerB.Y);
        return new Rectangle(x, y, width, height);
    }

    [Pure]
    public static Rectangle Moved(this Rectangle rectangle, Vector2 offsetAmount)
    {
        rectangle.Offset(offsetAmount);
        return rectangle;
    }
    
    [Pure]
    public static Rectangle Inflated(this Rectangle rectangle, Point inflateAmount)
    {
        var rectangle2 = rectangle;
        rectangle2.Inflate(inflateAmount.X, inflateAmount.Y);
        return rectangle2;
    }

    [Pure]
    public static Rectangle Inflated(this Rectangle rectangle, int x, int y)
    {
        return Inflated(rectangle, new Point(x, y));
    }
}