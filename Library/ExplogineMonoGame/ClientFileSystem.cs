using System.Collections.Generic;
using System.IO;
using ExplogineCore;

namespace ExplogineMonoGame;

public class ClientFileSystem
{
    public ClientFileSystem()
    {
        AppData = new VirtualFileSystem();
        Local = new VirtualFileSystem();
    }

    public ClientFileSystem(IFileSystem local, IFileSystem appData)
    {
        Local = local;
        AppData = appData;
    }

    public IFileSystem AppData { get; }
    public IFileSystem Local { get; }
}

public class VirtualFileSystem : IFileSystem
{
    public bool HasFile(string relativePathToFile)
    {
        return false;
    }

    public void CreateFile(string relativePathToFile)
    {
    }

    public void CreateOrOverwriteFile(string relativePathToFile)
    {
    }

    public void AppendToFile(string relativePathToFile, params string[] lines)
    {
    }

    public string ReadFile(string relativePathToFile)
    {
        return string.Empty;
    }

    public List<string> GetFilesAt(string targetRelativePath, string extension = "*", bool recursive = true)
    {
        return new List<string>();
    }

    public void WriteToFile(string relativeFileName, params string[] lines)
    {
    }
}
