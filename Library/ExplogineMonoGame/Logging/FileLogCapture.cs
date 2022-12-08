using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExplogineMonoGame.Logging;

public class FileLogCapture : ILogCapture
{
    private readonly List<LogMessage> _buffer = new();

    public FileLogCapture()
    {
        Client.Exited.Add(DumpBufferWithTimestamp);
    }

    public void CaptureMessage(LogMessage text)
    {
        _buffer.Add(text);
    }

    private void DumpBufferWithTimestamp()
    {
        var directory = "Logs";
        var fileName = Path.Join(directory, $"{DateTime.Now.ToFileTimeUtc()}.log");

        WriteBufferAsFilename(fileName);
    }

    public void WriteBufferAsFilename(string fileName)
    {
        Client.Debug.Log($"Creating file {fileName}");
        Client.FileSystem.Local.WriteToFile(fileName, GetLines().ToArray());
    }

    private IEnumerable<string> GetLines()
    {
        foreach (var line in _buffer)
        {
            yield return line.ToFileString();
        }
    }
}
