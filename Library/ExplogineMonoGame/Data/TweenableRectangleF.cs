using ExTween;

namespace ExplogineMonoGame.Data;

public class TweenableRectangleF : Tweenable<RectangleF>
{
    public TweenableRectangleF(RectangleF initializedValue) : base(initializedValue)
    {
    }

    public TweenableRectangleF(Getter getter, Setter setter) : base(getter, setter)
    {
    }

    public override RectangleF Lerp(RectangleF startingValue, RectangleF targetValue, float percent)
    {
        return RectangleF.Lerp(startingValue, targetValue, percent);
    }
}
