using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Debugging;

namespace ExplogineMonoGame;

internal class CartridgeChain
{
    private readonly LinkedList<ICartridge> _list = new();
    private bool _hasCrashed;
    private bool HasCurrent => _list.First != null;
    private ICartridge Current => _list.First!.Value;
    private ICartridge DebugCartridge { get; set; } = new DebugCartridge();
    public bool IsFrozen { get; set; }

    public event Action? LoadedLastCartridge;

    public void Update(float dt)
    {
        DebugCartridge.Update(dt);

        if (!IsFrozen)
        {
            UpdateCurrentCartridge(dt);
        }
    }

    public void UpdateCurrentCartridge(float dt)
    {
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
        if (HasCurrent)
        {
            CartridgeChain.StartCartridge(Current);
        }

        if (_list.Last == _list.First)
        {
            LoadedLastCartridge?.Invoke();
        }
    }

    private static void StartCartridge(ICartridge cartridge)
    {
        Client.Window.SetRenderResolution(cartridge.CartridgeConfig.RenderResolution);
        cartridge.OnCartridgeStarted();
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

    public void ValidateParameters(CommandLineParameters args)
    {
        foreach (var provider in GetAllCartridgesDerivedFrom<ICommandLineParameterProvider>())
        {
            provider.SetupFormalParameters(args);
        }
    }

    public void SetupLoadingCartridge(Loader loader)
    {
        var loadingCartridge = new LoadingCartridge(loader);
        CartridgeChain.StartCartridge(loadingCartridge);
        Prepend(loadingCartridge);
        Client.FinishedLoading.Add(DebugCartridge.OnCartridgeStarted);
    }

    public void Crash(Exception exception)
    {
        if (_hasCrashed)
        {
            // If we crashed while crashing, just exit
            Client.Exit();
            return;
        }

        _hasCrashed = true;
        var crashCartridge = new CrashCartridge(exception);
        _list.Clear();
        _list.AddFirst(crashCartridge);
        CartridgeChain.StartCartridge(crashCartridge);
        DebugCartridge = new BlankCartridge();
    }

    public IEnumerable<T> GetAllCartridgesDerivedFrom<T>()
    {
        foreach (var cartridge in GetAllCartridges())
        {
            if (cartridge is T derivedCartridge)
            {
                yield return derivedCartridge;
            }
        }
    }
}
