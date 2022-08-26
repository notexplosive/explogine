using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

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

    public void BeginSpriteBatch(SamplerState samplerState)
    {
        _spriteBatch.Begin(SpriteSortMode.BackToFront, null, samplerState, null, null, null, null);
    }

    public void EndSpriteBatch()
    {
        _spriteBatch.End();
    }

    public void DrawAsRectangle(Texture2D texture, Rectangle destinationRectangle)
    {
        DrawAsRectangle(texture, destinationRectangle, new DrawSettings());
    }

    public void DrawAtPosition(Texture2D texture, Vector2 position)
    {
        DrawAtPosition(texture, position, Scale2D.One, new DrawSettings());
    }

    public void DrawAsRectangle(Texture2D texture, Rectangle destinationRectangle, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        _spriteBatch.Draw(texture, destinationRectangle, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin.Value(destinationRectangle.Size), settings.FlipEffect, settings.Depth);
    }

    public void DrawAtPosition(Texture2D texture, Vector2 position, Scale2D scale2D, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        _spriteBatch.Draw(texture, position, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin.Value(texture.Bounds.Size), scale2D.Value, settings.FlipEffect, settings.Depth);
    }

    public void DrawString(SpriteFont font, string text, Point position, Scale2D scale2D, DrawSettings settings)
    {
        _spriteBatch.DrawString(font, text, position.ToVector2(), settings.Color, settings.Angle, settings.Origin.Value(Point.Zero),
            scale2D.Value, settings.FlipEffect, settings.Depth);
    }
}
