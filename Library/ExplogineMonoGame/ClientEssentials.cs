using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

internal class ClientEssentials : ICommandLineParameterProvider, ILoadEventProvider
{
    private readonly IApp _app;

    public ClientEssentials(IApp app)
    {
        _app = app;
    }
    
    public void AddCommandLineParameters(CommandLineParametersWriter parameters)
    {
        parameters.RegisterParameter<int>("randomSeed");
        parameters.RegisterParameter<bool>("fullscreen");
        parameters.RegisterParameter<string>("demo");
        parameters.RegisterParameter<int>("gameSpeed");
        parameters.RegisterParameter<bool>("help");
        parameters.RegisterParameter<bool>("skipIntro");
        parameters.RegisterParameter<bool>("debug");
        parameters.RegisterParameter<bool>("skipSnapshot");
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
            Client.InitializedGraphics.Add(() => _app.Window.SetFullscreen(true));
        }

        if (args.GetValue<bool>("help"))
        {
            Client.Debug.Log(args.HelpOutput());
        }

        if (args.GetValue<int>("gameSpeed") > 0)
        {
            Client.Debug.GameSpeed = args.GetValue<int>("gameSpeed");
        }
    }

    public IEnumerable<ILoadEvent?> LoadEvents(Painter painter)
    {
        yield return new AssetLoadEvent("white-pixel", "Engine Tools", () =>
        {
            var canvas = new Canvas(1, 1);
            Client.Graphics.PushCanvas(canvas);

            painter.BeginSpriteBatch();
            painter.Clear(Color.White);
            painter.EndSpriteBatch();

            Client.Graphics.PopCanvas();

            return canvas.AsTextureAsset();
        });
    }
}
