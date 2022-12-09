using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

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
        return new ImageLiteralInstruction(Client.Assets.GetAsset<StaticImageAsset>(imageName), scaleFactor);
    }
    
    public static Instruction Texture(string textureName, float scaleFactor = 1f)
    {
        return new TextureLiteralInstruction(Client.Assets.GetTexture(textureName), scaleFactor);
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
}
