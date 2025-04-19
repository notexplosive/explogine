using System.Globalization;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class ColorCommand : Instruction
{
    public ColorCommand(string[] args)
    {
        if (uint.TryParse(args[0], NumberStyles.HexNumber, null, out var hex))
        {
            Color = ColorExtensions.FromRgbHex(hex);
        }
        else
        {
            Color = Color.White;
        }
    }

    internal ColorCommand(Color color)
    {
        Color = color;
    }

    public Color Color { get; }

    public override void Do(TextRun textRun)
    {
        textRun.PushColor(Color);
    }

    public class Pop : Instruction
    {
        public override void Do(TextRun textRun)
        {
            textRun.PopColor();
        }
    }
}
