using System;

namespace ExTween;

public class DynamicTween : ITween
{
    private readonly Func<ITween> _generateFunction;
    private ITween? _generated;

    public DynamicTween(Func<ITween> generateFunction)
    {
        _generateFunction = generateFunction;
    }

    public ITweenDuration TotalDuration => GenerateIfNotAlready().TotalDuration;

    public float Update(float dt)
    {
        return GenerateIfNotAlready().Update(dt);
    }

    public bool IsDone()
    {
        return GenerateIfNotAlready().IsDone();
    }

    public void Reset()
    {
        _generated?.Reset();
        _generated = null;
    }

    public void JumpTo(float time)
    {
        GenerateIfNotAlready().JumpTo(time);
    }

    public void SkipToEnd()
    {
        GenerateIfNotAlready().SkipToEnd();
    }

    public ITween GenerateIfNotAlready()
    {
        return _generated ??= _generateFunction();
    }
}