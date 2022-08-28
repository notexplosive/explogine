using System;

namespace ExTween
{
    public class SequenceTween : TweenCollection, ITween
    {
        private int currentItemIndex;
        
        public bool IsLooping { get; set; }

        public SequenceTween()
        {
            this.currentItemIndex = 0;
        }
        
        public float Update(float dt)
        {
            if (this.Items.Count == 0)
            {
                return dt;
            }
            
            if (IsAtEnd())
            {
                if (IsLooping)
                {
                    Reset();
                }
                else
                {
                    return dt;
                }
            }

            var overflow = this.Items[this.currentItemIndex].Update(dt);

            if (this.Items[this.currentItemIndex].IsDone())
            {
                this.currentItemIndex++;
                return Update(overflow);
            }

            return overflow;
        }

        public bool IsDone()
        {
            return IsAtEnd() && !IsLooping;
        }

        private bool IsAtEnd()
        {
            return this.currentItemIndex >= this.Items.Count || this.Items.Count == 0;
        }

        public void Reset()
        {
            ResetAllItems();
            this.currentItemIndex = 0;
        }

        public ITweenDuration TotalDuration
        {
            get
            {
                var total = 0f;
                ForEachItem(item =>
                {
                    if (item.TotalDuration is KnownTweenDuration itemDuration)
                    {
                        total += itemDuration;
                    }
                });
                
                return new KnownTweenDuration(total);
            }
        }

        public SequenceTween Add(ITween tween)
        {
            this.Items.Add(tween);
            return this;
        }

        public void JumpTo(float targetTime)
        {
            Reset();

            var adjustedTargetTime = targetTime;
            
            for (int i = 0; i < this.Items.Count; i++)
            {
                var itemDuration = this.Items[i].TotalDuration;
                if (itemDuration is UnknownTweenDuration)
                {
                    // We don't know how long this tween is, so we have to update it manually
                    var overflow = this.Items[i].Update(adjustedTargetTime);
                    adjustedTargetTime -= overflow;

                    if (!this.Items[i].IsDone())
                    {
                        break;
                    }
                }
                else
                {
                    if (itemDuration is KnownTweenDuration exactTweenDuration && adjustedTargetTime >= exactTweenDuration)
                    {
                        adjustedTargetTime -= exactTweenDuration;
                        this.Items[i].Update(exactTweenDuration);
                    }
                    else
                    {
                        this.Items[i].Update(adjustedTargetTime);
                        this.currentItemIndex = i;
                        break;
                    }
                }
            }
        }
    }
}
