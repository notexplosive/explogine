using ExplogineCore.Data;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public class SoundPlayer
{
    internal SoundPlayer()
    {
    }

    public SoundEffectInstance Play(string name, SoundEffectSettings? options = null)
    {
        if (Client.Headless)
        {
            return null!;
        }
        
        var usedOptions = new SoundEffectSettings();
        if (options.HasValue)
        {
            usedOptions = options.Value;
        }

        SoundEffectInstance instance;
        if (usedOptions.Cached)
        {
            instance = Client.Assets.GetSoundEffectInstance(name);
            instance.Stop();
        }
        else
        {
            instance = Client.Assets.GetSoundEffect(name).CreateInstance();
        }

        instance.Pan = usedOptions.Pan;
        instance.Pitch = usedOptions.Pitch;
        instance.Volume = usedOptions.Volume;
        instance.IsLooped = usedOptions.Loop;
        instance.Play();

        return instance;
    }
}
