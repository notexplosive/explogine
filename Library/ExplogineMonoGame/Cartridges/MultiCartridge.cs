using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Cartridges;

/// <summary>
///     A Cartridge that contains many cartridges.
///     When asked to load, it loads all cartridges.
///     When asked for command line parameters, it provides them from all cartridges.
///     When asked to Update/Draw/etc, it does so for the "Current" cartridge.
/// </summary>
public class MultiCartridge : BasicGameCartridge
{
    private readonly List<ICartridge> _cartridges = new();
    private readonly HashSet<int> _startedCartridges = new();
    private int _currentCartridgeIndexImpl;

    public MultiCartridge(ICartridge primaryCartridge, params ICartridge[] extraCartridges)
    {
        _cartridges.Add(primaryCartridge);
        _cartridges.AddRange(extraCartridges);
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

    private ICartridge CurrentCartridge => _cartridges[CurrentCartridgeIndex];
    public override CartridgeConfig CartridgeConfig => CurrentCartridge.CartridgeConfig;

    public void RegenerateCartridge<T>() where T : ICartridge, new()
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
        _cartridges[i].Unload();

        var targetType = CurrentCartridge.GetType();
        var newCart = (ICartridge?) Activator.CreateInstance(targetType);
        _cartridges[i] = newCart ?? throw new Exception($"Failed to create a new {targetType.Name}, maybe it doesn't have a parameterless constructor?");

        if (i == CurrentCartridgeIndex)
        {
            StartCurrentCartridge();
        }
    }
    
    public void RegenerateCurrentCartridge()
    {
        RegenerateCartridge(CurrentCartridgeIndex);
    }

    public void SwapTo<T>() where T : ICartridge
    {
        for (var i = 0; i < _cartridges.Count; i++)
        {
            if (_cartridges[i] is T)
            {
                CurrentCartridgeIndex = i;
                return;
            }
        }

        throw new Exception($"Tried to swap to a Cartridge of type {typeof(T).Name}, but none was found");
    }

    public void SwapTo(int index)
    {
        CurrentCartridgeIndex = index;

        if (!_cartridges.IsWithinRange(index))
        {
            throw new Exception($"Tried to access Cartridge {index} with {_cartridges.Count} cartridges");
        }
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
        Client.Window.SetRenderResolution(CurrentCartridge.CartridgeConfig.RenderResolution);
        
        if (!_startedCartridges.Contains(CurrentCartridgeIndex))
        {
            CurrentCartridge.OnCartridgeStarted();
            _startedCartridges.Add(CurrentCartridgeIndex);
        }

    }

    public override void OnCartridgeStarted()
    {
        MetaStart();
        StartCurrentCartridge();
    }

    public override void Update(float dt)
    {
        MetaUpdate(dt);
        CurrentCartridge.Update(dt);
    }

    public override void Draw(Painter painter)
    {
        CurrentCartridge.Draw(painter);
        MetaDraw(painter);
    }

    public override void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        MetaUpdateInput(input);
        CurrentCartridge.UpdateInput(input, hitTestStack);
    }
    
    protected virtual IEnumerable<ILoadEvent?> MetaLoadEvents()
    {
        yield return null;
    }

    protected virtual void MetaStart()
    {
    }

    protected virtual void MetaUpdate(float dt)
    {
    }

    protected virtual void MetaDraw(Painter painter)
    {
    }

    protected virtual void MetaUpdateInput(InputFrameState input)
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
        foreach (var loadEvent in MetaLoadEvents())
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
