using System.Diagnostics;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public enum InnerOuter
{
    Inner,
    Outer
}

public class NinepatchSheet : Asset
{
    private readonly Texture2D _originalTexture;
    private readonly NinepatchRects _rects;
    private readonly Texture2D[] _textures;

    public NinepatchSheet(Texture2D sourceTexture, Rectangle outerRect, Rectangle innerRect) :
        base(null) // Implements own dispose function
    {
        _originalTexture = sourceTexture;
        Debug.Assert(sourceTexture.Width >= outerRect.Width, "Texture is to small");
        Debug.Assert(sourceTexture.Height >= outerRect.Height, "Texture is to small");

        _rects = new NinepatchRects(outerRect, innerRect);
        _textures = new Texture2D[9];

        for (var i = 0; i < 9; i++)
        {
            var rect = _rects.Raw[i];
            if (rect.Width * rect.Height > 0)
            {
                var cropTexture = Client.Graphics.CropTexture(rect, sourceTexture);
                _textures[i] = cropTexture;
            }
        }
    }

    public NinepatchSheet(Texture2D sourceTexture, Rectangle innerRectangle) :
        this(sourceTexture, sourceTexture.Bounds, innerRectangle)
    {
    }

    public override void Dispose()
    {
        foreach (var texture in _textures)
        {
            texture.Dispose();
        }
    }

    private Rectangle GenerateInnerDestinationRect(Rectangle outerDestinationRect)
    {
        return new Rectangle(
            outerDestinationRect.Left + _rects.LeftBuffer,
            outerDestinationRect.Top + _rects.TopBuffer,
            outerDestinationRect.Width - _rects.LeftBuffer - _rects.RightBuffer,
            outerDestinationRect.Height - _rects.TopBuffer - _rects.BottomBuffer);
    }

    private Rectangle GenerateOuterDestinationRect(Rectangle innerDestinationRect)
    {
        return new Rectangle(
            innerDestinationRect.Left - _rects.LeftBuffer,
            innerDestinationRect.Top - _rects.TopBuffer,
            innerDestinationRect.Width + _rects.LeftBuffer + _rects.RightBuffer,
            innerDestinationRect.Height + _rects.TopBuffer + _rects.BottomBuffer);
    }

    public NinepatchRects GenerateDestinationRects(Rectangle starter,
        InnerOuter gen = InnerOuter.Inner)
    {
        if (gen == InnerOuter.Inner)
        {
            var inner = GenerateInnerDestinationRect(starter);
            return new NinepatchRects(starter, inner);
        }

        var outer = GenerateOuterDestinationRect(starter);
        return new NinepatchRects(outer, starter);
    }

    public void DrawSection(Painter painter, NinepatchIndex index, Rectangle destinationRect,
        Depth layerDepth)
    {
        var dest = destinationRect;
        var source =
            new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile
        painter.DrawAtPosition(_textures[(int) index], dest.Location.ToVector2(), Scale2D.One,
            new DrawSettings {SourceRectangle = source, Color = Color.White, Depth = layerDepth});
    }

    public void DrawFullNinepatch(Painter painter, Rectangle starter, InnerOuter innerOuter,
        Depth layerDepth, float opacity = 1f)
    {
        DrawFullNinepatch(painter, GenerateDestinationRects(starter, innerOuter), layerDepth, opacity);
    }

    private void DrawFullNinepatch(Painter painter, NinepatchRects destinationRects, Depth layerDepth,
        float opacity = 1f)
    {
        Debug.Assert(_rects.IsValidNinepatch, "Attempted to draw an invalid Ninepatch.");

        for (var i = 0; i < 9; i++)
        {
            var dest = destinationRects.Raw[i];
            var source =
                new Rectangle(0, 0, dest.Width, dest.Height); // Source is the size of the destination rect so we tile

            painter.DrawAtPosition(_textures[i], dest.Location.ToVector2(), Scale2D.One,
                new DrawSettings
                    {SourceRectangle = source, Color = Color.White.WithMultipliedOpacity(opacity), Depth = layerDepth});
        }
    }

    public void DrawHorizontalThreepatch(Painter painter, Rectangle outer, Depth layerDepth)
    {
        DrawHorizontalThreepatch(painter, GenerateDestinationRects(outer), layerDepth);
    }

    public void DrawVerticalThreepatch(Painter painter, Rectangle outer, Depth layerDepth)
    {
        DrawVerticalThreepatch(painter, GenerateDestinationRects(outer), layerDepth);
    }

    public void DrawHorizontalThreepatch(Painter painter, NinepatchRects destinationRects, Depth layerDepth)
    {
        Debug.Assert(_rects.IsValidHorizontalThreepatch, "Attempted to draw an invalid horizontal Threepatch");

        DrawSection(painter, NinepatchIndex.LeftCenter, destinationRects.LeftCenter, layerDepth);
        DrawSection(painter, NinepatchIndex.Center, destinationRects.Center, layerDepth);
        DrawSection(painter, NinepatchIndex.RightCenter, destinationRects.RightCenter, layerDepth);
    }

    public void DrawVerticalThreepatch(Painter painter, NinepatchRects destinationRects, Depth layerDepth)
    {
        Debug.Assert(_rects.IsValidVerticalThreepatch, "Attempted to draw an invalid vertical Threepatch");

        DrawSection(painter, NinepatchIndex.TopCenter, destinationRects.TopCenter, layerDepth);
        DrawSection(painter, NinepatchIndex.Center, destinationRects.Center, layerDepth);
        DrawSection(painter, NinepatchIndex.BottomCenter, destinationRects.BottomCenter, layerDepth);
    }
}
