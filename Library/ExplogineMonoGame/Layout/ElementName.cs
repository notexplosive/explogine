namespace ExplogineMonoGame.Layout;

public readonly record struct ElementName(string Text) : IElementName
{
    public bool IsReal()
    {
        return true;
    }

    public override string ToString()
    {
        return Text;
    }

    public static implicit operator string(ElementName name)
    {
        return name.Text;
    }
}
