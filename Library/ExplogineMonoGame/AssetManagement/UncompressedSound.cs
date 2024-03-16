using Microsoft.Xna.Framework.Audio;

namespace ExplogineMonoGame.AssetManagement;

public class UncompressedSound
{
    public AudioChannels Channels { get; }
    public float[] Frames { get; }
    public int Length { get; }
    public int SampleRate { get; }

    public UncompressedSound(float[] frames, int length, AudioChannels channels, int sampleRate)
    {
        Frames = frames;
        Length = length;
        Channels = channels;
        SampleRate = sampleRate;
    }
}
