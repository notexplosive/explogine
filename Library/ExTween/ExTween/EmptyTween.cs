namespace ExTween;

public readonly struct EmptyTween : ITween
{
    public ITweenDuration TotalDuration => new KnownTweenDuration(0, 0);
    public float Update(float dt)
    {
        return dt;
    }

    public bool IsDone()
    {
        return true;
    }

    public void Reset()
    {
    }

    public void JumpTo(float time)
    {
    }

    public void SkipToEnd()
    {
    }
}
