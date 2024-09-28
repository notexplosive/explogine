using System;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace ExplogineMonoGame.AssetManagement;

public static class ReadOgg
{
    private static void ConvertFloatBufferToShortBuffer(float[] inBuffer, short[] outBuffer, int length)
    {
        // The float[] we get from NVorbis has the range [-1f,1f], we need to convert that to a short[] with a range of [-32768, 32767]
        for (var i = 0; i < length; i++)
        {
            var temp = (int) (short.MaxValue * inBuffer[i]);
            temp = Math.Clamp(temp, short.MinValue, short.MaxValue);
            outBuffer[i] = (short) temp;
        }
    }

    public static SoundEffect ReadSoundEffect(UncompressedSound uncompressedSound)
    {
        return new SoundEffect(ConvertFloatFramesToBytes(uncompressedSound.Frames), uncompressedSound.SampleRate, uncompressedSound.Channels);
    }

    public static byte[] ConvertFloatFramesToBytes(float[] frames)
    {
        // samples is a float[], we need a short[].
        var castBuffer = new short[frames.Length];
        ConvertFloatBufferToShortBuffer(frames, castBuffer, castBuffer.Length);

        // Now that we have the sound represented as a short[], we need to convert that to bytes. Each short is 2 bytes long, so we need 2X as many bytes as we have shorts.
        var bytes = new byte[castBuffer.Length * 2];

        // Extract the bytes from castBuffer and place them onto the bytes array
        for (var i = 0; i < castBuffer.Length; i++)
        {
            var b = BitConverter.GetBytes(castBuffer[i]);
            bytes[i * 2] = b[0];
            bytes[i * 2 + 1] = b[1];
        }

        return bytes;
    }

    public static UncompressedSound ReadVorbis(string fullFileName)
    {
        // VorbisReader comes from NVorbis.
        using var vorbis = new VorbisReader(fullFileName);

        // TotalSamples is actually in Frames, so we need to multiply it by channels to get Samples.
        var frames = new float[vorbis.TotalSamples * vorbis.Channels];

        // Read all samples, starting at index 0 and reading to the end.
        // This writes to the `samples` array.
        var length = vorbis.ReadSamples(frames, 0, frames.Length);

        // We will pass this directly to MonoGame.
        var sampleRate = vorbis.SampleRate;

        // Finally, we convert the vorbis.Channels count to the AudioChannels enum. Casting like this: `(AudioChannels) vorbis.Channels` would also work.
        var channels = vorbis.Channels == 2 ? AudioChannels.Stereo : AudioChannels.Mono;

        return new UncompressedSound(frames, length, channels, sampleRate);
    }
}
