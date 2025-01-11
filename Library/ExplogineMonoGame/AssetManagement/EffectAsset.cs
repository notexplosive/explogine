using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class EffectAsset : Asset
{
    public EffectAsset(Effect effect) : base(effect)
    {
        Effect = effect;
    }

    public Effect Effect { get; }
}
