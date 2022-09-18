namespace Machina;

public class Actor
{
    private readonly List<Component> _components = new();
    public bool Visible { get; set; } = true;
    public Transform Transform { get; } = new();

    public IEnumerable<Component> AllComponents()
    {
        foreach (var component in _components)
        {
            yield return component;
        }
    }

    public void AddComponent<T>() where T : Component, new()
    {
        new T();
    }
}
