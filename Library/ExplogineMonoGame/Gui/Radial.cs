using ExplogineCore.Data;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Gui;

public class Radial
{
    private readonly Gui _gui;
    private int _targetState;

    public Radial(Gui gui, Wrapped<int> state)
    {
        _gui = gui;
        State = state;
    }

    public Wrapped<int> State { get; }

    public void Add(RectangleF rectangle, string label, Depth depth)
    {
        _gui.RadialCheckbox(this, _targetState++, rectangle, label, depth);
    }
}
