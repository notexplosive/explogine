using System.Diagnostics.Contracts;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class LazyInitializedFont : IFontGetter
{
    public string SpriteFontPath { get; }
    private Font? _cache;

    public LazyInitializedFont(string spriteFontPath, int fontSize)
    {
        SpriteFontPath = spriteFontPath;
        FontSize = fontSize;
    }

    public int FontSize { get; }

    public static implicit operator Font(LazyInitializedFont lazyInitializedFont)
    {
        return lazyInitializedFont.GetFont();
    }

    [Pure]
    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        return GetFont().MeasureString(text, restrictedWidth);
    }

    public Font GetFont()
    {
        return _cache ??= BuildCache();
    }

    private Font BuildCache()
    {
        return Client.Assets.GetFont(SpriteFontPath, FontSize);
    }
}
