using ExTween;
using Microsoft.Xna.Framework;

namespace ExTweenMonoGame
{
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

    public class TweenableVector2 : Tweenable<Vector2>
    {
        public TweenableVector2(Vector2 initializedValue) : base(initializedValue)
        {
        }

        public TweenableVector2(Getter getter, Setter setter) : base(getter, setter)
        {
        }

        public TweenableVector2() : base(Vector2.Zero)
        {
        }

        public override Vector2 Lerp(Vector2 startingValue, Vector2 targetValue, float percent)
        {
            return startingValue + (targetValue - startingValue) * percent;
        }
    }

    public class TweenableVector3 : Tweenable<Vector3>
    {
        public TweenableVector3(Vector3 initializedValue) : base(initializedValue)
        {
        }

        public TweenableVector3(Getter getter, Setter setter) : base(getter, setter)
        {
        }

        public TweenableVector3() : base(Vector3.Zero)
        {
        }

        public override Vector3 Lerp(Vector3 startingValue, Vector3 targetValue, float percent)
        {
            return startingValue + (targetValue - startingValue) * percent;
        }
    }
}
