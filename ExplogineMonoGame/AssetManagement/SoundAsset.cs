using Microsoft.Xna.Framework.Audio;

namespace ExplogineMonoGame.AssetManagement;

public class SoundAsset : Asset
{
    public SoundAsset(string key, SoundEffect soundEffect) : base(key, soundEffect)
    {
        SoundEffect = soundEffect;
        SoundEffectInstance = soundEffect.CreateInstance();
    }

    public SoundEffectInstance SoundEffectInstance { get; }
    public SoundEffect SoundEffect { get; }
}
