using System;
using Microsoft.Xna.Framework.Audio;

namespace ExplogineMonoGame.AssetManagement;

public class SoundAsset : Asset
{
    public SoundAsset(SoundEffect soundEffect) : base(soundEffect)
    {
        SoundEffect = soundEffect;
        SoundEffectInstance = soundEffect.CreateInstance();
    }

    public SoundEffectInstance SoundEffectInstance { get; }
    public SoundEffect SoundEffect { get; }
    public TimeSpan Duration => SoundEffect.Duration;
}
