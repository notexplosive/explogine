using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Gui.Window;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Data;

public class CartridgePlayerContent<T> : IWindowContent, IUpdateHook where T : Cartridge
{
    private ICartridgePlayer? _cartridgePlayer;

    public void Update(float dt)
    {
        _cartridgePlayer?.Update(dt);
    }

    public void Initialize(InternalWindow parentWindow)
    {
        _cartridgePlayer = new CartridgePlayer<T>(parentWindow.Widget);
    }

    public void TearDown()
    {
        // Nothing to Dispose
    }

    public void DrawWindowContent(Painter painter)
    {
        _cartridgePlayer?.Draw(painter);
    }

    public void UpdateInputInWindow(ConsumableInput input, HitTestStack hitTestStack)
    {
        _cartridgePlayer?.UpdateInput(input, hitTestStack);
    }
}
