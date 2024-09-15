namespace ExplogineCore.Data;

public static class ListExtensions
{
    [Obsolete("Rename pending: use IsValidIndex instead")]
    public static bool IsWithinRange<T>(this IList<T> collection, int i)
    {
        return collection.IsValidIndex(i);

    }
    
    public static bool IsValidIndex<T>(this IList<T> collection, int i)
    {
        return i < collection.Count && i >= 0;
    }
}
