namespace MachinaLite;

public class WaitUntil(Func<bool> _isDone) : ICoroutineAction
{
    public bool IsComplete(float dt)
    {
        return _isDone();
    }
}
