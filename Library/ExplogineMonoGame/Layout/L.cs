using System;
using ExplogineCore.Data;

namespace ExplogineMonoGame.Layout;

public static class L
{
    public static Element Fixed(string name, float x, float y)
    {
        return new Element(new ElementName(name), new FixedEdgeSize(x), new FixedEdgeSize(y));
    }

    public static Element Group(Func<Element> func, RowSettings settings, Element[] children)
    {
        return func() with {Children = new ElementChildren(settings, children)};
    }

    public static Element FillVertical(string name, float horizontalSize)
    {
        return new Element(new ElementName(name), new FixedEdgeSize(horizontalSize), new StretchedEdgeSize());
    }

    public static Element FillHorizontal(string name, float verticalSize)
    {
        return new Element(new ElementName(name), new StretchedEdgeSize(), new FixedEdgeSize(verticalSize));
    }

    public static Element Fill(Orientation orientation, string name, float perpendicularSize)
    {
        switch (orientation)
        {
            case Orientation.Horizontal:
                return FillHorizontal(name, perpendicularSize);
            case Orientation.Vertical:
                return FillHorizontal(name, perpendicularSize);
            default:
                throw new Exception("Unknown orientation");
        }
    }

    public static Element FillBoth(string name)
    {
        return new Element(new ElementName(name), new StretchedEdgeSize(), new StretchedEdgeSize());
    }
}
