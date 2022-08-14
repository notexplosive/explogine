using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NotCore.Data;

namespace NotCore.AssetManagement;

/// <summary>
///     SpriteSheet that assumes the texture is arranged in a grid of frames where each frame is the same size
/// </summary>
public class GridBasedSpriteSheet : SpriteSheet
{
    private readonly int _columnCount;
    private readonly int _frameCount;
    private Point _frameSize;
    private readonly int _rowCount;

    public GridBasedSpriteSheet(string key, string textureName, Point frameSize) : this(
        key, Client.Assets.GetTexture(textureName), frameSize)
    {
    }

    public GridBasedSpriteSheet(string key, Texture2D texture, Point frameSize) : base(key, texture)
    {
        var isValid = texture.Width % frameSize.X == 0;
        isValid = isValid && texture.Height % frameSize.Y == 0;

        if (!isValid)
        {
            throw new Exception("Texture does not evenly divide by cell dimensions");
        }

        _frameSize = frameSize;
        _columnCount = texture.Width / frameSize.X;
        _rowCount = texture.Height / frameSize.Y;
        _frameCount = _columnCount * _rowCount;
    }

    public override int FrameCount => _frameCount;

    public override Rectangle GetSourceRectForFrame(int index)
    {
        var x = index % _columnCount;
        var y = index / _columnCount;
        return new Rectangle(new Point(x * _frameSize.X, y * _frameSize.Y), _frameSize);
    }

    public override void DrawFrame(Painter painter, int index, Vector2 position, float scale, float angle,
        XyBool flip, Depth layerDepth, Color tintColor, bool isCentered = true)
    {
        var isValid = index >= 0 && index <= _frameCount;
        if (!isValid)
        {
            throw new IndexOutOfRangeException();
        }

        var sourceRect = GetSourceRectForFrame(index);

        var adjustedFrameSize = _frameSize.ToVector2() * scale;
        var destRect = new Rectangle(position.ToPoint(), adjustedFrameSize.ToPoint());

        var offset = Vector2.Zero;
        if (isCentered)
        {
            offset = _frameSize.ToVector2() / 2;
        }

        var drawSettings = new RectangleDrawSettings
        {
            DestinationRect = destRect,
            SourceRectangle = sourceRect,
            Color = tintColor,
            Angle = angle,
            Origin = offset,
            Flip = flip,
            Depth = layerDepth
        };
        
        painter.Draw(Texture, drawSettings);
    }
}
