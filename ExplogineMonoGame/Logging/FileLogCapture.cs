using System;
using System.Collections.Generic;

namespace ExplogineMonoGame.Logging;

public class FileLogCapture : ILogCapture
{
    private List<string> _buffer = new();

    public FileLogCapture()
    {
        Client.Exited.Add(WriteBuffer);
    }
    
    public void CaptureMessage(string message)
    {
        _buffer.Add(message);
    }

    private void WriteBuffer()
    {
        var timeSpan = DateTime.Now - DateTime.UnixEpoch;
        var fileName = $"explogine-{Math.Floor(timeSpan.TotalMilliseconds)}.log";
        
        Client.Debug.Log($"Creating file {fileName}");
        Client.FileSystem.WriteFileToWorkingDirectory(fileName, string.Join(Environment.NewLine, _buffer));
    }
}
