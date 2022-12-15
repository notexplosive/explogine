namespace ExplogineCore.Data;

public class Axis
{
    public static readonly Axis X = new("X");
    public static readonly Axis Y = new("Y");
    private readonly string _name;

    private Axis()
    {
        // privatize constructor
        throw new Exception("Should not use default constructor for Axis; use static instances");
    }

    private Axis(string name)
    {
        _name = name;
    }

    public static IEnumerable<Axis> Each
    {
        get
        {
            yield return Axis.X;
            yield return Axis.Y;
        }
    }

    public override string ToString()
    {
        return _name;
    }

    public Axis Opposite()
    {
        if (this == Axis.X)
        {
            return Axis.Y;
        }

        if (this == Axis.Y)
        {
            return Axis.X;
        }

        throw new Exception("Unknown Axis");
    }

    public static Axis FromOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Data.Orientation.Horizontal:
                return Axis.X;
            case Data.Orientation.Vertical:
                return Axis.Y;
            default:
                throw new Exception($"Unknown Orientation {orientation}");
        }
    }

    public Orientation Orientation()
    {
        if (this == Axis.X)
        {
            return Data.Orientation.Horizontal;
        }

        if (this == Axis.Y)
        {
            return Data.Orientation.Vertical;
        }

        throw new Exception("Unknown Axis");
    }
}
