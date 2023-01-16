using ExplogineCore.Data;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Gui;

public class Label : IGuiWidget
{
    public Alignment Alignment { get; }
    public RectangleF Rectangle { get; }
    public Depth Depth { get; }
    public string Text { get; }

    public Label(RectangleF rectangle, Depth depth, string text, Alignment alignment)
    {
        Alignment = alignment;
        Rectangle = rectangle;
        Depth = depth;
        Text = text;
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }
}
