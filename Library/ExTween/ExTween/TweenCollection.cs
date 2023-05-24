using System;
using System.Collections;
using System.Collections.Generic;

namespace ExTween;

public abstract class TweenCollection : ITween
{
    protected readonly List<ITween> Items = new();

    public int ChildCount => Items.Count;

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

    /// <summary>
    /// Generic version of "Add" or "AddChannel", use this if you're not sure which type of TweenCollection you're dealing with
    /// </summary>
    /// <param name="tween"></param>
    public void AddItem(ITween tween)
    {
        Items.Add(tween);
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

    public abstract ITweenDuration TotalDuration { get; }
    public abstract float Update(float dt);
    public abstract bool IsDone();
    public abstract void Reset();
    public abstract void JumpTo(float time);
    public abstract void SkipToEnd();

    public override string ToString()
    {
        return $"TweenCollection[{Items.Count}]";
    }

    public IEnumerable<ITween> Children()
    {
        foreach (var item in Items)
        {
            yield return item;
        }
    }
}
