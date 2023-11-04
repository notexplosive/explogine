using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        return (uint) (color.A | (color.R << 24) | (color.G << 16) | (color.B << 8));
    }

    public static string ToRgbaHexString(this Color color)
    {
        return color.ToRgbaHex().ToString("X8");
    }

    public static bool IsFormattedAsRgbaHex(string colorString)
    {
        return ColorExtensions.TryFromRgbaHexString(colorString, out var _);
    }

    public static bool TryFromRgbaHexString(string colorString, [NotNullWhen(true)] out Color? result)
    {
        if (colorString.Length == 6)
        {
            colorString += "FF";
        }

        var valid = colorString.Length == 8;

        var r = byte.MinValue;
        var g = byte.MinValue;
        var b = byte.MinValue;
        var a = byte.MaxValue;

        valid = valid && byte.TryParse(colorString.AsSpan(0, 2), NumberStyles.HexNumber, null, out r);
        valid = valid && byte.TryParse(colorString.AsSpan(2, 2), NumberStyles.HexNumber, null, out g);
        valid = valid && byte.TryParse(colorString.AsSpan(4, 2), NumberStyles.HexNumber, null, out b);
        valid = valid && byte.TryParse(colorString.AsSpan(6, 2), NumberStyles.HexNumber, null, out a);

        result = valid ? new Color(r, g, b, a) : null;

        return valid;
    }

    public static Color FromRgbaHexString(string colorString)
    {
        if (ColorExtensions.TryFromRgbaHexString(colorString, out var color))
        {
            return color.Value;
        }

        throw new Exception(
            $"Color string `{colorString}` is not formatted correctly, expected RRGGBB[AA], eg: d80050 or d80050FF");
    }

    public static Color Added(this Color source, Color added)
    {
        var alpha = source.A + added.A;
        var r = source.R + added.R;
        var g = source.G + added.G;
        var b = source.B + added.B;
        return new Color(r, g, b, alpha);
    }

    public static Color DesaturatedBy(this Color color, float percent)
    {
        var l = 0.3*color.R + 0.6*color.G + 0.1*color.B;
        return new Color((byte)(color.R + percent * (l - color.R)), (byte)(color.G + percent * (l - color.G)), (byte)(color.B + percent * (l - color.B)));
    }
    
    public static Color DimmedBy(this Color color, float amount)
    {
        return color.BrightenedBy(-amount);
    }

    public static Color BrightenedBy(this Color color, float amount)
    {
        var alpha = color.A / 255f;
        var r = color.R / 255f + amount;
        var g = color.G / 255f + amount;
        var b = color.B / 255f + amount;
        return new Color(r, g, b, alpha);
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

    public static Color InvertRgb(Color color)
    {
        return new Color(byte.MaxValue - color.R, byte.MaxValue - color.G, byte.MaxValue - color.B);
    }
}
