using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class PushColor : Instruction, IStackInstruction<Color>
{
    public Color Color { get; }

    internal PushColor(Color color)
    {
        Color = color;
    }

    public void Do(Stack<Color> stack)
    {
        stack.Push(Color);
    }
}