using ExplogineCore.Data;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.TextFormatting;

public class FontScaleCommand : Instruction
{
    private readonly float _scale;

    public FontScaleCommand(string[] args)
    {
        if (args.IsValidIndex(0))
        {
            if (float.TryParse(args[0], out var result))
            {
                _scale = result;
            }
        }
    }

    public override void Do(TextRun textRun)
    {
        textRun.PushScale(_scale);
    }

    public class Pop : Instruction
    {
        public override void Do(TextRun textRun)
        {
            textRun.PopScale();
        }
    }
}
