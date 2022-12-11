using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class IndirectFont : IFontGetter
{
    private readonly Lazy<Font> _lazy;

    public IndirectFont(string spriteFontPath, int fontSize)
    {
        FontSize = fontSize;
        SpriteFontPath = spriteFontPath;
        _lazy = new Lazy<Font>(() => Client.Assets.GetFont(spriteFontPath, fontSize));
    }

    public string SpriteFontPath { get; }
    public int FontSize { get; }

    [Pure]
    public Font GetFont()
    {
        return _lazy.Value;
    }

    public static implicit operator Font(IndirectFont indirectFont)
    {
        return indirectFont.GetFont();
    }

    [Pure]
    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        return GetFont().MeasureString(text, restrictedWidth);
    }
}
