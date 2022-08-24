﻿using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SampleGame;

public class SampleGameCartridge : BasicGameCartridge
{
    private float _totalTime;

    protected override void OnStarted()
    {
        Console.WriteLine("Sample Cart Loaded");
    }

    public override void Update(float dt)
    {
        _totalTime += dt;

        // move to debug cart
        if (Client.Input.Keyboard.GetButton(Keys.P).WasPressed && !Client.DemoRecorder.IsPlaying)
        {
            Client.DemoRecorder.BeginPlayback();
        }
        
        if (Client.Input.Keyboard.GetButton(Keys.D).WasPressed && !Client.DemoRecorder.IsPlaying)
        {
            Client.DemoRecorder.DumpRecording();
        }
        // /move to debug cart
        
        if (Client.Input.Keyboard.GetButton(Keys.F).WasPressed)
        {
            Client.Window.SetFullscreen(!Client.Window.IsFullscreen);
        }
    }

    public override void Draw(Painter painter)
    {
        painter.Clear(Color.CornflowerBlue);
        painter.BeginSpriteBatch();
        painter.DrawAtPosition(Client.Assets.GetTexture("nesto/nesty/amongus"), new Vector2(100, _totalTime));
        painter.DrawAtPosition(Client.Assets.GetPreloadedObject<Canvas>("dynamic-asset").Texture, new Vector2(90, 90));
        painter.DrawAtPosition(Client.Assets.GetPreloadedObject<Canvas>("dynamic-asset2").Texture, new Vector2(150, 150));

        var player = Client.Assets.GetAsset<SpriteSheet>("player");
        painter.DrawAtPosition(Client.Assets.GetTexture("winking jack"), Client.Input.Mouse.Position, new Scale2D(0.25f),
            new DrawSettings {Origin = DrawOrigin.Center});

        var drawSettings = new DrawSettings
        {
            Angle = new Random().NextSingle(),
            Origin = DrawOrigin.Center
        };

        player.DrawFrame(painter, 0, Client.Input.Mouse.Position, Scale2D.One, drawSettings);

        painter.EndSpriteBatch();
    }

    public override void SetupFormalParameters(ParsedCommandLineArguments args)
    {
        args.RegisterParameter<int>("level");
    }

    public override IEnumerable<LoadEvent> LoadEvents(Painter painter)
    {
        yield return () => new GridBasedSpriteSheet("player", "player_new", new Point(32));
        yield return () =>
        {
            var canvas = new Canvas(25, 25);
            Client.Graphics.PushCanvas(canvas);
            painter.Clear(Color.Blue);
            Client.Graphics.PopCanvas();

            return canvas.AsAsset("dynamic-asset");
        };

        yield return () =>
        {
            var canvas = new Canvas(100, 100);
            Client.Graphics.PushCanvas(canvas);

            var canvas2 = new Canvas(10, 10);
            Client.Graphics.PushCanvas(canvas2);
            painter.Clear(Color.IndianRed);
            Client.Graphics.PopCanvas();

            var texture = canvas2.Texture;
            painter.BeginSpriteBatch();
            painter.Clear(Color.Transparent);
            painter.DrawAtPosition(texture, new Vector2(50, 50));
            painter.DrawAtPosition(texture, new Vector2(25, 50));
            painter.DrawAtPosition(texture, new Vector2(50, 25));
            painter.EndSpriteBatch();

            Client.Graphics.PopCanvas();

            return canvas.AsAsset("dynamic-asset2");
        };
    }
}