namespace Executor;

public class ButlerProgram : ExternalProgram
{
    public ButlerProgram(ILogger logger, string workingDirectory) : base("butler", workingDirectory, logger)
    {
    }

    public void Logout()
    {
        RunWithArgs(ProgramOutputLevel.AllowToConsole, "logout", "--assume-yes");
    }

    public void Login()
    {
        RunWithArgs(ProgramOutputLevel.AllowToConsole, "login");
    }

    public ProgramOutput Version()
    {
        return RunWithArgs(ProgramOutputLevel.AllowToConsole, "--version");
    }

    public ProgramOutput Push(string directoryToUpload, string itchUrl, string gameUrl, string channel)
    {
        return RunWithArgs(ProgramOutputLevel.AllowToConsole, "push", directoryToUpload,
            $"{itchUrl}/{gameUrl}:{channel}");
    }

    public bool Exists()
    {
        return RunWithArgs(ProgramOutputLevel.SuppressFromConsole, "--version").WasSuccessful;
    }
}
