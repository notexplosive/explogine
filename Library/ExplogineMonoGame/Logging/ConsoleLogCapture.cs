using System;

namespace ExplogineMonoGame.Logging;

public class ConsoleLogCapture : ILogCapture
{
    public void CaptureMessage(LogMessage message)
    {
        Console.WriteLine(message.ToFileString());
    }
}
