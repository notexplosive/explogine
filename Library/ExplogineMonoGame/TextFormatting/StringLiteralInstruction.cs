namespace ExplogineMonoGame.TextFormatting;

public class StringLiteralInstruction : Instruction
{
    internal StringLiteralInstruction(string text)
    {
        Text = text;
    }

    public string Text { get; }
}
