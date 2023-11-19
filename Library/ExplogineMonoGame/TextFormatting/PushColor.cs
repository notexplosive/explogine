using System.Collections.Generic;
using System.Globalization;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class PushColor : Instruction, IStackInstruction<Color>
{
    public PushColor(string[] args)
    {
        if (uint.TryParse(args[0], NumberStyles.HexNumber, null, out var hex))
        {
            Color = ColorExtensions.FromRgbHex(hex);
        }
        else
        {
            Color = Color.White;
        }
    }

    internal PushColor(Color color)
    {
        Color = color;
    }

    public Color Color { get; }

    public void Do(Stack<Color> stack)
    {
        stack.Push(Color);
    }
}
