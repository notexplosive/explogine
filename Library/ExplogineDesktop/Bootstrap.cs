using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;

namespace ExplogineDesktop;

public static class Bootstrap
{
    public static void Run(string[] args, WindowConfig config, ICartridge gameCartridge, params string[] extraArgs)
    {
        var combinedArgs = new List<string>();
        combinedArgs.AddRange(args);
        combinedArgs.AddRange(extraArgs);

        Client.Start(combinedArgs.ToArray(), config, gameCartridge, new DesktopPlatform());
    }
}
