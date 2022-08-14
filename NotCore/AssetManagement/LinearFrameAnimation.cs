using System;

namespace NotCore.AssetManagement;

/// <summary>
///     Specify the start point and length of the animation, animation starts at firstFrame, and ends on firstFrame +
///     length
/// </summary>
public readonly struct LinearFrameAnimation : IFrameAnimation
{
    private readonly int _firstFrame;

    public LinearFrameAnimation(LinearFrameAnimation copy) : this(copy._firstFrame, copy.Length, copy.Loop)
    {
    }

    public LinearFrameAnimation(int firstFrame = 0, int length = 1, bool loop = true)
    {
        _firstFrame = firstFrame;
        Length = length;
        Loop = loop;
    }

    public bool Equals(LinearFrameAnimation other)
    {
        return other._firstFrame == _firstFrame && other.Length == Length &&
               other.Loop == Loop;
    }

    public int LastFrame => _firstFrame + Length - 1;

    public int Length { get; }

    public bool Loop { get; }

    public int GetFrame(float elapsedTime)
    {
        if (Length == 1)
        {
            return _firstFrame;
        }

        if (Loop)
        {
            var alongDuration = elapsedTime % Length;
            return (int)(alongDuration + _firstFrame);
        }

        var frame = (int)elapsedTime + _firstFrame;
        return Math.Min(LastFrame, frame);
    }
}
