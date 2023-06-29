namespace ExplogineMonoGame.Layout;

internal readonly record struct ElementName(string Text) : IElementName
{
    public override string ToString()
    {
        return Text;
    }

    public static implicit operator string(ElementName name)
    {
        return name.Text;
    }

    public bool IsReal()
    {
        return true;
    }
}
