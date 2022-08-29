using System;
using System.Text;
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
        return GetRestrictedString(text, restrictedWidth).Text;
    }

    public RestrictedString GetRestrictedString(string text, float restrictedWidth)
    {
        var currentLineWidth = 0f;
        var maxWidth = 0f;
        var height = 0f;
        var heightOfOneLine = SpriteFont.LineSpacing * ScaleFactor;
        var lineBrokenString = new StringBuilder();

        if (text.Length > 0)
        {
            height = heightOfOneLine;
        }

        var spaceWidth = SpriteFont.MeasureString(" ").X;

        var token = new StringBuilder();

        void StartNewLine()
        {
            maxWidth = MathF.Max(maxWidth, currentLineWidth);
            height += heightOfOneLine;
            currentLineWidth = 0;
            lineBrokenString.Append('\n');
        }

        void AppendToken()
        {
            currentLineWidth += TokenWidth();
            lineBrokenString.Append(token);
            token = new StringBuilder(); // can we just call token.Clear()?
        }

        float TokenWidth()
        {
            return SpriteFont.MeasureString(token).X * ScaleFactor;
        }

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];
            if (character == '\t')
            {
                token.Append("   ");
            }
            else if (character == '\n')
            {
                AppendToken();
                StartNewLine();
                continue;
            }
            else
            {
                token.Append(character);
            }

            if (char.IsWhiteSpace(character) || i == text.Length - 1)
            {
                if (currentLineWidth + TokenWidth() >= restrictedWidth)
                {
                    StartNewLine();
                }

                AppendToken();
            }
        }

        return new RestrictedString(lineBrokenString.ToString(),
            new Vector2(MathF.Max(maxWidth, currentLineWidth), height));
    }

    public readonly record struct RestrictedString(string Text, Vector2 Size);

    public Font WithFontSize(int size)
    {
        return new Font(SpriteFont, size);
    }
}
