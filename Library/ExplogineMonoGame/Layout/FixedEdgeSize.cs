using System.Globalization;

namespace ExplogineMonoGame.Layout;

internal readonly record struct FixedEdgeSize(float Amount) : IEdgeSize
{
    public static implicit operator float(FixedEdgeSize size)
    {
        return size.Amount;
    }

    public string Serialized()
    {
        return Amount.ToString(CultureInfo.InvariantCulture);
    }
}
