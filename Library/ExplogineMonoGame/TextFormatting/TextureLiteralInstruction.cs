using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.TextFormatting;

public class TextureLiteralInstruction : Instruction, ILiteralInstruction
{
    private readonly float _scaleFactor;

    internal TextureLiteralInstruction(Texture2D texture, float scaleFactor = 1f)
    {
        _scaleFactor = scaleFactor;
        _texture = texture;
    }

    private readonly Texture2D _texture;

    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.FragmentImage(_texture, _scaleFactor, color);
    }
}
