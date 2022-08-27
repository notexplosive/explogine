using ExplogineCore;
using ExplogineMonoGame;

namespace ExplogineDesktop;

public class DesktopPlatform : IPlatformInterface
{
    public IFileSystem FileSystem { get; } = new DesktopFileSystem();
    public AbstractWindow AbstractWindow { get; } = new DesktopWindow();
}