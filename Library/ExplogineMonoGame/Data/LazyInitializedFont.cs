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
        if (_cache == null)
        {
            return Vector2.Zero;
        }
        return _cache.MeasureString(text, restrictedWidth);
    }

    public Font GetFont()
    {
        if (_cache == null)
        {
            throw new MissingContentException($"{_spriteFontPath} is not loaded yet");
        }
        return _cache;
    }

    public void BuildCache()
    {
        _cache = Client.Assets.GetFont(_spriteFontPath, FontSize);
    }
}
