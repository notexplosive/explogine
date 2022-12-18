using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class Button : IGuiWidget
{
    private readonly Action? _onPress;
    private readonly ButtonBehavior _behavior;

    public Button(RectangleF rectangle, string text, Action? onPress, Depth depth)
    {
        _behavior = new ButtonBehavior();
        Rectangle = rectangle;
        Text = text;
        _onPress = onPress;
        Depth = depth;
    }
    
    public RectangleF Rectangle { get; }
    public string Text { get; }
    public Depth Depth { get; }

    public bool IsHovered => _behavior.IsHovered;
    public bool IsEngaged => _behavior.IsEngaged;

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        if (_behavior.HasBeenClicked(input, hitTestStack, Rectangle, Depth))
        {
            _onPress?.Invoke();
        }
    }
}
