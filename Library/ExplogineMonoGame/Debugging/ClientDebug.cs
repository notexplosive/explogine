using System;
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

        DeveloperConsole.AddCommand("Help",
            Array.Empty<DeveloperConsole.ICommandParameter>(),
            args =>
            {
                foreach (var command in DeveloperConsole.Commands)
                {
                    Client.Debug.Log(command.Name);
                }
            });
        DeveloperConsole.AddCommand("Add",
            new DeveloperConsole.ICommandParameter[]
            {
                new DeveloperConsole.IntCommandParameter("Left"),
                new DeveloperConsole.IntCommandParameter("Right")
            },
            args =>
            {
                Client.Debug.Log(
                    (args[0] as DeveloperConsole.CommandArgumentValue<int>).Value +
                    (args[1] as DeveloperConsole.CommandArgumentValue<int>).Value
                );
            });
        
        DeveloperConsole.AddCommand("Echo",
            new DeveloperConsole.ICommandParameter[]
            {
                new DeveloperConsole.StringCommandParameter("Message"),
            },
            args =>
            {
                Client.Debug.Log((args[0] as DeveloperConsole.CommandArgumentValue<string>).Value);
            });
    }

    public LogOutput Output { get; } = new();
    public DebugLevel Level { get; internal set; }
    public bool IsActive => Level == DebugLevel.Active;
    public bool IsPassiveOrActive => Level == DebugLevel.Passive || IsActive;
    public FileLogCapture LogFile { get; }
    public int GameSpeed { get; set; } = 1;
    public DeveloperConsole DeveloperConsole { get; } = new();

    public void CycleDebugMode()
    {
        switch (Level)
        {
            case DebugLevel.None:
                Level = DebugLevel.Passive;
                break;
            case DebugLevel.Passive:
                Level = DebugLevel.Active;
                break;
            case DebugLevel.Active:
                Level = DebugLevel.Passive;
                break;
        }

        Client.Debug.Log($"Debug level set to {Level}");
    }

    public bool LaunchedAsDebugMode()
    {
        if (Client.Args.HasValue("debug"))
        {
            return Client.Args.GetValue<bool>("debug");
        }

#if DEBUG
        return true;
#else
        return false;
#endif
    }

    public void Log(object? message, params object?[] paramsObjects)
    {
        var output = CreateOutputString(message, paramsObjects);
        Output.Emit(new LogMessage(LogMessageType.Info, output));
    }

    public void LogError(object? message, params object?[] paramsObjects)
    {
        var output = CreateOutputString(message, paramsObjects);
        Output.Emit(new LogMessage(LogMessageType.Fail, output));
    }

    public void LogWarning(object? message, params object?[] paramsObjects)
    {
        var output = CreateOutputString(message, paramsObjects);
        Output.Emit(new LogMessage(LogMessageType.Warn, output));
    }

    private string CreateOutputString(object? message, params object?[] paramsObjects)
    {
        var output = new StringBuilder();

        output.Append(message);

        foreach (var param in paramsObjects)
        {
            output.Append("  ");
            output.Append(param);
        }

        return output.ToString();
    }
}
