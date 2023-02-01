using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public abstract class SpriteSheet : Asset
{
    public SpriteSheet(Texture2D texture) : base(null)
    {
        Texture = texture;
    }

    protected Texture2D Texture { get; }

    public Texture2D SourceTexture => Texture;

    public IFrameAnimation DefaultAnimation => new LinearFrameAnimation(0, FrameCount);

    public abstract int FrameCount { get; }

    public abstract void DrawFrameAtPosition(Painter painter, int index, Vector2 position, Scale2D scale,
        DrawSettings drawSettings);
    
    public abstract void DrawFrameAsRectangle(Painter painter, int index, RectangleF rectangleF,
        DrawSettings drawSettings);

    public abstract Rectangle GetSourceRectForFrame(int index);

    public ImageAsset GetImageAtFrame(int index)
    {
        return new ImageAsset(SourceTexture, GetSourceRectForFrame(index));
    }
}
