using System.Text;

namespace ExplogineCore;

public class ParsedCommandLineArguments
{
    private readonly Dictionary<string, string> _givenArgsTable = new();
    private readonly Dictionary<string, object> _registeredParameters = new();

    public ParsedCommandLineArguments(params string[] args)
    {
        bool HasValue(string s)
        {
            return s.Contains('=');
        }

        bool IsCommand(string arg)
        {
            return arg.StartsWith("--");
        }
        
        foreach (var arg in args)
        {
            if (IsCommand(arg))
            {
                var argWithoutDashes = arg.Remove(0, 2);
                if (HasValue(argWithoutDashes))
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
            stringBuilder.AppendLine($"--{parameterPair.Key}=<{parameterPair.Value.GetType().Name}> (default: \"{parameterPair.Value}\")");
        }
        
        return stringBuilder.ToString();
    }

    public void RegisterParameter<T>(string parameterName)
    {
        string value;
        if (_givenArgsTable.ContainsKey(parameterName))
        {
            value = _givenArgsTable[parameterName];
            _givenArgsTable.Remove(parameterName);
        }
        else
        {
            value = ParsedCommandLineArguments.GetDefaultAsString<T>();
        }

        if (typeof(T) == typeof(float))
        {
            _registeredParameters.Add(parameterName, float.Parse(value));
        }
        else if (typeof(T) == typeof(string))
        {
            _registeredParameters.Add(parameterName, value);
        }
        else if (typeof(T) == typeof(int))
        {
            _registeredParameters.Add(parameterName, int.Parse(value));
        }
        else if (typeof(T) == typeof(bool))
        {
            _registeredParameters.Add(parameterName, bool.Parse(value));
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

    public T GetValue<T>(string name)
    {
        if (_registeredParameters.ContainsKey(name))
        {
            return _registeredParameters[name] is T
                ? (T) _registeredParameters[name]
                : throw new Exception($"Wrong type requested for {name}");
        }

        throw new Exception($"Value is never set for {name}");
    }

    public List<string> UnboundArgs()
    {
        return _givenArgsTable.Keys.ToList();
    }
}
