using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class SubGuiWidget : IGuiWidget, IPreDrawWidget
{
    public SubGuiWidget()
    {
        InnerGui = new Gui();
    }

    public Gui InnerGui { get; }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        InnerGui.UpdateInput(input, hitTestStack);
    }

    public void PrepareDraw(Painter painter, IGuiTheme uiTheme)
    {
        InnerGui.PrepareCanvases(painter, uiTheme);
    }

    public void Draw(Painter painter, IGuiTheme uiTheme)
    {
        InnerGui.Draw(painter, uiTheme);
    }
}
