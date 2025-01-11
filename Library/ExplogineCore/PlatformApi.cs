using System.Runtime.InteropServices;

namespace ExplogineCore;

public static class PlatformApi
{
    public static bool IsAppBundle
    {
        get
        {
            if (OperatingSystem() == SupportedOperatingSystem.MacOs)
            {
                return AppDomain.CurrentDomain.BaseDirectory.Contains(".app/Contents/MacOS/");
            }

            return false;
        }
    }

    public static SupportedOperatingSystem OperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return SupportedOperatingSystem.Windows;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return SupportedOperatingSystem.MacOs;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return SupportedOperatingSystem.Linux;
        }

        // Untested
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")))
        {
            // not supported (yet??)
            return SupportedOperatingSystem.Unknown;
        }

        // Untested
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")))
        {
            // not supported (yet??)
            return SupportedOperatingSystem.Unknown;
        }

        return SupportedOperatingSystem.Unknown;
    }
}
