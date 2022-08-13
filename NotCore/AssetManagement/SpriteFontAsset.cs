using Microsoft.Xna.Framework.Graphics;

namespace NotCore.AssetManagement;

public class SpriteFontAsset : IAsset
{
    public SpriteFontAsset(string key, SpriteFont spriteFont)
    {
        Key = key;
        SpriteFont = spriteFont;
    }

    public SpriteFont SpriteFont { get; }
    public string Key { get; }
}
