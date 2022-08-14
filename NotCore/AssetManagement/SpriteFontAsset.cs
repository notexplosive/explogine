using Microsoft.Xna.Framework.Graphics;

namespace NotCore.AssetManagement;

public class SpriteFontAsset : Asset
{
    public SpriteFontAsset(string key, SpriteFont spriteFont) : base(key)
    {
        SpriteFont = spriteFont;
    }

    public SpriteFont SpriteFont { get; }
}
