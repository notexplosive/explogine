namespace ExplogineCore.Data;

public class Selectable
{
    public bool IsSelectedBy(ISelector selector)
    {
        return selector.IsSelected(this);
    }

    public void BecomeSelectedBy(ISelector selector)
    {
        if (!IsSelectedBy(selector))
        {
            selector.Select(this);
        }
    }

    public void DeselectFrom(ISelector selector)
    {
        if (IsSelectedBy(selector))
        {
            selector.ClearSelection();
        }
    }

    public event Action? Selected;

    public void OnSelected()
    {
        Selected?.Invoke();
    }
}
