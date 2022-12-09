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
        return (uint) (0xFF | color.R << 24 | color.G << 16 | color.B << 8);
    }
}
