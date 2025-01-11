namespace ExplogineMonoGame.Layout;

internal readonly record struct ElementBlankName : IElementName
{
    public bool IsReal()
    {
        return false;
    }

    public override string ToString()
    {
        return "$$$ NAMELESS $$$";
    }
}
