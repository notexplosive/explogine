namespace ExplogineCore.Data;

public class Axis
{
    public static readonly Axis X = new();
    public static readonly Axis Y = new();

    private Axis()
    {
    }

    public Axis Opposite
    {
        get
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
    }

    public static IEnumerable<Axis> Each {
        get
        {
            yield return Axis.X;
            yield return Axis.Y;
        }
    }

    public static Axis FromOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Horizontal:
                return Axis.X;
            case Orientation.Vertical:
                return Axis.Y;
            default:
                throw new Exception($"Unknown Orientation {orientation}");
        }
    }
}
