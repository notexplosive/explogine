using System;
using System.Collections.Generic;

namespace ExTween;

public abstract class TweenCollection
{
    protected readonly List<ITween> Items = new();

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
        Reset();
        Items.Clear();
    }

    public abstract void Reset();

    public override string ToString()
    {
        return $"TweenCollection[{Items.Count}]";
    }
}
