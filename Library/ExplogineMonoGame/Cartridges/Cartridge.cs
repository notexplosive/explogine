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
}
