using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui.Window;
using ExplogineMonoGame.Layout;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class SimpleGuiTheme : IGuiTheme
{
    public static LayoutElementGroup CheckboxLayoutTemplate = L.Root(
        new Style
            {PaddingBetweenElements = 5, Margin = new Vector2(0, 5), Alignment = Alignment.CenterLeft},
        L.FixedElement(8, 8),
        L.FixedElement("checkbox", 32, 32),
        L.FixedElement(8, 8),
        L.FillBoth("label"));

    public SimpleGuiTheme(Color primaryColor, Color secondaryColor, Color backgroundColor, IFontGetter fontGetter,
        Color? textInputForegroundColor = null, Color? textInputBackgroundColor = null, Color? selectionColor = null)
    {
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        BackgroundColor = backgroundColor;
        Font = fontGetter;
        TextInputBackgroundColor = textInputBackgroundColor ?? BackgroundColor;
        TextInputForegroundColor = textInputForegroundColor ?? PrimaryColor;
        SelectionColor = selectionColor ?? SecondaryColor;
    }

    public Color TextInputBackgroundColor { get; }
    public Color TextInputForegroundColor { get; }
    public Color SelectionColor { get; }
    public Color PrimaryColor { get; }
    public Color SecondaryColor { get; }
    public Color BackgroundColor { get; }

    public IFontGetter Font { get; }

    public void DrawTextInput(Painter painter, TextInputWidget textInputWidget)
    {
        painter.BeginSpriteBatch(textInputWidget.ScrollableArea.CanvasToScreen);
        var depth = Depth.Middle;

        painter.Clear(TextInputBackgroundColor);

        if (textInputWidget.Selected)
        {
            painter.DrawRectangle(textInputWidget.CursorRectangle,
                new DrawSettings {Depth = depth - 1, Color = TextInputForegroundColor});

            var random = new NoiseBasedRng(123);
            var selectionColor = SelectionColor;

            foreach (var selectionRect in textInputWidget.GetSelectionRectangles())
            {
                if (Client.Debug.IsActive)
                {
                    selectionColor = random.NextColor();
                }

                painter.DrawRectangle(selectionRect,
                    new DrawSettings {Depth = depth + 1, Color = selectionColor});
            }
        }

        painter.DrawStringWithinRectangle(textInputWidget.Font, textInputWidget.Text,
            textInputWidget.ContainerRectangle, textInputWidget.Alignment,
            new DrawSettings {Color = TextInputForegroundColor, Depth = depth});

        if (Client.Debug.IsActive)
        {
            textInputWidget.DrawDebugInfo(painter);
        }

        painter.EndSpriteBatch();
    }

    public void DrawWindowChrome(Painter painter, InternalWindowChrome chrome, bool isInFocus)
    {
        if (isInFocus)
        {
            painter.DrawRectangle(chrome.WholeWindowRectangle.Inflated(2, 2),
                new DrawSettings {Depth = chrome.Depth, Color = PrimaryColor});
        }
        else
        {
            painter.DrawRectangle(chrome.WholeWindowRectangle.Inflated(2, 2),
                new DrawSettings {Depth = chrome.Depth, Color = SecondaryColor});
        }

        var titleLayout = chrome.TitleLayout;

        foreach (var button in titleLayout.Buttons)
        {
            if (button.ButtonType != InternalWindowTitleBar.ControlButtonType.Empty)
            {
                var color = button.ButtonType switch
                {
                    InternalWindowTitleBar.ControlButtonType.Close => Color.Red,
                    InternalWindowTitleBar.ControlButtonType.Fullscreen => Color.LightCoral,
                    InternalWindowTitleBar.ControlButtonType.Minimize => Color.Orange,
                    _ => Color.White
                };

                painter.DrawRectangle(button.Rectangle, new DrawSettings {Color = color, Depth = chrome.Depth - 1});
            }
        }

        if (chrome.Icon != null)
        {
            painter.DrawAsRectangle(chrome.Icon.Texture, titleLayout.Icon,
                new DrawSettings
                    {Color = Color.White, Depth = chrome.Depth - 1, SourceRectangle = chrome.Icon.SourceRectangle});
        }

        var truncatedTitle = Font.Truncate(chrome.Title, titleLayout.TitleArea.Size);
        painter.DrawStringWithinRectangle(Font, truncatedTitle, titleLayout.TitleArea, Alignment.CenterLeft,
            new DrawSettings {Depth = chrome.Depth - 1});
    }

    public void DrawLabel(Painter painter, Label label)
    {
        var font = Font;
        if (label.FontSize.HasValue)
        {
            font = font.GetFont().WithHeight(label.FontSize.Value);
        }
        painter.DrawStringWithinRectangle(font, label.Text, label.Rectangle, label.Alignment,
            new DrawSettings {Depth = label.Depth});
    }

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

        painter.DrawStringWithinRectangle(Font, button.Label,
            button.Rectangle.Moved(isPressed ? new Vector2(1) : Vector2.Zero), Alignment.Center,
            new DrawSettings {Depth = button.Depth - 2, Color = SecondaryColor});
    }

    public void DrawCheckbox(Painter painter, Checkbox checkbox)
    {
        var layout = L.Compute(checkbox.Rectangle, SimpleGuiTheme.CheckboxLayoutTemplate);
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
                Color = TextInputForegroundColor,
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
            var insetRect = checkboxRect.Inflated(-8, -8);
            painter.DrawRectangle(
                insetRect,
                new DrawSettings {Depth = checkbox.Depth - 2, Color = SecondaryColor});
        }
    }

    public void DrawSlider(Painter painter, Slider slider)
    {
        painter.DrawRectangle(
            slider.BodyRectangle,
            new DrawSettings {Depth = slider.Depth, Color = PrimaryColor});
        painter.DrawRectangle(
            slider.ThumbEngaged ? slider.ThumbRectangle.Inflated(2, 2) : slider.ThumbRectangle,
            new DrawSettings {Depth = slider.Depth - 2, Color = SecondaryColor}
        );

        if (slider.BodyHovered || slider.ThumbHovered || slider.IsDragging)
        {
            painter.DrawLineRectangle(slider.BodyRectangle.Inflated(-2, -2),
                new LineDrawSettings
                    {Color = SecondaryColor, Depth = slider.Depth - 1, Thickness = slider.IsDragging ? 2f : 1f});
        }
    }

    public void DrawRadialCheckbox(Painter painter, RadialCheckbox radialCheckbox)
    {
        var isPressed = radialCheckbox.IsEngaged && radialCheckbox.IsHovered;
        painter.DrawRectangle(radialCheckbox.Rectangle,
            new DrawSettings
            {
                Depth = radialCheckbox.Depth,
                Color = isPressed || !radialCheckbox.IsToggled
                    ? ColorExtensions.Lerp(PrimaryColor, SecondaryColor, 0.25f)
                    : PrimaryColor
            });

        if (radialCheckbox.IsHovered)
        {
            painter.DrawLineRectangle(radialCheckbox.Rectangle.Inflated(-2, -2),
                new LineDrawSettings
                    {Color = SecondaryColor, Depth = radialCheckbox.Depth - 1, Thickness = isPressed ? 2f : 1f});
        }

        painter.DrawStringWithinRectangle(Font, radialCheckbox.Label,
            radialCheckbox.Rectangle.Moved(isPressed ? new Vector2(1) : Vector2.Zero), Alignment.Center,
            new DrawSettings {Depth = radialCheckbox.Depth - 2, Color = SecondaryColor});
    }

    public void DrawScrollbar(Painter painter, Scrollbar scrollBar)
    {
        var thumbColor = PrimaryColor;
        if (Math.Abs(scrollBar.ThumbScalar - 1f) < float.Epsilon)
        {
            thumbColor = SecondaryColor;
        }

        painter.DrawRectangle(scrollBar.BodyRectangle,
            new DrawSettings {Color = thumbColor, Depth = scrollBar.Depth});

        var thumbRect = scrollBar.ThumbRectangle;

        thumbRect.Inflate(-2, -2);

        painter.DrawRectangle(thumbRect,
            new DrawSettings {Color = SecondaryColor, Depth = scrollBar.Depth});

        painter.DrawRectangle(new RectangleF(scrollBar.BodyRectangle.BottomLeft, new Vector2(scrollBar.Thickness)),
            new DrawSettings {Color = BackgroundColor, Depth = scrollBar.Depth});
    }
}
