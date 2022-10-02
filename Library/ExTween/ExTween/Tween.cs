using System;

namespace ExTween;

public class Tween<T> : ITween
{
    private readonly Ease.Delegate _ease;
    private readonly T _targetValue;
    private readonly Tweenable<T> _tweenable;
    private T _startingValue;

    public Tween(Tweenable<T> tweenable, T targetValue, float duration, Ease.Delegate ease)
    {
        _tweenable = tweenable;
        _targetValue = targetValue;
        _ease = ease;
        _startingValue = tweenable.Value;
        TotalDuration = new KnownTweenDuration(duration);
        CurrentTime = 0;
    }

    public float CurrentTime { get; private set; }

    public ITweenDuration TotalDuration { get; }

    public float Update(float dt)
    {
        if (CurrentTime == 0)
        {
            // Re-set the starting value, it might have changed since constructor
            // (or we might be running the tween a second time)
            _startingValue = _tweenable.Value;
        }

        CurrentTime += dt;

        var overflow = CurrentTime - TotalDuration.Get();

        if (overflow > 0)
        {
            CurrentTime -= overflow;
        }

        ApplyTimeToValue();

        return Math.Max(overflow, 0);
    }

    public bool IsDone()
    {
        return CurrentTime >= TotalDuration.Get();
    }

    public void Reset()
    {
        CurrentTime = 0;
    }

    public void JumpTo(float time)
    {
        CurrentTime = Math.Clamp(time, 0, TotalDuration.Get());
        ApplyTimeToValue();
    }

    private void ApplyTimeToValue()
    {
        var percent = CurrentTime / TotalDuration.Get();

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
            result += $"{(int) (CurrentTime / TotalDuration.Get() * 100)}%";
        }
        else
        {
            result += "Unknown";
        }

        result += $" Value: {_tweenable.Value}";

        return result;
    }
}

public interface ITweenDuration
{
    public float Get();
}

public readonly struct KnownTweenDuration : ITweenDuration
{
    public KnownTweenDuration(float duration)
    {
        Value = duration;
    }

    private float Value { get; }

    public float Get()
    {
        return Value;
    }

    public static implicit operator float(KnownTweenDuration me)
    {
        return me.Get();
    }

    public override string ToString()
    {
        return Value.ToString("N4");
    }
}

public readonly struct UnknownTweenDuration : ITweenDuration
{
    public float Get()
    {
        throw new Exception("Value unknown");
    }
}
