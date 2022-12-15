using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public static class Layout
{
    /// <summary>
    ///     Creates a row of rectangles
    /// </summary>
    /// <param name="startingPosition">Top left position of the first rectangle</param>
    /// <param name="settings"></param>
    /// <param name="elements">Specifications for each element in the row</param>
    /// <returns></returns>
    public static RectangleF[] CreateRow(Vector2 startingPosition, RowSettings settings, Element[] elements)
    {
        var result = new RectangleF[elements.Length];
        var alongSize = new Vector2();
        for (var i = 0; i < elements.Length; i++)
        {
            // todo: axis should come from settings
            result[i] = new RectangleF(startingPosition + alongSize, elements[i].GetSize());
            alongSize += result[i].Size.JustAxis(settings.Axis);
            if (i < elements.Length - 1)
            {
                alongSize += new Vector2(settings.PaddingBetweenElements).JustAxis(settings.Axis);
            }
        }

        return result;
    }

    public readonly record struct RowSettings(Orientation Orientation, int PaddingBetweenElements)
    {
        public Axis Axis
        {
            get
            {
                return Orientation switch
                {
                    Orientation.Horizontal => Axis.X,
                    Orientation.Vertical => Axis.Y,
                    _ => throw new Exception("Invalid orientation")
                };
            }
        }
    }

    public interface IEdgeSize
    {
    }

    public readonly record struct FixedEdgeSize(float Amount) : IEdgeSize
    {
        public static implicit operator float(FixedEdgeSize size)
        {
            return size.Amount;
        }
    }

    public readonly record struct Element(IEdgeSize X, IEdgeSize Y)
    {
        public static Element Fixed(float x, float y)
        {
            return new Element(new FixedEdgeSize(x), new FixedEdgeSize(y));
        }

        public Vector2 GetSize()
        {
            if (X is FixedEdgeSize fixedX && Y is FixedEdgeSize fixedY)
            {
                return new Vector2(fixedX, fixedY);
            }

            throw new Exception("Cannot get size");
        }
    }
}
