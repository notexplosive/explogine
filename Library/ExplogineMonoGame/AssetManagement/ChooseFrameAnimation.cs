using System;

namespace ExplogineMonoGame.AssetManagement;

public class ChooseFrameAnimation : IFrameAnimation
{
    private readonly int[] _frames;

    public ChooseFrameAnimation(params int[] listOfFrames)
    {
        Loop = true;
        _frames = listOfFrames;
    }

    public ChooseFrameAnimation(bool loop, params int[] listOfFrames)
    {
        Loop = loop;
        _frames = listOfFrames;
    }

    public int Length => _frames.Length;

    public bool Loop { get; }

    public int GetFrame(float elapsedTime)
    {
        var isValid = _frames != null && _frames.Length != 0;
        if (!isValid)
        {
            throw new Exception("ChooseFrameAnimation was used but no frames were chosen");
        }

        if (_frames!.Length == 1)
        {
            return _frames[0];
        }

        if (Loop)
        {
            return _frames[(int) elapsedTime % _frames.Length];
        }

        return Math.Min(_frames[_frames.Length], _frames[(int) elapsedTime]);
    }
}
