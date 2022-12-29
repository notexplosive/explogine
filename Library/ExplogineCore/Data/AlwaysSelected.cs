namespace ExplogineCore.Data;

public class AlwaysSelected : ISelector
{
    public bool IsSelected(Selectable selectable)
    {
        return true;
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
