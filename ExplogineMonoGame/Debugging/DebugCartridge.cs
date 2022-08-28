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
    private readonly FrameStep _frameStep = new();
    private readonly LogOverlay _logOverlay = new();
    private readonly SnapshotTaker _snapshotTaker = new();
    private bool _useSnapshotTimer;

    private Depth DemoStatusDepth { get; } = Depth.Front + 15;
    private Depth ConsoleOverlayDepth { get; } = Depth.Front + 5;

    public void OnCartridgeStarted()
    {
        Client.Debug.Output.PushToStack(_logOverlay);

#if DEBUG
        Client.Debug.Level = DebugLevel.Passive;
        Client.Debug.Log("~~ Debug Build ~~");
        _useSnapshotTimer = true;
#endif

        if (Client.CommandLineArgs.GetValue<bool>("skipSnapshot"))
        {
            _useSnapshotTimer = false;
            Client.Debug.Log("Snapshot timer disabled");
        }

        if (Client.CommandLineArgs.GetValue<bool>("debug"))
        {
            Client.Debug.Level = DebugLevel.Passive;
        }
    }

    public void Update(float dt)
    {
        _demoInterface.Update(dt);
        _logOverlay.Update(dt);

        if (Client.FinishedLoading.IsReady)
        {
            _frameStep.Update(dt);
            _snapshotTaker.Update(dt);
        }
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(SamplerState.LinearWrap);

        _demoInterface.Draw(painter, DemoStatusDepth);

        if (Client.Debug.IsPassive)
        {
            _logOverlay.Draw(painter, ConsoleOverlayDepth);
        }

        painter.EndSpriteBatch();

        if (_useSnapshotTimer)
        {
            // We don't let the snapshot timer start until after we're done with at least one draw
            _snapshotTaker.StartTimer();
        }
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public void SetupFormalParameters(CommandLineParameters args)
    {
        args.RegisterParameter<bool>("debug");
        args.RegisterParameter<bool>("skipSnapshot");
    }

    public IEnumerable<LoadEvent> LoadEvents(Painter painter)
    {
        yield return new LoadEvent("demo-indicators",
            () => new GridBasedSpriteSheet("engine/demo-indicators", new Point(67, 23)));
    }
    
    public CartridgeConfig CartridgeConfig { get; } = new();
}
