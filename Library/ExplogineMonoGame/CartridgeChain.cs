using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Debugging;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

internal class CartridgeChain : IUpdateInputHook, IUpdateHook
{
    private readonly IApp _app;
    private readonly LinkedList<Cartridge> _list = new();
    private Cartridge _debugCartridge;
    private bool _hasCrashed;

    public CartridgeChain(IApp app)
    {
        _app = app;
        _debugCartridge = new DebugCartridge(app);
    }

    private bool HasCurrent => _list.First != null;
    private Cartridge Current => _list.First!.Value;
    public bool IsFrozen { get; set; }

    /// <summary>
    ///     The "Game Cartridge" is the last cartridge in the chain. This is what the user provided wrapped in a MetaCartridge.
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

    public void Update(float dt)
    {
        _debugCartridge.Update(dt);
        if (!IsFrozen)
        {
            UpdateCurrentCartridge(dt);
        }
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _debugCartridge.UpdateInput(input, hitTestStack.AddLayer(Matrix.Identity, Depth.Middle));
        Current.UpdateInput(input, hitTestStack.AddLayer(_app.Window.ScreenToCanvas, Depth.Middle + 1));
    }

    public event Action? AboutToLoadLastCartridge;

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
            _debugCartridge.Draw(painter);
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
            StartCartridgeAndSetRenderResolution(Current);
        }
    }

    private void StartCartridgeAndSetRenderResolution(Cartridge cartridge)
    {
        _app.Window.SetRenderResolution(cartridge.CartridgeConfig.RenderResolution);
        cartridge.OnCartridgeStarted();
    }

    public void AppendGameCartridge(Cartridge cartridge)
    {
        if (cartridge is MultiCartridge meta)
        {
            Append(meta);
        }
        else
        {
            Append(new MultiCartridge(_app, cartridge));
        }
    }

    public void Append(Cartridge cartridge)
    {
        _list.AddLast(cartridge);
    }

    private void Prepend(Cartridge cartridge)
    {
        _list.AddFirst(cartridge);
    }

    private IEnumerable<Cartridge> GetAllCartridges()
    {
        foreach (var cartridge in _list)
        {
            yield return cartridge;
        }

        yield return _debugCartridge;
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
        var loadingCartridge = new LoadingCartridge(_app, loader);
        StartCartridgeAndSetRenderResolution(loadingCartridge);
        Prepend(loadingCartridge);
        Client.FinishedLoading.Add(_debugCartridge.OnCartridgeStarted);
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
        var crashCartridge = new CrashCartridge(_app, exception);
        _list.Clear();
        _list.AddFirst(crashCartridge);
        StartCartridgeAndSetRenderResolution(crashCartridge);
        _debugCartridge = new BlankCartridge(_app);
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
        if (_debugCartridge is IEarlyDrawHook preDrawDebug)
        {
            preDrawDebug.EarlyDraw(painter);
        }
    }
}
