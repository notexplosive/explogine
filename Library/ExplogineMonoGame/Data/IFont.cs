using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public interface IFont
{
    public int FontSize { get; }
    public Vector2 MeasureString(string text, float? restrictedWidth = null);
    Font GetFont();
}