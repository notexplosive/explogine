using System.Collections.Generic;

namespace ExplogineMonoGame.TextFormatting;

internal interface IStackInstruction<T>
{
    public void Do(Stack<T> stack);
}
