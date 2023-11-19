using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class StringLiteralInstruction : Instruction, ILiteralInstruction
{
    internal StringLiteralInstruction(string text)
    {
        Text = text;
    }

    public string Text { get; }
    
    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.Fragment(font, Text, color);
    }

    public override void Do(TextRun textRun)
    {
        textRun.Fragments.Add(GetFragment(textRun.PeekFont(), textRun.PeekColor()));
    }
}