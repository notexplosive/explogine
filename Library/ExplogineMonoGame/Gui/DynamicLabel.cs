using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class DynamicLabel : IGuiWidget
{
    private readonly Action<Painter, IGuiTheme, RectangleF, Depth> _action;
    private readonly RectangleF _rectangle;
    private readonly Depth _depth;

    public DynamicLabel(RectangleF rectangle, Depth depth, Action<Painter, IGuiTheme, RectangleF, Depth> action)
    {
        _rectangle = rectangle;
        _depth = depth;
        _action = action;
    }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        // do nothing
    }

    public void Draw(Painter painter, IGuiTheme guiTheme)
    {
        _action(painter, guiTheme, _rectangle, _depth);
    }
}
