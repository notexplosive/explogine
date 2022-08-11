using Microsoft.Xna.Framework.Audio;

namespace NotCore;

public class SoundAsset : IAsset
{
    public SoundAsset(string key, SoundEffect soundEffect)
    {
        Key = key;
        SoundEffect = soundEffect;
        SoundEffectInstance = soundEffect.CreateInstance();
    }

    public SoundEffectInstance SoundEffectInstance { get; }
    public SoundEffect SoundEffect { get; }
    public string Key { get; }
}
