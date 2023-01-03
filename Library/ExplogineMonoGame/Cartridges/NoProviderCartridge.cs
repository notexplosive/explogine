using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Cartridges;

public abstract class NoProviderCartridge : ICartridge
{
    public abstract void OnCartridgeStarted();
    public abstract void Update(float dt);
    public abstract void Draw(Painter painter);
    public abstract void UpdateInput(ConsumableInput input, HitTestStack hitTestStack);
    public virtual void Unload()
    {
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public abstract CartridgeConfig CartridgeConfig { get; }
}
