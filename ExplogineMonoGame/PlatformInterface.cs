using ExplogineCore;

namespace ExplogineMonoGame;

public interface IPlatformInterface
{
    IFileSystem FileSystem { get; }
    IWindow Window { get; }
}
