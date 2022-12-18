using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class PrimitiveGuiTheme : IGuiTheme
{
    public static LayoutElementGroup CheckboxLayoutTemplate = new(
        new ArrangementSettings
            {PaddingBetweenElements = 5, Margin = new Vector2(0, 5), Alignment = Alignment.CenterLeft}, new[]
        {
            L.FixedElement("checkbox", 32, 32),
            L.FillBoth("label")
        });

    public PrimitiveGuiTheme(Color primaryColor, Color secondaryColor, Color backgroundColor, IFontGetter fontGetter)
    {
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        BackgroundColor = backgroundColor;
        Font = fontGetter;
    }

    public Color PrimaryColor { get; }
    public Color SecondaryColor { get; }
    public Color BackgroundColor { get; }

    public IFontGetter Font { get; }

    public void DrawButton(Painter painter, Button button)
    {
        var isPressed = button.IsEngaged && button.IsHovered;
        painter.DrawRectangle(button.Rectangle,
            new DrawSettings
            {
                Depth = button.Depth,
                Color = isPressed ? ColorExtensions.Lerp(PrimaryColor, SecondaryColor, 0.25f) : PrimaryColor
            });

        if (button.IsHovered)
        {
            painter.DrawLineRectangle(button.Rectangle.Inflated(-2, -2),
                new LineDrawSettings
                    {Color = SecondaryColor, Depth = button.Depth - 1, Thickness = isPressed ? 2f : 1f});
        }

        painter.DrawStringWithinRectangle(Font, button.Text,
            button.Rectangle.Moved(isPressed ? new Vector2(1) : Vector2.Zero), Alignment.Center,
            new DrawSettings {Depth = button.Depth - 2, Color = SecondaryColor});
    }

    public void DrawPanel(Painter painter, Panel panel)
    {
        painter.DrawRectangle(panel.Rectangle, new DrawSettings {Depth = panel.Depth, Color = BackgroundColor});
        painter.DrawAsRectangle(panel.Canvas.Texture, panel.Rectangle, new DrawSettings {Depth = panel.Depth - 1});
    }

    public void DrawCheckbox(Painter painter, Checkbox checkbox)
    {
        var layout = L.Create(checkbox.Rectangle, PrimitiveGuiTheme.CheckboxLayoutTemplate);
        var checkboxRect = layout.FindElement("checkbox").Rectangle;
        var labelRect = layout.FindElement("label").Rectangle;
        var isPressed = checkbox.IsEngaged && checkbox.IsHovered;

        painter.DrawRectangle(checkboxRect,
            new DrawSettings
            {
                Color = isPressed ? ColorExtensions.Lerp(PrimaryColor, SecondaryColor, 0.25f) : PrimaryColor,
                Depth = checkbox.Depth
            });

        painter.DrawStringWithinRectangle(Font, checkbox.Label, labelRect, Alignment.CenterLeft,
            new DrawSettings
            {
                Color = PrimaryColor,
                Depth = checkbox.Depth
            });

        if (checkbox.IsHovered)
        {
            painter.DrawLineRectangle(checkboxRect.Inflated(-2, -2),
                new LineDrawSettings
                    {Color = SecondaryColor, Depth = checkbox.Depth - 1, Thickness = isPressed ? 2f : 1f});
        }

        if (checkbox.State)
        {
            var corners = checkboxRect.Inflated(-8, -8);
            painter.DrawLine(corners.TopLeft, corners.BottomRight,
                new LineDrawSettings {Depth = checkbox.Depth - 2, Color = SecondaryColor, Thickness = 8});
            
            painter.DrawLine(corners.TopRight, corners.BottomLeft,
                new LineDrawSettings {Depth = checkbox.Depth - 2, Color = SecondaryColor, Thickness = 8});
        }
    }
}
