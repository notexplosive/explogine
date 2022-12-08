using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly record struct RestrictedString<TOutput>(TOutput[] Lines, Vector2 Size)
{
    public readonly string CombinedText = string.Join("\n", Lines);

    public static RestrictedString<TOutput> ExecuteStrategy<TChar>(RestrictedStringBuilder.IStrategy<TChar, TOutput> strategy,
        TChar[] text, float restrictedWidth)
    {
        if (text.Length == 0)
        {
            return new RestrictedString<TOutput>(Array.Empty<TOutput>(), Vector2.Zero);
        }

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];

            if (!strategy.IsNewline(character))
            {
                strategy.AppendTextToToken(character);
            }

            if (strategy.IsWhiteSpace(character) || i == text.Length - 1)
            {
                if (strategy.CurrentLineWidth + strategy.CurrentTokenWidth() >= restrictedWidth)
                {
                    strategy.StartNewLine();
                }

                strategy.AppendCurrentTokenToLineAndClearCurrentToken();
            }
            
            if (strategy.IsNewline(character))
            {
                strategy.AppendCurrentTokenToLineAndClearCurrentToken();
                strategy.StartNewLine();
            }
        }

        if (strategy.HasContentInCurrentLine())
        {
            strategy.FinishCurrentLine();
        }

        return strategy.Result;
    }
}
