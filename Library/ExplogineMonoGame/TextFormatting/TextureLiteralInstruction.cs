using System.Globalization;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class TextureLiteralInstruction : Instruction, ILiteralInstruction
{
    private readonly float _scaleFactor;
    private readonly IndirectTexture _texture;

    internal TextureLiteralInstruction(IndirectTexture texture, float scaleFactor = 1f)
    {
        _scaleFactor = scaleFactor;
        _texture = texture;
    }

    public TextureLiteralInstruction(string[] args) : this(
        new IndirectTexture(args[0]),
        args.Length > 1 ? float.Parse(args[1], CultureInfo.InvariantCulture) : 1f)
    {
    }

    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.FragmentImage(_texture.Get(), _scaleFactor, color);
    }
}
