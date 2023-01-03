using System;

namespace ExTween;

public class WaitSecondsTween : ITween
{
    private readonly float _duration;
    private float _timer;

    public WaitSecondsTween(float duration)
    {
        _duration = duration;
        _timer = duration;
    }

    public ITweenDuration TotalDuration => new KnownTweenDuration(_duration, Math.Min(_duration - _timer, _duration));

    public float Update(float dt)
    {
        if (IsDone())
        {
            return dt;
        }

        _timer -= dt;

        return Math.Max(-_timer, 0);
    }

    public bool IsDone()
    {
        return _timer <= 0;
    }

    public void Reset()
    {
        _timer = _duration;
    }

    public void JumpTo(float time)
    {
        _timer = _duration - time;
    }

    public void SkipToEnd()
    {
        _timer = 0;
    }
}
