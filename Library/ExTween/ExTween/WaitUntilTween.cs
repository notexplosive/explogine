using System;

namespace ExTween;

public class WaitUntilTween : ITween
{
    private readonly Func<bool> _condition;

    public WaitUntilTween(Func<bool> condition)
    {
        _condition = condition;
    }

    public float Update(float dt)
    {
        if (_condition())
        {
            // Instant tween, always overflows 100% of dt
            return dt;
        }

        return 0;
    }

    public bool IsDone()
    {
        return _condition();
    }

    public void Reset()
    {
        // no op
    }

    public void JumpTo(float time)
    {
        Update(time);
    }

    public void SkipToEnd()
    {
        Update(0);
    }

    public ITweenDuration TotalDuration => new UnknownTweenDuration();
}
