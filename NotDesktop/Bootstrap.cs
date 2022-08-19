using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;

namespace NotDesktop;

public static class Bootstrap
{
    public static void Run(string[] args, ICartridge gameCartridge)
    {
        Client.Start(args, gameCartridge, new DesktopFileSystem());
    }
}
