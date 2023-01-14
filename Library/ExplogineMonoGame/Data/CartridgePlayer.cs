using System;
using System.Collections.Generic;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Data;

public interface ICartridgePlayer : IUpdateHook, IUpdateInputHook, IDrawHook, ILoadEventProvider
{
}

public class CartridgePlayer<TCartridge> : ICartridgePlayer where TCartridge : Cartridge
{
    private readonly TCartridge _cartridge;

    public CartridgePlayer(IWindow window)
    {
        var constructedCartridge =
            (TCartridge?) Activator.CreateInstance(typeof(TCartridge), new App(window, new ClientFileSystem()));

        _cartridge = constructedCartridge ??
                     throw new Exception($"Activator could not create instance of {typeof(TCartridge).Name}");

        // assumes load events were already called
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

    public IEnumerable<ILoadEvent?> LoadEvents(Painter painter)
    {
        if (_cartridge is ILoadEventProvider provider)
        {
            foreach (var loadEvent in provider.LoadEvents(painter))
            {
                yield return loadEvent;
            }
        }
    }
}
