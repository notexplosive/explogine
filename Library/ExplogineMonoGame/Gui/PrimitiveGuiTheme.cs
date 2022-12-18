using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class PrimitiveGuiTheme : IGuiTheme
{
    public Color PrimaryColor { get; }
    public Color AccentColor { get; }

    public PrimitiveGuiTheme(Color primaryColor, Color accentColor)
    {
        PrimaryColor = primaryColor;
        AccentColor = accentColor;
    }
    
    public void DrawButton(Painter painter, Button button)
    {
        var isPressed = button.IsEngaged && button.IsHovered;
        painter.DrawRectangle(button.Rectangle,
            new DrawSettings
                {Depth = button.Depth, Color = isPressed ? ColorExtensions.Lerp(PrimaryColor, AccentColor, 0.25f) : PrimaryColor});

        if (button.IsHovered)
        {
            painter.DrawLineRectangle(button.Rectangle.Inflated(-2, -2),
                new LineDrawSettings {Color = AccentColor, Depth = button.Depth - 1});
        }

        painter.DrawStringWithinRectangle(Client.Assets.GetFont("engine/console-font", 32), button.Text,
            button.Rectangle.Moved(isPressed ? new Vector2(3) : Vector2.Zero), Alignment.Center,
            new DrawSettings {Depth = button.Depth - 2, Color = AccentColor});
    }
}
