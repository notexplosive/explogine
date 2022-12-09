using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExplogineCore;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.TextFormatting;

internal class Commands
{
    public static ScopedCommand Color = new(
        "color",
        args => new PushColor(args),
        () => new PopColor());

    public static ScopedCommand Font = new(
        "font",
        args => new PushFont(args),
        () => new PopFont());

    public static Command Image = new(
        "image",
        args => new ImageLiteralInstruction(args)
    );

    public static readonly Dictionary<string, ICommand> LookupTable = Reflection.GetStaticFieldsThatDeriveFromType<Commands, ICommand>();
}

public abstract class Instruction
{
    public static implicit operator Instruction(string str)
    {
        return new StringLiteralInstruction(str);
    }

    public static implicit operator Instruction(StaticImageAsset asset)
    {
        return new ImageLiteralInstruction(asset);
    }

    public static implicit operator Instruction(Texture2D texture)
    {
        return new TextureLiteralInstruction(texture);
    }

    public static Instruction FromString(string commandName, string[] args)
    {
        var isPopCommand = false;
        if (commandName.StartsWith('/'))
        {
            isPopCommand = true;
            commandName = commandName.Substring(1, commandName.Length - 1);
        }

        foreach (var command in Commands.LookupTable.Values)
        {
            if (command.Name != commandName)
            {
                continue;
            }

            try
            {
                switch (command)
                {
                    case ScopedCommand scopedCommand when isPopCommand:
                        return scopedCommand.CreatePop();
                    case ScopedCommand scopedCommand:
                        return scopedCommand.CreatePush(args);
                    case Command unscopedCommand:
                        return unscopedCommand.Create(args);
                }
            }
            catch(Exception e)
            {
                throw new Exception("Command parse failed", e);
            }
        }

        throw new Exception($"Command not found {commandName}");
    }
}

public interface ICommand
{
    public string Name { get; }
}

public record Command(string Name, Func<string[], Instruction> Create) : ICommand;

public record ScopedCommand(string Name, Func<string[], Instruction> CreatePush, Func<Instruction> CreatePop) : ICommand;
