using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class RadialCheckbox : IGuiWidget
{
    private readonly ButtonBehavior _behavior;
    private readonly Wrapped<int> _state;
    private readonly int _targetState;

    public RadialCheckbox(Wrapped<int> state, int targetState, RectangleF rectangle, string label, Depth depth)
    {
        _state = state;
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
    public bool IsToggled => _state.Value == _targetState;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (_behavior.UpdateInputAndReturnClicked(input, hitTestStack, Rectangle, Depth))
        {
            _state.Value = _targetState;
        }
    }
}
