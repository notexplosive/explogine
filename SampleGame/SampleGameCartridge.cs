using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SampleGame;

public class SampleGameCartridge : BasicGameCartridge
{
    private float _totalTime;

    public override void OnCartridgeStarted()
    {
        Console.WriteLine("Sample Cart Loaded");
    }

    public override void Update(float dt)
    {
        _totalTime += dt;

        if (Client.Input.Keyboard.GetButton(Keys.F).WasPressed)
        {
            Client.Window.SetFullscreen(!Client.Window.IsFullscreen);
        }
    }

    public override void Draw(Painter painter)
    {
        painter.Clear(Color.CornflowerBlue);
        painter.BeginSpriteBatch(SamplerState.PointWrap);
        
        painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"), new Rectangle(100,100, 500, 500), new DrawSettings{Color = Color.LightBlue});
        painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"), new Rectangle(150,150, 500, 500), new DrawSettings{Color = Color.LightGreen});

        painter.EndSpriteBatch();
    }

    public override void SetupFormalParameters(ParsedCommandLineArguments args)
    {
        args.RegisterParameter<int>("level");
    }

    public override IEnumerable<LoadEvent> LoadEvents(Painter painter)
    {
        yield return () =>
        {
            var canvas = new Canvas(1, 1);
            Client.Graphics.PushCanvas(canvas);
            
            painter.BeginSpriteBatch(SamplerState.PointWrap);
            painter.Clear(Color.White);
            painter.EndSpriteBatch();

            Client.Graphics.PopCanvas();

            return canvas.AsAsset("white-pixel");
        };
    }
}
