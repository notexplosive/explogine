using ExTween;
using Microsoft.Xna.Framework;

namespace ExTweenMonoGame;

public class TweenableColor : Tweenable<Color>
{
    public TweenableColor(Color initializedValue) : base(initializedValue)
    {
    }

    public TweenableColor(Getter getter, Setter setter) : base(getter, setter)
    {
    }

    public override Color Lerp(Color startingValue, Color targetValue, float percent)
    {
        return Color.Lerp(startingValue, targetValue, percent);
    }
}
