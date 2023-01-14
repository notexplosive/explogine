using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public class Painter
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private Texture2D? _pixelAsset;

    public Painter(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public bool IsSpriteBatchInProgress { get; private set; }

    public Texture2D PixelAsset => _pixelAsset ??= Client.Assets.GetTexture("white-pixel");

    public void Clear(Color color)
    {
        _graphicsDevice.Clear(color);
    }

    public void BeginSpriteBatch()
    {
        BeginSpriteBatch(Matrix.Identity);
    }

    public void BeginSpriteBatch(RectangleF viewBounds, Point outputDimensions, float angle = 0)
    {
        BeginSpriteBatch(viewBounds.CanvasToScreen(outputDimensions, angle));
    }

    public void BeginSpriteBatch(Matrix matrix)
    {
        _spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearWrap, null, null, null, matrix);
        IsSpriteBatchInProgress = true;
    }

    public void EndSpriteBatch()
    {
        _spriteBatch.End();
        IsSpriteBatchInProgress = false;
    }

    /// <summary>
    ///     Only to be used by the CrashCartridge if we threw an exception
    /// </summary>
    internal void ResetToCleanState()
    {
        if (IsSpriteBatchInProgress)
        {
            EndSpriteBatch();
        }

        while (!Client.Graphics.IsAtTopCanvas())
        {
            Client.Graphics.PopCanvas();
        }
    }

    #region Draw Strings

    public void DrawScaledStringAtPosition(IFontGetter fontLike, string text, Vector2 position, Scale2D scale,
        DrawSettings settings)
    {
        var font = fontLike.GetFont();
        if (font is Font realFont)
        {
            _spriteBatch.DrawString(
                realFont.SpriteFont,
                text,
                position,
                settings.Color,
                settings.Angle,
                settings.Origin.Value(font.MeasureString(text).ToPoint()) / font.ScaleFactor,
                scale.Value * font.ScaleFactor,
                settings.FlipEffect,
                settings.Depth);
        }
        else
        {
            throw new Exception("Attempted to draw text without a backing SpriteFont");
        }
    }

    public void DrawStringAtPosition(IFontGetter font, string text, Vector2 position, DrawSettings settings)
    {
        DrawScaledStringAtPosition(font, text, position, Scale2D.One, settings);
    }

    public void DrawDebugStringAtPosition(string text, Vector2 position, DrawSettings settings)
    {
        DrawStringAtPosition(Client.Assets.GetFont("engine/console-font", 32), text, position, settings);
    }

    public void DrawFormattedStringAtPosition(FormattedText formattedText, Vector2 position, Alignment alignment,
        DrawSettings settings)
    {
        var rectangle = new RectangleF(position, formattedText.OneLineSize()).ToRectangle();
        var movedRectangle = rectangle.Moved(-settings.Origin.Value(rectangle.Size));
        DrawFormattedStringWithinRectangle(formattedText, movedRectangle, alignment, settings);
    }

    public void DrawFormattedStringWithinRectangle(FormattedText formattedText, RectangleF rectangle,
        Alignment alignment, DrawSettings settings)
    {
        // First we move the rect by the offset so the resulting rect is always in the location you asked for it,
        // and is then rotated around the origin
        var movedRectangle = rectangle.Moved(settings.Origin.Value(rectangle.Size));
        var rectTopLeft = movedRectangle.Location;

        void DrawLetter(FormattedText.FormattedGlyph glyph)
        {
            var letterOrigin = (rectTopLeft - glyph.Position) / glyph.Data.ScaleFactor;

            if (glyph.Data is FormattedText.CharGlyphData fragmentChar)
            {
                if (fragmentChar.Font is Font realFont)
                {
                    var finalColor = fragmentChar.Color ?? settings.Color;
                    if (fragmentChar.Color.HasValue)
                    {
                        finalColor = finalColor.WithMultipliedOpacity((float) settings.Color.A / byte.MaxValue);
                    }

                    _spriteBatch.DrawString(
                        realFont.SpriteFont,
                        fragmentChar.Text.ToString(),
                        rectTopLeft,
                        finalColor,
                        settings.Angle,
                        letterOrigin,
                        Vector2.One * fragmentChar.Font.ScaleFactor,
                        settings.FlipEffect,
                        settings.Depth);
                }
                else
                {
                    throw new Exception("Attempted to draw string with Font that has no SpriteFont");
                }
            }

            if (glyph.Data is FormattedText.ImageGlyphData fragmentImage)
            {
                DrawAtPosition(
                    fragmentImage.Image.Get().Texture,
                    rectTopLeft,
                    new Scale2D(new Vector2(fragmentImage.ScaleFactor)),
                    settings with
                    {
                        SourceRectangle = fragmentImage.Image.Get().SourceRectangle,
                        Color = fragmentImage.Color ?? Color.White,
                        Origin = new DrawOrigin(letterOrigin)
                    });
            }
        }

        foreach (var letterPosition in formattedText.GetGlyphs(rectangle, alignment))
        {
            DrawLetter(letterPosition);
        }
    }

    public void DrawStringWithinRectangle(IFontGetter fontLike, string text, RectangleF rectangle, Alignment alignment,
        DrawSettings settings)
    {
        var formattedText = new FormattedText(fontLike.GetFont(), text);
        DrawFormattedStringWithinRectangle(formattedText, rectangle, alignment, settings);
    }

    #endregion

    # region Draw At Position

    public void DrawAtPosition(Texture2D texture, Vector2 position)
    {
        DrawAtPosition(texture, position, Scale2D.One, new DrawSettings());
    }

    public void DrawAtPosition(Texture2D texture, Vector2 position, Scale2D scale2D, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        _spriteBatch.Draw(texture, position, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin.Value(settings.SourceRectangle.Value.Size), scale2D.Value, settings.FlipEffect,
            settings.Depth);
    }

    #endregion

    # region Draw Rectangles

    public void DrawRectangle(RectangleF rectangle, DrawSettings drawSettings)
    {
        DrawAtPosition(PixelAsset, rectangle.Location, new Scale2D(rectangle.Size), drawSettings);
    }

    public void DrawAsRectangle(Texture2D texture, RectangleF destinationRectangle)
    {
        DrawAsRectangle(texture, destinationRectangle, new DrawSettings());
    }

    public void DrawAsRectangle(Texture2D texture, RectangleF destinationRectangle, DrawSettings settings)
    {
        if (Client.Debug.IsActive)
        {
            DrawRectangle(destinationRectangle, new DrawSettings
            {
                Depth = settings.Depth + 1,
                Angle = settings.Angle,
                Origin = settings.Origin,
                Color = Color.White.WithMultipliedOpacity(0.25f)
            });
        }
        settings.SourceRectangle ??= texture.Bounds;

        // the origin is relative to the source rect, but we pass it in assume its scaled with the destination rect
        var origin = settings.Origin.Value(settings.SourceRectangle.Value.Size);

        // destination is downcast to a Rectangle
        _spriteBatch.Draw(texture, destinationRectangle.ToRectangle(), settings.SourceRectangle, settings.Color,
            settings.Angle,
            origin, settings.FlipEffect, settings.Depth);
    }

    #endregion

    #region Line Figures

    public void DrawLinePolygon(Polygon polygon, LineDrawSettings settings)
    {
        for (var i = 1; i < polygon.VertexCount; i++)
        {
            DrawLine(polygon[i - 1], polygon[i], settings);
        }

        DrawLine(polygon[^1], polygon[0], settings);
    }

    public void DrawLineRectangle(RectangleF rectangle, LineDrawSettings settings)
    {
        DrawLinePolygon(rectangle.ToPolygon(), settings);
    }

    public void DrawLine(Vector2 start, Vector2 end, LineDrawSettings settings)
    {
        var relativeEnd = end - start;
        var length = relativeEnd.Length();
        var rect = new RectangleF(start, new Vector2(length, settings.Thickness));
        var unitX = Vector2.UnitX;

        var angle = MathF.Acos(Vector2.Dot(unitX, relativeEnd) / (unitX.Length() * relativeEnd.Length()));

        if (start == end)
        {
            // edge case: overwrite the values and just draw a box
            rect = new RectangleF(start, new Vector2(settings.Thickness));
            angle = 0;
        }

        if (relativeEnd.Y < 0)
        {
            angle = -angle;
        }

        DrawRectangle(rect,
            new DrawSettings
            {
                Color = settings.Color,
                Depth = settings.Depth,
                Angle = angle,
                Origin = new DrawOrigin(new Vector2(0, 0.5f))
            });
    }

    #endregion
}
