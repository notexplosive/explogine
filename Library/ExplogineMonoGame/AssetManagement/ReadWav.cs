using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace ExplogineMonoGame.AssetManagement;

/// <summary>
///     Heavily borrowed from MonoGame source code
/// </summary>
public static class ReadWav
{
    public static UncompressedSound ReadWavFileSingleChannel(string fileName, bool isRightChannel)
    {
        var initialSound = ReadWavFile(fileName);

        if (initialSound.Channels == AudioChannels.Mono)
        {
            return initialSound;
        }
        
        var offset = isRightChannel ? 1 : 0;
        var newFrames = new float[initialSound.Length];
        for (var i = 0; i < initialSound.Length / 2; i++)
        {
            newFrames[i] = initialSound.Frames[i * 2 + offset];
        }
        
        return new UncompressedSound(newFrames, initialSound.Length / 2, AudioChannels.Mono, initialSound.SampleRate);
    }
    
    public static UncompressedSound ReadWavFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"Could not locate audio file {Path.GetFileName(fileName)}.");
        }

        var content = CreateWavContent(fileName);

        // Validate the format of the input.
        if (content.SampleRate < 8000 || content.SampleRate > 48000)
        {
            throw new Exception(
                $"Audio file {Path.GetFileName(fileName)} contains audio data with unsupported sample rate of {content.SampleRate}KHz. Supported sample rates are from 8KHz up to 48KHz.");
        }

        return content;
    }

    private static UncompressedSound CreateWavContent(string fileName)
    {
        byte[] rawData;

        // Must be opened in read mode otherwise it fails to open
        // read-only files (found in some source control systems)
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            rawData = new byte[stream.Length];
            var read = stream.Read(rawData, 0, rawData.Length);
            if (read != rawData.Length)
            {
                Client.Debug.LogWarning("Did not read whole file stream");
            }
        }

        var wavBytes = StripRiffWaveHeader(rawData, out var format);

        if (format == null)
        {
            throw new Exception($"Could not determine format of {fileName}");
        }

        var validPcm = format.Format == 1 && (format.BitsPerSample == 8 ||
                                              format.BitsPerSample == 16 ||
                                              format.BitsPerSample == 24);
        var validAdcmp = (format.Format == 2 || format.Format == 17) &&
                         format.BitsPerSample == 4;
        var validIeeeFloat = format.Format == 3 && format.BitsPerSample == 32;
        if (!(validPcm || validAdcmp || validIeeeFloat))
        {
            throw new Exception(
                $"Audio file {Path.GetFileName(fileName)} contains audio data with unsupported format of {format.Format} and bit depth of {format.BitsPerSample}. Supported bit depths are unsigned 8-bit, signed 16-bit, signed 24-bit for PCM(1) and 32-bit for IEEE Float(3).");
        }

        var bitsPerSample = format.BitsPerSample;

        var floatBuffer = ConvertBytesToFloats(wavBytes, bitsPerSample);

        var channels = AudioChannels.Mono;
        if (format.ChannelCount == 2)
        {
            channels = AudioChannels.Stereo;
        }

        return new UncompressedSound(floatBuffer, floatBuffer.Length, channels, format.SampleRate);
    }

    private static float[] ConvertBytesToFloats(byte[] wavBytes, int bitsPerSample)
    {
        var numberOfBytesPerSample = bitsPerSample / 8;
        var numberOfSamples = wavBytes.Length / numberOfBytesPerSample;
        if (numberOfBytesPerSample == 2)
        {
            return BitMagic.ConvertArrayOfBytesToNormalizedFloats<Int16>(wavBytes,numberOfSamples, numberOfBytesPerSample);
        }
        
        if (numberOfBytesPerSample == 4)
        {
            return BitMagic.ConvertArrayOfBytesToNormalizedFloats<Int32>(wavBytes,numberOfSamples, numberOfBytesPerSample);
        }
        
        if (numberOfBytesPerSample == 8)
        {
            return BitMagic.ConvertArrayOfBytesToNormalizedFloats<Int64>(wavBytes,numberOfSamples, numberOfBytesPerSample);
        }
        
        throw new Exception($"Unsupported bit depth: {bitsPerSample}");
    }

    private static byte[] StripRiffWaveHeader(byte[] data, out AudioFormat? audioFormat)
    {
        audioFormat = null;

        using var reader = new BinaryReader(new MemoryStream(data));
        var signature = new string(reader.ReadChars(4));
        if (signature != "RIFF")
        {
            return data;
        }

        reader.ReadInt32(); // riff_chunck_size

        var format = new string(reader.ReadChars(4));
        if (format != "WAVE")
        {
            return data;
        }

        // Look for the data chunk.
        while (true)
        {
            var chunkSignature = new string(reader.ReadChars(4));
            if (chunkSignature.ToLowerInvariant() == "data")
            {
                break;
            }

            if (chunkSignature.ToLowerInvariant() == "fmt ")
            {
                var fmtLength = reader.ReadInt32();
                var formatTag = reader.ReadInt16();
                var channels = reader.ReadInt16();
                var sampleRate = reader.ReadInt32();
                var avgBytesPerSec = reader.ReadInt32();
                var blockAlign = reader.ReadInt16();
                var bitsPerSample = reader.ReadInt16();
                audioFormat = new AudioFormat(avgBytesPerSec, bitsPerSample, blockAlign, channels, formatTag,
                    sampleRate);

                fmtLength -= 2 + 2 + 4 + 4 + 2 + 2;
                if (fmtLength < 0)
                {
                    throw new InvalidOperationException("riff wave header has unexpected format");
                }

                reader.BaseStream.Seek(fmtLength, SeekOrigin.Current);
            }
            else
            {
                reader.BaseStream.Seek(reader.ReadInt32(), SeekOrigin.Current);
            }
        }

        var dataSize = reader.ReadInt32();
        data = reader.ReadBytes(dataSize);

        return data;
    }

    public class AudioFormat
    {
        private readonly List<byte> _nativeWaveFormat;

        internal AudioFormat(
            int averageBytesPerSecond,
            int bitsPerSample,
            int blockAlign,
            int channelCount,
            int format,
            int sampleRate)
        {
            AverageBytesPerSecond = averageBytesPerSecond;
            BitsPerSample = bitsPerSample;
            BlockAlign = blockAlign;
            ChannelCount = channelCount;
            Format = format;
            SampleRate = sampleRate;

            _nativeWaveFormat = ConstructNativeWaveFormat();
        }

        /// <summary>
        ///     Gets the average bytes processed per second.
        /// </summary>
        /// <value>Average bytes processed per second.</value>
        public int AverageBytesPerSecond { get; }

        /// <summary>
        ///     Gets the bit depth of the audio content.
        /// </summary>
        /// <value>If the audio has not been processed, the source bit depth; otherwise, the bit depth of the new format.</value>
        public int BitsPerSample { get; }

        /// <summary>
        ///     Gets the number of bytes per sample block, taking channels into consideration. For example, for 16-bit stereo audio
        ///     (PCM format), the size of each sample block is 4 bytes.
        /// </summary>
        /// <value>Number of bytes, per sample block.</value>
        public int BlockAlign { get; }

        /// <summary>
        ///     Gets the number of channels.
        /// </summary>
        /// <value>If the audio has not been processed, the source channel count; otherwise, the new channel count.</value>
        public int ChannelCount { get; }

        /// <summary>
        ///     Gets the format of the audio content.
        /// </summary>
        /// <value>If the audio has not been processed, the format tag of the source content; otherwise, the new format tag.</value>
        public int Format { get; }

        /// <summary>
        ///     Gets the raw byte buffer for the format. For non-PCM formats, this buffer contains important format-specific
        ///     information beyond the basic format information exposed in other properties of the AudioFormat type.
        /// </summary>
        /// <value>The raw byte buffer represented in a collection.</value>
        public ReadOnlyCollection<byte> NativeWaveFormat => _nativeWaveFormat.AsReadOnly();

        /// <summary>
        ///     Gets the sample rate of the audio content.
        /// </summary>
        /// <value>If the audio has not been processed, the source sample rate; otherwise, the new sample rate.</value>
        public int SampleRate { get; }

        private List<byte> ConstructNativeWaveFormat()
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            writer.Write((short) Format);
            writer.Write((short) ChannelCount);
            writer.Write(SampleRate);
            writer.Write(AverageBytesPerSecond);
            writer.Write((short) BlockAlign);
            writer.Write((short) BitsPerSample);
            writer.Write((short) 0);

            var bytes = new byte[memory.Position];
            memory.Seek(0, SeekOrigin.Begin);
            var read = memory.Read(bytes, 0, bytes.Length);
            return bytes.ToList();
        }
    }
}
