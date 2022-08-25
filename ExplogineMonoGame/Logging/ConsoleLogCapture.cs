using System;

namespace ExplogineMonoGame.Logging;

public class ConsoleLogCapture : ILogCapture
{
    public void CaptureMessage(string message)
    {
        Console.WriteLine(message);
    }
}