﻿using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NotCore;

namespace SampleGame;


public class SampleGameCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    private float _totalTime;

    public void Update(float dt)
    {
        _totalTime += dt;

        var demoVal = Client.CommandLineArguments.GetCommandLineValue<CommandLineString>("demo");
        var snazzinessVal = Client.CommandLineArguments.GetCommandLineValue<CommandLineBool>("snazziness");
    }

    public void Draw(Painter painter)
    {
        painter.Clear(Color.CornflowerBlue);
        painter.BeginSpriteBatch();
        painter.Draw(Client.Assets.GetTexture("nesto/nesty/amongus"), new Vector2(100, _totalTime));
        painter.Draw(Client.Assets.GetTexture("winking jack"), new Vector2(_totalTime, 100));
        painter.Draw(Client.Assets.GetDynamicAsset<Canvas>("dynamic-asset").Texture, new Vector2(90, 90));
        painter.Draw(Client.Assets.GetDynamicAsset<Canvas>("dynamic-asset2").Texture, new Vector2(150, 150));
        painter.EndSpriteBatch();
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public IEnumerable<Loader.LoadEvent> LoadEvents(Painter painter)
    {
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

    public IEnumerable<ICommandLineParameter> GetFormalParameters()
    {
        yield return new CommandLineBool("snazziness");
    }
}
