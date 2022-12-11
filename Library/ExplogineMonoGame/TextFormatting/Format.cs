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
        return new ImageLiteralInstruction(
            new Lazy<StaticImageAsset>(() => Client.Assets.GetAsset<StaticImageAsset>(imageName)), scaleFactor);
    }

    public static Instruction Texture(string textureName, float scaleFactor = 1f)
    {
        return new TextureLiteralInstruction(new IndirectTexture(textureName),
            scaleFactor);
    }

    public static FormattedText FromInstructions(IFontGetter startingFont, Color startingColor,
        Instruction[] instructions)
    {
        var fragments = new List<FormattedText.IFragment>();
        var fonts = new Stack<IFontGetter>();
        fonts.Push(startingFont);

        var colors = new Stack<Color>();
        colors.Push(startingColor);

        foreach (var instruction in instructions)
        {
            if (instruction is ILiteralInstruction literalInstruction)
            {
                fragments.Add(literalInstruction.GetFragment(fonts.Peek(), colors.Peek()));
            }

            if (instruction is IStackInstruction<Color> colorInstruction)
            {
                colorInstruction.Do(colors);
            }

            if (instruction is IStackInstruction<IFontGetter> fontInstruction)
            {
                fontInstruction.Do(fonts);
            }
        }

        if (colors.Count != 1)
        {
            Client.Debug.LogWarning($"Colors stack was {colors.Count} when it should be 1");
        }

        if (fonts.Count != 1)
        {
            Client.Debug.LogWarning($"Fonts stack was {fonts.Count} when it should be 1");
        }

        return new FormattedText(fragments.ToArray());
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

            result.Add(Instruction.FromString(commandName.ToString(),
                parameters.ToString().Split(parametersSeparator)));
            currentToken.Clear();
        }

        var currentMode = ParserState.ReadingLiteral;

        foreach (var character in text)
        {
            switch (currentMode)
            {
                case ParserState.ReadingLiteral when character == commandStartChar:
                    SaveTokenAsLiteral();
                    currentMode = ParserState.ReadingCommandName;
                    break;
                case ParserState.ReadingCommandName when character == commandEndChar:
                    SaveTokenAsCommand();
                    currentMode = ParserState.ReadingLiteral;
                    break;
                case ParserState.ReadingLiteral or ParserState.ReadingCommandName:
                    currentToken.Append(character);
                    break;
            }
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
