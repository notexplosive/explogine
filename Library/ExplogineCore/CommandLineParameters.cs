namespace ExplogineCore;

public class CommandLineParameters
{
    private readonly HashSet<string> _boundArgs = new();
    private readonly Dictionary<string, string> _givenArgsTable = new();
    private readonly List<string> _orderedArgs = new();

    public CommandLineParameters(params string[] args)
    {
        Writer = new CommandLineParametersWriter(this);
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
            if (arg.Contains(' '))
            {
                throw new Exception("Individual args should not contain spaces, did you forget to use params?");
            }

            if (IsCommand(arg))
            {
                var argWithoutDashes = arg.Remove(0, 2);
                if (CommandHasValue(argWithoutDashes))
                {
                    var split = argWithoutDashes.Split('=');
                    _givenArgsTable[split[0].ToLower()] = split[1];
                }
                else
                {
                    _givenArgsTable[argWithoutDashes.ToLower()] = "true";
                }
            }
            else
            {
                _orderedArgs.Add(arg);
            }
        }
    }

    public CommandLineArguments Args { get; }
    public CommandLineParametersWriter Writer { get; }

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
            value = GetDefaultAsString<T>();
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

    internal IEnumerable<string> OrderedArgs()
    {
        return _orderedArgs;
    }
}
