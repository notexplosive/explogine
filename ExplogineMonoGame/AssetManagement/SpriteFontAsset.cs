using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class SpriteFontAsset : Asset
{
    public SpriteFontAsset(string key, SpriteFont spriteFont) : base(key)
    {
        SpriteFont = spriteFont;
    }

    public SpriteFont SpriteFont { get; }
}
