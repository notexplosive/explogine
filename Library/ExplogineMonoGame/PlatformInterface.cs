using ExplogineCore;

namespace ExplogineMonoGame;

public interface IPlatformInterface
{
    AbstractWindow AbstractWindow { get; }
    IFileSystem ContentFileSystem { get; }
    IFileSystem LocalFileSystem { get; }
}
