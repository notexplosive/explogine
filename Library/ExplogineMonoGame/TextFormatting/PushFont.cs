using System.Collections.Generic;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.TextFormatting;

public class PushFont : Instruction, IStackInstruction<IFontGetter>
{
    public IFontGetter Font { get; }

    internal PushFont(IFontGetter font)
    {
        Font = font;
    }

    public PushFont(string[] args) : this(new IndirectFont(args[0], int.Parse(args[1])))
    {
    }

    public void Do(Stack<IFontGetter> stack)
    {
        stack.Push(Font);
    }
}