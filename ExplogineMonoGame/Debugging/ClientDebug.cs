using System.Text;
using ExplogineMonoGame.Logging;

namespace ExplogineMonoGame.Debugging;

public class ClientDebug
{
    public ClientDebug()
    {
        LogFile = new FileLogCapture();
        Output.AddParallel(LogFile);
        Output.PushToStack(new ConsoleLogCapture());
    }

    public LogOutput Output { get; } = new();
    public DebugLevel Level { get; internal set; }
    public bool IsActive => Level == DebugLevel.Active;
    public bool IsPassive => Level == DebugLevel.Passive || IsActive;
    public FileLogCapture LogFile { get; }

    public void Log(object message, params object[] paramsObjects)
    {
        var output = new StringBuilder();

        output.Append(message);

        foreach (var param in paramsObjects)
        {
            output.Append("  ");
            output.Append(param);
        }

        Output.Emit(output.ToString());
    }
}
