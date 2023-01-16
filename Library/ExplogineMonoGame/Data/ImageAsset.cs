using System;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class ImageAsset : Asset
{
    private readonly IndirectTexture _indirectTexture;
    public Texture2D Texture => _indirectTexture.Get();
    public Rectangle SourceRectangle { get; }

    public ImageAsset(Texture2D texture, Rectangle sourceRectangle, bool ownsTexture = false) : base(ownsTexture ? texture : null)
    {
        _indirectTexture = new IndirectTexture(texture);
        SourceRectangle = sourceRectangle;
    }

    public ImageAsset(IndirectTexture indirectTexture, Rectangle sourceRectangle) : base(null)
    {
        _indirectTexture = indirectTexture;
        SourceRectangle = sourceRectangle;
    }
}
