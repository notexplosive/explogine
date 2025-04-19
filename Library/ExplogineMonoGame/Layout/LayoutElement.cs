using System;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Layout;

public readonly record struct LayoutElement(
    IElementName Name,
    IEdgeSize X,
    IEdgeSize Y,
    LayoutElementGroup? Children = null)
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
}
