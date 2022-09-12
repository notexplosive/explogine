using System;

namespace ExTween;

public class MultipleDynamicMultiplexTween : MultipleDynamicTween<MultiplexTween>
{
    public MultipleDynamicMultiplexTween(int count, Func<int, ITween> generateFunction) : base(count, generateFunction)
    {
    }
}
