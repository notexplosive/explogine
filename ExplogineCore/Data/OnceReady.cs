namespace ExplogineCore.Data;

public class OnceReady
{
    private event Action? Readied;

    public bool IsReady { get; private set; }

    public void Add(Action action)
    {
        if (IsReady)
        {
            action();
        }
        else
        {
            Readied += action;
        }
    }
    
    public void BecomeReady()
    {
        IsReady = true;
        Readied?.Invoke();
        Readied = null;
    }
}
