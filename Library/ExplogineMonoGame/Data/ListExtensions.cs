using System;
using System.Collections.Generic;

namespace ExplogineMonoGame.Data;

public static class ListExtensions
{
    public static bool IsWithinRange<T>(this IList<T> collection, int i)
    {
        return i < collection.Count && i >= 0;
    }
}
