using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NotCore.Data;

namespace NotCore.AssetManagement;

public abstract class SpriteSheet : Asset
{
    public SpriteSheet(string key, Texture2D texture) : base(key)
    {
        Texture = texture;
    }

    protected Texture2D Texture { get; }

    public Texture2D SourceTexture => Texture;

    public IFrameAnimation DefaultAnimation => new LinearFrameAnimation(0, FrameCount);

    public abstract int FrameCount { get; }

    public abstract void DrawFrame(Painter painter, int index, Vector2 position, Scale2D scale,
        DrawSettings drawSettings);

    public abstract Rectangle GetSourceRectForFrame(int index);
}
