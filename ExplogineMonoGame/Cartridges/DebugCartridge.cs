using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Cartridges;

public class DebugCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    private float _totalTime;

    private Depth DemoStatusDepth { get; } = Depth.Front + 15;

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
    }

    public void Update(float dt)
    {
        _totalTime += dt;

        if (Client.Input.Keyboard.Modifiers.Control)
        {
            if (Client.Input.Keyboard.GetButton(Keys.P).WasPressed && !Client.Demo.IsPlaying)
            {
                Client.Demo.BeginPlayback();
            }

            if (Client.Input.Keyboard.GetButton(Keys.D).WasPressed && !Client.Demo.IsPlaying)
            {
                Client.Demo.DumpRecording();
            }
        }

        if (Client.Demo.IsPlaying)
        {
            if (Client.HumanInput.Keyboard.GetButton(Keys.Escape).WasPressed)
            {
                Client.Demo.Stop();
            }
        }
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(SamplerState.LinearWrap);
        DrawDemoRecordingOverlay(painter);

        painter.DrawString(Client.Assets.GetSpriteFont("engine/console-font"), "Debug text goes here", Vector2.Zero,
            Scale2D.One / 2, new DrawSettings());
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

    private void DrawDemoRecordingOverlay(Painter painter)
    {
        var spriteSheet = Client.Assets.GetAsset<SpriteSheet>("demo-indicators");
        var isPlaying = Client.Demo.IsPlaying;
        var isRecording = Client.Demo.IsRecording;

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
                    new DrawSettings {Depth = DemoStatusDepth});
            }
        }
    }
}

public enum DebugLevel
{
    None,
    Passive,
    Active
}
