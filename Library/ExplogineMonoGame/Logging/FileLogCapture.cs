using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExplogineCore;

namespace ExplogineMonoGame.Logging;

public class FileLogCapture : ILogCapture
{
    private readonly List<LogMessage> _buffer = new();
    private readonly RealFileSystem.StreamDescriptor _stream;

    public FileLogCapture()
    {
        // This doesn't use ClientFileSystem because it might not be ready in time
        Directory = new RealFileSystem(Client.AppDataFullPath);
        var fileName = Path.Join("Logs", $"{DateTime.Now.ToFileTimeUtc()}.log");
        _stream = Directory.OpenFileStream(fileName);
        Client.Exited.Add(_stream.Close);
    }

    public RealFileSystem Directory { get; }

    public void CaptureMessage(LogMessage message)
    {
        _buffer.Add(message);
        _stream.Write(message.ToFileString());
        
        // Flush on every write, even on release builds
        _stream.Flush();
    }

    public void WriteBufferAsFilename(string fileName)
    {
        Directory.WriteToFile(fileName, GetLines().ToArray());
    }

    private IEnumerable<string> GetLines()
    {
        foreach (var line in _buffer)
        {
            yield return line.ToFileString();
        }
    }
}
