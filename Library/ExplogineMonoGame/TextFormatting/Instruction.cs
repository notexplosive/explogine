using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.TextFormatting;

public abstract class Instruction
{
    public static implicit operator Instruction(string str)
    {
        return new StringLiteralInstruction(str);
    }

    public static implicit operator Instruction(IndirectAsset<ImageAsset> indirectAsset)
    {
        return new ImageLiteralInstruction(indirectAsset);
    }

    public static implicit operator Instruction(Texture2D texture)
    {
        return new TextureLiteralInstruction(new IndirectTexture(texture));
    }

    public static Instruction? TryFromString(string commandName, string[] args)
    {
        var isPopCommand = false;
        if (commandName.StartsWith('/'))
        {
            isPopCommand = true;
            commandName = commandName.Substring(1, commandName.Length - 1);
        }

        foreach (var keyPair in FormattingCommands.SupportedCommands)
        {
            if (!String.Equals(keyPair.Key, commandName, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            var command = keyPair.Value;

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

        return null;
    }
}

public interface ICommand
{
}

public record Command(Func<string[], Instruction> Create) : ICommand;

public record ScopedCommand
    (Func<string[], Instruction> CreatePush, Func<Instruction> CreatePop) : ICommand;
