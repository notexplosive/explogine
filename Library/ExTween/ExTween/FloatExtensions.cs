namespace ExTween;

public static class FloatExtensions
{
    public static float Lerp(float startingValue, float targetValue, float percent)
    {
        startingValue = Sanitized(startingValue);
        targetValue = Sanitized(targetValue);
        return startingValue + (targetValue - startingValue) * percent;
    }

    private static float Sanitized(float value)
    {
        if (float.IsNaN(value))
        {
            return 0;
        }

        if (float.IsNegativeInfinity(value))
        {
            return float.MinValue;
        }

        if (float.IsInfinity(value))
        {
            return float.MaxValue;
        }

        return value;
    }
}

public static class DoubleExtensions
{
    public static double LerpAlongFloatPercent(double startingValue, double targetValue, float percent)
    {
        return startingValue + (targetValue - startingValue) * percent;
    }
}
