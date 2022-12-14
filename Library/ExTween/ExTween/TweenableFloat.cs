namespace ExTween;

public class TweenableFloat : Tweenable<float>
{
    public TweenableFloat() : base(0)
    {
    }

    public TweenableFloat(float i) : base(i)
    {
    }

    public TweenableFloat(Getter getter, Setter setter) : base(getter, setter)
    {
    }

    public override float Lerp(float startingValue, float targetValue, float percent)
    {
        return FloatExtensions.Lerp(startingValue, targetValue, percent);
    }

    public override string ValueAsString()
    {
        return $"{Value:F3}";
    }
}
