using System;

namespace ExTween;

public class MultiplexTween : TweenCollection, ITween
{
    public float Update(float dt)
    {
        float totalOverflow = 0;
        var hasInitializedOverflow = false;
        foreach (var tween in Items)
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
        }

        return totalOverflow;
    }

    public bool IsDone()
    {
        var result = true;
        ForEachItem(item => result = result && item.IsDone());
        return result;
    }

    public override void Reset()
    {
        ResetAllItems();
    }

    public void JumpTo(float time)
    {
        Reset();
        ForEachItem(item => { item.JumpTo(time); });
    }

    public ITweenDuration TotalDuration
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
}
