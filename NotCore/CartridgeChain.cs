using System;
using System.Collections.Generic;
using NotCore.Cartridges;

namespace NotCore;

internal class CartridgeChain : ILoadEventProvider
{
    private readonly LinkedList<ICartridge> _list = new();

    private ICartridge Current => _list.First!.Value;

    public IEnumerable<LoadEvent> LoadEvents(Painter painter)
    {
        foreach (var cartridge in GetAllCartridges())
        {
            if (cartridge is not ILoadEventProvider preloadCartridge)
            {
                continue;
            }

            foreach (var loadEvent in preloadCartridge.LoadEvents(Client.Graphics.Painter))
            {
                yield return loadEvent;
            }
        }
    }

    public void Update(float dt)
    {
        Current.Update(dt);

        if (Current.ShouldLoadNextCartridge())
        {
            IncrementCartridge();
        }
    }

    public void Draw(Painter painter)
    {
        Current.Draw(painter);
    }

    private void IncrementCartridge()
    {
        _list.RemoveFirst();
    }

    public void Append(ICartridge cartridge)
    {
        _list.AddLast(cartridge);
    }

    public void Prepend(ICartridge cartridge)
    {
        _list.AddFirst(cartridge);
    }

    public IEnumerable<ICartridge> GetAllCartridges()
    {
        foreach (var cartridge in _list)
        {
            yield return cartridge;
        }
    }

    public void ForeachPreload(Action<LoadEvent> callback)
    {
        foreach (var loadEvent in LoadEvents(Client.Graphics.Painter))
        {
            callback(loadEvent);
        }
    }

    public void ValidateParameters(CommandLineArguments args)
    {
        foreach (var cartridge in GetAllCartridges())
        {
            if (cartridge is not ICommandLineParameterProvider provider)
            {
                continue;
            }

            provider.SetupFormalParameters(args);
        }
    }
}
