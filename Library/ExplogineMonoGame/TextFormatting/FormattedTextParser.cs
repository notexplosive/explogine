using System.Collections.Generic;

namespace ExplogineMonoGame.TextFormatting;

public class FormattedTextParser
{
    private readonly Dictionary<string, ICommand> _commands = FormattingCommands.DefaultCommands;

    public static FormattedTextParser Default { get; } = new();

    public void AddCommand(string commandName, ICommand command)
    {
        _commands[commandName] = command;
    }

    public IEnumerable<KeyValuePair<string, ICommand>> GetCommands()
    {
        return _commands;
    }
}
