using System.Collections.Generic;

namespace ExplogineMonoGame.Logging;

public class LogOutput
{
    private readonly List<ILogCapture> _multiplex = new();
    private readonly List<ILogCapture> _stack = new();

    internal void Emit(string message)
    {
        if (_stack.Count > 0)
        {
            var topOfStack = _stack[^1];
            topOfStack.CaptureMessage(message);
        }

        foreach (var item in _multiplex)
        {
            item.CaptureMessage(message);
        }
    }

    public void PushToStack(ILogCapture capture)
    {
        _stack.Add(capture);
    }

    public void RemoveFromStack(ILogCapture capture)
    {
        _stack.Remove(capture);
    }

    public void AddParallel(ILogCapture capture)
    {
        _multiplex.Add(capture);
    }
}
