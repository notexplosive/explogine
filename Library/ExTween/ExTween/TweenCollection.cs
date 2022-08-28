using System;
using System.Collections.Generic;

namespace ExTween
{
    public class TweenCollection
    {
        protected readonly List<ITween> Items = new List<ITween>();

        public int ChildrenWithDurationCount
        {
            get
            {
                var i = 0;
                foreach (var item in Items)
                {
                    if (item.TotalDuration is KnownTweenDuration known && known > 0)
                    {
                        i++;
                    }
                }

                return i;
            }
        }

        protected void ForEachItem(Action<ITween> action)
        {
            foreach (var item in Items)
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
            Items.Clear();
        }

        public override string ToString()
        {
            return $"TweenCollection[{Items.Count}]";
        }
    }
}
