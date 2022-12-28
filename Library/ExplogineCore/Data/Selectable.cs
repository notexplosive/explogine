namespace ExplogineCore.Data;

public class Selectable
{
    public bool IsSelectedBy(ISelector selector)
    {
        return selector.GetSelected() == this;
    }

    public void BecomeSelectedBy(ISelector selector)
    {
        selector.Select(this);
    }

    public void DeselectFrom(ISelector selector)
    {
        if (IsSelectedBy(selector))
        {
            selector.ClearSelection();
        }
    }
}
