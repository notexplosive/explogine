using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Debugging;

public class DebugCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    private readonly DemoInterface _demoInterface = new();
    private readonly LogOverlay _logOverlay = new();

    private Depth DemoStatusDepth { get; } = Depth.Front + 15;
    private Depth ConsoleOverlayDepth { get; } = Depth.Front + 5;

    public void OnCartridgeStarted()
    {
#if DEBUG
        Client.Debug.DebugLevel = DebugLevel.Passive;
#endif

        if (Client.ParsedCommandLineArguments.GetValue<bool>("debug"))
        {
            Client.Debug.DebugLevel = DebugLevel.Passive;
        }
        else
        {
            Client.Debug.DebugLevel = DebugLevel.None;
        }

        Client.Debug.Output.PushToStack(_logOverlay);
    }

    public void Update(float dt)
    {
        _demoInterface.Update(dt);
        _logOverlay.Update(dt);
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(SamplerState.LinearWrap);

        _demoInterface.Draw(painter, DemoStatusDepth);
        _logOverlay.Draw(painter, ConsoleOverlayDepth);

        painter.EndSpriteBatch();
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public void SetupFormalParameters(ParsedCommandLineArguments args)
    {
        args.RegisterParameter<bool>("debug");
    }

    public IEnumerable<LoadEvent> LoadEvents(Painter painter)
    {
        yield return () => new GridBasedSpriteSheet("demo-indicators", "engine/demo-indicators", new Point(67, 23));
    }
}