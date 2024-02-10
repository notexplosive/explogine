namespace ExplogineCore.Data;

public class Wrapped<T> where T : struct
{
    private T _value;
    private bool _valueChangeInProgress;

    public Wrapped() : this(default)
    {
    }

    public event Action<T>? ValueChanged;

    public Wrapped(T value)
    {
        _value = value;
    }

    public T Value
    {
        get => _value;
        set
        {
            if (_valueChangeInProgress)
            {
                _value = value;
                return;
            }
            
            _valueChangeInProgress = true;
            ValueChanged?.Invoke(value);
            _value = value;
            _valueChangeInProgress = false;
        }
    }

    public static implicit operator T(Wrapped<T> wrapped)
    {
        return wrapped.Value;
    }

    public override string ToString()
    {
        var str = _value.ToString();
        return str ?? "null";
    }

    public void SetWithoutEvent(T newValue)
    {
        // Set without calling ValueChanged
        _value = newValue;
    }
}
