namespace ExplogineCore.Data;

public class AlwaysSelected : ISelector
{
    private readonly Selectable _selectable;

    public AlwaysSelected(Selectable selectable)
    {
        _selectable = selectable;
    }

    public Selectable GetSelected()
    {
        return _selectable;
    }

    public void Select(Selectable selectable)
    {
        // Do nothing
    }

    public void ClearSelection()
    {
        // Do nothing
    }
}
