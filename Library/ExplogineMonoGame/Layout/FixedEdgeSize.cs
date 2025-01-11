using System.Globalization;

namespace ExplogineMonoGame.Layout;

internal readonly record struct FixedEdgeSize(float Amount) : IEdgeSize
{
    public string Serialized()
    {
        return Amount.ToString(CultureInfo.InvariantCulture);
    }

    public static implicit operator float(FixedEdgeSize size)
    {
        return size.Amount;
    }
}
