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
}