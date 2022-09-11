namespace ExplogineCore.Data;

public class NoiseBasedRng
{
    private readonly Noise _noise;
    private int _position;

    public NoiseBasedRng(int seed) : this(new Noise(seed))
    {
    }

    public NoiseBasedRng(Noise noise)
    {
        _noise = noise;
    }

    public uint NextUInt()
    {
        return _noise.UIntAt(_position++);
    }

    public byte NextByte()
    {
        return (byte) NextUInt();
    }

    public int NextPositiveInt(int max = int.MaxValue)
    {
        return Math.Abs((int) NextUInt()) % max;
    }

    public int NextInt()
    {
        return (int) NextUInt();
    }

    public double NextDouble()
    {
        const int max = int.MaxValue / 2;
        return Math.Abs(NextPositiveInt(max)) / (double) max;
    }

    public float NextFloat()
    {
        return (float) NextDouble();
    }

    public bool NextBool()
    {
        return NextUInt() % 2 == 0;
    }

    public float NextRadian()
    {
        return NextFloat() * MathF.PI * 2;
    }

    public Noise NextNoise()
    {
        return new Noise(NextPositiveInt());
    }
    
    public void Shuffle<T>(IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = NextPositiveInt(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public T GetRandomElement<T>(IList<T> list)
    {
        return list[NextPositiveInt(list.Count)];
    }

    public int NextSign()
    {
        return NextBool() ? 1 : -1;
    }

    public int NextInt(int low, int high)
    {
        return NextPositiveInt(low + high - 1) + low;
    }
}
