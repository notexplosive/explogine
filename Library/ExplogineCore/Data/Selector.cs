namespace ExplogineCore.Data;

public class Selector : ISelector
{
    private Selectable? _selected;

    public Selectable? GetSelected()
    {
        return _selected;
    }

    public void Select(Selectable selectable)
    {
        _selected = selectable;
        selectable.OnSelected();
    }

    public void ClearSelection()
    {
        _selected = null;
    }
}