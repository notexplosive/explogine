using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Data;

public interface ICartridgePlayer : IUpdateHook, IUpdateInputHook, IDrawHook
{
}

public class CartridgePlayer<TCartridge> : ICartridgePlayer where TCartridge : Cartridge
{
    private readonly TCartridge _cartridge;

    public CartridgePlayer(IWindow window)
    {
        var runtime = new Runtime(window, new ClientFileSystem());
        _cartridge = Cartridge.CreateInstance<TCartridge>(runtime);

        // Assumes LoadEvents were already run before CartridgePlayer was created
        _cartridge.OnCartridgeStarted();
    }

    public void Update(float dt)
    {
        _cartridge.Update(dt);
    }

    public void Draw(Painter painter)
    {
        _cartridge.Draw(painter);
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _cartridge.UpdateInput(input, hitTestStack);
    }
}
