using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Cartridges;

/// <summary>
///     A Cartridge that contains many cartridges.
///     When asked to load, it loads all cartridges.
///     When asked for command line parameters, it provides them from all cartridges.
///     When asked to Update/Draw/etc, it does so for the "Current" cartridge.
/// </summary>
public class MultiCartridge : BasicGameCartridge
{
    private readonly List<Cartridge> _cartridges = new();
    private readonly HashSet<int> _startedCartridges = new();
    private int _currentCartridgeIndexImpl;

    public MultiCartridge(IRuntime runtime, params Cartridge[] startingCartridges) : base(runtime)
    {
        _cartridges.AddRange(startingCartridges);
    }

    private int CurrentCartridgeIndex
    {
        get => _currentCartridgeIndexImpl;
        set
        {
            _currentCartridgeIndexImpl = value;
            StartCurrentCartridge();
        }
    }

    protected Cartridge? CurrentCartridge =>
        _cartridges.IsWithinRange(CurrentCartridgeIndex) ? _cartridges[CurrentCartridgeIndex] : null;

    public override CartridgeConfig CartridgeConfig
    {
        get
        {
            if (CurrentCartridge == null)
            {
                return new CartridgeConfig();
            }

            return CurrentCartridge.CartridgeConfig;
        }
    }

    public void Add(Cartridge cartridge)
    {
        _cartridges.Add(cartridge);
    }

    public void RegenerateCartridge<T>() where T : Cartridge
    {
        for (var i = 0; i < _cartridges.Count; i++)
        {
            if (_cartridges[i] is T)
            {
                RegenerateCartridge(i);
            }
        }
    }

    public void RegenerateCartridge(int i)
    {
        _startedCartridges.Remove(i);
        
        // Commented this out because we never "reload" the cartridge... I think we'll just suffer the possible memory leak
        // _cartridges[i].Unload();


        if (CurrentCartridge != null)
        {
            var targetType = _cartridges[i].GetType();
            _cartridges[i] = Cartridge.CreateInstance(targetType, Runtime);

            if (i == CurrentCartridgeIndex)
            {
                StartCurrentCartridge();
            }
        }
    }

    public void RegenerateCurrentCartridge()
    {
        RegenerateCartridge(CurrentCartridgeIndex);
    }

    public T SwapTo<T>() where T : Cartridge
    {
        for (var i = 0; i < _cartridges.Count; i++)
        {
            if (_cartridges[i] is T)
            {
                CurrentCartridgeIndex = i;
                return (CurrentCartridge as T)!;
            }
        }

        throw new Exception($"Tried to swap to a Cartridge of type {typeof(T).Name}, but none was found");
    }

    public Cartridge SwapTo(int index)
    {
        CurrentCartridgeIndex = index;

        if (!_cartridges.IsWithinRange(index))
        {
            throw new Exception($"Tried to access Cartridge {index} with {_cartridges.Count} cartridges");
        }

        return CurrentCartridge!;
    }

    public void SwapToPrevious()
    {
        var index = CurrentCartridgeIndex - 1;
        if (index < 0)
        {
            index = _cartridges.Count - 1;
        }

        SwapTo(index);
    }

    public void SwapToNext()
    {
        var index = CurrentCartridgeIndex + 1;
        if (index > _cartridges.Count - 1)
        {
            index = 0;
        }

        SwapTo(index);
    }

    private void StartCurrentCartridge()
    {
        if (CurrentCartridge != null)
        {
            Runtime.Window.SetRenderResolution(CurrentCartridge.CartridgeConfig);
        }

        if (!_startedCartridges.Contains(CurrentCartridgeIndex))
        {
            CurrentCartridge?.OnCartridgeStarted();
            _startedCartridges.Add(CurrentCartridgeIndex);
        }
    }

    public override void OnCartridgeStarted()
    {
        BeforeStart();
        if (_cartridges.Count == 0)
        {
            throw new Exception("No cartridge to run!");
        }

        StartCurrentCartridge();
    }

    public override void Update(float dt)
    {
        BeforeUpdate(dt);
        CurrentCartridge?.Update(dt);
    }

    public override void Draw(Painter painter)
    {
        CurrentCartridge?.Draw(painter);
        AfterDraw(painter);
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        BeforeUpdateInput(input, hitTestStack);
        CurrentCartridge?.UpdateInput(input, hitTestStack);
        AfterUpdateInput(input, hitTestStack);
    }

    protected virtual IEnumerable<ILoadEvent?> ExtraLoadEvents()
    {
        yield return null;
    }

    protected virtual void BeforeStart()
    {
    }

    protected virtual void BeforeUpdate(float dt)
    {
    }

    protected virtual void AfterDraw(Painter painter)
    {
    }

    protected virtual void BeforeUpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    protected virtual void AfterUpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public override void Unload()
    {
        foreach (var cartridge in _cartridges)
        {
            cartridge.Unload();
        }
    }

    public override void AddCommandLineParameters(CommandLineParametersWriter parameters)
    {
        foreach (var cartridge in _cartridges)
        {
            if (cartridge is ICommandLineParameterProvider provider)
            {
                provider.AddCommandLineParameters(parameters);
            }
        }
    }

    public override IEnumerable<ILoadEvent?> LoadEvents(Painter painter)
    {
        foreach (var loadEvent in ExtraLoadEvents())
        {
            yield return loadEvent;
        }

        foreach (var cartridge in _cartridges)
        {
            if (cartridge is ILoadEventProvider provider)
            {
                foreach (var loadEvent in provider.LoadEvents(painter))
                {
                    yield return loadEvent;
                }
            }
        }
    }
}
