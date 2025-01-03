using System;

namespace ExplogineMonoGame.Logging;

public class ConsoleLogCapture : ILogCapture
{
    private readonly LogMessageType _filter;

    public ConsoleLogCapture(LogMessageType filter)
    {
        _filter = filter;
    }
    
    public void CaptureMessage(LogMessage message)
    {
        if (message.Type.HasFlag(_filter))
        {
            Console.WriteLine(message.ToFileString());
        }
    }
}
