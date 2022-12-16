namespace ExplogineMonoGame.Layout;

internal readonly record struct FixedEdgeSize(float Amount) : IEdgeSize
{
    public static implicit operator float(FixedEdgeSize size)
    {
        return size.Amount;
    }
}
