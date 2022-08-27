﻿using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Debugging;

namespace ExplogineMonoGame;

internal class CartridgeChain : ILoadEventProvider
{
    private readonly LinkedList<ICartridge> _list = new();

    private ICartridge Current => _list.First!.Value;
    private ICartridge DebugCartridge { get; set; } = new DebugCartridge();
    public event Action? LoadedLastCartridge;

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

    public void DrawCurrentCartridge(Painter painter)
    {
        Current.Draw(painter);
    }

    public void DrawDebugCartridge(Painter painter)
    {
        if (Client.FinishedLoading.IsReady)
        {
            DebugCartridge.Draw(painter);
        }
    }

    private void IncrementCartridge()
    {
        _list.RemoveFirst();
        _list.First?.Value.OnCartridgeStarted();

        if (_list.Last == _list.First)
        {
            LoadedLastCartridge?.Invoke();
        }
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
        foreach (var loadEvent in LoadEvents(Client.Graphics.Painter))
        {
            loader.AddLoadEvent(loadEvent);
        }

        var loadingCartridge = new LoadingCartridge(loader);
        loadingCartridge.OnCartridgeStarted();
        Prepend(loadingCartridge);
        Client.FinishedLoading.Add(DebugCartridge.OnCartridgeStarted);
    }

    public void Crash(Exception exception)
    {
        _list.Clear();
        _list.AddFirst(new BlankCartridge());
        var crashCartridge = new CrashCartridge(exception);
        crashCartridge.OnCartridgeStarted();
        DebugCartridge = crashCartridge;
    }
}
