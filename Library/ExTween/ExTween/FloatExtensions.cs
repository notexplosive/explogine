namespace ExTween;

public static class FloatExtensions
{
    public static float Lerp(float startingValue, float targetValue, float percent)
    {
        return startingValue + (targetValue - startingValue) * percent;
    }
}

public static class DoubleExtensions
{
    public static double LerpAlongFloatPercent(double startingValue, double targetValue, float percent)
    {
        return startingValue + (targetValue - startingValue) * percent;
    }
}
