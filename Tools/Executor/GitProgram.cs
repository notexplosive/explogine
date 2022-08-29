namespace Executor;

public class GitProgram : ExternalProgram
{
    public GitProgram(string workingDirectory, ILogger logger) : base("git", workingDirectory, logger)
    {
    }

    public void Init(ProgramOutputLevel outputLevel = ProgramOutputLevel.SuppressFromConsole)
    {
        RunWithArgs(outputLevel, "init");
    }

    public ProgramOutput Version()
    {
        return RunWithArgs(ProgramOutputLevel.AllowToConsole, "--version");
    }

    public bool Exists()
    {
        return RunWithArgs(ProgramOutputLevel.SuppressFromConsole, "--version").WasSuccessful;
    }

    public void AddAll(ProgramOutputLevel outputLevel)
    {
        RunWithArgs(outputLevel, "add", ".");
    }

    public void CommitWithMessage(ProgramOutputLevel outputLevel, string commitMessage)
    {
        RunWithArgs(outputLevel, "commit", "-m", commitMessage);
    }

    public void AddSubmodule(ProgramOutputLevel outputLevel, string url)
    {
        RunWithArgs(outputLevel, "submodule", "add", url);
    }
}
