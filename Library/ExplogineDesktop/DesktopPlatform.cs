using ExplogineCore;
using ExplogineMonoGame;

namespace ExplogineDesktop;

public class DesktopPlatform : IPlatformInterface
{
    public AbstractWindow AbstractWindow { get; } = new DesktopWindow();
    public IFileSystem ContentFileSystem { get; } = new RealFileSystem(Client.ContentFullPath);
}
