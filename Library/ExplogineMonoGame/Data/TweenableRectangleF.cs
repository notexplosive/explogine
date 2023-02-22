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

    public static RectangleF LerpRectangleF(RectangleF startingValue, RectangleF targetValue, float percent)
    {
        var x = FloatLerp(startingValue.Location.X, targetValue.Location.X, percent);
        var y = FloatLerp(startingValue.Location.Y, targetValue.Location.Y, percent);
        var width = FloatLerp(startingValue.Size.X, targetValue.Size.X, percent);
        var height = FloatLerp(startingValue.Size.Y, targetValue.Size.Y, percent);

        return new RectangleF(x, y, width, height);
    }
    
    public override RectangleF Lerp(RectangleF startingValue, RectangleF targetValue, float percent)
    {
        return LerpRectangleF(startingValue, targetValue, percent);
    }

    private static float FloatLerp(float startingValue, float targetValue, float percent)
    {
        return startingValue + (targetValue - startingValue) * percent;
    }
}
