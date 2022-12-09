using System.Collections.Generic;

namespace ExplogineMonoGame.TextFormatting;

public abstract class PopInstruction<T> : Instruction, IStackInstruction<T>
{
    public void Do(Stack<T> stack)
    {
        stack.Pop();
    }
}
