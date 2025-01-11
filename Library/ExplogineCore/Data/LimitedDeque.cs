namespace ExplogineCore.Data;

public class LimitedDeque<T> where T : class
{
    private readonly LinkedList<T> _content = new();
    private readonly int _sizeLimit = int.MaxValue;
    private int _cachedLength;

    public LimitedDeque()
    {
    }

    public LimitedDeque(int sizeLimit)
    {
        _sizeLimit = sizeLimit;
    }

    public void Push(T item)
    {
        _content.AddFirst(item);
        _cachedLength++;

        if (_cachedLength > _sizeLimit)
        {
            _content.RemoveLast();
            _cachedLength--;
        }
    }

    /// <summary>
    ///     Throws if you don't have content
    /// </summary>
    public T PopUnsafe()
    {
        var item = _content.First;
        if (item != null)
        {
            _content.RemoveFirst();
            _cachedLength--;
            return item.Value;
        }

        throw new Exception("Content was empty!");
    }

    public bool HasContent()
    {
        return _content.First != null;
    }

    public void Clear()
    {
        _content.Clear();
    }
}
