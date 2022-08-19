using System;
using ExplogineCore;
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
    private static readonly CartridgeChain CartridgeChain = new();
    public static Graphics Graphics { get; private set; } = null!;
    public static InputFrameState Input { get; private set; }
    public static IFileSystem FileSystem { get; private set; } = new EmptyFileSystem();
    public static ParsedCommandLineArguments ParsedCommandLineArguments { get; private set; } = new();
    public static Assets Assets { get; } = new();
    public static SoundPlayer SoundPlayer { get; } = new();
    public static Demo DemoRecorder { get; } = new();

    private static bool IsReady { get; set; }

    public static string ContentBaseDirectory => "Content";

    public static event Action? Readied;

    /// <summary>
    ///     Entrypoint for Platform (ie: Desktop)
    /// </summary>
    /// <param name="args">Args passed via command line</param>
    /// <param name="gameCartridge">Cartridge for your game</param>
    /// <param name="fileSystem">FileSystem plugin for your platform</param>
    public static void Start(string[] args, ICartridge gameCartridge, IFileSystem fileSystem)
    {
        Client.FileSystem = fileSystem;
        Client.CartridgeChain.Append(new IntroCartridge());
        Client.CartridgeChain.Append(gameCartridge);
        Client.ParsedCommandLineArguments = new ParsedCommandLineArguments(args);

        using var game = new NotGame();
        Client.currentGame = game;
        game.Run();
    }

    public static void RunWhenReady(Action action)
    {
        if (Client.IsReady)
        {
            action();
        }
        else
        {
            Client.Readied += action;
        }
    }

    private static void RunAndClearReadyEvents()
    {
        Client.Readied?.Invoke();
        Client.Readied = null;
    }

    public static void Exit()
    {
        Client.currentGame.Exit();
    }

    internal static void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
        Client.Graphics = new Graphics(graphics, graphicsDevice);
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
        Client.RunAndClearReadyEvents();
    }
}
