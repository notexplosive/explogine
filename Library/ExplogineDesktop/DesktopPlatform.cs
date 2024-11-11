using ExplogineCore;
using ExplogineMonoGame;

namespace ExplogineDesktop;

public class DesktopPlatform : IPlatformInterface
{
    public RealWindow PlatformWindow { get; } = new DesktopWindow();
    
    [Obsolete("This will not work on macOS")]
    public IFileSystem ContentFileSystem { get; } = new RealFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content"));
}
