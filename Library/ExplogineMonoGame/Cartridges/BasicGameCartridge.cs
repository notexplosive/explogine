using System.Collections.Generic;
using ExplogineCore;

namespace ExplogineMonoGame.Cartridges;

public abstract class BasicGameCartridge : Cartridge, ILoadEventProvider, ICommandLineParameterProvider, IHotReloadable
{
    protected BasicGameCartridge(IRuntime runtime) : base(runtime)
    {
    }

    public abstract void AddCommandLineParameters(CommandLineParametersWriter parameters);

    public abstract void OnHotReload();
    public abstract IEnumerable<ILoadEvent?> LoadEvents(Painter painter);

    public override void Unload()
    {
    }

    public override bool ShouldLoadNextCartridge()
    {
        return false;
    }
}
