using System.Collections.Generic;

namespace NotCore.Cartridges;

public abstract class SimpleGameCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    public abstract void OnCartridgeStarted();
    public abstract void Update(float dt);
    public abstract void Draw(Painter painter);

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public abstract void SetupFormalParameters(ParsedCommandLineArguments args);
    public abstract IEnumerable<LoadEvent> LoadEvents(Painter painter);

}
