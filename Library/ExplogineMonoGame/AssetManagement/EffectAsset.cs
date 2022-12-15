using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class EffectAsset : Asset
{
    public Effect Effect { get; }

    public EffectAsset(Effect effect) : base(effect)
    {
        Effect = effect;
    }
}
