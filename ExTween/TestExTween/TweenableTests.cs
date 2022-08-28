using ExTween;
using FluentAssertions;
using Xunit;

namespace TestExTween
{
    public class TweenableTests
    {
        [Fact]
        public void create_custom_tweenable_type()
        {
            // Parameterless constructor generates 
            var defaultPoint = new TweenablePoint2D();

            var initializedPoint = new TweenablePoint2D(new Point2D(5, 5));

            var capturedPoint = new Point2D(-10, 10);
            new TweenablePoint2D(() => capturedPoint, val => capturedPoint = val);

            defaultPoint.Value.Should().Be(new Point2D(0, 0));
            initializedPoint.Value.Should().Be(new Point2D(5, 5));
            capturedPoint.Should().Be(new Point2D(-10, 10));
        }

        [Fact]
        public void interpolate_custom_tweenable_type()
        {
            var tweenable = new TweenablePoint2D(new Point2D(0, 0));
            
            // Tweens always follow the format: new Tween<MyType>(TweenableMyType, [...])
            // Ideally that type parameter could be inferred but .NET 3.1 isn't smart enough.
            var tween = new Tween<Point2D>(tweenable, new Point2D(100,400), 1, Ease.Linear);

            tween.Update(0.75f);

            tweenable.Value.Should().Be(new Point2D(75, 300));
        }

        /// <summary>
        ///     A very simple 2D point to demonstrate how to create tweenables
        /// </summary>
        private struct Point2D
        {
            public Point2D(float x, float y)
            {
                X = x;
                Y = y;
            }

            // You don't need { setters } for these values, your custom type can be readonly if you want it to be.
            public float X { get; }
            public float Y { get; }
        }

        /// <summary>
        ///     A tweenable wrapper around Point2D.
        ///     C# requires you to implement the constructors, but the base constructor does all the work.
        ///     You will need to define the lerp function.
        /// </summary>
        private class TweenablePoint2D : Tweenable<Point2D>
        {
            // Required: You must define the initializedValue constructor, but you should just forward to the base
            // This allows us to do `new TweenablePoint2D(new Point2D(x,y))`
            public TweenablePoint2D(Point2D initializedValue) : base(initializedValue)
            {
                // boilerplate: leave empty
            }

            // Required: You must define the getter/setter constructor, but you should just forward to the base
            // This allows us to do `new TweenablePoint2D(()=>myPoint, val => myPoint = val)`
            public TweenablePoint2D(Getter getter, Setter setter) : base(getter, setter)
            {
                // boilerplate: leave empty
            }

            // Optional we can define a parameterless constructor if there is a sensible "zero" value of the custom type
            public TweenablePoint2D() : base(new Point2D(0, 0))
            {
            }

            // Required: Define whatever a "Lerp" means for your object.
            // Lerp functions usually look like: `start + (target - start) * percent`
            public override Point2D Lerp(Point2D startingValue, Point2D targetValue, float percent)
            {
                return
                    new Point2D(
                        startingValue.X + (targetValue.X - startingValue.X) * percent,
                        startingValue.Y + (targetValue.Y - startingValue.Y) * percent
                    );
            }
        }
    }
}
