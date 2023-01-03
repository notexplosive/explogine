using System;

namespace ExTween;

public class CallbackTween : ITween
{
    private readonly Action _behavior;
    private bool _hasExecuted;

    public CallbackTween(Action behavior)
    {
        _behavior = behavior;
    }

    public float Update(float dt)
    {
        if (!_hasExecuted)
        {
            _behavior();
            _hasExecuted = true;
        }

        // Instant tween, always overflows 100% of dt
        return dt;
    }

    public void SkipToEnd()
    {
        Update(0);
    }

    public bool IsDone()
    {
        return _hasExecuted;
    }

    public void Reset()
    {
        _hasExecuted = false;
    }

    public void JumpTo(float time)
    {
        Update(time);
    }

    public ITweenDuration TotalDuration => new KnownTweenDuration(0, 0);
}