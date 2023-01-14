using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Cartridges;

public abstract class Cartridge : IUpdateInputHook, IDrawHook, IUpdateHook
{
    public abstract CartridgeConfig CartridgeConfig { get; }
    public abstract void OnCartridgeStarted();
    public abstract bool ShouldLoadNextCartridge();
    public abstract void Unload();
    public abstract void UpdateInput(ConsumableInput input, HitTestStack hitTestStack);
    public abstract void Draw(Painter painter);
    public abstract void Update(float dt);
}
