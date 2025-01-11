using System;
using System.Diagnostics.Contracts;
using ExplogineMonoGame.AssetManagement;
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
    public IFont GetFont()
    {
        return _lazy.Value;
    }

    [Pure]
    public string Truncate(string text, Vector2 bounds)
    {
        return GetFont().Truncate(text, bounds);
    }

    public bool Exists()
    {
        return Client.Assets.HasAsset<SpriteFontAsset>(SpriteFontPath);
    }

    [Pure]
    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        return GetFont().MeasureString(text, restrictedWidth);
    }

    public IndirectFont WithFontSize(int newFontSize)
    {
        return new IndirectFont(SpriteFontPath, newFontSize);
    }
}
