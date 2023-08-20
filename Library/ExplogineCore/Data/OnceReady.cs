namespace ExplogineCore.Data;

public class OnceReady
{
    public bool IsReady { get; private set; }
    private event Action? Readied;

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

    public void ClearAndBecomeUnready()
    {
        IsReady = false;
        Readied = null;
    }
}
