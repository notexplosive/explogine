using System;
using System.Diagnostics;
using ExplogineCore;

namespace ExplogineMonoGame.Data;

public static class PlatformFileApi
{
    public static string? OpenFileDialogue(string title, ExtensionDescription extensionDescription)
    {
        if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.Linux)
        {
            return LinuxChooseFile(false, title, extensionDescription);
        }

        if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.MacOs)
        {
            return MacChooseFile(false, title, extensionDescription);
        }

        if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.Windows)
        {
            return WindowsChooseFile(false, title, extensionDescription);
        }

        Client.Debug.LogError("Unsupported Operating System");
        return null;
    }

    public static string? SaveFileDialogue(string title, ExtensionDescription extensionDescription)
    {
        if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.Linux)
        {
            return LinuxChooseFile(true, title, extensionDescription);
        }

        if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.MacOs)
        {
            return MacChooseFile(true, title, extensionDescription);
        }

        if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.Windows)
        {
            return WindowsChooseFile(true, title, extensionDescription);
        }

        Client.Debug.LogError("Unsupported Operating System");
        return null;
    }

    private static string? WindowsChooseFile(bool isSave, string title, ExtensionDescription extensionDescription)
    {
        var fileName = string.Empty;
        var openFileNameStruct = WindowsIntegration.CreateOpenFileName(title, extensionDescription);

        if (isSave)
        {
            if (WindowsIntegration.GetSaveFileName(openFileNameStruct))
            {
                fileName = openFileNameStruct.lpstrFile;
            }
        }
        else
        {
            if (WindowsIntegration.GetOpenFileName(openFileNameStruct))
            {
                fileName = openFileNameStruct.lpstrFile;
            }
        }

        if (!string.IsNullOrEmpty(fileName))
        {
            return EnsureHasExtension(fileName, extensionDescription);
        }

        return null;
    }

    private static string? EnsureHasExtension(string? input, ExtensionDescription extensionDescription)
    {
        var extension = "." + extensionDescription.ExtensionWithoutDot;
        if (input != null && !input.EndsWith(extension))
        {
            return input + extension;
        }

        return input;
    }

    private static string? LinuxChooseFile(bool isSave, string title, ExtensionDescription description)
    {
        using var process = RunShellProgram("zenity",
            $"--file-selection {(isSave ? "--save --confirm-overwrite" : "")} --file-filter=\"{description.Name} | *.{description.ExtensionWithoutDot}\" --title=\"{title}\"");

        var selectedFile = process?.StandardOutput.ReadLine();
        if (!string.IsNullOrEmpty(selectedFile))
        {
            return EnsureHasExtension(selectedFile, description);
        }

        return null;
    }

    private static string? MacChooseFile(bool isSave, string title, ExtensionDescription description)
    {
        var fileExtension = description.ExtensionWithoutDot;

        if (isSave)
        {
            return RunAppleScript($@"
POSIX path of (choose file name with prompt ""{title}"" default name ""Untitled.{description.ExtensionWithoutDot}"")
");
        }

        return RunAppleScript($@"
try
    POSIX path of (choose file of type {{""png"", ""{fileExtension}""}} with prompt ""{title}"")
on error
    return """"
end try
");
    }

    private static string? RunAppleScript(string appleScript)
    {
        var escapedScript = appleScript.Replace("\"", "\\\"");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "osascript",
                Arguments = $"-e \"{escapedScript}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        // Execute the process and capture the output
        process.Start();
        var result = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        if (string.IsNullOrEmpty(result))
        {
            return null;
        }

        return result;
    }

    private static Process? RunShellProgram(string programName, string finalArgs)
    {
        Client.Debug.Log($"{programName} {finalArgs}");
        Console.WriteLine($"{programName} {finalArgs}");
        var processStartInfo = new ProcessStartInfo
        {
            FileName = programName,
            Arguments = finalArgs,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return Process.Start(processStartInfo);
    }

    public static void OpenBrowser(string url)
    {
        switch (PlatformApi.OperatingSystem())
        {
            case SupportedOperatingSystem.Windows:
                Process.Start("explorer", url);
                break;
            case SupportedOperatingSystem.Linux:
                Process.Start("xdg-open", url);
                break;
            case SupportedOperatingSystem.MacOs:
                Process.Start("open", url);
                break;
        }
    }

    public readonly record struct ExtensionDescription(string ExtensionWithoutDot, string Name);
}
