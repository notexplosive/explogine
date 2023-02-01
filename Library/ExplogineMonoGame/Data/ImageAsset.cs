using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class ImageAsset : Asset
{
    private readonly IndirectTexture _indirectTexture;

    public ImageAsset(Texture2D texture, Rectangle sourceRectangle, bool ownsTexture = false) : base(ownsTexture
        ? texture
        : null)
    {
        _indirectTexture = new IndirectTexture(texture);
        SourceRectangle = sourceRectangle;
    }

    public ImageAsset(IndirectTexture indirectTexture, Rectangle sourceRectangle) : base(null)
    {
        _indirectTexture = indirectTexture;
        SourceRectangle = sourceRectangle;
    }

    public Texture2D Texture => _indirectTexture.Get();
    public Rectangle SourceRectangle { get; }

    public void DrawAtPosition(Painter painter, Vector2 position, Scale2D scale, DrawSettings settings)
    {
        settings.SourceRectangle ??= SourceRectangle;
        painter.DrawAtPosition(Texture, position, scale, settings);
    }

    public void DrawAsRectangle(Painter painter, RectangleF rectangle, DrawSettings settings)
    {
        settings.SourceRectangle ??= SourceRectangle;
        painter.DrawAsRectangle(Texture, rectangle, settings);
    }
}
