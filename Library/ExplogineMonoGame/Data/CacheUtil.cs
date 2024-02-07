using System.Collections.Generic;

namespace ExplogineMonoGame.Data;

public static class CacheUtil
{
    private static readonly Dictionary<char, string> CharToStringTable = new();

    public static string CharToString(char c)
    {
        // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
        if (!CacheUtil.CharToStringTable.ContainsKey(c))
        {
            CacheUtil.CharToStringTable.Add(c, c.ToString());
        }
        
        return CacheUtil.CharToStringTable[c];
    }
}
