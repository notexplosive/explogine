namespace ExTween;

public interface IValueTween
{
    /// <summary>
    /// Only really used for debugging and rendering
    /// </summary>
    /// <returns></returns>
    public string TweenableValueAsString();

    int TweenableHashCode();
}
