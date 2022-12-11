using System;
using System.Globalization;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class ImageLiteralInstruction : Instruction, ILiteralInstruction
{
    internal ImageLiteralInstruction(Lazy<StaticImageAsset> image, float scaleFactor = 1f)
    {
        Image = image;
        ScaleFactor = scaleFactor;
    }

    public ImageLiteralInstruction(string[] args) : this(
        new Lazy<StaticImageAsset>(() => Client.Assets.GetAsset<StaticImageAsset>(args[0])),
        args.Length > 1 ? float.Parse(args[1], CultureInfo.InvariantCulture) : 1f)
    {
    }

    public Lazy<StaticImageAsset> Image { get; }
    public float ScaleFactor { get; }

    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.FragmentImage(Image.Value, ScaleFactor, color);
    }
}
