using System;

namespace ExplogineMonoGame.Logging;

public class ConsoleLogCapture : ILogCapture
{
    public void CaptureMessage(LogMessage text)
    {
        Console.WriteLine(text.ToFileString());
    }
}
