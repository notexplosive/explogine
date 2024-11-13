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

    internal static RealWindow PlatformWindow => (Runtime.Window as RealWindow)!;
    internal static bool IsInFocus => Headless || currentGame.IsActive;

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
    public static CommandLineArguments Args => commandLineParameters.Args;

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

    /// <summary>
    ///     Fully qualified path of the directory the executable lives in.
    /// </summary>
    public static string LocalFullPath => AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    ///     Fully qualified path of our subfolder in the "AppData" directory (or whatever this platform's equivalent is)
    /// </summary>
    public static string AppDataFullPath =>
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NotExplosive",
            Assembly.GetEntryAssembly()!.GetName().Name);

    /// <summary>
    ///     Indicates if we're in a Test-Only environment. (ie: Client.Start has not been run)
    ///     In Headless mode, we have no Window, no Assets, and no Graphics.
    /// </summary>
    public static bool Headless { get; private set; } = true;

    public static float TotalElapsedTime { get; private set; }

    public static void HeadlessStart(string[] argsArray)
    {
        // Setup Command Line
        commandLineParameters = new CommandLineParameters(argsArray);
        Essentials.AddCommandLineParameters(commandLineParameters.Writer);
        Essentials.ExecuteCommandLineArgs(commandLineParameters.Args);
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
        HeadlessStart(argsArray);
        Debug.LogVerbose("Headless start completed");

        // Setup Platform
        Headless = false;

        var window = platform.PlatformWindow;
        var localFileSystem = new RealFileSystem(LocalFullPath);

        if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.MacOs && PlatformApi.IsAppBundle)
        {
            // If we're a macOS App Bundle we should point to the "Resources" directory
            localFileSystem = new RealFileSystem($"{LocalFullPath}/../Resources");
        }

        var appdataFileSystem = new RealFileSystem(AppDataFullPath);
        var fileSystem = new ClientFileSystem(localFileSystem, appdataFileSystem);
        Runtime.Setup(window, fileSystem);
        startingConfig = windowConfig;

        var skipIntro = commandLineParameters.Args.GetValue<bool>("skipIntro") ||
                        Debug.LaunchedAsDebugMode();
        // Setup Cartridges
        if (!skipIntro)
        {
            CartridgeChain.Append(new IntroCartridge(Runtime, "NotExplosive.net",
                Random.Dirty.NextUInt(), 0.25f));
        }

        // Don't plug in the game cartridge until we're initialized
        InitializedGraphics.Add(() =>
            CartridgeChain.AppendGameCartridge(gameCartridgeCreator(Runtime)));
        CartridgeChain.AboutToLoadLastCartridge += Demo.Begin;

        // Setup Game
        SafeRun(() =>
        {
            using var game = new ExplogineGame();
            currentGame = game;

            // Setup Exit Handler
            currentGame.Exiting += (_, _) =>
            {
                Debug.LogVerbose("Exited gracefully (running exit hooks)");
                Exited.BecomeReady();
            };

            // Launch
            // -- No code beyond this point will be run - game.Run() initiates the game loop -- //
            Debug.LogVerbose("Running game");
            game.Run();
        });
    }

    private static void SafeRun(Action function)
    {
#if DEBUG
        function();
#else
        try
        {
            function();
        }
        catch(Exception e)
        {
            Client.Debug.LogVerbose("ERROR: " + e.Message, $"\n{e.StackTrace}");
        }
#endif
    }

    public static void Exit()
    {
        currentGame.Exit();
    }

    internal static void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, Game game)
    {
        Graphics = new Graphics(graphics, graphicsDevice);
        PlatformWindow.Setup(game.Window, startingConfig);

        InitializedGraphics.BecomeReady();
    }

    internal static void LoadContent(ContentManager contentManager)
    {
        loader = new Loader(Runtime, contentManager);
        loader.AddLoadEvents(Demo);
        loader.AddLoadEvents(Essentials);
        loader.AddLoadEvents(CartridgeChain.GetAllCartridgesDerivedFrom<ILoadEventProvider>());
        CartridgeChain.SetupLoadingCartridge(loader);
        CartridgeChain.ValidateParameters(commandLineParameters.Writer);

        foreach (var arg in commandLineParameters.Args.UnboundArgs())
        {
            Debug.LogWarning($"Was passed unregistered arg: {arg}");
        }

        Input = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        HumanInput = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
    }

    internal static void UnloadContent()
    {
        Assets.UnloadAll();
    }

    internal static void Update(float dt)
    {
        for (var i = 0; i < Debug.GameSpeed; i++)
        {
            HumanInput = HumanInput.Next(InputSnapshot.Human);
            Input = Demo.ProcessInput(Input);
            var hitTest = new HitTestRoot();
            CartridgeChain.UpdateInput(new ConsumableInput(Input), hitTest.BaseStack);
            hitTest.Resolve(Input.Mouse.Position());
            CartridgeChain.Update(dt);
            PlatformWindow.TextEnteredBuffer = new TextEnteredBuffer();
            Cursor.Resolve();

            TotalElapsedTime += dt;
        }
    }

    internal static void Draw()
    {
        Graphics.PushCanvas(PlatformWindow.ClientCanvas.Internal);
        Graphics.Painter.Clear(Color.Black);
        CartridgeChain.DrawCurrentCartridge(Graphics.Painter);
        Graphics.PopCanvas();

        CartridgeChain.PrepareDebugCartridge(Graphics.Painter);

        PlatformWindow.ClientCanvas.Draw(Graphics.Painter);
        CartridgeChain.DrawDebugCartridge(Graphics.Painter);
    }
}
