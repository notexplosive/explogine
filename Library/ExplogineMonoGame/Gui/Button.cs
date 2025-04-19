using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Gui;

public class Button : IGuiWidget
{
    private readonly ButtonBehavior _behavior;
    private readonly Action? _onPress;

    public Button(RectangleF rectangle, string label, Action? onPress, Depth depth)
    {
        _behavior = new ButtonBehavior();
        Rectangle = rectangle;
        Label = label;
        _onPress = onPress;
        Depth = depth;
    }

    public RectangleF Rectangle { get; }
    public string Label { get; }
    public Depth Depth { get; }

    public bool IsHovered => _behavior.IsHovered;
    public bool IsEngaged => _behavior.IsEngaged;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (_behavior.UpdateInputAndReturnClicked(input, hitTestStack, Rectangle, Depth))
        {
            _onPress?.Invoke();
        }
    }
}
