namespace ExplogineCore.Data;

public record struct Xy<T>(T X, T Y)
{
    public T GetAxis(Axis axis)
    {
        if (axis == Axis.X)
        {
            return X;
        }

        if (axis == Axis.Y)
        {
            return Y;
        }

        throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
    }

    public void SetAxis(Axis axis, T value)
    {
        if (axis == Axis.X)
        {
            X = value;
        }
        else if (axis == Axis.Y)
        {
            Y = value;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }
    }
}
