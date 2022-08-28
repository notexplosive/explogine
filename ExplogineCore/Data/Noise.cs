namespace ExplogineCore.Data;

public readonly struct Noise
{
    private readonly uint _seed;

    public Noise(int seed)
    {
        _seed = (uint) seed;
    }

    public uint UIntAt(int position)
    {
        return Squirrel3.Noise(position, _seed);
    }

    public byte ByteAt(int position)
    {
        return (byte) UIntAt(position);
    }

    public int PositiveIntAt(int position, int max = int.MaxValue)
    {
        return Math.Abs((int) UIntAt(position)) % max;
    }

    public int IntAt(int position, int max = int.MaxValue)
    {
        return (int) UIntAt(position) % max;
    }

    public double DoubleAt(int position)
    {
        const int max = int.MaxValue / 2;
        return Math.Abs(PositiveIntAt(position, max)) / (double) max;
    }

    public float FloatAt(int position)
    {
        return (float) DoubleAt(position);
    }

    public bool BoolAt(int position)
    {
        return UIntAt(position) % 2 == 0;
    }

    public float RadianAt(int position)
    {
        return FloatAt(position) * MathF.PI * 2;
    }

    public Noise NoiseAt(int position)
    {
        return new Noise(PositiveIntAt(position));
    }
}
