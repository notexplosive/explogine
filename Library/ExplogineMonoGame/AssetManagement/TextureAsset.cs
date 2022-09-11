using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

/// <summary>
/// Asset that can be retrieved with `Assets.GetTexture()`
/// </summary>
public class TextureAsset : Asset
{
    public TextureAsset(Texture2D texture) : base(texture)
    {
        Texture = texture;
    }

    public Texture2D Texture { get; }
}
