using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class SpriteFontAsset : Asset
{
    public SpriteFontAsset(SpriteFont spriteFont) : base(null)
    {
        SpriteFont = spriteFont;
    }

    public SpriteFont SpriteFont { get; }
}
