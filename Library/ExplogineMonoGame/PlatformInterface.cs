using ExplogineCore;

namespace ExplogineMonoGame;

public interface IPlatformInterface
{
    PlatformAgnosticWindow PlatformWindow { get; }
    IFileSystem ContentFileSystem { get; }
}
