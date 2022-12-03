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

    public void UpdateInput(AllDeviceFrameState input)
    {
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }
    
    public CartridgeConfig CartridgeConfig { get; } = new();
}
