namespace ExplogineCore.Data;

public struct SoundEffectOptions
{
    public bool Cached { get; set; }
    public bool Loop { get; set; }
    public float Volume { get; set; }
    public float Pan { get; set; }
    public float Pitch { get; set; }

    public SoundEffectOptions()
    {
        Cached = true;
        Loop = false;
        Volume = 0.5f;
        Pan = 0f;
        Pitch = 0f;
    }
}
