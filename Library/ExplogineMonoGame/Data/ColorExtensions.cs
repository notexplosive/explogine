using ExTween;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public static class ColorExtensions
{
    public static Color WithMultipliedOpacity(this Color color, float opacity)
    {
        var resultColor = new Color(color, opacity);

        resultColor.R = (byte) (resultColor.R * opacity);
        resultColor.G = (byte) (resultColor.G * opacity);
        resultColor.B = (byte) (resultColor.B * opacity);

        return resultColor;
    }

    public static Color FromRgbHex(uint hex)
    {
        return new Color((byte) ((hex & 0xFF0000) >> 16), (byte) ((hex & 0x00FF00) >> 8), (byte) (hex & 0x0000FF));
    }

    public static uint ToRgbaHex(this Color color)
    {
        return (uint) (0xFF | (color.R << 24) | (color.G << 16) | (color.B << 8));
    }

    public static string ToRgbaHexString(this Color color)
    {
        return color.ToRgbaHex().ToString("X");
    }

    public static Color Lerp(Color colorA, Color colorB, float percent)
    {
        var maxByteAsFloat = (float) byte.MaxValue;

        var a = new[]
        {
            colorA.R / maxByteAsFloat,
            colorA.G / maxByteAsFloat,
            colorA.B / maxByteAsFloat,
            colorA.A / maxByteAsFloat
        };

        var b = new[]
        {
            colorB.R / maxByteAsFloat,
            colorB.G / maxByteAsFloat,
            colorB.B / maxByteAsFloat,
            colorB.A / maxByteAsFloat
        };

        var result = new float[a.Length];

        for (var i = 0; i < a.Length; i++)
        {
            result[i] = FloatExtensions.Lerp(a[i], b[i], percent);
        }

        return new Color(
            result[0],
            result[1],
            result[2],
            result[3]
        );
    }
}
