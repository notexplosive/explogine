using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class PrimitiveGuiTheme : IGuiTheme
{
    public PrimitiveGuiTheme(Color primaryColor, Color accentColor, Color backgroundColor, IFontGetter fontGetter)
    {
        PrimaryColor = primaryColor;
        AccentColor = accentColor;
        BackgroundColor = backgroundColor;
        Font = fontGetter;
    }

    public Color PrimaryColor { get; }
    public Color AccentColor { get; }
    public Color BackgroundColor { get; }

    public IFontGetter Font { get; }

    public void DrawButton(Painter painter, Button button)
    {
        var isPressed = button.IsEngaged && button.IsHovered;
        painter.DrawRectangle(button.Rectangle,
            new DrawSettings
            {
                Depth = button.Depth,
                Color = isPressed ? ColorExtensions.Lerp(PrimaryColor, AccentColor, 0.25f) : PrimaryColor
            });

        if (button.IsHovered)
        {
            painter.DrawLineRectangle(button.Rectangle.Inflated(-2, -2),
                new LineDrawSettings {Color = AccentColor, Depth = button.Depth - 1, Thickness = isPressed ? 2f : 1f});
        }

        painter.DrawStringWithinRectangle(Font, button.Text,
            button.Rectangle.Moved(isPressed ? new Vector2(1) : Vector2.Zero), Alignment.Center,
            new DrawSettings {Depth = button.Depth - 2, Color = AccentColor});
    }

    public void DrawPanel(Painter painter, Panel panel)
    {
        painter.DrawRectangle(panel.Rectangle, new DrawSettings {Depth = panel.Depth, Color = BackgroundColor});
        painter.DrawAsRectangle(panel.Canvas.Texture, panel.Rectangle, new DrawSettings {Depth = panel.Depth - 1});
    }
}
