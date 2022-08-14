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
        Draw(texture, position, Vector2.One, new DrawSettings());
    }

    public void Draw(Texture2D texture, Rectangle destinationRectangle, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        _spriteBatch.Draw(texture, destinationRectangle, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin, settings.FlipEffect, settings.Depth);
    }

    public void Draw(Texture2D texture, Vector2 position, Vector2 scale, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        _spriteBatch.Draw(texture, position, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin, scale, settings.FlipEffect, settings.Depth);
    }
}

public struct DrawSettings
{
    public DrawSettings()
    {
        Depth = default;
        Angle = 0;
        Origin = default;
        Flip = default;
        Color = Color.White;
        SourceRectangle = null;
    }

    public Color Color { get; set; }
    public Rectangle? SourceRectangle { get; set; }
    public Depth Depth { get; set; }
    public float Angle { get; set; }
    public Vector2 Origin { get; set; }
    public XyBool Flip { get; set; }

    public SpriteEffects FlipEffect => (Flip.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                                       (Flip.Y ? SpriteEffects.FlipVertically : SpriteEffects.None);
}
