namespace ExTween;

public readonly struct KnownTweenDuration : ITweenDuration
{
    public float CurrentTime { get; }

    public KnownTweenDuration(float duration, float currentTime)
    {
        CurrentTime = currentTime;
        Duration = duration;
    }

    public float Duration { get; }

    public float GetDuration()
    {
        return Duration;
    }

    public float GetCurrentTime()
    {
        return CurrentTime;
    }

    public static implicit operator float(KnownTweenDuration me)
    {
        return me.GetDuration();
    }

    public override string ToString()
    {
        return Duration.ToString("N4");
    }
}
