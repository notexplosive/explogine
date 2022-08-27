using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Debugging;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Logging;
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
    public static readonly OnceReady FinishedLoading = new();
    public static readonly OnceReady Exited = new();

    /// <summary>
    ///     Wrapper around the MonoGame Graphics objects (Device & DeviceManager)
    /// </summary>
    public static Graphics Graphics { get; private set; } = null!;

    /// <summary>
    ///     The current user input state for this frame, use this to get Keyboard, Mouse, or Gamepad input state.
    ///     This value is provided by either the human user pressing buttons, or by the Demo playback.
    /// </summary>
    public static InputFrameState Input { get; private set; }

    /// <summary>
    ///     Same as Client.Input, except it's unaffected to Demo Playback.
    ///     Prefer Client.Input if you don't understand/care about the difference.
    /// </summary>
    public static InputFrameState HumanInput { get; private set; }

    /// <summary>
    ///     Wrapper for accessing the Filesystem of your platform.
    /// </summary>
    public static IFileSystem FileSystem { get; private set; } = new EmptyFileSystem();

    /// <summary>
    ///     Wrapper for accessing the Window of your platform.
    /// </summary>
    public static IWindow Window { get; private set; } = null!;

    /// <summary>
    ///     The Args passed via command line.
    /// </summary>
    public static ParsedCommandLineArguments ParsedCommandLineArguments { get; private set; } = new();

    /// <summary>
    ///     Gives you access to static Assets (aka: Content), as well as dynamic assets.
    /// </summary>
    public static Assets Assets { get; } = new();

    /// <summary>
    ///     Convenient way to play one-off sounds.
    /// </summary>
    public static SoundPlayer SoundPlayer { get; } = new();

    /// <summary>
    ///     Demo Recorder/Playback.
    /// </summary>
    public static Demo Demo { get; } = new();

    /// <summary>
    ///     HitTest stack that keeps track of all HitTestTargets registered this frame.
    /// </summary>
    public static HitTestStack HitTesting { get; } = new();

    /// <summary>
    ///     Debug tools.
    /// </summary>
    public static ClientDebug Debug { get; } = new();
    
    /// <summary>
    ///     The Canvas that renders the actual game content to the screen.
    /// </summary>
    public static RenderCanvas RenderCanvas { get; } = new();

    public static string ContentBaseDirectory => "Content";

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
        // Setup Platform
        Client.Window = platform.Window;
        Client.FileSystem = platform.FileSystem;
        Client.startingConfig = windowConfig;

        // Setup Logging
        Client.Debug.Output.PushToStack(new ConsoleLogCapture());
        Client.Debug.Output.AddParallel(new FileLogCapture());

        // Setup Cartridges
        Client.CartridgeChain.Append(new IntroCartridge());
        Client.CartridgeChain.Append(gameCartridge);
        Client.CartridgeChain.LoadedLastCartridge += Client.Demo.OnStartup;

        // Setup Command Line
        Client.ParsedCommandLineArguments = new ParsedCommandLineArguments(args);

        // Setup Game
        using var game = new NotGame();
        Client.currentGame = game;

        // Setup Exit Handler
        Client.currentGame.Exiting += (_, _) => { Client.Exited.BecomeReady(); };

        // Launch
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
        Client.RenderCanvas.Setup();
        Client.Window.Resized += RenderCanvas.OnWindowResized;
        Client.Window.Setup(game.Window, Client.startingConfig);
    }

    internal static void LoadContent(ContentManager contentManager)
    {
        Client.loader = new Loader(contentManager);
        Client.CartridgeChain.SetupLoadingCartridge(Client.loader);
        Client.CartridgeChain.ValidateParameters(Client.ParsedCommandLineArguments);

        foreach (var arg in Client.ParsedCommandLineArguments.UnboundArgs())
        {
            Client.Debug.Log($"Was passed unregistered arg: {arg}");
        }

        Client.Input = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        Client.HumanInput = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
    }

    internal static void UnloadContent()
    {
        Client.Assets.UnloadAll();
    }

    internal static void Update(float dt)
    {
        Client.HitTesting.Clear();
        Client.HumanInput = Client.HumanInput.Next(InputSnapshot.Human);
        Client.Input = Client.Demo.ProcessInput(Client.Input);
        Client.CartridgeChain.Update(dt);
        Client.HitTesting.Resolve(Client.Input.Mouse.Position(Client.RenderCanvas.ScreenToCanvas));
    }

    internal static void Draw()
    {
        Client.RenderCanvas.DrawWithin(painter =>
        {
            Client.CartridgeChain.Draw(painter);
        });

        Client.RenderCanvas.Draw(Client.Graphics.Painter);
    }
}
