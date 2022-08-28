using System;

namespace ExTween
{
    public class WaitSecondsTween : ITween
    {
        private readonly float duration;
        private float timer;

        public WaitSecondsTween(float duration)
        {
            this.duration = duration;
            timer = duration;
        }

        public ITweenDuration TotalDuration => new KnownTweenDuration(duration);

        public float Update(float dt)
        {
            if (IsDone())
            {
                return dt;
            }

            timer -= dt;

            return Math.Max(-timer, 0);
        }

        public bool IsDone()
        {
            return timer <= 0;
        }

        public void Reset()
        {
            timer = duration;
        }

        public void JumpTo(float time)
        {
            timer = duration - time;
        }
    }
}
