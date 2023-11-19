using System.Collections.Generic;

namespace ExplogineMonoGame.TextFormatting;

public interface IStackInstruction<T>
{
    public void Do(Stack<T> stack);
}
