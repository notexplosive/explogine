using System;
using System.Collections.Generic;

namespace ExTween
{
    public class TweenCollection
    {
        protected readonly List<ITween> Items = new List<ITween>();

        protected void ForEachItem(Action<ITween> action)
        {
            foreach (var item in this.Items)
            {
                action(item);
            }
        }

        public void ResetAllItems()
        {
            ForEachItem(item => item.Reset());
        }

        public void Clear()
        {
            this.Items.Clear();
        }

        public int ChildrenWithDurationCount
        {
            get
            {
                var i = 0;
                foreach (var item in this.Items)
                {
                    if (item.TotalDuration is KnownTweenDuration known && known > 0)
                    {
                        i++;
                    }
                }

                return i;
            }
        }

        public override string ToString()
        {
            return $"TweenCollection[{this.Items.Count}]";
        }
    }
}
