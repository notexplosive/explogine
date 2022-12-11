using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class LazyInitializedFont : IFontGetter
{
    private Font? _cache;

    public LazyInitializedFont(string spriteFontPath, int fontSize)
    {
        SpriteFontPath = spriteFontPath;
        FontSize = fontSize;
    }

    public string SpriteFontPath { get; }

    public int FontSize { get; }

    public Font GetFont()
    {
        return _cache ??= BuildCache();
    }

    public static implicit operator Font(LazyInitializedFont lazyInitializedFont)
    {
        return lazyInitializedFont.GetFont();
    }

    [Pure]
    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        return GetFont().MeasureString(text, restrictedWidth);
    }

    private Font BuildCache()
    {
        return Client.Assets.GetFont(SpriteFontPath, FontSize);
    }
}
