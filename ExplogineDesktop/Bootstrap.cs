using ExplogineCore;
using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;

namespace ExplogineDesktop;

public static class Bootstrap
{
    public static void Run(string[] args, WindowConfig config, ICartridge gameCartridge)
    {
        Client.Start(args, config, gameCartridge, new DesktopPlatform());
    }
}
