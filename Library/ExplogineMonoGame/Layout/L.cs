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
