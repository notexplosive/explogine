using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Cartridges;

/// <summary>
/// A Cartridge that contains many cartridges.
///
/// When asked to load, it loads all cartridges.
/// When asked for command line parameters, it provides them from all cartridges.
///
/// When asked to Update/Draw/etc, it does so for the "Current" cartridge.
/// </summary>
public class MultiCartridge : BasicGameCartridge
{
    private readonly List<ICartridge> _cartridges = new();
    private readonly HashSet<int> _startedCartridges = new();
    private int _currentCartridgeIndex;

    public MultiCartridge(ICartridge primaryCartridge, params ICartridge[] extraCartridges)
    {
        _cartridges.Add(primaryCartridge);
        _cartridges.AddRange(extraCartridges);
        _currentCartridgeIndex = 0;
    }

    private ICartridge CurrentCartridge => _cartridges[_currentCartridgeIndex];
    public override CartridgeConfig CartridgeConfig => CurrentCartridge.CartridgeConfig;

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

    public void SwapToCartridge(int index)
    {
        _currentCartridgeIndex = index;

        if (!_cartridges.IsWithinRange(index))
        {
            throw new Exception($"Tried to access Cartridge {index} with {_cartridges.Count} cartridges");
        }

        if (!_startedCartridges.Contains(index))
        {
            StartCurrentCartridge();
        }
        
        Client.Window.SetRenderResolution(CurrentCartridge.CartridgeConfig.RenderResolution);
    }
    
    private void StartCurrentCartridge()
    {
        CurrentCartridge.OnCartridgeStarted();
        _startedCartridges.Add(_currentCartridgeIndex);
    }

    public override void OnCartridgeStarted()
    {
        StartCurrentCartridge();
    }

    public override void Update(float dt)
    {
        CurrentCartridge.Update(dt);
    }

    public override void Draw(Painter painter)
    {
        CurrentCartridge.Draw(painter);
    }

    public override void UpdateInput(InputFrameState input)
    {
        CurrentCartridge.UpdateInput(input);
    }
}
