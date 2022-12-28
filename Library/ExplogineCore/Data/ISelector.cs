namespace ExplogineCore.Data;

public interface ISelector
{
    Selectable? GetSelected();
    void Select(Selectable selectable);
    void ClearSelection();
}
