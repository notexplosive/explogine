namespace ExTween;

public interface ITween
{
    public ITweenDuration TotalDuration { get; }

    /// <summary>
    ///     Updates the tween and returns the overflow.
    /// </summary>
    /// <param name="dt"></param>
    /// <returns>The amount overflowed, eg: if a tween only had 0.1 seconds left but `dt` was 0.3, there is an overflow of 0.2. </returns>
    public float Update(float dt);

    public bool IsDone();
    public void Reset();
    public void JumpTo(float time);
    public void SkipToEnd();
}
