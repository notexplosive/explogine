using System;
using ExplogineCore.Data;
using Microsoft.Xna.Framework.Audio;

namespace ExplogineMonoGame.Data;

public static class SoundEffectInstanceExtensions
{
    public static void Play(this SoundEffectInstance sound, SoundEffectSettings settings)
    {
        if (settings.Cached)
        {
            sound.Stop();
        }

        sound.Pan = Math.Clamp(settings.Pan, -1, 1);
        sound.Pitch = Math.Clamp(settings.Pitch, -1, 1);
        sound.Volume = Math.Clamp(settings.Volume, 0, 1);
        sound.IsLooped = settings.Loop;

        sound.Play();
    }
}
