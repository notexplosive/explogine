namespace ExplogineCore;

public class CommandLineParametersWriter
{
    private readonly CommandLineParameters _commandLineParameters;

    public CommandLineParametersWriter(CommandLineParameters parameters)
    {
        _commandLineParameters = parameters;
    }

    public void RegisterParameter<T>(string parameterName)
    {
        _commandLineParameters.RegisterParameter<T>(parameterName);
    }
}
