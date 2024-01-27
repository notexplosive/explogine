using ExplogineCore;

namespace ExplogineMonoGame.Cartridges;

public interface ICommandLineParameterProvider
{
    /// <summary>
    ///     Dynamically add new command line parameters
    /// </summary>
    /// <param name="parameters"></param>
    public void AddCommandLineParameters(CommandLineParametersWriter parameters);
}
