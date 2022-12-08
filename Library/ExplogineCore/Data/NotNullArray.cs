using System.Collections;

namespace ExplogineCore.Data;

public readonly struct NotNullArray<T> : IEnumerable<T>
{
    private readonly T[]? _content;

    public NotNullArray()
    {
        _content = Array.Empty<T>();
    }

    public NotNullArray(T[] array)
    {
        _content = array;
    }

    public T this[Index i] => _content![i];

    public void Set(int i, T val)
    {
        _content![i] = val;
    }

    public int Length
    {
        get
        {
            if (_content == null)
            {
                return 0;
            }

            return _content.Length;
        }
    }

    public static implicit operator NotNullArray<T>(T[] array)
    {
        return new NotNullArray<T>(array);
    }

    public static implicit operator T[](NotNullArray<T> notNullArray)
    {
        if (notNullArray._content == null)
        {
            return Array.Empty<T>();
        }

        return notNullArray._content;
    }

    public bool Contains(T key)
    {
        if (_content == null)
        {
            return false;
        }

        return _content.Contains(key);
    }

    public bool Any(Func<T, bool> func)
    {
        if (_content == null)
        {
            return false;
        }

        return _content.Any(func);
    }

    public bool Equals(NotNullArray<T> other)
    {
        if (other.Length != Length)
        {
            return false;
        }

        for (var i = 0; i < Length; i++)
        {
            if (!object.Equals(other[i], this[i]))
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_content != null)
        {
            foreach (var item in _content)
            {
                yield return item;
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is NotNullArray<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _content != null ? _content.GetHashCode() : 0;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static bool operator ==(NotNullArray<T> left, NotNullArray<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NotNullArray<T> left, NotNullArray<T> right)
    {
        return !left.Equals(right);
    }
}
