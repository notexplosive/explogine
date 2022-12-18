using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Layout;

namespace ExplogineMonoGame.Gui;

public class Checkbox : IGuiWidget
{
    private readonly ButtonBehavior _behavior;
    public Depth Depth { get; }
    public string Label { get; }
    public RectangleF Rectangle { get; }
    public Wrapped<bool> State { get; }
    public bool IsHovered => _behavior.IsHovered;
    public bool IsEngaged => _behavior.IsEngaged;

    public Checkbox(RectangleF totalRectangle, string label, Depth depth, Wrapped<bool> state)
    {
        _behavior = new ButtonBehavior();
        Label = label;
        Depth = depth;
        State = state;
        Rectangle = totalRectangle;
    }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        if (_behavior.HasBeenClicked(input, hitTestStack, Rectangle, Depth))
        {
            State.Value = !State.Value;
        }
    }
}
