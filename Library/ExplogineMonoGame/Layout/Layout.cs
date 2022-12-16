using System;
using ExplogineCore.Data;

namespace ExplogineMonoGame.Layout;

public static class Layout
{
    public static LayoutElement FixedElement(float x, float y)
    {
        return new LayoutElement(new ElementBlankName(), new FixedEdgeSize(x), new FixedEdgeSize(y));
    }

    public static LayoutElement FixedElement(string name, float x, float y)
    {
        return Layout.FixedElement(x, y) with {Name = new ElementName(name)};
    }

    public static LayoutElement Group(Func<LayoutElement> func, ArrangementSettings settings, LayoutElement[] children)
    {
        return func() with {Children = new ElementChildren(settings, children)};
    }

    public static LayoutElement FillVertical(string name, float horizontalSize)
    {
        return Layout.FillVertical(horizontalSize) with {Name = new ElementName(name)};
    }

    public static LayoutElement FillVertical(float horizontalSize)
    {
        return new LayoutElement(new ElementBlankName(), new FixedEdgeSize(horizontalSize), new StretchedEdgeSize());
    }

    public static LayoutElement FillHorizontal(string name, float verticalSize)
    {
        return Layout.FillHorizontal(verticalSize) with {Name = new ElementName(name)};
    }

    public static LayoutElement FillHorizontal(float verticalSize)
    {
        return new LayoutElement(new ElementBlankName(), new StretchedEdgeSize(), new FixedEdgeSize(verticalSize));
    }

    public static LayoutElement Fill(Orientation orientation, string name, float perpendicularSize)
    {
        switch (orientation)
        {
            case Orientation.Horizontal:
                return Layout.FillHorizontal(name, perpendicularSize);
            case Orientation.Vertical:
                return Layout.FillHorizontal(name, perpendicularSize);
            default:
                throw new Exception("Unknown orientation");
        }
    }

    public static LayoutElement FillBoth(string name)
    {
        return new LayoutElement(new ElementName(name), new StretchedEdgeSize(), new StretchedEdgeSize());
    }
}
