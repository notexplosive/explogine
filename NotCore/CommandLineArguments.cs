using System;
using System.Collections.Generic;
using System.Linq;

namespace NotCore;

public class CommandLineArguments
{
    private readonly Dictionary<string, string> _argTable = new();
    private readonly Dictionary<string, ICommandLineParameter> _parameterValueTable = new();

    public CommandLineArguments() : this(Array.Empty<string>())
    {
        
    }
    
    public CommandLineArguments(string[] args)
    {
        foreach (var arg in args)
        {
            if (IsCommand(arg))
            {
                var argWithoutDashes = arg.Remove(0, 2);
                if (HasValue(argWithoutDashes))
                {
                    var split = argWithoutDashes.Split('=');
                    _argTable.Add(split[0], split[1]);
                }
                else
                {
                    _argTable.Add(argWithoutDashes, "true");
                }
            }
        }
    }

    private bool HasValue(string s)
    {
        return s.Contains('=');
    }

    private bool IsCommand(string arg)
    {
        return arg.StartsWith("--");
    }

    internal void BindToParameter(ICommandLineParameter commandLineParameter)
    {
        var name = commandLineParameter.Name;

        var argValue = GetArgTableEntry(name);
        if (argValue == null)
        {
            Console.WriteLine($"Unknown arg {name}");
            return;
        }

        try
        {
            switch (commandLineParameter)
            {
                case CommandLineBool commandLineBool:
                    commandLineBool.Value = bool.Parse(argValue);
                    break;
                case CommandLineInt commandLineInt:
                    commandLineInt.Value = int.Parse(argValue);
                    break;
                case CommandLineString commandLineString:
                    commandLineString.Value = argValue;
                    break;
            }
        }catch(Exception)
        {
            Console.WriteLine($"Parse failed for argument {name}, {argValue} is invalid");
        }

        _argTable.Remove(name);
        _parameterValueTable.Add(commandLineParameter.Name, commandLineParameter);
    }

    public T? GetCommandLineValue<T>(string name) where T : class, ICommandLineParameter
    {
        if (_parameterValueTable.ContainsKey(name))
        {
            return _parameterValueTable[name] as T;
        }

        return null;
    }

    internal List<string> UnboundArgs()
    {
        return _argTable.Keys.ToList();
    }

    private string? GetArgTableEntry(string name)
    {
        _argTable.TryGetValue(name, out var value);
        return value;
    }
}
