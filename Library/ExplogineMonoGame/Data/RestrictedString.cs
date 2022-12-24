using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly record struct RestrictedString<TOutput>(TOutput[] Lines, Vector2 Size)
{
    public readonly string CombinedText = string.Join("\n", Lines);

    public static RestrictedString<TOutput> ExecuteStrategy<TChar>(
        RestrictedStringBuilder.IStrategy<TChar, TOutput> strategy,
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

            if (strategy.IsNewline(character))
            {
                if (strategy.CurrentLineWidth + strategy.CurrentTokenWidth() >= restrictedWidth)
                {
                    strategy.FinishLine();
                    strategy.StartNewLine();
                }
                
                strategy.FinishToken();
                strategy.AppendManualLinebreak(character);
                strategy.FinishLine();
                strategy.StartNewLine();
            }
            else if (strategy.IsWhiteSpace(character) || i == text.Length - 1)
            {
                if (strategy.CurrentLineWidth + strategy.CurrentTokenWidth() >= restrictedWidth)
                {
                    strategy.FinishLine();
                    strategy.StartNewLine();
                }

                strategy.FinishToken();
            }
        }
        
        strategy.AddTerminatorToCurrentLine();

        if (strategy.HasContentInCurrentLine())
        {
            strategy.FinishLine();
        }


        return strategy.Result;
    }
}
