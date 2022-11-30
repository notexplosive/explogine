using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public static class NoiseExtensions
{
    public static Vector2 NextPositiveVector2(this NoiseBasedRng noiseBasedRng)
    {
        return new Vector2(noiseBasedRng.NextFloat(), noiseBasedRng.NextFloat());
    }
    
    public static Color NextColor(this NoiseBasedRng noiseBasedRng)
    {
        return new Color(noiseBasedRng.NextByte(), noiseBasedRng.NextByte(), noiseBasedRng.NextByte());
    }
}
