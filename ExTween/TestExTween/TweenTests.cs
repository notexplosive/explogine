using ExTween;
using FluentAssertions;
using Xunit;

namespace TestExTween
{
    public class TweenTests
    {
        [Fact]
        public void int_linear_lerp()
        {
            var tweenable = new TweenableInt(-100);
            var tween = new Tween<int>(tweenable, 100, 1, Ease.Linear);

            tween.Update(0.25f);
            tween.Update(0.25f);

            tween.CurrentTime.Should().Be(0.5f);
            tweenable.Value.Should().Be(0);
        }
        
        [Fact]
        public void captured_float()
        {
            // This represents some external data. Maybe it's your altitude off the ground?
            float captureMe = 0f;
            // To capture argument, we have to do this boilerplate, there's no way to shorten this.
            //      You must have a GETTER function that returns captured variable
            //      And a SETTER function that takes a value, and assigns it to the captured variable
            var tweenable = new TweenableFloat(() => captureMe, val => captureMe = val);
         
            // Create a tween and advance through it
            var tween = new Tween<float>(tweenable, 3.14f, 1, Ease.Linear);
            tween.Update(1f);

            captureMe.Should().Be(3.14f); // the value has changed!
            tweenable.Value.Should().Be(3.14f); // the tweenable agrees that the value has changed.
        }

        [Fact]
        public void long_duration()
        {
            var tweenable = new TweenableInt(0);

            var tween = new Tween<int>(tweenable, 100, 50, Ease.Linear);
            tween.Update(5);

            tweenable.Value.Should().Be(10);
        }
        
        [Fact]
        public void finished_value_tweens_report_when_done()
        {
            var tweenable = new TweenableInt(0);
            var tween = new Tween<int>(tweenable, 100, 5, Ease.Linear);

            tween.Update(2);
            var wasDone = tween.IsDone();
            tween.Update(3);

            wasDone.Should().BeFalse();
            tween.IsDone().Should().BeTrue();
        }
        
        [Fact]
        public void finished_wait_tweens_report_when_done()
        {
            var tween = new WaitSecondsTween(5);

            tween.Update(2);
            var wasDone = tween.IsDone();
            tween.Update(3);

            wasDone.Should().BeFalse();
            tween.IsDone().Should().BeTrue();
        }

        [Fact]
        public void zero_time_tween_finishes_instantly()
        {
            var tween = new WaitSecondsTween(0);

            tween.Update(0);

            tween.IsDone().Should().BeTrue();
        }
        
        [Fact]
        public void tweens_can_be_reset()
        {
            var tweenable = new TweenableInt(0);
            
            var tween = new Tween<int>(tweenable,100, 1, Ease.Linear);

            tween.Update(1);
            var wasDone = tween.IsDone();
            tween.Reset();

            // Tweenable value is unchanged
            tweenable.Value.Should().Be(100);
            // Tween was done when it finished the first time
            wasDone.Should().BeTrue();
            // Tween is currently not done because it was reset
            tween.IsDone().Should().BeFalse();
        }

        [Fact]
        public void tween_update_returns_overflow()
        {
            var tweenable = new TweenableInt(0);
            
            var tween = new Tween<int>(tweenable,100, 1, Ease.Linear);

            var firstOverflow = tween.Update(0.5f);
            var secondOverflow = tween.Update(0.75f);

            // no overflow, tween consumed the whole time
            firstOverflow.Should().Be(0f);
            secondOverflow.Should().Be(0.25f);
        }

        [Fact]
        public void wait_tween_overflows()
        {
            var tweenable = new WaitSecondsTween(1);

            var firstOverflow = tweenable.Update(0.5f);
            var secondOverflow = tweenable.Update(1f);
            var thirdOverflow = tweenable.Update(0.25f);

            firstOverflow.Should().Be(0);
            secondOverflow.Should().Be(0.5f);
            thirdOverflow.Should().Be(0.25f);
        }
        
        [Fact]
        public void tween_leaves_value_alone_when_overflowed()
        {
            var tweenable = new TweenableInt(0);
            
            var tween = new Tween<int>(tweenable,100, 1, Ease.Linear);

            var overflow = tween.Update(20); // 20 seconds is significantly longer than 1 second

            overflow.Should().Be(19);
            tweenable.Value.Should().Be(100);
        }

        [Fact]
        public void tween_can_jump_to_arbitrary_value()
        {
            var tweenable = new TweenableInt(0);
            
            var tween = new Tween<int>(tweenable,100, 1, Ease.Linear);

            tween.JumpTo(0.3f);
            var valueAt30Percent = tweenable.Value;
            tween.JumpTo(0f);
            var valueAt0Percent = tweenable.Value;
            tween.JumpTo(0.6f);
            var valueAt60Percent = tweenable.Value;
            
            valueAt30Percent.Should().Be(30);
            valueAt0Percent.Should().Be(0);
            valueAt60Percent.Should().Be(60);
        }
    }
}