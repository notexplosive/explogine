using System;

namespace ExTween;

public class Tween<T> : ITween, IValueTween
{
    private readonly Ease.Delegate _ease;
    private readonly T _targetValue;
    private readonly Tweenable<T> _tweenable;
    private T _startingValue;
    private readonly float _duration;

    public Tween(Tweenable<T> tweenable, T targetValue, float duration, Ease.Delegate ease)
    {
        _tweenable = tweenable;
        _targetValue = targetValue;
        _ease = ease;
        _startingValue = tweenable.Value;
        _duration = duration;
        CurrentTime = 0;
    }

    public float CurrentTime { get; private set; }

    public ITweenDuration TotalDuration => new KnownTweenDuration(_duration, CurrentTime);

    public float Update(float dt)
    {
        if (CurrentTime == 0)
        {
            // Re-set the starting value, it might have changed since constructor
            // (or we might be running the tween a second time)
            _startingValue = _tweenable.Value;
        }

        CurrentTime += dt;

        var overflow = CurrentTime - TotalDuration.GetDuration();

        if (overflow > 0)
        {
            CurrentTime = TotalDuration.GetDuration();
        }

        ApplyTimeToValue();

        return Math.Max(overflow, 0);
    }

    public bool IsDone()
    {
        return CurrentTime >= TotalDuration.GetDuration();
    }

    public void Reset()
    {
        CurrentTime = 0;
    }

    public void JumpTo(float time)
    {
        CurrentTime = Math.Clamp(time, 0, TotalDuration.GetDuration());
        ApplyTimeToValue();
    }

    public void SkipToEnd()
    {
        CurrentTime = TotalDuration.GetDuration();
        ApplyTimeToValue();
    }

    private void ApplyTimeToValue()
    {
        var percent = CurrentTime / TotalDuration.GetDuration();

        _tweenable.Value = _tweenable.Lerp(
            _startingValue,
            _targetValue,
            _ease(percent));
    }

    public override string ToString()
    {
        var result = $"({_startingValue}) -> ({_targetValue}), Progress: ";
        if (TotalDuration is KnownTweenDuration)
        {
            result += $"{(int) (CurrentTime / TotalDuration.GetDuration() * 100)}%";
        }
        else
        {
            result += "Unknown";
        }

        result += $" Value: {_tweenable.Value}";

        return result;
    }

    public string TweenableValueAsString()
    {
        return _tweenable.ValueAsString();
    }

    public int TweenableHashCode()
    {
        return _tweenable.GetHashCode();
    }
}