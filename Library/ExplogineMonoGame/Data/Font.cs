using System;
using System.Diagnostics.Contracts;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Font : IFont
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
    public float Height => FontSize;

    public IFont GetFont()
    {
        // no-op
        return this;
    }

    /// <summary>
    /// WARNING: Tremendously inefficient. This does a whole FormattedText layout computation multiple times
    /// </summary>
    /// <param name="text"></param>
    /// <param name="bounds"></param>
    /// <returns>Substring of given text, truncated to fit within bounds</returns>
    [Pure]
    public string Truncate(string text, Vector2 bounds)
    {
        // If bounds height was less than the height of the font, pretend like it wasn't (otherwise nothing would fit)
        bounds.Y = Math.Max(bounds.Y, Height);

        if (bounds.X <= 0)
        {
            return string.Empty;
        }

        Vector2 Restrict(string textToRestrict, float width)
        {
            return RestrictedStringBuilder
                .FromFragments(new FormattedText.IFragment[] {new FormattedText.Fragment(this, textToRestrict)}, width)
                .Size;
        }
        
        (bool, string) Attempt(int length)
        {
            var attemptText = text.Substring(0, length);
            var restrictedBounds = Restrict(attemptText, bounds.X);

            if (restrictedBounds.X <= bounds.X && restrictedBounds.Y <= bounds.Y)
            {
                return (true, attemptText);
            }

            return (false, attemptText);
        }

        string BinarySearchAttempt(int length)
        {
            var beginning = 0;
            var end = length;
            var result = text;

            while (beginning <= end)
            {
                var middle = (beginning + end) / 2;
                var currentAttempt = Attempt(middle);
                if (currentAttempt.Item1)
                {
                    beginning = middle + 1;
                    result = currentAttempt.Item2;
                }
                else
                {
                    end = middle - 1;
                }
            }

            return result;
        }

        var firstAttempt = Attempt(text.Length);
        if (firstAttempt.Item1)
        {
            return firstAttempt.Item2;
        }

        return BinarySearchAttempt(text.Length);
    }

    public bool Exists()
    {
        return true;
    }

    [Pure]
    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        if (!restrictedWidth.HasValue)
        {
            // SpriteFont.MeasureString chokes on tab characters >:(
            text = text.Replace("\t", "  ");
            return SpriteFont.MeasureString(text) * ScaleFactor;
        }

        return GetRestrictedString(text, restrictedWidth.Value).Size;
    }

    public IFont WithHeight(int newScaleFactor)
    {
        return new Font(SpriteFont, newScaleFactor);
    }

    public string Linebreak(string text, float restrictedWidth)
    {
        return GetRestrictedString(text, restrictedWidth).CombinedText;
    }

    public RestrictedString<string> GetRestrictedString(string text, float restrictedWidth)
    {
        return RestrictedStringBuilder.FromText(text, restrictedWidth, this);
    }

    public Font WithFontSize(int size)
    {
        return new Font(SpriteFont, size);
    }
}
