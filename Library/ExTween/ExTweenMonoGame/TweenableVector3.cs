using ExTween;
using Microsoft.Xna.Framework;

namespace ExTweenMonoGame;

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
