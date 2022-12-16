using System;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Layout;

public static class L
{
    public static Element Fixed(string name, float x, float y)
    {
        return new Element(new ElementName(name), new FixedEdgeSize(x), new FixedEdgeSize(y));
    }

    public static Element StretchedHorizontal(string name, float verticalSize)
    {
        return new Element(new ElementName(name), new StretchedEdgeSize(), new FixedEdgeSize(verticalSize));
    }

    public static Element StretchedVertical(string name, float horizontalSize)
    {
        return new Element(new ElementName(name), new FixedEdgeSize(horizontalSize), new StretchedEdgeSize());
    }

    public static Element FixedSpacer(float size)
    {
        return new Element(new ElementBlankName(), new FixedEdgeSize(size), new FixedEdgeSize(size));
    }

    public static Element StretchedBoth(string name)
    {
        return new Element(new ElementName(name), new StretchedEdgeSize(), new StretchedEdgeSize());
    }

    public static IElement StretchedAlong(string name, float perpendicularSize)
    {
        return new DynamicElement(alongAxis =>
        {
            return alongAxis.ReturnIfXElseY(
                () => new Element(new ElementName(name), new StretchedEdgeSize(),
                    new FixedEdgeSize(perpendicularSize)),
                () => new Element(new ElementName(name), new FixedEdgeSize(perpendicularSize),
                    new StretchedEdgeSize())
            );
        });
    }

    public static IElement StretchedPerpendicular(string name, float alongSize)
    {
        return new DynamicElement(alongAxis =>
        {
            return alongAxis.ReturnIfXElseY(
                () => new Element(new ElementName(name), new FixedEdgeSize(alongSize), new StretchedEdgeSize()),
                () => new Element(new ElementName(name), new StretchedEdgeSize(), new FixedEdgeSize(alongSize))
            );
        });
    }
}

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