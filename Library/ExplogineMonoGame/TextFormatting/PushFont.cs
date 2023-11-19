using System.Collections.Generic;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.TextFormatting;

public class PushFont : Instruction, IStackInstruction<IFontGetter>
{
    public IFontGetter? Font { get; }

    internal PushFont(IFontGetter font)
    {
        Font = font;
    }

    public PushFont(string[] args)
    {
        if (args.IsValidIndex(1))
        {
            if (int.TryParse(args[1], out var result))
            {
                Font = new IndirectFont(args[0], result);
            }
        }
    }

    public void Do(Stack<IFontGetter> stack)
    {
        if (Font != null && Font.Exists())
        {
            stack.Push(Font);
        }
    }
}