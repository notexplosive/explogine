namespace ExplogineCore.Data;

public interface ISelector
{
    bool IsSelected(Selectable selectable);
    void Select(Selectable selectable);
    void ClearSelection();
}