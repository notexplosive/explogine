using System;
using System.Collections.Generic;
using System.Text;
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
    public float ScaleFactor { get; }
    public int FontSize { get; }

    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        if (!restrictedWidth.HasValue)
        {
            restrictedWidth = float.MaxValue;
        }

        return GetRestrictedString(text, restrictedWidth.Value).Size;
    }

    public Font GetFont()
    {
        // no-op
        return this;
    }

    public string Linebreak(string text, float restrictedWidth)
    {
        return GetRestrictedString(text, restrictedWidth).CombinedText;
    }

    public RestrictedString GetRestrictedString(string text, float restrictedWidth)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new RestrictedString(Array.Empty<string>(), Vector2.Zero);
        }
        
        var currentLineWidth = 0f;
        var maxWidth = 0f;
        var height = 0f;
        var heightOfOneLine = SpriteFont.LineSpacing * ScaleFactor;
        var currentLine = new StringBuilder();

        if (text.Length > 0)
        {
            height = heightOfOneLine;
        }

        var spaceWidth = SpriteFont.MeasureString(" ").X;

        var token = new StringBuilder();
        var result = new List<string>();

        void FinishCurrentLine()
        {
            maxWidth = MathF.Max(maxWidth, currentLineWidth);
            currentLineWidth = 0;
            result.Add(currentLine.ToString());
        }
        
        void StartNewLine()
        {
            FinishCurrentLine();
            height += heightOfOneLine;
            currentLine.Clear();
        }

        void AppendToken()
        {
            currentLineWidth += TokenWidth();
            currentLine.Append(token.ToString());
            token.Clear();
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

        if (currentLine.Length > 0)
        {
            FinishCurrentLine();
        }

        return new RestrictedString(result.ToArray(),
            new Vector2(MathF.Max(maxWidth, currentLineWidth), height));
    }

    public readonly record struct RestrictedString(string[] Lines, Vector2 Size)
    {
        public readonly string CombinedText = string.Join("\n", Lines);
    }

    public Font WithFontSize(int size)
    {
        return new Font(SpriteFont, size);
    }
}