using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class Button : IGuiWidget
{
    private readonly Action? _onPress;

    public Button(RectangleF rectangle, string text, Action? onPress, Depth depth)
    {
        Rectangle = rectangle;
        Text = text;
        _onPress = onPress;
        Depth = depth;
    }

    public bool IsHovered { get; private set; }
    public bool IsEngaged { get; private set; }
    public RectangleF Rectangle { get; }
    public string Text { get; }
    public Depth Depth { get; }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        hitTestStack.BeforeResolved += ClearHovered;
        hitTestStack.AddZone(Rectangle, Depth, BecomeHovered);

        var wasMouseReleased = input.Mouse.GetButton(MouseButton.Left).WasReleased;
        var wasMousePressed = input.Mouse.GetButton(MouseButton.Left).WasPressed;

        if (IsHovered)
        {
            if (IsEngaged && wasMouseReleased)
            {
                _onPress?.Invoke();
            }

            if (wasMousePressed)
            {
                IsEngaged = true;
            }
        }

        if (wasMouseReleased)
        {
            IsEngaged = false;
        }
    }

    private void BecomeHovered()
    {
        IsHovered = true;
    }

    private void ClearHovered()
    {
        IsHovered = false;
    }
}
