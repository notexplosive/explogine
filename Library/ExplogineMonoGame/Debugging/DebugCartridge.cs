using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

public class DebugCartridge : Cartridge, ILoadEventProvider, IEarlyDrawHook
{
    private readonly DemoInterface _demoInterface;
    private readonly FrameStep _frameStep;
    private readonly LogOverlay _logOverlay;
    private readonly SnapshotTaker _snapshotTaker;
    private readonly FramerateCounter _framerateCounter;
    private bool _useSnapshotTimer;

    public DebugCartridge(IRuntime runtime) : base(runtime)
    {
        _demoInterface = new(runtime);
        _frameStep = new(runtime);
        _logOverlay = new(runtime);
        _snapshotTaker = new(runtime);
        _framerateCounter = new();
    }
    
    private Depth DemoStatusDepth { get; } = Depth.Front + 15;
    private Depth ConsoleOverlayDepth { get; } = Depth.Front + 5;
    private Depth FrameStepDepth { get; } = Depth.Front + 20;

    public override void OnCartridgeStarted()
    {
        Client.Debug.Output.PushToStack(_logOverlay);

        if (Client.Debug.LaunchedAsDebugMode())
        {
            Client.Debug.Log("~~ Debug Build ~~");
            _useSnapshotTimer = true;
            Client.Debug.Level = DebugLevel.Passive;
        }

        if (Client.Args.GetValue<bool>("skipSnapshot"))
        {
            _useSnapshotTimer = false;
            Client.Debug.Log("Snapshot timer disabled");
        }

        
    }

    public override void Update(float dt)
    {
        _demoInterface.Update(dt);
        _logOverlay.Update(dt);
        _frameStep.UpdateGraphic(dt);
        _snapshotTaker.Update(dt);
        _framerateCounter.Update(dt);
    }
    
    public void EarlyDraw(Painter painter)
    {
        // We no longer do anything here, but we might someday
    }

    public override void Draw(Painter painter)
    { 
        if (Client.Debug.IsPassiveOrActive)
        {
            _framerateCounter.Draw(painter);
            _logOverlay.Draw(painter, ConsoleOverlayDepth);
        }
        
        painter.BeginSpriteBatch();
        
        _demoInterface.Draw(painter, DemoStatusDepth);
        _frameStep.Draw(painter, FrameStepDepth);

        painter.EndSpriteBatch();

        if (_useSnapshotTimer)
        {
            // We don't let the snapshot timer start until after we're done with at least one draw
            _snapshotTaker.StartTimer();
        }

    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (input.Keyboard.Modifiers.ControlShift && input.Keyboard.GetButton(Keys.OemTilde).WasPressed)
        {
            Client.Debug.CycleDebugMode();
        }
        
        if (input.Keyboard.Modifiers.ControlShift && input.Keyboard.GetButton(Keys.F12, true).WasPressed)
        {
            _useSnapshotTimer = !_useSnapshotTimer;
            Client.Debug.Log($"Snapshots timer is now {(_useSnapshotTimer ? "Enabled" : "Disabled")}");

            if (!_useSnapshotTimer)
            {
                _snapshotTaker.StopTimer();
            }
        }
        
        if (Client.FinishedLoading.IsReady)
        {
            _snapshotTaker.UpdateInput(input, hitTestStack);
            _frameStep.UpdateInput(input, hitTestStack);

            if (Client.Debug.IsPassiveOrActive)
            {
                _logOverlay.UpdateInput(input, hitTestStack);
            }
        }
    }

    public override bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public override void Unload()
    {
    }

    public IEnumerable<ILoadEvent?> LoadEvents(Painter painter)
    {
        yield return new AssetLoadEvent("demo-indicators", "Engine Tools",
            () => new GridBasedSpriteSheet("engine/demo-indicators", new Point(67, 23)));
    }
    
    public override CartridgeConfig CartridgeConfig { get; } = new();
}