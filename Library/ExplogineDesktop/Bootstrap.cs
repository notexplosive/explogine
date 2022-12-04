using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;

namespace ExplogineDesktop;

public static class Bootstrap
{
    public static void Run(string[] args, WindowConfig config, ICartridge gameCartridge, params string[] extraArgs)
    {
        var combinedArgs = new List<string>();
        // extraArgs come first so args can overwrite them
        combinedArgs.AddRange(extraArgs);
        combinedArgs.AddRange(args);

        Client.Start(combinedArgs.ToArray(), config, gameCartridge, new DesktopPlatform());
    }
}
