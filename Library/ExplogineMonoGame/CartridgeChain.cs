using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Debugging;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

internal class CartridgeChain : IUpdateInputHook, IUpdateHook
{
    private readonly LinkedList<ICartridge> _list = new();
    private bool _hasCrashed;
    private bool HasCurrent => _list.First != null;
    private ICartridge Current => _list.First!.Value;
    private ICartridge DebugCartridge { get; set; } = new DebugCartridge();
    public bool IsFrozen { get; set; }

    /// <summary>
    /// The "Game Cartridge" is the last cartridge in the chain. This is what the user provided wrapped in a MetaCartridge.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public MultiCartridge GameCartridge
    {
        get
        {
            if (_list.Last?.Value is MultiCartridge multiCartridge)
            {
                return multiCartridge;
            }

            throw new Exception($"Attempted to get {nameof(CartridgeChain.GameCartridge)} before it was available.");
        }
    }

    public event Action? AboutToLoadLastCartridge;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        DebugCartridge.UpdateInput(input, hitTestStack.AddLayer(Matrix.Identity, Depth.Middle));
        Current.UpdateInput(input, hitTestStack.AddLayer(Client.Window.Canvas.ScreenToCanvas, Depth.Middle + 1));
    }

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

        if (_list.Last == _list.First)
        {
            AboutToLoadLastCartridge?.Invoke();
        }

        if (HasCurrent)
        {
            CartridgeChain.StartCartridgeAndSetRenderResolution(Current);
        }
    }

    private static void StartCartridgeAndSetRenderResolution(ICartridge cartridge)
    {
        Client.Window.SetRenderResolution(cartridge.CartridgeConfig.RenderResolution);
        cartridge.OnCartridgeStarted();
    }

    public void AppendGameCartridge(ICartridge cartridge)
    {
        if (cartridge is MultiCartridge meta)
        {
            Append(meta);
        }
        else
        {
            Append(new MultiCartridge(cartridge));
        }
    }

    public void Append(ICartridge cartridge)
    {
        _list.AddLast(cartridge);
    }

    private void Prepend(ICartridge cartridge)
    {
        _list.AddFirst(cartridge);
    }

    private IEnumerable<ICartridge> GetAllCartridges()
    {
        foreach (var cartridge in _list)
        {
            yield return cartridge;
        }

        yield return DebugCartridge;
    }

    public void ValidateParameters(CommandLineParametersWriter writer)
    {
        foreach (var provider in GetAllCartridgesDerivedFrom<ICommandLineParameterProvider>())
        {
            provider.AddCommandLineParameters(writer);
        }
    }

    public void SetupLoadingCartridge(Loader loader)
    {
        var loadingCartridge = new LoadingCartridge(loader);
        CartridgeChain.StartCartridgeAndSetRenderResolution(loadingCartridge);
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
        CartridgeChain.StartCartridgeAndSetRenderResolution(crashCartridge);
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

    public void PrepareDebugCartridge(Painter painter)
    {
        if (DebugCartridge is IEarlyDrawHook preDrawDebug)
        {
            preDrawDebug.EarlyDraw(painter);
        }
    }
}
