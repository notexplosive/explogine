using System;
using System.Collections.Generic;

namespace ExplogineMonoGame.Data;

public static class ListExtensions
{
    public static bool IsWithinRange<T>(this IList<T> collection, Index i)
    {
        return i.Value < collection.Count && i.Value >= 0;
    }
}
