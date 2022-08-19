using System.Collections.Generic;
using ExplogineCore;

namespace ExplogineMonoGame.Cartridges;

public interface ICartridge
{
    public void OnCartridgeStarted();
    public void Update(float dt);
    public void Draw(Painter painter);
    public bool ShouldLoadNextCartridge();
}

public interface ILoadEventProvider
{
    public IEnumerable<LoadEvent> LoadEvents(Painter painter);
}

public interface ICommandLineParameterProvider
{
    public void SetupFormalParameters(ParsedCommandLineArguments args);
}
