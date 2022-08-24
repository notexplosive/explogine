using System;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public static class Client
{
    private static Game currentGame = null!;
    private static Loader loader = null!;
    private static WindowConfig startingConfig;
    private static readonly CartridgeChain CartridgeChain = new();
    public static Graphics Graphics { get; private set; } = null!;
    public static InputFrameState Input { get; private set; }
    public static IFileSystem FileSystem { get; private set; } = new EmptyFileSystem();
    public static IWindow Window { get; private set; } = null!;
    public static ParsedCommandLineArguments ParsedCommandLineArguments { get; private set; } = new();
    public static Assets Assets { get; } = new();
    public static SoundPlayer SoundPlayer { get; } = new();
    public static Demo DemoRecorder { get; } = new();
    private static bool IsReady { get; set; }
    public static string ContentBaseDirectory => "Content";
    public static readonly When Initialized = new();
    public static readonly When FinishedLoading = new();

    /// <summary>
    ///     Entrypoint for Platform (ie: Desktop)
    /// </summary>
    /// <param name="args">Args passed via command line</param>
    /// <param name="windowConfig">Config object for client startup</param>
    /// <param name="gameCartridge">Cartridge for your game</param>
    /// <param name="platform">Platform plugin for your platform</param>
    public static void Start(string[] args, WindowConfig windowConfig, ICartridge gameCartridge,
        IPlatformInterface platform)
    {
        Client.Window = platform.Window;
        Client.FileSystem = platform.FileSystem;
        Client.CartridgeChain.Append(new IntroCartridge());
        Client.CartridgeChain.Append(gameCartridge);
        Client.ParsedCommandLineArguments = new ParsedCommandLineArguments(args);

        // We don't use the property here on purpose, Client isn't ready yet.
        Client.startingConfig = windowConfig;

        using var game = new NotGame();
        Client.currentGame = game;
        game.Run();
    }

    public static void Exit()
    {
        Client.currentGame.Exit();
    }

    internal static void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, Game game)
    {
        Client.Graphics = new Graphics(graphics, graphicsDevice);
        Client.Window.Setup(game.Window, Client.startingConfig);
        Client.Initialized.Invoke();
    }

    internal static void LoadContent(ContentManager contentManager)
    {
        Client.loader = new Loader(contentManager);
        Client.CartridgeChain.ForeachPreload(loadEvent => Client.loader.AddDynamicLoadEvent(loadEvent));
        Client.CartridgeChain.Prepend(new LoadingCartridge(Client.loader));

        Client.CartridgeChain.ValidateParameters(Client.ParsedCommandLineArguments);
        foreach (var arg in Client.ParsedCommandLineArguments.UnboundArgs())
        {
            Console.WriteLine($"Unknown arg: {arg}");
        }

        Client.Input = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        Client.IsReady = true;
    }

    internal static void UnloadContent()
    {
        Client.loader.Unload();
        Client.Assets.UnloadAllDynamicContent();
    }

    internal static void Update(float dt)
    {
        if (Client.DemoRecorder.IsPlaying)
        {
            var state = Client.DemoRecorder.GetNextRecordedState();
            Client.Input = Client.Input.Next(state);
        }
        else
        {
            var humanState = InputSnapshot.Human;
            if (Client.DemoRecorder.IsRecording)
            {
                Client.DemoRecorder.AddRecord(humanState);
            }

            Client.Input = Client.Input.Next(humanState);
        }

        Client.CartridgeChain.Update(dt);
    }

    internal static void Draw()
    {
        Client.CartridgeChain.Draw(Client.Graphics.Painter);
    }

    internal static void TriggerDoneLoading()
    {
        Client.IsReady = true;
        Client.FinishedLoading.Invoke();
    }
}
