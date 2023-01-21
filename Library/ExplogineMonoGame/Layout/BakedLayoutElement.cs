using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Layout;

public readonly record struct BakedLayoutElement(RectangleF Rectangle, string Name, int NestingLevel)
{
    public static implicit operator RectangleF(BakedLayoutElement layoutElement)
    {
        return layoutElement.Rectangle;
    }
}
