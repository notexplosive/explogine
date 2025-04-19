using ExTween;
using Microsoft.Xna.Framework;

namespace ExTweenMonoGame;

public class TweenablePoint : Tweenable<Point>
{
    public TweenablePoint(Point initializedValue) : base(initializedValue)
    {
    }

    public TweenablePoint(Getter getter, Setter setter) : base(getter, setter)
    {
    }

    public TweenablePoint() : base(Point.Zero)
    {
    }

    public override Point Lerp(Point startingValue, Point targetValue, float percent)
    {
        return startingValue
               + new Point((int) (targetValue.X * percent), (int) (targetValue.Y * percent))
               - new Point((int) (startingValue.X * percent), (int) (startingValue.Y * percent));
    }
}
