using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Font
{
    public Font(SpriteFont spriteFont, int size)
    {
        SpriteFont = spriteFont;
        LineSpacing = size;
        ScaleFactor = (float) LineSpacing / SpriteFont.LineSpacing;
    }

    public SpriteFont SpriteFont { get; }
    public float ScaleFactor { get; }

    public int LineSpacing { get; }

    public Vector2 MeasureString(string text, float? restrictedWidth)
    {
        if (!restrictedWidth.HasValue)
        {
            return SpriteFont.MeasureString(text) * ScaleFactor;
        }


        return GetRestrictedString(text, restrictedWidth.Value).Size;
    }

    public string Linebreak(string text, float restrictedWidth)
    {
        return GetRestrictedString(text, restrictedWidth).Text;
    }

    private RestrictedString GetRestrictedString(string text, float restrictedWidth)
    {
        var currentLineWidth = 0f;
        var maxWidth = 0f;
        var height = 0f;
        var heightOfOneLine = SpriteFont.LineSpacing * ScaleFactor;
        var lineBrokenString = string.Empty;

        if (text.Length > 0)
        {
            height = heightOfOneLine;
        }

        var spaceWidth = SpriteFont.MeasureString(" ").X;
        
        foreach (var token in text.Split())
        {
            var tokenWidth = (SpriteFont.MeasureString(token).X + spaceWidth) * ScaleFactor;

            if (currentLineWidth + tokenWidth >= restrictedWidth)
            {
                maxWidth = MathF.Max(maxWidth, currentLineWidth);
                height += heightOfOneLine;
                currentLineWidth = 0;
                lineBrokenString += '\n';
            }
            
            currentLineWidth += tokenWidth;
            lineBrokenString += token + ' ';
        }

        return new RestrictedString(lineBrokenString, new Vector2(MathF.Max(maxWidth, currentLineWidth), height));
    }

    private readonly record struct RestrictedString(string Text, Vector2 Size);
}
