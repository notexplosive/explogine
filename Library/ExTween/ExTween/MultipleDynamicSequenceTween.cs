using System;

namespace ExTween;

public class MultipleDynamicSequenceTween : MultipleDynamicTween<SequenceTween>
{
    public MultipleDynamicSequenceTween(int count, Func<int, ITween> generateFunction) : base(count, generateFunction)
    {
    }
}
