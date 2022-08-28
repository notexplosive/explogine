using System;
using ExTween;
using FluentAssertions;
using Xunit;

namespace TestExTween
{
    public class SequenceTweenTests
    {
        [Fact]
        public void sequences_can_have_just_one_item()
        {
            var tweenable = new TweenableFloat(0);
            var sequence = new SequenceTween();
            sequence.Add(new Tween<float>(tweenable, 100, 1, Ease.Linear));
        }

        [Fact]
        public void transition_to_next_item()
        {
            var tweenable = new TweenableFloat(0);
            var sequence = new SequenceTween()
                .Add(new Tween<float>(tweenable, 100, 0.5f, Ease.Linear))
                .Add(new Tween<float>(tweenable, 200, 1, Ease.Linear));

            sequence.Update(0.75f);

            tweenable.Value.Should().Be(125);
        }

        [Fact]
        public void consecutive_callback_tweens_fire_all_at_once()
        {
            var tweenable = new TweenableInt(0);
            var tween = new SequenceTween();
            var hitCount = 0;

            void Hit()
            {
                hitCount++;
            }

            tween.Add(new WaitSecondsTween(1f));
            tween.Add(new CallbackTween(Hit));
            tween.Add(new CallbackTween(Hit));
            tween.Add(new CallbackTween(Hit));
            tween.Add(new Tween<int>(tweenable, 10, 1f, Ease.Linear));

            tween.Update(1.5f);

            tweenable.Value.Should().Be(5);
            hitCount.Should().Be(3);
        }

        [Fact]
        public void wait_until_should_halt_sequence_when_false()
        {
            var tween = new SequenceTween();
            var hitCount = 0;

            void Hit()
            {
                hitCount++;
            }

            tween.Add(new CallbackTween(Hit));
            tween.Add(new WaitUntilTween(() => false));
            tween.Add(new CallbackTween(Hit));

            tween.Update(0);
            tween.Update(0);

            hitCount.Should().Be(1);
        }

        [Fact]
        public void wait_until_should_permit_sequence_when_true()
        {
            var tween = new SequenceTween();
            var hitCount = 0;

            void Hit()
            {
                hitCount++;
            }

            tween.Add(new CallbackTween(Hit));
            tween.Add(new WaitUntilTween(() => true));
            tween.Add(new CallbackTween(Hit));

            tween.Update(0);

            hitCount.Should().Be(2);
        }

        [Fact]
        public void adding_to_a_sequence_after_its_done_makes_it_not_done()
        {
            var tweenable = new TweenableInt();
            var tween = new SequenceTween();
            tween.Add(new Tween<int>(tweenable, 100, 1, Ease.Linear));

            tween.Update(1.2f);
            tween.Add(new Tween<int>(tweenable, 120, 1, Ease.Linear));
            tween.Update(0.5f);

            tweenable.Value.Should().Be(110);
        }

        [Fact]
        public void jump_to_value_within_a_single_sequence_item()
        {
            var tweenable = new TweenableInt();

            var tween = new SequenceTween()
                .Add(new CallbackTween(() => { tweenable.Value = 0; }))
                .Add(new Tween<int>(tweenable, 100, 1, Ease.Linear))
                .Add(new Tween<int>(tweenable, -100, 1, Ease.Linear));

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

        [Fact]
        public void condition_blocks_jump_to()
        {
            var hitCount = 0;
            void Hit()
            {
                hitCount++;
            }
            
            var tween = new SequenceTween()
                    .Add(new CallbackTween(Hit))
                    .Add(new WaitUntilTween(() => false))
                    .Add(new CallbackTween(Hit))
                ;

            tween.JumpTo(1);
            
            hitCount.Should().Be(1);
        }
        
        [Fact]
        public void condition_permits_jump_to()
        {
            var hitCount = 0;
            var permit = false;
            void Hit()
            {
                hitCount++;
            }
            
            var tween = new SequenceTween()
                    .Add(new CallbackTween(Hit))
                    .Add(new CallbackTween(()=>permit = true))
                    .Add(new WaitUntilTween(() => permit))
                    .Add(new CallbackTween(Hit))
                ;

            tween.JumpTo(1);
            
            hitCount.Should().Be(2);
        }

        [Fact]
        public void random_access_sequence_tween()
        {
            var tweenable = new TweenableInt();

            var tween = new SequenceTween()
                    // You need a callback at the start that sets everything to their starting values
                    .Add(new CallbackTween(() => { tweenable.Value = 0; }))
                    .Add(new Tween<int>(tweenable, 100, 1, Ease.Linear))
                    .Add(new Tween<int>(tweenable, 200, 1, Ease.Linear))
                    .Add(new Tween<int>(tweenable, 400, 1, Ease.Linear))
                ;

            int ExpectedValue(float time)
            {
                if (time < 1)
                {
                    return (int) (time * 100);
                }

                if (time < 2)
                {
                    return (int) ((time - 1) * 100) + 100;
                }

                if (time < 3)
                {
                    return (int) ((time - 2) * 200) + 100 + 100;
                }

                return 400;
            }

            // random access means RANDOM access
            var random = new Random(0x0badf00d);
            for (var i = 0; i < 1000; i++)
            {
                var time = (float) random.NextDouble() * 3;
                tween.JumpTo(time);
                tweenable.Value.Should().Be(ExpectedValue(time));
            }
        }
    }
}
