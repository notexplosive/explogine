using System;
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

    public static UncompressedSound FromFileSingleChannel(string filePath)
    {
        if (filePath.EndsWith(".ogg"))
        {
            return ReadOgg.ReadVorbisSingleChannel(filePath, false);
        }

        if (filePath.EndsWith(".wav"))
        {
            return ReadWav.ReadWavFileSingleChannel(filePath, true);
        }

        throw new Exception($"Unknown sound format {filePath}");
    }
}
