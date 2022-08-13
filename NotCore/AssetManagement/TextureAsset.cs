using Microsoft.Xna.Framework.Graphics;

namespace NotCore.AssetManagement;

public class TextureAsset : IAsset
{
    public TextureAsset(string key, Texture2D texture)
    {
        Key = key;
        Texture = texture;
    }

    public Texture2D Texture { get; }
    public string Key { get; }
}
