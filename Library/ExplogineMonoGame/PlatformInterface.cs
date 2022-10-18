using ExplogineCore;

namespace ExplogineMonoGame;

public interface IPlatformInterface
{
    PlatformAgnosticWindow PlatformAgnosticWindow { get; }
    IFileSystem ContentFileSystem { get; }
}
