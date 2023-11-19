using System.Collections.Generic;
using ExplogineCore;

namespace ExplogineMonoGame.TextFormatting;

public static class FormattingCommands
{
    internal static readonly Dictionary<string, ICommand> SupportedCommands =
        Reflection.GetStaticFieldsThatDeriveFromType<BuiltInCommands, ICommand>();

    public static void SetScopedCommand(string commandName, ScopedCommand command)
    {
        FormattingCommands.SupportedCommands[commandName] = command;
    }

    public static void SetCommand(string commandName, Command command)
    {
        FormattingCommands.SupportedCommands[commandName] = command;
    }
}
