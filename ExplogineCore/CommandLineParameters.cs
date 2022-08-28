namespace ExplogineCore;

public class CommandLineParameters
{
    private readonly Dictionary<string, string> _givenArgsTable = new();
    private readonly HashSet<string> _boundArgs = new();
    
    public CommandLineArguments Args { get; }

    public CommandLineParameters(params string[] args)
    {
        Args = new CommandLineArguments(this);
        
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
    }

    internal Dictionary<string, object> RegisteredParameters { get; } = new();

    public void RegisterParameter<T>(string parameterName)
    {
        string value;
        var sanitizedParameterName = parameterName.ToLower();
        if (_givenArgsTable.ContainsKey(sanitizedParameterName))
        {
            _boundArgs.Add(sanitizedParameterName);
            value = _givenArgsTable[sanitizedParameterName];
            _givenArgsTable.Remove(sanitizedParameterName);
        }
        else
        {
            value = CommandLineParameters.GetDefaultAsString<T>();
        }

        if (typeof(T) == typeof(float))
        {
            RegisteredParameters.Add(sanitizedParameterName, float.Parse(value));
        }
        else if (typeof(T) == typeof(string))
        {
            RegisteredParameters.Add(sanitizedParameterName, value);
        }
        else if (typeof(T) == typeof(int))
        {
            RegisteredParameters.Add(sanitizedParameterName, int.Parse(value));
        }
        else if (typeof(T) == typeof(bool))
        {
            RegisteredParameters.Add(sanitizedParameterName, bool.Parse(value));
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

    internal bool HasValue(string name)
    {
        var sanitizedName = name.ToLower();
        return _boundArgs.Contains(sanitizedName);
    }
    
    internal List<string> UnboundArgs()
    {
        return _givenArgsTable.Keys.ToList();
    }
}
