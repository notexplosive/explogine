using System;
using ExTween;
using FluentAssertions;
using Xunit;

namespace TestExTween
{
    public class IntegrationTests
    {
        [Fact]
        public void circle_integration_test()
        {
            var radius = 1f;
            var duration = 1f;
            
            var tweenableX = new TweenableFloat(radius);
            var tweenableY = new TweenableFloat(0);

            // tween that represents a circular pattern, starts at the same spot where it ends
            var tween = new SequenceTween()
                    .Add(new CallbackTween(() =>
                    {
                        // We need this callback in order for the JumpTo to work
                        tweenableX.Value = radius;
                        tweenableY.Value = 0;
                    }))

                    .Add(new MultiplexTween()
                        .AddChannel(new Tween<float>(tweenableX, 0, duration / 4f, Ease.SineSlowFast))
                        .AddChannel(new Tween<float>(tweenableY, radius, duration / 4f, Ease.SineFastSlow)))
                    .Add(new MultiplexTween()
                        .AddChannel(new Tween<float>(tweenableX, -radius, duration / 4f, Ease.SineFastSlow))
                        .AddChannel(new Tween<float>(tweenableY, 0, duration / 4f, Ease.SineSlowFast)))
                    .Add(new MultiplexTween()
                        .AddChannel(new Tween<float>(tweenableX, 0, duration / 4f, Ease.SineSlowFast))
                        .AddChannel(new Tween<float>(tweenableY, -radius, duration / 4f, Ease.SineFastSlow)))
                    .Add(new MultiplexTween()
                        .AddChannel(new Tween<float>(tweenableX, radius, duration / 4f, Ease.SineFastSlow))
                        .AddChannel(new Tween<float>(tweenableY, 0, duration / 4f, Ease.SineSlowFast)))
                ;

            // baseline of a circle to compare to
            float ExpectedX(float time)
            {
                var radians = time / duration * MathF.PI * 2;
                return MathF.Cos(radians);
            }

            float ExpectedY(float time)
            {
                var radians = time / duration * MathF.PI * 2;
                return MathF.Sin(radians);
            }
            
            // random access any arbitrary point in the tween
            var random = new Random(0x0badf00d);
            for (var i = 0; i < 100; i++)
            {
                var targetVal = (float) random.NextDouble();

                tween.JumpTo(targetVal);
                tweenableX.Value.Should().BeApproximately(ExpectedX(targetVal),0.001f);
                tweenableY.Value.Should().BeApproximately(ExpectedY(targetVal),0.001f);
            }
        }
    }
}
