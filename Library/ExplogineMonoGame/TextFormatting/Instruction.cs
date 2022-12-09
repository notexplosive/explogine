namespace ExplogineMonoGame.TextFormatting;

public abstract class Instruction
{
    public static implicit operator Instruction(string str)
    {
        return new StringLiteralInstruction(str);
    }
}