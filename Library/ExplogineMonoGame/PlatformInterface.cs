using ExplogineCore;

namespace ExplogineMonoGame;

public interface IPlatformInterface
{
    RealWindow PlatformWindow { get; }
    IFileSystem ContentFileSystem { get; }
}
