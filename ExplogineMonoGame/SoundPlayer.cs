using ExplogineCore.Data;
using Microsoft.Xna.Framework.Audio;

namespace ExplogineMonoGame;

public class SoundPlayer
{
    internal SoundPlayer()
    {
    }

    public SoundEffectInstance Play(string name, SoundEffectOptions? options = null)
    {
        var usedOptions = new SoundEffectOptions();
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