﻿using System;
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

    public void DrawRectangle(RectangleF rectangle, DrawSettings drawSettings)
    {
        DrawAtPosition(PixelAsset, rectangle.Location, new Scale2D(rectangle.Size), drawSettings);
    }

    public void DrawAsRectangle(Texture2D texture, RectangleF destinationRectangle)
    {
        DrawAsRectangle(texture, destinationRectangle, new DrawSettings());
    }

    public void DrawAtPosition(Texture2D texture, Vector2 position)
    {
        DrawAtPosition(texture, position, Scale2D.One, new DrawSettings());
    }

    public void DrawAsRectangle(Texture2D texture, RectangleF destinationRectangle, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        var origin = settings.Origin.Value(destinationRectangle.Size);

        var scaleX = destinationRectangle.Size.X / settings.SourceRectangle.Value.Size.X;
        var scaleY = destinationRectangle.Size.Y / settings.SourceRectangle.Value.Size.Y;

        // the origin is relative to the source rect, but we pass it in assume its scaled with the destination rect
        origin.X /= scaleX;
        origin.Y /= scaleY;

        // destination is downcast to a Rectangle
        _spriteBatch.Draw(texture, destinationRectangle.ToRectangle(), settings.SourceRectangle, settings.Color,
            settings.Angle,
            origin, settings.FlipEffect, settings.Depth);
    }

    public void DrawAtPosition(Texture2D texture, Vector2 position, Scale2D scale2D, DrawSettings settings)
    {
        settings.SourceRectangle ??= texture.Bounds;
        _spriteBatch.Draw(texture, position, settings.SourceRectangle, settings.Color, settings.Angle,
            settings.Origin.Value(texture.Bounds.Size), scale2D.Value, settings.FlipEffect, settings.Depth);
    }

    public void DrawScaledStringAtPosition(IFont fontLike, string text, Point position, Scale2D scale,
        DrawSettings settings)
    {
        var font = fontLike.GetFont();
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

    public void DrawStringAtPosition(IFont font, string text, Point position, DrawSettings settings)
    {
        DrawScaledStringAtPosition(font, text, position, Scale2D.One, settings);
    }

    public void DrawDebugStringAtPosition(string text, Point position, DrawSettings settings)
    {
        DrawStringAtPosition(Client.Assets.GetFont("engine/console-font", 32), text, position, settings);
    }

    public void DrawStringWithinRectangle(IFont fontLike, string text, Rectangle rectangle, Alignment alignment,
        DrawSettings settings)
    {
        var font = fontLike.GetFont();
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
}
