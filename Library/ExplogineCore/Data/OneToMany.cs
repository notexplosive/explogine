using System.Collections;

namespace ExplogineCore.Data;

public class OneToMany<TKey, TValue> :IEnumerable<KeyValuePair<TKey,TValue>>  where TKey : notnull
{
    private readonly Dictionary<TKey, List<TValue>> _dictionary = new();

    public void Add(TKey key, TValue value)
    {
        InitializeList(key);
        _dictionary[key].Add(value);
    }

    public List<TValue> Get(TKey key)
    {
        return _dictionary[key];
    }

    private void InitializeList(TKey key)
    {
        if (!_dictionary.ContainsKey(key))
        {
            _dictionary.Add(key, new List<TValue>());
        }
    }
    
    public IEnumerable<TValue> Values
    {
        get
        {
            foreach (var item in this)
            {
                yield return item.Value;
            }
        }
    }
    
    public IEnumerable<TKey> Keys
    {
        get
        {
            foreach (var item in this)
            {
                yield return item.Key;
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator()
    {
        foreach (var key in _dictionary.Keys)
        {
            foreach (var value in _dictionary[key])
            {
                yield return new KeyValuePair<TKey, TValue>(key, value);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
