using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NotCore.Data;

namespace NotCore;

public class Painter
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    public Painter(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public void Clear(Color color)
    {
        _graphicsDevice.Clear(color);
    }

    public void BeginSpriteBatch()
    {
        _spriteBatch.Begin();
    }

    public void EndSpriteBatch()
    {
        _spriteBatch.End();
    }

    public void Draw(Texture2D texture, Vector2 position)
    {
        _spriteBatch.Draw(texture, position, texture.Bounds, Color.White);
    }

    public void Draw(Texture2D texture, Rectangle destRect, Rectangle sourceRect, Color tintColor, float angle, Vector2 offset, XyBool flip, Depth depth)
    {
        _spriteBatch.Draw(texture, destRect, sourceRect, tintColor, angle, offset,
            (flip.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
            (flip.Y ? SpriteEffects.FlipVertically : SpriteEffects.None), depth.AsFloat);
    }
}
