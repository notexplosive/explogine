using System;
using ExTween;
using FluentAssertions;
using Xunit;

namespace TestExTween
{
    public class MultiplexTweenTests
    {
        [Fact]
        public void multiplex_tweens_increment_both_children()
        {
            var tweenableA = new TweenableInt(0);
            var tweenableB = new TweenableInt(0);

            var tween = new MultiplexTween()
                .AddChannel(new Tween<int>(tweenableA, 100, 1, Ease.Linear))
                .AddChannel(new Tween<int>(tweenableB, 200, 1, Ease.Linear));

            tween.Update(0.5f);

            tweenableA.Value.Should().Be(50);
            tweenableB.Value.Should().Be(100);
        }

        [Fact]
        public void multiplex_tween_continues_when_one_tween_is_done()
        {
            var tweenableA = new TweenableInt(0);
            var tweenableB = new TweenableInt(0);

            var tween = new MultiplexTween()
                .AddChannel(new Tween<int>(tweenableA, 100, 1, Ease.Linear))
                .AddChannel(new Tween<int>(tweenableB, 200, 2, Ease.Linear));

            tween.Update(1.5f);

            tweenableA.Value.Should().Be(100);
            tweenableB.Value.Should().Be(150);
        }
        
        [Fact]
        public void multiplex_tween_reports_correct_overflow()
        {
            var tweenableA = new TweenableInt(0);
            var tweenableB = new TweenableInt(0);

            var tween = new MultiplexTween()
                .AddChannel(new Tween<int>(tweenableA, 100, 0.25f, Ease.Linear))
                .AddChannel(new Tween<int>(tweenableB, 200, 1f, Ease.Linear));

            var overflow = tween.Update(1.2f);

            overflow.Should().BeApproximately(0.2f, 0.00001f);
        }

        [Fact]
        public void multiplex_only_fires_callback_once_when_stalled()
        {
            int hitCount = 0;

            var tween = new MultiplexTween()
                .AddChannel(new WaitSecondsTween(2))
                .AddChannel(new CallbackTween(() => { hitCount++; }));

            tween.Update(0.25f);
            tween.Update(0.25f);

            hitCount.Should().Be(1);
        }

        [Fact]
        public void multiplex_tween_reports_correct_duration()
        {
            var tween = new MultiplexTween()
                .AddChannel(new WaitSecondsTween(2))
                .AddChannel(new WaitSecondsTween(7))
                .AddChannel(new WaitSecondsTween(4))
                .AddChannel(new WaitSecondsTween(5));

            tween.TotalDuration.Get().Should().Be(7);
        }
        
        [Fact]
        public void adding_to_a_multiplex_after_its_done_makes_it_not_done()
        {
            var tweenableA = new TweenableInt(0);
            var tweenableB = new TweenableInt(0);
            var tween = new MultiplexTween();
            tween.AddChannel(new Tween<int>(tweenableA, 100, 1, Ease.Linear));

            tween.Update(1.2f);
            tween.AddChannel(new Tween<int>(tweenableB, 100, 1, Ease.Linear));
            tween.Update(0.5f);

            tweenableA.Value.Should().Be(100);
            tweenableB.Value.Should().Be(50);
        }

        [Fact]
        public void random_access_multiplex_tween()
        {
            var tweenableX = new TweenableFloat();
            var tweenableY = new TweenableFloat();

            var tween = new MultiplexTween()
                .AddChannel(new Tween<float>(tweenableX, 100, 1, Ease.Linear))
                .AddChannel(new Tween<float>(tweenableY, 50, 1, Ease.Linear))
                ;


            float ExpectedX(float x)
            {
                return x * 100;
            }

            float ExpectedY(float y)
            {
                return y * 50;
            }
            
            tween.JumpTo(0.25f);
            tweenableX.Value.Should().Be(ExpectedX(0.25f));
            tweenableY.Value.Should().Be(ExpectedY(0.25f));
            
            tween.JumpTo(0.0f);
            tweenableX.Value.Should().Be(ExpectedX(0.0f));
            tweenableY.Value.Should().Be(ExpectedY(0.0f));
            
            tween.JumpTo(0.05f);
            tweenableX.Value.Should().Be(ExpectedX(0.05f));
            tweenableY.Value.Should().Be(ExpectedY(0.05f));
            
            // random access means RANDOM access
            var random = new Random(0x0badf00d);
            for (int i = 0; i < 1000; i++)
            {
                var time = (float)random.NextDouble();
                tween.JumpTo(time);
                tweenableX.Value.Should().Be(ExpectedX(time));
                tweenableY.Value.Should().Be(ExpectedY(time));
            }
        }
    }
}
