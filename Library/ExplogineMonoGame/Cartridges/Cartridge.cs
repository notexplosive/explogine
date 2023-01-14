using System;
using System.Collections.Generic;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Cartridges;

public abstract class Cartridge : IUpdateInputHook, IDrawHook, IUpdateHook
{
    protected Cartridge(IRuntime runtime)
    {
        Runtime = runtime;
    }

    public IRuntime Runtime { get; }
    public abstract CartridgeConfig CartridgeConfig { get; }
    public abstract void Draw(Painter painter);
    public abstract void Update(float dt);
    public abstract void UpdateInput(ConsumableInput input, HitTestStack hitTestStack);
    public abstract void OnCartridgeStarted();
    public abstract bool ShouldLoadNextCartridge();
    public abstract void Unload();

    public static TCartridge CreateInstance<TCartridge>(IRuntime runtime) where TCartridge : Cartridge
    {
        var constructedCartridge =
            (TCartridge?) Activator.CreateInstance(typeof(TCartridge), runtime);
        return constructedCartridge ??
               throw new Exception($"Activator could not create instance of {typeof(TCartridge).Name} using `new {typeof(TCartridge).Name}({nameof(Runtime)}),` maybe this constructor isn't supported?");
    }
    
    public static Cartridge CreateInstance(Type type, IRuntime runtime)
    {
        var constructedCartridge = (Cartridge?) Activator.CreateInstance(type, runtime);
        return constructedCartridge ??
               throw new Exception($"Activator could not create instance of {type.Name} using `new {type.Name}({nameof(Runtime)}),` maybe this constructor isn't supported?");
    }

    public static IEnumerable<ILoadEvent?> GetLoadEventsForCartridge<TCartridge>(IRuntime runtime) where TCartridge : Cartridge
    {
        var cartridge = Cartridge.CreateInstance<TCartridge>(runtime);
        if (cartridge is ILoadEventProvider provider)
        {
            foreach (var loadEvent in provider.LoadEvents(Client.Graphics.Painter))
            {
                yield return loadEvent;
            }
        }
    }
}
