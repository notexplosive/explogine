using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public interface IFontGetter
{
    IFont GetFont();
    string Truncate(string text, Vector2 bounds);
}
