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
            this.timer = duration;
        }

        public ITweenDuration TotalDuration => new KnownTweenDuration(this.duration);
        
        public float Update(float dt)
        {
            if (IsDone())
            {
                return dt;
            }
            
            this.timer -= dt;

            return Math.Max(-this.timer, 0);
        }

        public bool IsDone()
        {
            return this.timer <= 0;
        }

        public void Reset()
        {
            this.timer = this.duration;
        }

        public void JumpTo(float time)
        {
            this.timer = this.duration - time;
        }
    }
}
