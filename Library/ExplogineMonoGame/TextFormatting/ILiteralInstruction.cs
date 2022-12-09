using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public interface ILiteralInstruction
{
    FormattedText.IFragment GetFragment(IFontGetter font, Color color);
}
