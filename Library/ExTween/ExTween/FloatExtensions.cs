namespace ExTween;

public static class FloatExtensions
{
    public static float Lerp(float startingValue, float targetValue, float percent)
    {
        return startingValue + (targetValue - startingValue) * percent;
    }
}

