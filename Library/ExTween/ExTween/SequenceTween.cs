﻿namespace ExTween;

public class SequenceTween : TweenCollection, ITween
{
    private int _currentItemIndex;

    public SequenceTween()
    {
        _currentItemIndex = 0;
    }

    public bool IsLooping { get; set; }

    public float Update(float dt)
    {
        if (Items.Count == 0)
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

        var overflow = Items[_currentItemIndex].Update(dt);

        if (Items[_currentItemIndex].IsDone())
        {
            _currentItemIndex++;
            return Update(overflow);
        }

        return overflow;
    }

    public bool IsDone()
    {
        return IsAtEnd() && !IsLooping;
    }

    public override void Reset()
    {
        ResetAllItems();
        _currentItemIndex = 0;
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

    public void JumpTo(float targetTime)
    {
        Reset();

        var adjustedTargetTime = targetTime;

        for (var i = 0; i < Items.Count; i++)
        {
            var itemDuration = Items[i].TotalDuration;
            if (itemDuration is UnknownTweenDuration)
            {
                // We don't know how long this tween is, so we have to update it manually
                var overflow = Items[i].Update(adjustedTargetTime);
                adjustedTargetTime -= overflow;

                if (!Items[i].IsDone())
                {
                    break;
                }
            }
            else
            {
                if (itemDuration is KnownTweenDuration exactTweenDuration && adjustedTargetTime >= exactTweenDuration)
                {
                    adjustedTargetTime -= exactTweenDuration;
                    Items[i].Update(exactTweenDuration);
                }
                else
                {
                    Items[i].Update(adjustedTargetTime);
                    _currentItemIndex = i;
                    break;
                }
            }
        }
    }

    private bool IsAtEnd()
    {
        return _currentItemIndex >= Items.Count || Items.Count == 0;
    }

    public SequenceTween Add(ITween tween)
    {
        AddItem(tween);
        return this;
    }
}
