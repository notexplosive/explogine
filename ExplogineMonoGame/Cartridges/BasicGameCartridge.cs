using System.Collections.Generic;
using ExplogineCore;

namespace ExplogineMonoGame.Cartridges;

public abstract class BasicGameCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    public void OnCartridgeStarted()
    {
        var demoVal = Client.ParsedCommandLineArguments.GetValue<string>("demo");
        if (!string.IsNullOrEmpty(demoVal))
        {
            switch(demoVal)
            {
                case "record":
                    Client.DemoRecorder.BeginRecording();
                    break;
                case "playback":
                    Client.DemoRecorder.LoadFile("default.demo");
                    Client.DemoRecorder.BeginPlayback();
                    break;
            }
        }
        OnStarted();
    }

    protected abstract void OnStarted();
    public abstract void Update(float dt);
    public abstract void Draw(Painter painter);

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public abstract void SetupFormalParameters(ParsedCommandLineArguments args);
    public abstract IEnumerable<LoadEvent> LoadEvents(Painter painter);

}
