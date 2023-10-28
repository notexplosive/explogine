using System;
using System.Collections.Generic;
using System.Text;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.TextFormatting;

public static class Format
{
    public static Instruction Push(Color color)
    {
        return new PushColor(color);
    }

    public static Instruction Push(IFontGetter font)
    {
        return new PushFont(font);
    }

    public static Instruction PopColor()
    {
        return new PopColor();
    }

    public static Instruction PopFont()
    {
        return new PopFont();
    }

    public static Instruction Image(string imageName, float scaleFactor = 1f)
    {
        return new ImageLiteralInstruction(imageName, scaleFactor);
    }

    public static Instruction Texture(string textureName, float scaleFactor = 1f)
    {
        return new TextureLiteralInstruction(new IndirectTexture(textureName),
            scaleFactor);
    }

    public static FormattedText FromInstructions(IFontGetter startingFont, Color startingColor,
        Instruction[] instructions)
    {
        return new FormattedText(startingFont, startingColor, instructions);
    }

    public static Instruction[] StringToInstructions(string text)
    {
        var result = new List<Instruction>();
        var currentToken = new StringBuilder();

        var commandStartChar = '[';
        var commandEndChar = ']';
        var parametersStartChar = '(';
        var parametersEndChar = ')';
        var parametersSeparator = ',';
        var escapeCharacter = '\\';

        void SaveTokenAsLiteral()
        {
            if (currentToken.Length > 0)
            {
                result.Add(currentToken.ToString());
                currentToken.Clear();
            }
        }

        void SaveTokenAsCommand()
        {
            var token = currentToken.ToString();

            var commandName = new StringBuilder();
            var parameters = new StringBuilder();
            var collectingCommandName = true;
            foreach (var character in token)
            {
                if (collectingCommandName)
                {
                    if (character == parametersStartChar)
                    {
                        collectingCommandName = false;
                    }
                    else
                    {
                        commandName.Append(character);
                    }
                }
                else
                {
                    if (character == parametersEndChar)
                    {
                        break;
                    }

                    parameters.Append(character);
                }
            }

            try
            {
                var instruction = Instruction.FromString(commandName.ToString(),
                    parameters.ToString().Split(parametersSeparator));
                result.Add(instruction);
            }
            catch (Exception)
            {
                // ignore
            }

            currentToken.Clear();
        }

        var currentMode = ParserState.ReadingLiteral;

        char? prevCharacter = null;
        foreach (var character in text)
        {
            switch (currentMode)
            {
                case ParserState.ReadingLiteral when character == commandStartChar && prevCharacter != escapeCharacter:
                    SaveTokenAsLiteral();
                    currentMode = ParserState.ReadingCommandName;
                    break;
                case ParserState.ReadingCommandName when character == commandEndChar && prevCharacter != escapeCharacter:
                    SaveTokenAsCommand();
                    currentMode = ParserState.ReadingLiteral;
                    break;
                case ParserState.ReadingLiteral or ParserState.ReadingCommandName:
                    if (character != escapeCharacter || prevCharacter == escapeCharacter)
                    {
                        currentToken.Append(character);
                    }
                    break;
            }

            prevCharacter = character;
        }

        SaveTokenAsLiteral();

        return result.ToArray();
    }

    private enum ParserState
    {
        ReadingLiteral,
        ReadingCommandName
    }
}
