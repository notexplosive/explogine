using ExplogineCore.Data;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Gui;

public class Label : IGuiWidget
{
    public Label(RectangleF rectangle, Depth depth, string text, Alignment alignment, int? fontSize)
    {
        Alignment = alignment;
        Rectangle = rectangle;
        Depth = depth;
        Text = text;
        FontSize = fontSize;
    }

    public Alignment Alignment { get; }
    public RectangleF Rectangle { get; }
    public Depth Depth { get; }
    public string Text { get; }
    public int? FontSize { get; set; }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }
}
