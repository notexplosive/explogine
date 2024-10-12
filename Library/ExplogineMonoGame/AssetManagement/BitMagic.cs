using System;

namespace ExplogineMonoGame.AssetManagement;

public static class BitMagic
{
    public static float[] ConvertArrayOfBytesToNormalizedFloats<T>(byte[] bytes, int numberOfSamples, int numberOfBytesPerSample)
    {
        var result = new float[numberOfSamples];
        if (typeof(T) == typeof(short))
        {
            for (var sampleIndex = 0; sampleIndex < numberOfSamples; sampleIndex++)
            {
                var intermediateInt = BitConverter.ToInt16(bytes, sampleIndex * numberOfBytesPerSample);
                result[sampleIndex] = (float) intermediateInt / short.MaxValue;
            }

            return result;
        }

        if (typeof(T) == typeof(int))
        {
            for (var sampleIndex = 0; sampleIndex < numberOfSamples; sampleIndex++)
            {
                var intermediateInt = BitConverter.ToInt32(bytes, sampleIndex * numberOfBytesPerSample);
                result[sampleIndex] = (float) intermediateInt / int.MaxValue;
            }

            return result;
        }

        if (typeof(T) == typeof(long))
        {
            for (var sampleIndex = 0; sampleIndex < numberOfSamples; sampleIndex++)
            {
                var intermediateInt = BitConverter.ToInt64(bytes, sampleIndex * numberOfBytesPerSample);
                result[sampleIndex] = (float) intermediateInt / int.MaxValue;
            }

            return result;
        }

        throw new Exception($"Unsupported format {typeof(T).Name}");
    }
}
