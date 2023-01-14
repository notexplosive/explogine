namespace ExplogineMonoGame.Cartridges;

public abstract class NoProviderCartridge : Cartridge
{
    protected NoProviderCartridge(IApp app) : base(app)
    {
    }

    public override void Unload()
    {
    }

    public override bool ShouldLoadNextCartridge()
    {
        return false;
    }
}
