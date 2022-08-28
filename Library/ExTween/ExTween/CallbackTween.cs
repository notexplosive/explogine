using System;

namespace ExTween
{
    public class CallbackTween : ITween
    {
        private readonly Action behavior;
        private bool hasExecuted;

        public CallbackTween(Action behavior)
        {
            this.behavior = behavior;
        }

        public float Update(float dt)
        {
            if (!hasExecuted)
            {
                behavior();
                hasExecuted = true;
            }

            // Instant tween, always overflows 100% of dt
            return dt;
        }

        public bool IsDone()
        {
            return hasExecuted;
        }

        public void Reset()
        {
            hasExecuted = false;
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
            if (condition())
            {
                // Instant tween, always overflows 100% of dt
                return dt;
            }

            return 0;
        }

        public bool IsDone()
        {
            return condition();
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
