using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;

namespace ExplogineDesktop;

public static class Bootstrap
{
    public static void Run(string[] args, ICartridge gameCartridge)
    {
        Client.Start(args, gameCartridge, new DesktopFileSystem());
    }
}
