using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Cartridges;

public class BlankCartridge : Cartridge
{
    public BlankCartridge(App app) : base(app)
    {
    }

    public override CartridgeConfig CartridgeConfig { get; } = new();

    public override void OnCartridgeStarted()
    {
    }

    public override void Update(float dt)
    {
    }

    public override void Draw(Painter painter)
    {
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public override bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public override void Unload()
    {
    }
}
