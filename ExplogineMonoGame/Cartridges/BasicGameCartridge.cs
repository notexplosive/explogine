using System.Collections.Generic;
using ExplogineCore;

namespace ExplogineMonoGame.Cartridges;

public abstract class BasicGameCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
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
