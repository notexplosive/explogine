using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui.Window;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public interface IGuiTheme
{
    void DrawButton(Painter painter, Button button);
    void DrawCheckbox(Painter painter, Checkbox checkbox);
    void DrawSlider(Painter painter, Slider slider);
    void DrawRadialCheckbox(Painter painter, RadialCheckbox radialCheckbox);
    void DrawScrollbar(Painter painter, Scrollbar scrollBar);
    Color BackgroundColor { get; }
    IFontGetter Font { get; }
    void DrawTextInput(Painter painter, TextInputWidget textInputWidget);
    void DrawWindowChrome(Painter painter, VirtualWindow.Chrome chrome, bool isInFocus);
}
