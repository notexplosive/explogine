using System;

namespace ExTween;

public class MultiplexTween : TweenCollection
{
    public override float Update(float dt)
    {
        float totalOverflow = 0;
        var hasInitializedOverflow = false;
        // This is a standard for loop instead of a foreach so a multiplex can add to itself
        for (var i = 0; i < Items.Count; i++)
        {
            var tween = Items[i];
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
        }

        return totalOverflow;
    }

    public override bool IsDone()
    {
        var result = true;
        ForEachItem(item => result = result && item.IsDone());
        return result;
    }

    public override void Reset()
    {
        ResetAllItems();
    }

    public override void JumpTo(float time)
    {
        Reset();
        ForEachItem(item => { item.JumpTo(time); });
    }

    public override ITweenDuration TotalDuration
    {
        get
        {
            var result = 0f;
            var currentTimeResult = 0f;
            foreach (var item in Items)
            {
                if (item.TotalDuration is KnownTweenDuration itemDuration)
                {
                    result = Math.Max(result, itemDuration);
                    currentTimeResult = MathF.Max(currentTimeResult, itemDuration.CurrentTime);
                }
            }

            return new KnownTweenDuration(result, currentTimeResult);
        }
    }

    public MultiplexTween AddChannel(ITween tween)
    {
        Items.Add(tween);
        return this;
    }
    
    public override void SkipToEnd()
    {
        ForEachItem(item => { item.SkipToEnd(); });
    }
}
