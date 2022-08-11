using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NotCore;

public class NotGame : Game
{
    private readonly GraphicsDeviceManager _graphics;

    public NotGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = Client.ContentBaseDirectory;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Client.Initialize(GraphicsDevice, _graphics);
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
        Client.UpdateInputState();
        Client.Update((float) gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Client.Draw();
        base.Draw(gameTime);
    }
}
