namespace Executor;

public class ProgramOutput
{
    public bool WasSuccessful { get; }

    public ProgramOutput(bool wasSuccessful)
    {
        WasSuccessful = wasSuccessful;
    }
}
