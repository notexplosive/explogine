﻿using Microsoft.Xna.Framework.Audio;

namespace NotCore.AssetManagement;

public class SoundAsset : Asset
{
    public SoundAsset(string key, SoundEffect soundEffect) : base(key)
    {
        SoundEffect = soundEffect;
        SoundEffectInstance = soundEffect.CreateInstance();
    }

    public SoundEffectInstance SoundEffectInstance { get; }
    public SoundEffect SoundEffect { get; }
}
