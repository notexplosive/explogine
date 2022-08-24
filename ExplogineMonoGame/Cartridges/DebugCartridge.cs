using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public class DebugCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    private float _totalTime;

    public void OnCartridgeStarted()
    {
    }

    public void Update(float dt)
    {
        _totalTime += dt;
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch();
        DrawDemoRecordingOverlay(painter);
        painter.EndSpriteBatch();
    }

    private void DrawDemoRecordingOverlay(Painter painter)
    {
        var spriteSheet = Client.Assets.GetAsset<SpriteSheet>("demo-indicators");
        var isPlaying = Client.DemoRecorder.IsPlaying;
        var isRecording = Client.DemoRecorder.IsRecording;

        if (isRecording || isPlaying)
        {
            var frame = 0;
            if (isRecording)
            {
                frame = 1;
            }
            
            if (MathF.Sin(_totalTime * 10) > 0)
            {
                spriteSheet.DrawFrame(
                    painter,
                    frame,
                    new Vector2(Client.Graphics.WindowSize.X - spriteSheet.GetSourceRectForFrame(0).Width, 0),
                    Scale2D.One,
                    new DrawSettings {Depth = Depth.Front});
            }
        }
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
        yield return () => new GridBasedSpriteSheet("demo-indicators", "engine/DemoIndicators", new Point(67, 23));
    }
}
