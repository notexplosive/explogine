namespace Executor;

public class DotnetProgram : ExternalProgram
{
    public DotnetProgram(string workingDirectory, ILogger logger) : base("dotnet", workingDirectory, logger)
    {
    }

    public ProgramOutput PublishExe_Special(string csprojPath, string outputDirectory)
    {
        return RunWithArgs(ProgramOutputLevel.AllowToConsole,
            "publish",
            csprojPath,
            "-c", "Release",
            "-r", "win-x64",
            "/p:PublishReadyToRun=false",
            "/p:TieredCompilation=false",
            "/p:IncludeNativeLibrariesForSelfExtract=true",
            "--self-contained",
            "--output", outputDirectory);
    }

    /// <summary>
    ///     This is the normal publish we used to run from release_build.bat
    /// </summary>
    /// <param name="csprojPath"></param>
    /// <param name="outputDirectory"></param>
    /// <returns></returns>
    public ProgramOutput NormalPublish(string csprojPath, string outputDirectory)
    {
        return RunWithArgs(ProgramOutputLevel.AllowToConsole,
            "publish",
            csprojPath,
            "-c", "Release",
            "-r", "win-x64",
            "/p:PublishReadyToRun=false",
            "/p:TieredCompilation=false",
            "--self-contained",
            "--output", outputDirectory);
    }

    public ProgramOutput Version()
    {
        return RunWithArgs(ProgramOutputLevel.AllowToConsole, "--version");
    }

    public bool Exists()
    {
        return RunWithArgs(ProgramOutputLevel.SuppressFromConsole, "--version").WasSuccessful;
    }

    public void AddToSln(ProgramOutputLevel outputLevel, string path)
    {
        RunWithArgs(outputLevel, "sln", "add", path);
    }

    public void NewSln(ProgramOutputLevel outputLevel)
    {
        RunWithArgs(outputLevel, "new", "sln");
    }
}
