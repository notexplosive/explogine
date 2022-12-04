using Microsoft.Xna.Framework;
#if !DEBUG
using System;
#endif

namespace ExplogineMonoGame;

public class ExplogineGame : Game
{
    private readonly GraphicsDeviceManager _graphics;

    public ExplogineGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = Client.ContentBaseDirectory;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Client.Initialize(GraphicsDevice, _graphics, this);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Client.LoadContent(Content);
    }

    protected override void UnloadContent()
    {
        Client.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        void UpdateInternal()
        {
            Client.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }
#if DEBUG
        UpdateInternal();
#else
        try
        {
            UpdateInternal();
        }
        catch (Exception e)
        {
            Client.CartridgeChain.Crash(e);
        }
#endif
    }

    protected override void Draw(GameTime gameTime)
    {
        void DrawInternal()
        {
            Client.Draw();
            base.Draw(gameTime);
        }
#if DEBUG
        DrawInternal();
#else
        try
        {
            DrawInternal();
        }
        catch (Exception e)
        {
            Client.CartridgeChain.Crash(e);
        }
#endif
    }
}
