namespace ExplogineMonoGame.Data;

public class HoverState
{
    public bool IsHovered { get; private set; }

    public static implicit operator bool(HoverState hoverState)
    {
        return hoverState.IsHovered;
    }

    public void Unset()
    {
        IsHovered = false;
    }

    public void Set()
    {
        IsHovered = true;
    }

    public override string ToString()
    {
        return IsHovered.ToString();
    }
}
