using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.Cartridges;

namespace ExplogineMonoGame;

internal class CartridgeChain : ILoadEventProvider
{
    private readonly LinkedList<ICartridge> _list = new();

    private ICartridge Current => _list.First!.Value;
    public DebugCartridge DebugCartridge { get; } = new();

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
        DebugCartridge.Update(dt);
        Current.Update(dt);

        if (Current.ShouldLoadNextCartridge())
        {
            IncrementCartridge();
        }
    }

    public void Draw(Painter painter)
    {
        Current.Draw(painter);
        
        if (Client.FinishedLoading.IsReady)
        {
            DebugCartridge.Draw(Client.Graphics.Painter);
        }
    }

    private void IncrementCartridge()
    {
        _list.RemoveFirst();
        _list.First?.Value.OnCartridgeStarted();
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

        yield return DebugCartridge;
    }

    public void ForeachPreload(Action<LoadEvent> callback)
    {
        foreach (var loadEvent in LoadEvents(Client.Graphics.Painter))
        {
            callback(loadEvent);
        }
    }

    public void ValidateParameters(ParsedCommandLineArguments args)
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

    public void SetupLoadingCartridge(Loader loader)
    {
        ForeachPreload(loader.AddDynamicLoadEvent);
        var loadingCartridge = new LoadingCartridge(loader);
        loadingCartridge.OnCartridgeStarted();
        Prepend(loadingCartridge);
        Client.FinishedLoading.Add(DebugCartridge.OnCartridgeStarted);
    }
}
