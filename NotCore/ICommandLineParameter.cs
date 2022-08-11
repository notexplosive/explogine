namespace NotCore;

public interface ICommandLineParameter
{
    public string Name { get; }
}

public class CommandLineBool : ICommandLineParameter
{
    public CommandLineBool(string name)
    {
        Name = name;
        Value = false;
    }

    public bool Value { get; set; }
    public string Name { get; }
}

public class CommandLineInt : ICommandLineParameter
{
    public CommandLineInt(string name)
    {
        Name = name;
        Value = 0;
    }

    public int Value { get; set; }
    public string Name { get; }
}

public class CommandLineString : ICommandLineParameter
{
    public CommandLineString(string name)
    {
        Name = name;
        Value = string.Empty;
    }

    public string Value { get; set; }
    public string Name { get; }
}