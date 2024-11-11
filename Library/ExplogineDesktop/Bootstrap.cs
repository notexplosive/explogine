using System.Diagnostics;
using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;

namespace ExplogineDesktop;

public static class Bootstrap
{
    public static void Run(string[] args, WindowConfig config, Func<IRuntime, Cartridge> cartridgeCreator, params string[] extraArgs)
    {
        Client.Debug.LogVerbose("Starting Bootstrap.Run");
        var combinedArgs = new List<string>();
        // extraArgs come first so args can overwrite them
        combinedArgs.AddRange(extraArgs);
        combinedArgs.AddRange(args);
        
        Client.Debug.LogVerbose($"Final args: {string.Join(" ", combinedArgs)}");

        Client.Start(combinedArgs.ToArray(), config, cartridgeCreator, new DesktopPlatform());
    }
}
