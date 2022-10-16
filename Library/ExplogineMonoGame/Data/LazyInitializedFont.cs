using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class LazyInitializedFont : IFont
{
    private readonly string _spriteFontPath;
    private Font? _cache;

    public LazyInitializedFont(string spriteFontPath, int fontSize)
    {
        _spriteFontPath = spriteFontPath;
        FontSize = fontSize;
    }

    public int FontSize { get; }

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
        return Client.Assets.GetFont(_spriteFontPath, FontSize);
    }
}
