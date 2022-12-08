using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Logging;

public record LogMessage(LogMessageType Type, string Text)
{
    public string ToFileString()
    {
        return $"{Type.ToString().ToUpper()}: {Text}";
    }

    public static Color GetColorFromType(LogMessageType contentType)
    {
        switch (contentType)
        {
            case LogMessageType.Warn:
                return Color.Yellow;
            case LogMessageType.Fail:
                return Color.Orange;
            default:
                return Color.White;
        }
    }
}
