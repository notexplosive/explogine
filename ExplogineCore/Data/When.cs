namespace ExplogineCore.Data;

public class When
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
    
    public void Invoke()
    {
        _isReady = true;
        Readied?.Invoke();
        Readied = null;
    }
}
