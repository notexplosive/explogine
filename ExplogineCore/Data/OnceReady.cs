namespace ExplogineCore.Data;

public class OnceReady
{
    private event Action? Readied;

    private bool _isReady; 
    
    public void Add(Action action)
    {
        if (_isReady)
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
        _isReady = true;
        Readied?.Invoke();
        Readied = null;
    }
}
