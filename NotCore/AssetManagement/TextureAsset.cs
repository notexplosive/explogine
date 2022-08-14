using Microsoft.Xna.Framework.Graphics;

namespace NotCore.AssetManagement;

public class TextureAsset : Asset
{
    public TextureAsset(string key, Texture2D texture) : base(key)
    {
        Texture = texture;
    }

    public Texture2D Texture { get; }
}
