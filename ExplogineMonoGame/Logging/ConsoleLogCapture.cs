using System;

namespace ExplogineMonoGame.Logging;

public class ConsoleLogCapture : ILogCapture
{
    public void CaptureMessage(string text)
    {
        Console.WriteLine(text);
    }
}