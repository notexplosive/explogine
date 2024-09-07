using System;
using ExplogineCore.Data;

namespace ExplogineMonoGame;

public class ClientRandom
{
    private int _seed;

    internal ClientRandom()
    {
        Seed = 0;
        Dirty = new NoiseBasedRng((int) DateTime.Now.ToFileTimeUtc());
    }

    public int Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            CleanNoise = new Noise(_seed);
            Clean = new NoiseBasedRng(CleanNoise);
        }
    }

    public NoiseBasedRng Dirty { get; }
    public NoiseBasedRng Clean { get; private set; } = new(0);
    public Noise CleanNoise { get; private set; }
}
