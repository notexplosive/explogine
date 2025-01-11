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

    public static Instruction? TryFromString(string commandName, string[] args, FormattedTextParser parser)
    {
        var isPopCommand = false;
        if (commandName.StartsWith('/'))
        {
            isPopCommand = true;
            commandName = commandName.Substring(1, commandName.Length - 1);
        }

        foreach (var (key, command) in parser.GetCommands())
        {
            if (!string.Equals(key, commandName, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

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

    public abstract void Do(TextRun textRun);
}
