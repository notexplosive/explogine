using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class ImageLiteralInstruction : Instruction, ILiteralInstruction
{
    public StaticImageAsset Image { get; }
    public float ScaleFactor { get; }

    internal ImageLiteralInstruction(StaticImageAsset image, float scaleFactor = 1f)
    {
        Image = image;
        ScaleFactor = scaleFactor;
    }

    public ImageLiteralInstruction(string[] args) : this(
        Client.Assets.GetAsset<StaticImageAsset>(args[0]),
        args.Length > 1 ? float.Parse(args[1]) : 1f)
    {
    }

    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.FragmentImage(Image, ScaleFactor, color);
    }
}
