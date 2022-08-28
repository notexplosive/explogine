using System;

namespace ExTween
{
    public class CallbackTween : ITween
    {
        private bool hasExecuted;
        private readonly Action behavior;

        public CallbackTween(Action behavior)
        {
            this.behavior = behavior;
        }
        
        public float Update(float dt)
        {
            if (!this.hasExecuted)
            {
                this.behavior();
                this.hasExecuted = true;
            }

            // Instant tween, always overflows 100% of dt
            return dt;
        }

        public bool IsDone()
        {
            return this.hasExecuted;
        }

        public void Reset()
        {
            this.hasExecuted = false;
        }
        
        public void JumpTo(float time)
        {
            Update(time);
        }

        public ITweenDuration TotalDuration => new KnownTweenDuration(0);
    }

    public class WaitUntilTween : ITween
    {
        private readonly Func<bool> condition;

        public WaitUntilTween(Func<bool> condition)
        {
            this.condition = condition;
        }
        
        public float Update(float dt)
        {
            if (this.condition())
            {
                // Instant tween, always overflows 100% of dt
                return dt;
            }

            return 0;
        }

        public bool IsDone()
        {
            return this.condition();
        }

        public void Reset()
        {
            // no op
        }

        public void JumpTo(float time)
        {
            Update(time);
        }

        public ITweenDuration TotalDuration => new UnknownTweenDuration();
    }
}
