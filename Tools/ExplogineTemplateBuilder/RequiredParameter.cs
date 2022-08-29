using ExplogineCore;

namespace ExplogineTemplateBuilder;

public class RequiredParameter<T> : RequiredParameter
{
    private readonly string _description;

    public RequiredParameter(string name, string description)
    {
        Name = name;
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
    public string Name { get; protected set; }
    public abstract void Bind(CommandLineParametersWriter writer);
    public abstract string HelpString();
}
