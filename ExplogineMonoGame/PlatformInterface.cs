using ExplogineCore;

namespace ExplogineMonoGame;

public interface IPlatformInterface
{
    IFileSystem FileSystem { get; }
    AbstractWindow AbstractWindow { get; }
}
