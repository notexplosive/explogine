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
    public static InputFrameState HumanInput { get; private set; }
    public static IFileSystem FileSystem { get; private set; } = new EmptyFileSystem();
    public static IWindow Window { get; private set; } = null!;
    public static ParsedCommandLineArguments ParsedCommandLineArguments { get; private set; } = new();
    public static Assets Assets { get; } = new();
    public static SoundPlayer SoundPlayer { get; } = new();
    public static Demo Demo { get; } = new();
    public static ClientDebug Debug { get; } = new();
    public static string ContentBaseDirectory => "Content";
    public static readonly OnceReady FinishedLoading = new();

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
        Client.startingConfig = windowConfig;
        Client.CartridgeChain.LoadedLastCartridge += Client.Demo.OnStartup;

        using var game = new NotGame();
        Client.currentGame = game;
        
        // -- No code beyond this point will be run - game.Run() initiates the game loop -- //
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
    }

    internal static void LoadContent(ContentManager contentManager)
    {
        Client.loader = new Loader(contentManager);
        Client.CartridgeChain.SetupLoadingCartridge(Client.loader);
        Client.CartridgeChain.ValidateParameters(Client.ParsedCommandLineArguments);

        foreach (var arg in Client.ParsedCommandLineArguments.UnboundArgs())
        {
            Console.WriteLine($"Was passed unregistered arg: {arg}");
        }

        Client.Input = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        Client.HumanInput = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
    }

    internal static void UnloadContent()
    {
        Client.loader.Unload();
        Client.Assets.UnloadAllDynamicContent();
    }

    internal static void Update(float dt)
    {
        Client.HumanInput = Client.HumanInput.Next(InputSnapshot.Human);
        Client.Input = Client.Demo.ProcessInput(Client.Input);
        Client.CartridgeChain.Update(dt);
    }

    internal static void Draw()
    {
        Client.CartridgeChain.Draw(Client.Graphics.Painter);
    }
}
