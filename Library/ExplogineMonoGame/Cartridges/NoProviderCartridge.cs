namespace ExplogineMonoGame.Cartridges;

public abstract class NoProviderCartridge : Cartridge
{
    protected NoProviderCartridge(IRuntime runtime) : base(runtime)
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
