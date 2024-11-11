using System.Text;

namespace ExplogineCore;

public class CommandLineArguments
{
    private readonly CommandLineParameters _parameters;

    internal CommandLineArguments(CommandLineParameters parameters)
    {
        _parameters = parameters;
    }

    public string HelpOutput()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("Help:");
        foreach (var parameterPair in _parameters.RegisteredParameters)
        {
            stringBuilder.AppendLine(
                $"--{parameterPair.Key}=<{parameterPair.Value.GetType().Name}> (default: \"{parameterPair.Value}\")");
        }

        return stringBuilder.ToString();
    }

    public T GetValue<T>(string name)
    {
        var sanitizedName = name.ToLower();
        if (_parameters.RegisteredParameters.ContainsKey(sanitizedName))
        {
            return _parameters.RegisteredParameters[sanitizedName] is T
                ? (T) _parameters.RegisteredParameters[sanitizedName]
                : throw new Exception($"Wrong type requested for {sanitizedName}");
        }

        throw new Exception($"{sanitizedName} was never registered");
    }

    public bool HasValue(string arg)
    {
        return _parameters.HasValue(arg);
    }

    /// <summary>
    /// Args that were passed to the command line but don't map to anything understood by the application
    /// </summary>
    public List<string> UnboundArgs()
    {
        return _parameters.UnboundArgs();
    }

    /// <summary>
    /// Args that do not have dashes (eg: a file path)
    /// </summary>
    public IEnumerable<string> OrderedArgs()
    {
        return _parameters.OrderedArgs();
    }
}
