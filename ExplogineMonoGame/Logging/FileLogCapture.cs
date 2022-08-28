using System;
using System.Collections.Generic;
using System.IO;

namespace ExplogineMonoGame.Logging;

public class FileLogCapture : ILogCapture
{
    private readonly List<string> _buffer = new();

    public FileLogCapture()
    {
        Client.Exited.Add(DumpBufferWithTimestamp);
    }

    public void CaptureMessage(string text)
    {
        _buffer.Add(text);
    }

    private void DumpBufferWithTimestamp()
    {
        var directory = "logs";
        Directory.CreateDirectory(directory);
        var fileName = Path.Join(directory, $"{DateTime.Now.ToFileTimeUtc()}.log");

        WriteBufferAsFilename(fileName);
    }

    public void WriteBufferAsFilename(string fileName)
    {
        Client.Debug.Log($"Creating file {fileName}");
        Client.FileSystem.WriteFileToWorkingDirectory(fileName, string.Join(Environment.NewLine, _buffer));
    }
}
