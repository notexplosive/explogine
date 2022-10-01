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
    private readonly int _frameCount;
    private readonly int _rowCount;
    private Point _frameSize;

    public GridBasedSpriteSheet(string textureName, Point frameSize) : this(Client.Assets.GetTexture(textureName),
        frameSize)
    {
    }

    public GridBasedSpriteSheet(Texture2D texture, Point frameSize) : base(texture)
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

    public override void DrawFrame(Painter painter, int index, Vector2 position, Scale2D scale,
        DrawSettings drawSettings)
    {
        var isValid = index >= 0 && index <= _frameCount-1;
        if (!isValid)
        {
            throw new IndexOutOfRangeException();
        }

        var adjustedFrameSize = _frameSize.ToVector2() * scale.Value;
        var destinationRect = new Rectangle(position.ToPoint(), adjustedFrameSize.ToPoint());

        drawSettings.SourceRectangle ??= GetSourceRectForFrame(index);

        painter.DrawAsRectangle(Texture, destinationRect, drawSettings);
    }
}
