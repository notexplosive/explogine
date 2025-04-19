using ExplogineMonoGame;

namespace ExplogineDesktop;

public class DesktopPlatform : IPlatformInterface
{
    public RealWindow PlatformWindow { get; } = new DesktopWindow();
}
