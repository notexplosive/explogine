using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Font : IFontGetter, IFont
{
    public Font(SpriteFont spriteFont, int size)
    {
        SpriteFont = spriteFont;
        FontSize = size;
        ScaleFactor = (float) FontSize / SpriteFont.LineSpacing;
    }

    public SpriteFont SpriteFont { get; }
    public int FontSize { get; }
    public float ScaleFactor { get; }

    public Font GetFont()
    {
        // no-op
        return this;
    }

    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        if (!restrictedWidth.HasValue)
        {
            restrictedWidth = float.MaxValue;
        }

        return GetRestrictedString(text, restrictedWidth.Value).Size;
    }

    public string Linebreak(string text, float restrictedWidth)
    {
        return GetRestrictedString(text, restrictedWidth).CombinedText;
    }

    public RestrictedString<string> GetRestrictedString(string text, float restrictedWidth)
    {
        return RestrictedStringBuilder.FromText(text, restrictedWidth, SpriteFont, ScaleFactor);
    }

    public Font WithFontSize(int size)
    {
        return new Font(SpriteFont, size);
    }
}