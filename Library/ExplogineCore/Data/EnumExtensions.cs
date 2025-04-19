using System.Diagnostics.Contracts;

namespace ExplogineCore.Data;

public static class EnumExtensions
{
    [Pure]
    public static TEnum Next<TEnum>(TEnum current) where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues<TEnum>().ToList();
        var nextIndex = enumValues.IndexOf(current) + 1;

        if (enumValues.IsValidIndex(nextIndex))
        {
            return enumValues[nextIndex];
        }

        return enumValues.First();
    }

    [Pure]
    public static TEnum Previous<TEnum>(TEnum current) where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues<TEnum>().ToList();
        var previousIndex = enumValues.IndexOf(current) - 1;

        if (enumValues.IsValidIndex(previousIndex))
        {
            return enumValues[previousIndex];
        }

        return enumValues.Last();
    }
}
