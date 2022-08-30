namespace ExTween;

public class TweenableInt : Tweenable<int>
{
    public TweenableInt() : base(0)
    {
    }

    public TweenableInt(int i) : base(i)
    {
    }

    public TweenableInt(Getter getter, Setter setter) : base(getter, setter)
    {
    }

    public override int Lerp(int startingValue, int targetValue, float percent)
    {
        return (int) (startingValue + (targetValue - startingValue) * percent);
    }
}
