namespace ExplogineCore.Data;

public class Wrapped<T> where T : struct
{
    private T _value;

    public Wrapped() : this(default)
    {
    }

    public Wrapped(T value)
    {
        _value = value;
    }

    public T Value
    {
        get => _value;
        set => _value = value;
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
}
