using NotCore;

namespace NotDesktop;

public static class Bootstrap
{
    public static void Run(ICartridge gameCartridge)
    {
        Client.Start(gameCartridge, new DesktopFileSystem());
    }
}
