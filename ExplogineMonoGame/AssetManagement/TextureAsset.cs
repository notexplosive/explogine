using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class TextureAsset : Asset
{
    public TextureAsset(Texture2D texture) : base(texture)
    {
        Texture = texture;
    }

    public Texture2D Texture { get; }
}
