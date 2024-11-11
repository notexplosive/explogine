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
    // The `OnceReady` initialization needs to happen at the top, other static initializers depend on these
    public static readonly OnceReady FinishedLoading = new();
    public static readonly OnceReady InitializedGraphics = new();

    public static readonly OnceReady Exited = new();
    //

    private static Game currentGame = null!;
    private static Loader loader = null!;
    private static WindowConfig startingConfig;
    private static CommandLineParameters commandLineParameters = new();
    internal static readonly ClientRuntime Runtime = new();
    internal static readonly CartridgeChain CartridgeChain = new();

    internal static RealWindow PlatformWindow => (Client.Runtime.Window as RealWindow)!;
    internal static bool IsInFocus => Client.Headless || Client.currentGame.IsActive;

    /// <summary>
    ///     Wrapper around the MonoGame Graphics objects (Device and DeviceManager)
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
    ///     Controls the state of the hardware cursor (particularly: the graphic used to display the cursor itself)
    ///     If you want to get the Position of the cursor, use Client.Input instead.
    /// </summary>
    public static HardwareCursor Cursor { get; } = new();

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

    public static float TotalElapsedTime { get; private set; }

    public static void HeadlessStart(string[] argsArray)
    {
        // Setup Command Line
        Client.commandLineParameters = new CommandLineParameters(argsArray);
        Client.Essentials.AddCommandLineParameters(Client.commandLineParameters.Writer);
        Client.Essentials.ExecuteCommandLineArgs(Client.commandLineParameters.Args);
    }

    /// <summary>
    ///     Entrypoint for Platform (ie: Desktop), this is called automatically and should not be called in your code
    /// </summary>
    /// <param name="argsArray">Args passed via command line</param>
    /// <param name="windowConfig">Config object for client startup</param>
    /// <param name="gameCartridgeCreator">A method that will return the cartridge for your game</param>
    /// <param name="platform">Platform plugin for your platform</param>
    public static void Start(string[] argsArray, WindowConfig windowConfig,
        Func<IRuntime, Cartridge> gameCartridgeCreator,
        IPlatformInterface platform)
    {
        Client.HeadlessStart(argsArray);
        Client.Debug.LogVerbose("Headless start completed");
        
        // Setup Platform
        Client.Headless = false;
        
        var window = platform.PlatformWindow;
        var fileSystem = new ClientFileSystem(
            new RealFileSystem(AppDomain.CurrentDomain.BaseDirectory),
            new RealFileSystem(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NotExplosive", Assembly.GetEntryAssembly()!.GetName().Name))
        );
        Client.Runtime.Setup(window, fileSystem);
        Client.startingConfig = windowConfig;

        var skipIntro = Client.commandLineParameters.Args.GetValue<bool>("skipIntro") ||
                        Client.Debug.LaunchedAsDebugMode();
        // Setup Cartridges
        if (!skipIntro)
        {
            Client.CartridgeChain.Append(new IntroCartridge(Client.Runtime, "NotExplosive.net",
                Client.Random.Dirty.NextUInt(), 0.25f));
        }

        // Don't plug in the game cartridge until we're initialized
        Client.InitializedGraphics.Add(() =>
            Client.CartridgeChain.AppendGameCartridge(gameCartridgeCreator(Client.Runtime)));
        Client.CartridgeChain.AboutToLoadLastCartridge += Client.Demo.Begin;

        // Setup Game
        Client.Debug.LogVerbose("Creating Game");
        try
        {
            var game = new ExplogineGame();

            Client.Debug.LogVerbose("Game Created");
        Client.currentGame = game;

        // Setup Exit Handler
            Client.Debug.LogVerbose("Wiring up Exit Handlers");
            Client.currentGame.Exiting += (_, _) =>
            {
                Client.Debug.LogVerbose("Exited gracefully (running exit hooks)");
                Client.Exited.BecomeReady();
            };

        // Launch
        // -- No code beyond this point will be run - game.Run() initiates the game loop -- //
            Client.Debug.LogVerbose("Running game");
        game.Run();
            game.Dispose();
        }
        catch(Exception e)
        {
            Client.Debug.LogVerbose("ERROR: " + e.Message, $"\n{e.StackTrace}");
        }
    }

    public static void Exit()
    {
        Client.currentGame.Exit();
    }

    internal static void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, Game game)
    {
        Client.Graphics = new Graphics(graphics, graphicsDevice);
        Client.PlatformWindow.Setup(game.Window, Client.startingConfig);

        Client.InitializedGraphics.BecomeReady();
    }

    internal static void LoadContent(ContentManager contentManager)
    {
        Client.loader = new Loader(Client.Runtime, contentManager);
        Client.loader.AddLoadEvents(Client.Demo);
        Client.loader.AddLoadEvents(Client.Essentials);
        Client.loader.AddLoadEvents(Client.CartridgeChain.GetAllCartridgesDerivedFrom<ILoadEventProvider>());
        Client.CartridgeChain.SetupLoadingCartridge(Client.loader);
        Client.CartridgeChain.ValidateParameters(Client.commandLineParameters.Writer);

        foreach (var arg in Client.commandLineParameters.Args.UnboundArgs())
        {
            Client.Debug.LogWarning($"Was passed unregistered arg: {arg}");
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
            Client.HumanInput = Client.HumanInput.Next(InputSnapshot.Human);
            Client.Input = Client.Demo.ProcessInput(Client.Input);
            var hitTest = new HitTestRoot();
            Client.CartridgeChain.UpdateInput(new ConsumableInput(Client.Input), hitTest.BaseStack);
            hitTest.Resolve(Client.Input.Mouse.Position());
            Client.CartridgeChain.Update(dt);
            Client.PlatformWindow.TextEnteredBuffer = new TextEnteredBuffer();
            Client.Cursor.Resolve();
            
            TotalElapsedTime += dt;
        }

    }

    internal static void Draw()
    {
        Client.Graphics.PushCanvas(Client.PlatformWindow.ClientCanvas.Internal);
        Client.Graphics.Painter.Clear(Color.Black);
        Client.CartridgeChain.DrawCurrentCartridge(Client.Graphics.Painter);
        Client.Graphics.PopCanvas();

        Client.CartridgeChain.PrepareDebugCartridge(Client.Graphics.Painter);

        Client.PlatformWindow.ClientCanvas.Draw(Client.Graphics.Painter);
        Client.CartridgeChain.DrawDebugCartridge(Client.Graphics.Painter);
    }
}
