using ExplogineCore.Data;
using Microsoft.Xna.Framework.Audio;

namespace ExplogineMonoGame;

public class SoundPlayer
{
    internal SoundPlayer()
    {
    }

    public SoundEffectInstance Play(string name, SoundEffectSettings options)
    {
        if (Client.Headless)
        {
            return null!;
        }

        SoundEffectInstance instance;
        if (options.Cached)
        {
            instance = Client.Assets.GetSoundEffectInstance(name);
            instance.Stop();
        }
        else
        {
            instance = Client.Assets.GetSoundEffect(name).CreateInstance();
        }

        instance.Pan = options.Pan;
        instance.Pitch = options.Pitch;
        instance.Volume = options.Volume;
        instance.IsLooped = options.Loop;
        instance.Play();

        return instance;
    }
}
