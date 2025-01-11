using System;
using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class SelectFrameSpriteSheet : SpriteSheet
{
    private readonly List<Rectangle> _frames = new();

    public SelectFrameSpriteSheet(Texture2D texture) : base(texture)
    {
    }

    public override int FrameCount => _frames.Count;

    public override void DrawFrameAtPosition(Painter painter, int index, Vector2 position, Scale2D scale,
        DrawSettings drawSettings)
    {
        var adjustedFrameSize = GetSourceRectForFrame(index).Size.ToVector2() * scale.Value;
        var destinationRect = new RectangleF(position, adjustedFrameSize);
        DrawFrameAsRectangle(painter, index, destinationRect, drawSettings);
    }

    public override Rectangle GetSourceRectForFrame(int index)
    {
        if (_frames.Count == 0)
        {
            throw new Exception("Sprite sheet has no frames");
        }

        return _frames[index % _frames.Count];
    }

    public void AddFrame(Rectangle frameRectangle)
    {
        _frames.Add(frameRectangle);
    }
}
