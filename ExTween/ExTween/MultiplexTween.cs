using System;

namespace ExTween
{
    public class MultiplexTween : TweenCollection, ITween
    {
        public float Update(float dt)
        {
            float totalOverflow = 0;
            var hasInitializedOverflow = false;
            ForEachItem(
                tween =>
                {
                    var pendingOverflow = tween.Update(dt);
                    if (!hasInitializedOverflow)
                    {
                        hasInitializedOverflow = true;
                        totalOverflow = pendingOverflow;
                    }
                    else
                    {
                        totalOverflow = MathF.Min(totalOverflow, pendingOverflow);
                    }
                });

            return totalOverflow;
        }

        public bool IsDone()
        {
            var result = true;
            ForEachItem(item => result = result && item.IsDone());
            return result;
        }

        public void Reset()
        {
            ResetAllItems();
        }

        public void JumpTo(float time)
        {
            Reset();
            ForEachItem(item => { item.JumpTo(time); });
        }

        public MultiplexTween AddChannel(ITween tween)
        {
            this.Items.Add(tween);
            return this;
        }

        public ITweenDuration TotalDuration
        {
            get
            {
                var result = 0f;
                ForEachItem(item =>
                {
                    if (item.TotalDuration is KnownTweenDuration itemDuration)
                    {
                        result = Math.Max(result, itemDuration);
                    }
                });
                
                return new KnownTweenDuration(result);
            }
        }
    }
}
