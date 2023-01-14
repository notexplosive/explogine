using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Data;

public class CartridgePlayerContent : IWindowContent, IUpdateHook
{
    private ICartridgePlayer? _cartridgePlayer;

    public void DrawWindowContent(Painter painter)
    {
        _cartridgePlayer?.Draw(painter);
    }

    public void UpdateInputInWindow(ConsumableInput input, HitTestStack hitTestStack)
    {
        _cartridgePlayer?.UpdateInput(input, hitTestStack);
    }
        
    public void Update(float dt)
    {
        _cartridgePlayer?.Update(dt);
    }

    public void AttachWindow<T>(IWindow windowWidget) where T : Cartridge
    {
        _cartridgePlayer = new CartridgePlayer<T>(windowWidget);
    }
}