using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class RadialCheckbox : IGuiWidget
{
    private readonly ButtonBehavior _behavior;
    private readonly Radial _radial;
    private readonly int _targetState;

    public RadialCheckbox(Radial radial, int targetState, RectangleF rectangle, string label, Depth depth)
    {
        _radial = radial;
        _targetState = targetState;
        Rectangle = rectangle;
        Label = label;
        Depth = depth;
        _behavior = new ButtonBehavior();
    }

    public RectangleF Rectangle { get; }
    public string Label { get; }
    public Depth Depth { get; }
    public bool IsHovered => _behavior.IsHovered;
    public bool IsEngaged => _behavior.IsEngaged;
    public bool IsToggled => _radial.State.Value == _targetState;

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        if (_behavior.UpdateInputAndReturnClicked(input, hitTestStack, Rectangle, Depth))
        {
            _radial.State.Value = _targetState;
        }
    }
}
