using ExplogineCore;

namespace NewProject;

public class RequiredParameter<T> : RequiredParameter
{
    private readonly string _description;

    public RequiredParameter(string name, string description) : base(name)
    {
        _description = description;
    }

    public override void Bind(CommandLineParametersWriter writer)
    {
        writer.RegisterParameter<T>(Name);
    }

    public override string HelpString()
    {
        return $"{Name}: {_description}";
    }
}

public abstract class RequiredParameter
{
    protected RequiredParameter(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public abstract void Bind(CommandLineParametersWriter writer);
    public abstract string HelpString();
}
