using System;

namespace ExplogineMonoGame.AssetManagement;

public readonly struct ChooseFrameAnimation : IFrameAnimation
{
    private readonly int[] frames = Array.Empty<int>();

    public ChooseFrameAnimation(params int[] listOfFrames)
    {
        Loop = true;
        frames = listOfFrames;
    }

    public ChooseFrameAnimation(bool loop, params int[] listOfFrames)
    {
        Loop = loop;
        frames = listOfFrames;
    }

    public int Length => frames.Length;

    public bool Loop { get; }

    public int GetFrame(float elapsedTime)
    {
        var isValid = frames != null && frames.Length != 0;
        if (!isValid)
        {
            throw new Exception("ChooseFrameAnimation was used but no frames were chosen");
        }

        if (frames!.Length == 1)
        {
            return frames[0];
        }

        if (Loop)
        {
            return frames[(int) elapsedTime % frames.Length];
        }

        return Math.Min(frames[frames.Length], frames[(int) elapsedTime]);
    }
}
