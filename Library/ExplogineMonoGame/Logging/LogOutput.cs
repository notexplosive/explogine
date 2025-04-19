using System.Collections.Generic;

namespace ExplogineMonoGame.Logging;

public class LogOutput
{
    private readonly List<ILogCapture> _multiplex = new();
    private readonly object _multiplexLock = new();
    private readonly List<ILogCapture> _stack = new();

    private readonly object _stackLock = new();

    internal void Emit(LogMessage message)
    {
        lock (_stackLock)
        {
            if (_stack.Count > 0)
            {
                var topOfStack = _stack[^1];
                {
                    topOfStack.CaptureMessage(message);
                }
            }
        }

        lock (_multiplexLock)
        {
            foreach (var item in _multiplex)
            {
                item.CaptureMessage(message);
            }
        }
    }

    public void PushToStack(ILogCapture capture)
    {
        lock (_stackLock)
        {
            _stack.Add(capture);
        }
    }

    public void RemoveFromStack(ILogCapture capture)
    {
        lock (_stackLock)
        {
            _stack.Remove(capture);
        }
    }

    public void AddParallel(ILogCapture capture)
    {
        lock (_multiplexLock)
        {
            _multiplex.Add(capture);
        }
    }
}
