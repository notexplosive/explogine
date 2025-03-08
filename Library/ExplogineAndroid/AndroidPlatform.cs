using ExplogineMonoGame;

namespace ExplogineAndroid;

public class AndroidPlatform : IPlatformInterface
{
    public RealWindow PlatformWindow { get; } = new AndroidWindow();
}
