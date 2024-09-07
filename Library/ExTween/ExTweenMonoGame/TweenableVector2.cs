using ExTween;
using Microsoft.Xna.Framework;

namespace ExTweenMonoGame
{
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
}
