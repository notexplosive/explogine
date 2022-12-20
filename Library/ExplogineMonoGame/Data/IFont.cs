using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public interface IFont : IFontGetter
{
    public float ScaleFactor { get; }
    float Height { get; }
    Vector2 MeasureString(string text, float? restrictedWidth = null);
}
