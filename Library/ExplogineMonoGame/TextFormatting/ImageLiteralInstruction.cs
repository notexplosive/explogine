using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.TextFormatting;

public class ImageLiteralInstruction : Instruction, ILiteralInstruction
{
    internal ImageLiteralInstruction(StaticImageAsset image, float scaleFactor = 1f)
    {
        _image = image;
        _scaleFactor = scaleFactor;
    }

    private readonly StaticImageAsset _image;
    private readonly float _scaleFactor;

    public FormattedText.IFragment GetFragment(IFontGetter font, Color color)
    {
        return new FormattedText.FragmentImage(_image, _scaleFactor, color);
    }
}
