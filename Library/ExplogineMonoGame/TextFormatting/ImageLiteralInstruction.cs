using System;
using System.Globalization;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class ImageLiteralInstruction : Instruction, ILiteralInstruction
{
    internal ImageLiteralInstruction(IndirectAsset<ImageAsset> image, float scaleFactor = 1f)
    {
        Image = image;
        ScaleFactor = scaleFactor;
    }

    public ImageLiteralInstruction(string[] args) : this(
        args[0],
        args.Length > 1 ? float.Parse(args[1], CultureInfo.InvariantCulture) : 1f)
    {
    }

    public IndirectAsset<ImageAsset> Image { get; }
    public float ScaleFactor { get; }

    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.FragmentImage(Image, ScaleFactor, color);
    }

    public override void Do(TextRun textRun)
    {
        textRun.Fragments.Add(GetFragment(textRun.PeekFont(), textRun.PeekColor()));
    }
}
