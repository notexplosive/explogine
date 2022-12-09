using System;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class StaticImageAsset : Asset
{
    public Texture2D Texture { get; }
    public Rectangle SourceRectangle { get; }

    public StaticImageAsset(Texture2D texture, Rectangle sourceRectangle, bool ownsTexture = false) : base(ownsTexture ? texture : null)
    {
        Texture = texture;
        SourceRectangle = sourceRectangle;
    }
}
