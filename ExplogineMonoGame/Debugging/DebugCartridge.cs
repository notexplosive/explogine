using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Debugging;

public class DebugCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    private readonly DemoInterface _demoInterface = new();
    private readonly LogOverlay _logOverlay = new();

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
        _logOverlay.Draw(painter);

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

internal class LogOverlay : ILogCapture
{
    private readonly List<string> _linesBuffer = new();
    private float _timer;

    private float Opacity => Math.Clamp(_timer, 0f, 1f);

    public void CaptureMessage(string message)
    {
        _linesBuffer.Add(message);
        _timer = 5;
    }

    public void Update(float dt)
    {
        _timer -= dt;
    }

    public void Draw(Painter painter)
    {
        var font = Client.Assets.GetSpriteFont("engine/console-font");
        var scale = 0.5f;
        var heightOfOneLine = font.LineSpacing * scale;
        var currentY = 0;
        foreach (var message in _linesBuffer)
        {
            painter.DrawString(font, message, new Point(0, currentY),
                Scale2D.One * scale, new DrawSettings {Color = Color.White.WithMultipliedOpacity(Opacity)});

            // todo: calculate height of each line, render linebreaks for long lines

            currentY += (int) heightOfOneLine;
        }
    }
}
