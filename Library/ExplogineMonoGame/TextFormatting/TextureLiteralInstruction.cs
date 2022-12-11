using System;
using System.Globalization;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.TextFormatting;

public class TextureLiteralInstruction : Instruction, ILiteralInstruction
{
    private readonly float _scaleFactor;
    private readonly Lazy<Texture2D> _texture;

    internal TextureLiteralInstruction(Lazy<Texture2D> texture, float scaleFactor = 1f)
    {
        _scaleFactor = scaleFactor;
        _texture = texture;
    }

    public TextureLiteralInstruction(string[] args) : this(
        new Lazy<Texture2D>(Client.Assets.GetTexture(args[0])),
        args.Length > 1 ? float.Parse(args[1], CultureInfo.InvariantCulture) : 1f)
    {
    }

    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.FragmentImage(_texture.Value, _scaleFactor, color);
    }
}
