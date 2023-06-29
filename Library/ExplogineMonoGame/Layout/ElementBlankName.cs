namespace ExplogineMonoGame.Layout;

internal readonly record struct ElementBlankName : IElementName
{
    public override string ToString()
    {
        return "$$$ NAMELESS $$$";
    }

    public bool IsReal()
    {
        return false;
    }
}
