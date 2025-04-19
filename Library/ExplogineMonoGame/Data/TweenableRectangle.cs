using ExTween;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class TweenableRectangle : Tweenable<Rectangle>
{
    public TweenableRectangle(Rectangle initializedValue) : base(initializedValue)
    {
    }

    public TweenableRectangle(Getter getter, Setter setter) : base(getter, setter)
    {
    }

    public override Rectangle Lerp(Rectangle startingValue, Rectangle targetValue, float percent)
    {
        var x = IntLerp(startingValue.Location.X, targetValue.Location.X, percent);
        var y = IntLerp(startingValue.Location.Y, targetValue.Location.Y, percent);
        var width = IntLerp(startingValue.Size.X, targetValue.Size.X, percent);
        var height = IntLerp(startingValue.Size.Y, targetValue.Size.Y, percent);

        return new Rectangle(x, y, width, height);
    }

    private int IntLerp(int startingValue, int targetValue, float percent)
    {
        return (int) (startingValue + (targetValue - startingValue) * percent);
    }
}
