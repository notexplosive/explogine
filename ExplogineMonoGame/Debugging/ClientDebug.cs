using System.Text;
using ExplogineMonoGame.Logging;

namespace ExplogineMonoGame.Debugging;

public class ClientDebug
{
    public LogOutput Output { get; } = new();
    public DebugLevel DebugLevel { get; set; }
    public bool IsActive => DebugLevel == DebugLevel.Active || IsPassive;
    public bool IsPassive => DebugLevel == DebugLevel.Passive;

    public void Log(object firstObject, params object[] paramsObjects)
    {
        var output = new StringBuilder();

        output.Append(firstObject);

        foreach (var param in paramsObjects)
        {
            output.Append("  ");
            output.Append(param);
        }

        Output.Emit(output.ToString());
    }
}