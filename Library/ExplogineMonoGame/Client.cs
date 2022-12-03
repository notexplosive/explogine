using System;
using System.IO;
using System.Reflection;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Debugging;
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
    private static CommandLineParameters commandLineParameters = new();

    internal static readonly CartridgeChain CartridgeChain = new();
    public static readonly OnceReady FinishedLoading = new();
    public static readonly OnceReady InitializedGraphics = new();
    public static readonly OnceReady Exited = new();
    public static bool IsInFocus => Client.Headless || Client.currentGame.IsActive;

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
    public static ClientFileSystem FileSystem { get; private set; } = new();

    /// <summary>
    ///     Wrapper for accessing the Window of your platform.
    /// </summary>
    public static PlatformAgnosticWindow Window { get; private set; } = new();

    /// <summary>
    ///     Args passed via command line
    /// </summary>
    public static CommandLineArguments Args => Client.commandLineParameters.Args;

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
    ///     Debug tools.
    /// </summary>
    public static ClientDebug Debug { get; } = new();

    /// <summary>
    ///     Gives access to Clean and Dirty random and noise.
    ///     Clean Random is seeded and can be globally set, anything you want to be reproducible should derive from Clean
    ///     Random.
    ///     Dirty Random has no guaranteed seed. Any time you just need "a random number" and don't care where it came from,
    ///     use Dirty Random.
    /// </summary>
    public static ClientRandom Random { get; } = new();

    /// <summary>
    ///     The Canvas that renders the actual game content to the screen.
    /// </summary>
    public static RenderCanvas RenderCanvas { get; } = new();

    private static ClientEssentials Essentials { get; } = new();

    public static string ContentBaseDirectory => "Content";

    public static string ContentFullPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Client.ContentBaseDirectory);

    public static string LocalFullPath => AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    ///     Indicates if we're in a Test-Only environment. (ie: Client.Start has not been run)
    ///     In Headless mode, we have no Window, no Assets, and no Graphics.
    /// </summary>
    public static bool Headless { get; private set; } = true;

    /// <summary>
    ///     Entrypoint for Platform (ie: Desktop)
    /// </summary>
    /// <param name="argsArray">Args passed via command line</param>
    /// <param name="windowConfig">Config object for client startup</param>
    /// <param name="gameCartridge">Cartridge for your game</param>
    /// <param name="platform">Platform plugin for your platform</param>
    public static void Start(string[] argsArray, WindowConfig windowConfig, ICartridge gameCartridge,
        IPlatformInterface platform)
    {
        // Setup Platform
        Client.Headless = false;
        Client.Window = platform.PlatformAgnosticWindow;
        Client.FileSystem =
            new ClientFileSystem(
                new RealFileSystem(AppDomain.CurrentDomain.BaseDirectory),
                new RealFileSystem(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "NotExplosive", Assembly.GetEntryAssembly()!.GetName().Name))
            );
        Client.startingConfig = windowConfig;

        // Setup Command Line
        Client.commandLineParameters = new CommandLineParameters(argsArray);
        Client.Essentials.AddCommandLineParameters(Client.commandLineParameters.Writer);
        Client.Essentials.ExecuteCommandLineArgs(Client.commandLineParameters.Args);

        var skipIntro = Client.commandLineParameters.Args.GetValue<bool>("skipIntro") ||
                        Client.Debug.LaunchedAsDebugMode();
        // Setup Cartridges
        if (!skipIntro)
        {
            Client.CartridgeChain.Append(new IntroCartridge());
        }

        Client.CartridgeChain.Append(gameCartridge);
        Client.CartridgeChain.AboutToLoadLastCartridge += Client.Demo.Begin;

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
        Client.Window.RenderResolutionChanged += Client.RenderCanvas.ResizeCanvas;
        Client.Window.Setup(game.Window, Client.startingConfig);

        Client.InitializedGraphics.BecomeReady();
    }

    internal static void LoadContent(ContentManager contentManager)
    {
        Client.loader = new Loader(contentManager);
        Client.loader.AddLoadEvents(Client.Demo);
        Client.loader.AddLoadEvents(Client.Essentials);
        Client.loader.AddLoadEvents(Client.CartridgeChain.GetAllCartridgesDerivedFrom<ILoadEventProvider>());
        Client.CartridgeChain.SetupLoadingCartridge(Client.loader);
        Client.CartridgeChain.ValidateParameters(Client.commandLineParameters.Writer);

        foreach (var arg in Client.commandLineParameters.Args.UnboundArgs())
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
        for (var i = 0; i < Client.Debug.GameSpeed; i++)
        {
            Client.Window.ResolveSetCursor();
            Client.HumanInput = Client.HumanInput.Next(InputSnapshot.Human);
            Client.Input = Client.Demo.ProcessInput(Client.Input);
            Client.CartridgeChain.Update(dt);
            Client.Window.TextEnteredBuffer = new TextEnteredBuffer();
        }
    }

    internal static void Draw()
    {
        Client.RenderCanvas.DrawWithin(painter => { Client.CartridgeChain.DrawCurrentCartridge(painter); });
        Client.RenderCanvas.Draw(Client.Graphics.Painter);
        Client.CartridgeChain.DrawDebugCartridge(Client.Graphics.Painter);
    }
}
