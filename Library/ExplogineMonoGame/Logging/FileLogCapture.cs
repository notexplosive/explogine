using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExplogineCore;

namespace ExplogineMonoGame.Logging;

public class FileLogCapture : ILogCapture
{
    private readonly List<LogMessage> _buffer = new();
    private readonly RealFileSystem _fileSystem;

    public FileLogCapture()
    {
        // This doesn't use Client.App.FileSystem because it might not be ready in time
        _fileSystem = new RealFileSystem(AppDomain.CurrentDomain.BaseDirectory);
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
        _fileSystem.WriteToFile(fileName, GetLines().ToArray());
    }

    private IEnumerable<string> GetLines()
    {
        foreach (var line in _buffer)
        {
            yield return line.ToFileString();
        }
    }
}
