using System.Text;

namespace ExplogineCore;

public class ParsedCommandLineArguments
{
    private readonly Dictionary<string, string> _givenArgsTable = new();
    private readonly Dictionary<string, object> _registeredParameters = new();
    private readonly HashSet<string> _usedArgs = new();

    public ParsedCommandLineArguments(params string[] args)
    {
        bool CommandHasValue(string s)
        {
            return s.Contains('=');
        }

        bool IsCommand(string arg)
        {
            return arg.StartsWith("--");
        }

        foreach (var arg in args)
        {
            var sanitizedArg = arg.ToLower();
            if (IsCommand(sanitizedArg))
            {
                var argWithoutDashes = sanitizedArg.Remove(0, 2);
                if (CommandHasValue(argWithoutDashes))
                {
                    var split = argWithoutDashes.Split('=');
                    _givenArgsTable.Add(split[0], split[1]);
                }
                else
                {
                    _givenArgsTable.Add(argWithoutDashes, "true");
                }
            }
        }

        RegisterParameter<bool>("help");
    }

    public string HelpOutput()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("Help:");
        foreach (var parameterPair in _registeredParameters)
        {
            stringBuilder.AppendLine(
                $"--{parameterPair.Key}=<{parameterPair.Value.GetType().Name}> (default: \"{parameterPair.Value}\")");
        }

        return stringBuilder.ToString();
    }

    public void RegisterParameter<T>(string parameterName)
    {
        string value;
        var sanitizedParameterName = parameterName.ToLower();
        if (_givenArgsTable.ContainsKey(sanitizedParameterName))
        {
            _usedArgs.Add(sanitizedParameterName);
            value = _givenArgsTable[sanitizedParameterName];
            _givenArgsTable.Remove(sanitizedParameterName);
        }
        else
        {
            value = ParsedCommandLineArguments.GetDefaultAsString<T>();
        }

        if (typeof(T) == typeof(float))
        {
            _registeredParameters.Add(sanitizedParameterName, float.Parse(value));
        }
        else if (typeof(T) == typeof(string))
        {
            _registeredParameters.Add(sanitizedParameterName, value);
        }
        else if (typeof(T) == typeof(int))
        {
            _registeredParameters.Add(sanitizedParameterName, int.Parse(value));
        }
        else if (typeof(T) == typeof(bool))
        {
            _registeredParameters.Add(sanitizedParameterName, bool.Parse(value));
        }
    }

    private static string GetDefaultAsString<T>()
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(float))
        {
            return "0";
        }

        if (typeof(T) == typeof(string))
        {
            return string.Empty;
        }

        if (typeof(T) == typeof(bool))
        {
            return "false";
        }

        throw new Exception("Unsupported type");
    }

    public bool HasValue(string name)
    {
        var sanitizedName = name.ToLower();
        return _usedArgs.Contains(sanitizedName);
    }

    public T GetValue<T>(string name)
    {
        var sanitizedName = name.ToLower();
        if (_registeredParameters.ContainsKey(sanitizedName))
        {
            return _registeredParameters[sanitizedName] is T
                ? (T) _registeredParameters[sanitizedName]
                : throw new Exception($"Wrong type requested for {sanitizedName}");
        }

        throw new Exception($"{sanitizedName} was never registered");
    }

    public List<string> UnboundArgs()
    {
        return _givenArgsTable.Keys.ToList();
    }
}
