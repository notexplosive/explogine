using ExplogineCore.Data;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.TextFormatting;

public class FontCommand : Instruction
{
    internal FontCommand(IFontGetter font)
    {
        Font = font;
    }

    public FontCommand(string[] args)
    {
        if (args.IsValidIndex(1))
        {
            if (int.TryParse(args[1], out var result))
            {
                Font = new IndirectFont(args[0], result);
            }
        }
    }

    public IFontGetter? Font { get; }

    public override void Do(TextRun textRun)
    {
        if (Font != null && Font.Exists())
        {
            textRun.PushFont(Font);
        }
    }

    public class Pop : Instruction
    {
        public override void Do(TextRun textRun)
        {
            textRun.PopFont();
        }
    }
}
