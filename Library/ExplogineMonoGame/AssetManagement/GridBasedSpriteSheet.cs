using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

/// <summary>
///     SpriteSheet that assumes the texture is arranged in a grid of frames where each frame is the same size
/// </summary>
public class GridBasedSpriteSheet : SpriteSheet
{
    private readonly int _columnCount;
    private readonly int _rowCount;

    public GridBasedSpriteSheet(string textureName, Point frameSize) : this(new IndirectTexture(textureName).Get(),
        frameSize)
    {
    }

    public GridBasedSpriteSheet(Texture2D texture, Point frameSize) : base(texture)
    {
        if (frameSize.X > texture.Width || frameSize.Y > texture.Height)
        {
            Client.Debug.LogWarning(
                $"Frame size ({frameSize.X}, {frameSize.Y}) is too big for texture: {texture.Name} ({texture.Width}, {texture.Height})");
        }

        var isValid = texture.Width % frameSize.X == 0;
        isValid = isValid && texture.Height % frameSize.Y == 0;

        if (!isValid)
        {
            Client.Debug.LogWarning(
                $"Texture {texture.Width}, {texture.Height} does not evenly divide by cell dimensions {frameSize.X}, {frameSize.Y}");
        }

        FrameSize = frameSize;
        _columnCount = texture.Width / frameSize.X;
        _rowCount = texture.Height / frameSize.Y;
    }

    public override int FrameCount => _columnCount * _rowCount;
    public Point FrameSize { get; }

    public override Rectangle GetSourceRectForFrame(int index)
    {
        var x = index % _columnCount;
        var y = index / _columnCount;
        return new Rectangle(new Point(x * FrameSize.X, y * FrameSize.Y), FrameSize);
    }

    public override void DrawFrameAtPosition(Painter painter, int index, Vector2 position, Scale2D scale,
        DrawSettings drawSettings)
    {
        var isValid = index >= 0 && index <= FrameCount - 1;
        if (!isValid)
        {
            throw new IndexOutOfRangeException();
        }

        var adjustedFrameSize = FrameSize.ToVector2() * scale.Value;
        var destinationRect = new RectangleF(position, adjustedFrameSize);

        DrawFrameAsRectangle(painter, index, destinationRect, drawSettings);
    }
}
