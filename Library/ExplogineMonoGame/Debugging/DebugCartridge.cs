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

public class DebugCartridge : ICartridge, ILoadEventProvider, IEarlyDrawer
{
    private readonly DemoInterface _demoInterface = new();
    private readonly FrameStep _frameStep = new();
    private readonly LogOverlay _logOverlay = new();
    private readonly SnapshotTaker _snapshotTaker = new();
    private bool _useSnapshotTimer;

    private Depth DemoStatusDepth { get; } = Depth.Front + 15;
    private Depth ConsoleOverlayDepth { get; } = Depth.Front + 5;
    private Depth FrameStepDepth { get; } = Depth.Front + 20;

    public void OnCartridgeStarted()
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

    public void Update(float dt)
    {
        _demoInterface.Update(dt);
        _logOverlay.Update(dt);
        _frameStep.UpdateGraphic(dt);
    }
    
    public void EarlyDraw(Painter painter)
    {
        // We no longer do anything here, but we might someday
    }

    public void Draw(Painter painter)
    { 
        if (Client.Debug.IsPassiveOrActive)
        {
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

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (input.Keyboard.Modifiers.ControlShift && input.Keyboard.GetButton(Keys.OemTilde).WasPressed)
        {
            Client.Debug.CycleDebugMode();
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

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public void Unload()
    {
    }

    public IEnumerable<ILoadEvent?> LoadEvents(Painter painter)
    {
        yield return new AssetLoadEvent("demo-indicators",
            () => new GridBasedSpriteSheet("engine/demo-indicators", new Point(67, 23)));
    }
    
    public CartridgeConfig CartridgeConfig { get; } = new();
}
