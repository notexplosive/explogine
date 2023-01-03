using System;

namespace ExTween;

public readonly struct UnknownTweenDuration : ITweenDuration
{
    public float GetDuration()
    {
        throw new Exception("Value unknown");
    }

    public float GetCurrentTime()
    {
        throw new Exception("Value unknown");
    }
}
