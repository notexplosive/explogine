using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class PushColor : Instruction, IStackInstruction<Color>
{
    public Color Color { get; }

    public PushColor(string[] args)
    {
        var hex = uint.Parse(args[0], System.Globalization.NumberStyles.HexNumber);
        Color = ColorExtensions.FromRgbHex(hex);
    }

    internal PushColor(Color color)
    {
        Color = color;
    }

    public void Do(Stack<Color> stack)
    {
        stack.Push(Color);
    }
}