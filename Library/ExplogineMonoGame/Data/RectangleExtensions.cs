using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public static class RectangleExtensions
{
    public static RectangleF ToRectangleF(this Rectangle rectangle)
    {
        return new RectangleF(rectangle);
    }
}