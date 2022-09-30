namespace ExplogineCore;

public interface ICoroutineAction
{
    public bool IsComplete(float dt);
}

public class WaitUntil : ICoroutineAction
{
    private readonly Func<bool> _condition;

    public WaitUntil(Func<bool> condition)
    {
        _condition = condition;
    }

    public bool IsComplete(float dt)
    {
        return _condition();
    }

    public bool IsDone()
    {
        return _condition();
    }
}

public class WaitSeconds : ICoroutineAction
{
    private float _seconds;

    public WaitSeconds(float seconds)
    {
        _seconds = seconds;
    }

    public bool IsComplete(float dt)
    {
        _seconds -= dt;
        return _seconds <= 0f;
    }
}