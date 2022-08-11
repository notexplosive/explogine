using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NotCore;

public static class Client
{
    private static Game currentGame = null!;
    private static Loader loader = null!;
    public static Graphics Graphics { get; private set; } = null!;
    private static readonly CartridgeChain CartridgeChain = new();
    public static InputState Input { get; private set; }
    public static IFileSystem FileSystem { get; private set; } = new EmptyFileSystem();
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

        foreach (var cartridge in Client.CartridgeChain.GetAll())
        {
            if (cartridge is not ICartridgeWithPreload preloadCartridge)
            {
                continue;
            }

            foreach (var preloadEvent in preloadCartridge.Preload(Client.Graphics.Painter))
            {
                Client.loader.AddDynamicLoadEvent(preloadEvent);
            }
        }
        
        Client.CartridgeChain.Prepend(new LoadingCartridge(Client.loader));
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
    /// <param name="gameCartridge">Cartridge for your game</param>
    /// <param name="fileSystem">FileSystem plugin for your platform</param>
    public static void Start(ICartridge gameCartridge, IFileSystem fileSystem)
    {
        Client.FileSystem = fileSystem;
        Client.CartridgeChain.Append(new IntroCartridge());
        Client.CartridgeChain.Append(gameCartridge);
        
        using var game = new NotGame();
        Client.currentGame = game;
        game.Run();
    }

    internal static void TriggerDoneLoading()
    {
        Client.ContentLoaded?.Invoke();
    }
}
