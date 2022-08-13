using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NotCore;

public static class Client
{
    private static Game currentGame = null!;
    private static Loader loader = null!;
    private static readonly CartridgeChain CartridgeChain = new();
    public static Graphics Graphics { get; private set; } = null!;
    public static InputState Input { get; private set; }
    public static IFileSystem FileSystem { get; private set; } = new EmptyFileSystem();
    public static CommandLineArguments CommandLineArguments { get; private set; } = new();
    public static Assets Assets { get; } = new();
    public static SoundPlayer SoundPlayer { get; } = new();

    public static string ContentBaseDirectory => "Content";

    public static event Action? Initialized;
    public static event Action? ContentLoaded;

    public static void Exit()
    {
        Client.currentGame.Exit();
    }

    internal static void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
        Client.Graphics = new Graphics(graphics, graphicsDevice);
        Client.Initialized?.Invoke();
    }

    internal static void LoadContent(ContentManager contentManager)
    {
        Client.loader = new Loader(contentManager);
        Client.CartridgeChain.ForeachPreload(loadEvent => Client.loader.AddDynamicLoadEvent(loadEvent));
        Client.CartridgeChain.Prepend(new LoadingCartridge(Client.loader));

        Client.CartridgeChain.ValidateParameters(Client.CommandLineArguments);
        foreach (var arg in Client.CommandLineArguments.UnboundArgs())
        {
            Console.WriteLine($"Unknown arg: {arg}");
        }
    }

    internal static void UnloadContent()
    {
        Client.loader.Unload();
        Client.Assets.UnloadAllDynamicContent();
    }

    internal static void UpdateInputState()
    {
        Client.Input = InputState.ComputeHumanInputState(Client.Input);
    }

    internal static void Update(float dt)
    {
        Client.CartridgeChain.Update(dt);
    }

    internal static void Draw()
    {
        Client.CartridgeChain.Draw(Client.Graphics.Painter);
    }

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
        Client.CommandLineArguments = new CommandLineArguments(args);

        using var game = new NotGame();
        Client.currentGame = game;
        game.Run();
    }

    internal static void TriggerDoneLoading()
    {
        Client.ContentLoaded?.Invoke();
    }
}
