using MoonSharp.Interpreter;

namespace ExplogineCore.Lua;

public static class LuaUtilities
{
    /// <summary>
    ///     Enumerates a LuaTable as if it were an array, walking along each positive index until we hit a null
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DynValue> EnumerateArrayLike(Table table)
    {
        var i = 1;
        while (table.Get(i).Type != DataType.Nil || table.Get(i + 1).Type != DataType.Nil)
        {
            yield return table.Get(i);
            i++;
        }
    }

    public static T GetTableUserdataValueOrFallback<T>(Table table, string key, T fallback)
    {
        var result = table.Get(key);

        if (result != null && result.Type == DataType.UserData)
        {
            return result.ToObject<T>();
        }

        return fallback;
    }

    public static string? GetTableStringValueOrNull(Table table, string key)
    {
        var result = table.Get(key);

        if (result != null)
        {
            return result.CastToString();
        }

        return null;
    }

    public static string GetTableStringValueOrFallback(Table table, string key, string fallback)
    {
        var result = table.Get(key);

        if (result != null)
        {
            return result.CastToString();
        }

        return fallback;
    }
}
