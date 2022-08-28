using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

internal class ClientEssentials : ICommandLineParameterProvider, ILoadEventProvider
{
    public void SetupFormalParameters(CommandLineParameters parameters)
    {
        parameters.RegisterParameter<int>("randomSeed");
        parameters.RegisterParameter<bool>("fullscreen");
        parameters.RegisterParameter<string>("demo");
        parameters.RegisterParameter<bool>("help");
    }

    public void ExecuteCommandLineArgs(CommandLineArguments args)
    {
        if (args.HasValue("randomSeed"))
        {
            Client.Random.Seed = args.GetValue<int>("randomSeed");
        }
        else
        {
            Client.Random.Seed = (int) DateTime.Now.ToFileTimeUtc();
        }

        if (args.GetValue<bool>("fullscreen"))
        {
            Client.InitializedGraphics.Add(() => Client.Window.SetFullscreen(true));
        }

        if (args.GetValue<bool>("help"))
        {
            Client.Debug.Log(args.HelpOutput());
        }
    }

    public IEnumerable<LoadEvent> LoadEvents(Painter painter)
    {
        yield return new LoadEvent("white-pixel", () =>
        {
            var canvas = new Canvas(1, 1);
            Client.Graphics.PushCanvas(canvas);

            painter.BeginSpriteBatch(SamplerState.PointWrap);
            painter.Clear(Color.White);
            painter.EndSpriteBatch();

            Client.Graphics.PopCanvas();

            return canvas.AsTextureAsset();
        });
    }

    public void SetupLoadEvents(Loader loader)
    {
        loader.AddLoadEvents(this);
    }
}
