﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NotCore;
using NotCore.AssetManagement;
using NotCore.Cartridges;
using NotCore.Data;

namespace SampleGame;

public class SampleCartridge : SimpleGameCartridge
{
    private float _totalTime;

    protected override void Load()
    {
        Console.WriteLine("Sample Cart Loaded");
    }

    public override void Update(float dt)
    {
        _totalTime += dt;
    }

    public override void Draw(Painter painter)
    {
        painter.Clear(Color.CornflowerBlue);
        painter.BeginSpriteBatch();
        painter.Draw(Client.Assets.GetTexture("nesto/nesty/amongus"), new Vector2(100, _totalTime));
        painter.Draw(Client.Assets.GetPreloadedObject<Canvas>("dynamic-asset").Texture, new Vector2(90, 90));
        painter.Draw(Client.Assets.GetPreloadedObject<Canvas>("dynamic-asset2").Texture, new Vector2(150, 150));

        var player = Client.Assets.GetAsset<SpriteSheet>("player");
        player.DrawFrame(painter, 0, Client.Input.Mouse.Position, 1f, new Random().NextSingle(), new XyBool(),
            Depth.Max, Color.White);

        painter.Draw(Client.Assets.GetTexture("winking jack"), Client.Input.Mouse.Position, new Vector2(0.25f), new DrawSettings {Origin = DrawOrigin.Center});

        painter.EndSpriteBatch();
    }

    public override void SetupFormalParameters(CommandLineArguments args)
    {
        args.AddParameter<int>("level");
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
            painter.Draw(texture, new Vector2(50, 50));
            painter.Draw(texture, new Vector2(25, 50));
            painter.Draw(texture, new Vector2(50, 25));
            painter.EndSpriteBatch();

            Client.Graphics.PopCanvas();

            return canvas.AsAsset("dynamic-asset2");
        };
    }
}
