using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

internal interface IGuiElement
{
    void UpdateInput(InputFrameState input, HitTestStack hitTestStack);
}
