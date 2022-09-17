using ExplogineCore;
using ExplogineMonoGame;

namespace ExplogineDesktop;

public class DesktopPlatform : IPlatformInterface
{
    public AbstractWindow AbstractWindow { get; } = new DesktopWindow();
    public IFileSystem ContentFileSystem { get; } = new RealFileSystem(Client.ContentFullPath);
    public IFileSystem LocalFileSystem { get; } = new RealFileSystem(AppDomain.CurrentDomain.BaseDirectory);
}
