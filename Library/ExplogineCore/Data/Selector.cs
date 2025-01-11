namespace ExplogineCore.Data;

public class Selector : ISelector
{
    private Selectable? _selected;

    public bool IsSelected(Selectable selectable)
    {
        return _selected == selectable;
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

    public Selectable? GetSelected()
    {
        return _selected;
    }
}
