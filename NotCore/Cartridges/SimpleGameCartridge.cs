using System.Collections.Generic;

namespace NotCore.Cartridges;

public abstract class SimpleGameCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    protected SimpleGameCartridge()
    {
        Client.RunWhenReady(Load);
    }

    public abstract void Update(float dt);
    public abstract void Draw(Painter painter);

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public abstract void SetupFormalParameters(CommandLineArguments args);
    public abstract IEnumerable<LoadEvent> LoadEvents(Painter painter);

    protected abstract void Load();
}
