using System.Text;

namespace ExplogineCore;

public class CommandLineArguments
{
    public CommandLineArguments(params string[] args)
    {
        Registry = new CommandLineParameters(args);
    }

    public CommandLineParameters Registry { get; }

    public string HelpOutput()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("Help:");
        foreach (var parameterPair in Registry.RegisteredParameters)
        {
            stringBuilder.AppendLine(
                $"--{parameterPair.Key}=<{parameterPair.Value.GetType().Name}> (default: \"{parameterPair.Value}\")");
        }

        return stringBuilder.ToString();
    }

    public T GetValue<T>(string name)
    {
        var sanitizedName = name.ToLower();
        if (Registry.RegisteredParameters.ContainsKey(sanitizedName))
        {
            return Registry.RegisteredParameters[sanitizedName] is T
                ? (T) Registry.RegisteredParameters[sanitizedName]
                : throw new Exception($"Wrong type requested for {sanitizedName}");
        }

        throw new Exception($"{sanitizedName} was never registered");
    }

    public bool HasValue(string arg)
    {
        return Registry.HasValue(arg);
    }

    public List<string> UnboundArgs()
    {
        return Registry.UnboundArgs();
    }
}
