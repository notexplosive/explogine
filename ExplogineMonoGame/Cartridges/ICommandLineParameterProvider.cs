using ExplogineCore;

namespace ExplogineMonoGame.Cartridges;

public interface ICommandLineParameterProvider
{
    public void AddCommandLineParameters(CommandLineParameters parameters);
}
