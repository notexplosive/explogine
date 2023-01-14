using ExplogineCore;
using ExplogineMonoGame;

namespace ExplogineDesktop;

public class DesktopPlatform : IPlatformInterface
{
    public PlatformAgnosticWindow PlatformWindow { get; } = new DesktopWindow();
    public IFileSystem ContentFileSystem { get; } = new RealFileSystem(Client.ContentFullPath);
}
