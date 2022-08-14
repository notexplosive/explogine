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
        Draw(texture, new PositionDrawSettings {Position = position});
    }

    public void Draw(Texture2D texture, RectangleDrawSettings settings)
    {
        _spriteBatch.Draw(texture, settings.DestinationRect, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin, settings.FlipEffect, settings.Depth);
    }

    public void Draw(Texture2D texture, PositionDrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        
        _spriteBatch.Draw(texture, settings.Position, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin, settings.Scale, settings.FlipEffect, settings.Depth);
    }
}

public interface IDrawSettings
{
    Color Color { get; set; }
    public Depth Depth { get; set; }
    public float Angle { get; set; }
    public Vector2 Origin { get; set; }
    public XyBool Flip { get; set; }
}

public struct PositionDrawSettings : IDrawSettings
{
    public PositionDrawSettings()
    {
        SourceRectangle = default;
        Depth = default;
        Angle = default;
        Origin = default;
        Flip = new XyBool();
        Position = default;
        Color = Color.White;
    }

    public Color Color { get; set; }
    public Rectangle? SourceRectangle { get; set; }
    public Depth Depth { get; set; }
    public float Angle { get; set; }
    public Vector2 Origin { get; set; }
    public XyBool Flip { get; set; }
    public Vector2 Position { get; set; }

    public SpriteEffects FlipEffect => (Flip.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                                       (Flip.Y ? SpriteEffects.FlipVertically : SpriteEffects.None);

    public Vector2 Scale { get; set; } = Vector2.One;
}

public struct RectangleDrawSettings : IDrawSettings
{
    public RectangleDrawSettings()
    {
        DestinationRect = default;
        SourceRectangle = default;
        Depth = default;
        Angle = default;
        Origin = default;
        Flip = new XyBool();
        Color = Color.White;
    }

    public Rectangle DestinationRect { get; set; }
    public Color Color { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public Depth Depth { get; set; }
    public float Angle { get; set; }
    public Vector2 Origin { get; set; }
    public XyBool Flip { get; set; }

    public SpriteEffects FlipEffect => (Flip.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                                       (Flip.Y ? SpriteEffects.FlipVertically : SpriteEffects.None);
}
