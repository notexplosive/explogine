using System;
using System.Collections.Generic;
using System.Linq;

namespace NotCore;

public class CommandLineArguments
{
    private readonly Dictionary<string, string> _givenArgsTable = new();
    private readonly Dictionary<string, object> _parametersWithValues = new();

    public CommandLineArguments(params string[] args)
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
    }

    public void AddParameter<T>(string parameterName)
    {
        string value;
        if (_givenArgsTable.ContainsKey(parameterName))
        {
            value = _givenArgsTable[parameterName];
            _givenArgsTable.Remove(parameterName);
        }
        else
        {
            value = CommandLineArguments.GetDefaultAsString<T>();
        }

        if (typeof(T) == typeof(float))
        {
            _parametersWithValues.Add(parameterName, float.Parse(value));
        }
        else if (typeof(T) == typeof(string))
        {
            _parametersWithValues.Add(parameterName, value);
        }
        else if (typeof(T) == typeof(int))
        {
            _parametersWithValues.Add(parameterName, int.Parse(value));
        }
        else if (typeof(T) == typeof(bool))
        {
            _parametersWithValues.Add(parameterName, bool.Parse(value));
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
        if (_parametersWithValues.ContainsKey(name))
        {
            return _parametersWithValues[name] is T
                ? (T) _parametersWithValues[name]
                : throw new Exception($"Wrong type requested for {name}");
        }

        throw new Exception($"Value is never set for {name}");
    }

    internal List<string> UnboundArgs()
    {
        return _givenArgsTable.Keys.ToList();
    }
}
