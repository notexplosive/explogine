using System;

namespace ExTween;

public abstract class MultipleDynamicTween<T> : ITween where T : TweenCollection, ITween, new()
{
    private readonly int _count;
    private readonly Func<int, ITween> _generateFunction;
    private T? _generated;

    public MultipleDynamicTween(int count, Func<int, ITween> generateFunction)
    {
        _count = count;
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

    private T GenerateIfNotAlready()
    {
        if (_generated == null)
        {
            var result = new T();
            for (var i = 0; i < _count; i++)
            {
                 result.AddItem(_generateFunction(i));
            }

            _generated = result;
        }

        return _generated;
    }
}
