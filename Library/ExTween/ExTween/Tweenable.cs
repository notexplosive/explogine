namespace ExTween;

public abstract class Tweenable<T>
{
    public delegate T Getter();

    public delegate void Setter(T value);

    private readonly Getter _getter;
    private readonly Setter _setter;

    protected Tweenable(T initializedValue)
    {
        var capturedValue = initializedValue;
        _getter = () => capturedValue;
        _setter = value => capturedValue = value;
        Value = initializedValue;
    }

    protected Tweenable(Getter getter, Setter setter)
    {
        _getter = getter;
        _setter = setter;
    }

    public T Value
    {
        get => _getter();
        set => _setter(value);
    }

    public static implicit operator T(Tweenable<T> tweenable)
    {
        return tweenable.Value;
    }

    public ITween CallbackSetTo(T destination)
    {
        return new CallbackTween(() => Value = destination);
    }

    public ITween TweenTo(T destination, float duration, Ease.Delegate ease)
    {
        return new Tween<T>(this, destination, duration, ease);
    }

    /// <summary>
    ///     The equivalent of: `startingValue + (targetValue - startingValue) * percent` for the template type.
    /// </summary>
    /// <param name="startingValue">Starting value of the interpolation</param>
    /// <param name="targetValue">Ending value of the interpolation</param>
    /// <param name="percent">Progress along interpolation from 0f to 1f</param>
    /// <returns>The interpolated value</returns>
    public abstract T Lerp(T startingValue, T targetValue, float percent);

    public override string ToString()
    {
        if (Value != null)
        {
            return $"Tweenable: {Value.ToString()}";
        }

        return "Tweenable: (null value)";
    }

    public virtual string ValueAsString()
    {
        if (Value == null)
        {
            return "null";
        }
        return Value.ToString();
    }
}
