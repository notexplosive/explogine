using System;
using System.Collections;
using System.Collections.Generic;

namespace NotCore;

internal class CartridgeChain
{
    private readonly LinkedList<ICartridge> _list = new();

    private ICartridge Current => _list.First!.Value;

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

    public void ForeachPreload(Action<Loader.LoadEvent> callback)
    {
        foreach (var cartridge in GetAllCartridges())
        {
            if (cartridge is not ILoadEventProvider preloadCartridge)
            {
                continue;
            }

            foreach (var loadEvent in preloadCartridge.LoadEvents(Client.Graphics.Painter))
            {
                callback(loadEvent);
            }
        }
    }

    public void ForeachCommandLineParam(Action<ICommandLineParameter> callback)
    {
        foreach (var cartridge in GetAllCartridges())
        {
            if (cartridge is not ICommandLineParameterProvider provider)
            {
                continue;
            }

            foreach (var arg in provider.GetFormalParameters())
            {
                callback(arg);
            }
        }
    }

    public void ValidateParameters()
    {
        var parameters = new List<ICommandLineParameter>();
        ForeachCommandLineParam(param => parameters.Add(param));

        HashSet<string> names = new();

        foreach (var parameter in parameters)
        {
            if (!names.Add(parameter.Name))
            {
                throw new Exception($"Command line Parameter {parameter.Name} is defined twice");
            }
        }
    }
}
