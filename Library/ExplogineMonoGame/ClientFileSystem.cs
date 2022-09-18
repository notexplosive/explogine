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