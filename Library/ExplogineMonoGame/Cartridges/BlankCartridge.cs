using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Cartridges;

public class BlankCartridge : ICartridge
{
    public void OnCartridgeStarted()
    {
    }

    public void Update(float dt)
    {
    }

    public void Draw(Painter painter)
    {
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public void Unload()
    {
    }

    public CartridgeConfig CartridgeConfig { get; } = new();
}
