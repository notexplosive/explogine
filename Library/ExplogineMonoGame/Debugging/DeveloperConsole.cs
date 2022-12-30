using System;
using System.Collections.Generic;
using System.Text;

namespace ExplogineMonoGame.Debugging;

public class DeveloperConsole
{
    public List<ConsoleCommand> Commands { get; } = new();

    public void AddCommand(string name, ICommandParameter[] parameters, Action<ICommandArgument[]> action)
    {
        Commands.Add(new ConsoleCommand(name, parameters, action));
    }

    public void ReceiveCommand(params string[] tokens)
    {
        var command = FindCommandByName(tokens[0]);
        if (command.HasValue)
        {
            try
            {
                command.Value.ExecuteWithArgs(tokens);
            }
            catch (Exception e)
            {
                Client.Debug.LogError(e.Message);
            }
        }
        else
        {
            Client.Debug.LogError($"Unrecognized command: {tokens[0]}");
        }
    }

    private ConsoleCommand? FindCommandByName(string token)
    {
        foreach (var command in Commands)
        {
            if (string.Compare(token, command.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return command;
            }
        }

        return null;
    }

    public readonly record struct ConsoleCommand(string Name, ICommandParameter[] Parameters,
        Action<ICommandArgument[]> Action)
    {
        public void ExecuteWithArgs(string[] tokens)
        {
            if (tokens.Length - 1 != Parameters.Length)
            {
                throw new Exception($"Wrong number of arguments, got {tokens.Length - 1}, needed {Parameters.Length}\nExpected usage: {Usage()}");
            }

            var finalArguments = new ICommandArgument[Parameters.Length];

            for (var i = 0; i < Parameters.Length; i++)
            {
                // tokens[0] is the name of the command, so we +1 the indexer
                var argString = tokens[i + 1];
                var parameter = Parameters[i];

                finalArguments[i] = parameter.ToArgument(argString);
            }

            Action(finalArguments);
        }

        private string Usage()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Name);
            stringBuilder.Append(' ');
            foreach (var parameter in Parameters)
            {
                stringBuilder.Append('<');
                stringBuilder.Append(parameter.Name);
                stringBuilder.Append('>');
                stringBuilder.Append(' ');
            }

            return stringBuilder.ToString();
        }
    }

    public interface ICommandParameter
    {
        ICommandArgument ToArgument(string argString);
        public string Name { get; }
    }

    public readonly record struct IntCommandParameter(string Name) : ICommandParameter
    {
        public ICommandArgument ToArgument(string argString)
        {
            return new CommandArgumentValue<int>(int.Parse(argString));
        }
    }
    
    public readonly record struct StringCommandParameter(string Name) : ICommandParameter
    {
        public ICommandArgument ToArgument(string argString)
        {
            return new CommandArgumentValue<string>(argString);
        }
    }

    public interface ICommandArgument
    {
    }

    public record CommandArgumentValue<T>(T Value) : ICommandArgument;
}
