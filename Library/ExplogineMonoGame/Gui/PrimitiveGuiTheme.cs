using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class PrimitiveGuiTheme : IGuiTheme
{
    public void DrawButton(Painter painter, Button button)
    {
        var isPressed = button.IsEngaged && button.IsHovered;
        painter.DrawRectangle(button.Rectangle,
            new DrawSettings
                {Depth = button.Depth, Color = isPressed ? Color.LightGray : Color.White});

        if (button.IsHovered)
        {
            painter.DrawLineRectangle(button.Rectangle.Inflated(-2, -2),
                new LineDrawSettings {Color = Color.Blue, Depth = button.Depth - 1});
        }

        painter.DrawStringWithinRectangle(Client.Assets.GetFont("engine/console-font", 32), button.Text,
            button.Rectangle.Moved(isPressed ? new Vector2(3) : Vector2.Zero), Alignment.Center,
            new DrawSettings {Depth = button.Depth - 2, Color = Color.Black});
    }
}
