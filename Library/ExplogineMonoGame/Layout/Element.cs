using System;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Layout;

public readonly record struct Element
    (IElementName Name, IEdgeSize X, IEdgeSize Y, ElementChildren? Children = null) : IElement
{
    public IEdgeSize GetAxis(Axis axis)
    {
        if (axis == Axis.X)
        {
            return X;
        }

        if (axis == Axis.Y)
        {
            return Y;
        }

        throw new Exception("Unknown axis");
    }

    public Vector2 GetSize()
    {
        if (X is FixedEdgeSize fixedX && Y is FixedEdgeSize fixedY)
        {
            return new Vector2(fixedX, fixedY);
        }

        throw new Exception("Cannot get size");
    }

    public Element WithChildren(RowSettings rowSettings, IElement[] elements)
    {
        return this with {Children = new ElementChildren(rowSettings, elements)};
    }
}