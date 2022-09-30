using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public class Painter
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private Texture2D? _pixelAsset;
    private bool _spriteBatchIsInProgress;

    public Painter(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public Texture2D PixelAsset => _pixelAsset ??= Client.Assets.GetTexture("white-pixel");

    public void Clear(Color color)
    {
        _graphicsDevice.Clear(color);
    }

    public void BeginSpriteBatch(SamplerState samplerState)
    {
        BeginSpriteBatch(samplerState, Matrix.Identity);
    }

    public void BeginSpriteBatch(SamplerState samplerState, Matrix matrix)
    {
        _spriteBatch.Begin(SpriteSortMode.BackToFront, null, samplerState, null, null, null, matrix);
        _spriteBatchIsInProgress = true;
    }

    public void EndSpriteBatch()
    {
        _spriteBatch.End();
        _spriteBatchIsInProgress = false;
    }

    /// <summary>
    ///     Only to be used by the CrashCartridge if we threw an exception
    /// </summary>
    internal void ResetToCleanState()
    {
        if (_spriteBatchIsInProgress)
        {
            EndSpriteBatch();
        }

        while (!Client.Graphics.IsAtTopCanvas())
        {
            Client.Graphics.PopCanvas();
        }
    }

    public void DrawRectangle(Rectangle rectangle, DrawSettings drawSettings)
    {
        DrawAsRectangle(PixelAsset, rectangle, drawSettings);
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
        var origin = settings.Origin.Value(destinationRectangle.Size);
        var size = destinationRectangle.Size;

        origin.X /= size.X;
        origin.Y /= size.Y;
        
        _spriteBatch.Draw(texture, destinationRectangle, settings.SourceRectangle, settings.Color, settings.Angle,
            origin, settings.FlipEffect, settings.Depth);
    }

    public void DrawAtPosition(Texture2D texture, Vector2 position, Scale2D scale2D, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        _spriteBatch.Draw(texture, position, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin.Value(texture.Bounds.Size), scale2D.Value, settings.FlipEffect, settings.Depth);
    }

    public void DrawScaledStringAtPosition(Font font, string text, Point position, Scale2D scale, DrawSettings settings)
    {
        _spriteBatch.DrawString(
            font.SpriteFont,
            text,
            position.ToVector2(),
            settings.Color,
            settings.Angle,
            settings.Origin.Value(font.MeasureString(text).ToPoint()) / font.ScaleFactor,
            scale.Value * font.ScaleFactor,
            settings.FlipEffect,
            settings.Depth);
    }

    public void DrawStringAtPosition(Font font, string text, Point position, DrawSettings settings)
    {
        DrawScaledStringAtPosition(font, text, position, Scale2D.One, settings);
    }

    public void DrawStringWithinRectangle(Font font, string text, Rectangle rectangle, Alignment alignment,
        DrawSettings settings)
    {
        var restrictedString = font.GetRestrictedString(text, rectangle.Width);
        var size = restrictedString.Size;
        var relativePosition = alignment.GetRelativePositionOfElement(rectangle.Size.ToVector2(), size);
        var brokenText = restrictedString.Text;
        var origin = settings.Origin.Value(rectangle.Size) / font.ScaleFactor;
        _spriteBatch.DrawString(
            font.SpriteFont,
            brokenText,
            (rectangle.Location.ToVector2() + relativePosition).ToPoint().ToVector2(), // gross truncating
            settings.Color,
            settings.Angle,
            origin,
            Vector2.One * font.ScaleFactor,
            settings.FlipEffect,
            settings.Depth);
    }

    public void FillRectangle(Rectangle rectangle, Color color, Depth depth)
    {
        DrawAsRectangle(Client.Assets.GetTexture("white-pixel"), rectangle,
            new DrawSettings {Depth = depth, Color = color});
    }
}
