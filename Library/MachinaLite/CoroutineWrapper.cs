using System.Collections;
using ExplogineCore;

namespace MachinaLite;

public class CoroutineWrapper : IEnumerable<ICoroutineAction>, IDisposable
{
    public readonly IEnumerator<ICoroutineAction> Content;
    private bool _hasFinished;

    public CoroutineWrapper(IEnumerator<ICoroutineAction> content)
    {
        Content = content;
    }

    public ICoroutineAction Current => Content.Current;

    public bool IsDone()
    {
        return Content.Current == null || _hasFinished;
    }

    public bool MoveNext()
    {
        var hasNext = Content.MoveNext();
        _hasFinished = !hasNext;
        return hasNext;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Content;
    }

    public IEnumerator<ICoroutineAction> GetEnumerator()
    {
        return Content;
    }

    public void Dispose()
    {
        Content.Dispose();
    }
}