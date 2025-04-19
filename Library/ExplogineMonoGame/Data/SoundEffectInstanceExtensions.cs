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

        sound.Pan = settings.Pan;
        sound.Pitch = settings.Pitch;
        sound.Volume = settings.Volume;
        sound.IsLooped = settings.Loop;

        sound.Play();
    }
}
