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

    public void Do(Stack<IFontGetter> stack)
    {
        stack.Push(Font);
    }
}