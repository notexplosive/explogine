using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Font : IFontGetter, IFont
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

    public Font GetFont()
    {
        // no-op
        return this;
    }

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
        return GetRestrictedString(text, restrictedWidth).CombinedText;
    }

    public RestrictedString GetRestrictedString(string text, float restrictedWidth)
    {
        return RestrictedString.FromText(text, restrictedWidth, SpriteFont, ScaleFactor);
    }

    public Font WithFontSize(int size)
    {
        return new Font(SpriteFont, size);
    }

    public readonly record struct RestrictedString(string[] Lines, Vector2 Size)
    {
        public readonly string CombinedText = string.Join("\n", Lines);

        public static RestrictedString FromText(string text, float restrictedWidth, SpriteFont spriteFont,
            float scaleFactor)
        {
            return RestrictedString.ExecuteStrategy(new TextStrategy(spriteFont, scaleFactor), text.ToCharArray(),
                restrictedWidth);
        }

        public static RestrictedString FromFragments(FormattedText.Fragment[] fragments, float restrictedWidth)
        {
            var combinedText = string.Empty;
            foreach (var fragment in fragments)
            {
                combinedText += fragment.Text;
            }

            var currentFragmentIndex = 0;
            var charIndexWithinCurrentFragment = 0;
            var lettersAsFragments = new FormattedText.Fragment[combinedText.Length];
            for (var i = 0; i < lettersAsFragments.Length; i++)
            {
                var currentFragment = fragments[currentFragmentIndex];
                lettersAsFragments[i] = currentFragment with {Text = combinedText[i].ToString()};
                charIndexWithinCurrentFragment++;
                if (charIndexWithinCurrentFragment > currentFragment.NumberOfChars - 1)
                {
                    currentFragmentIndex++;
                    charIndexWithinCurrentFragment = 0;
                }
            }

            return RestrictedString.ExecuteStrategy(new FragmentStrategy(), lettersAsFragments,
                restrictedWidth);
        }

        private static RestrictedString ExecuteStrategy<T>(IStrategy<T> strategy, T[] text, float restrictedWidth)
        {
            if (text.Length == 0)
            {
                return new RestrictedString(Array.Empty<string>(), Vector2.Zero);
            }

            for (var i = 0; i < text.Length; i++)
            {
                var character = text[i];
                if (strategy.IsNewline(character))
                {
                    strategy.AppendCurrentTokenToLineAndClearCurrentToken();
                    strategy.StartNewLine();
                    continue;
                }

                strategy.AppendTextToToken(character);

                if (strategy.IsWhiteSpace(character) || i == text.Length - 1)
                {
                    if (strategy.CurrentLineWidth + strategy.CurrentTokenWidth() >= restrictedWidth)
                    {
                        strategy.StartNewLine();
                    }

                    strategy.AppendCurrentTokenToLineAndClearCurrentToken();
                }
            }

            if (strategy.HasContentInCurrentLine())
            {
                strategy.FinishCurrentLine();
            }

            return strategy.Result;
        }

        private interface IStrategy<in TChar>
        {
            RestrictedString Result { get; }
            float CurrentLineWidth { get; }
            void FinishCurrentLine();
            void StartNewLine();
            void AppendCurrentTokenToLineAndClearCurrentToken();
            float CurrentTokenWidth();
            void AppendTextToToken(TChar character);
            bool HasContentInCurrentLine();
            bool IsNewline(TChar character);
            bool IsWhiteSpace(TChar character);
        }

        private class FragmentStrategy : IStrategy<FormattedText.Fragment>
        {
            private readonly List<string> _resultLines = new();
            private readonly List<FormattedText.Fragment> _currentLineFragments = new();
            private readonly List<FormattedText.Fragment> _currentTokenFragments = new();
            private Vector2 _totalSize;

            public Vector2 CurrentLineSize
            {
                get
                {
                    var width = 0f;
                    var height = 0f;
                    foreach (var fragment in _currentLineFragments)
                    {
                        width += fragment.Size.X;
                        height = MathF.Max(height, fragment.Size.Y);
                    }

                    return new Vector2(width, height);
                }
            }
            
            public Vector2 CurrentTokenSize
            {
                get
                {
                    var width = 0f;
                    var height = 0f;
                    foreach (var fragment in _currentTokenFragments)
                    {
                        width += fragment.Size.X;
                        height = MathF.Max(height, fragment.Size.Y);
                    }

                    return new Vector2(width, height);
                }
            }

            public string CurrentLineString
            {
                get
                {
                    var result = string.Empty;
                    foreach (var fragment in _currentLineFragments)
                    {
                        result += fragment.Text;
                    }

                    return result;
                }
            }

            public RestrictedString Result => new(_resultLines.ToArray(), _totalSize);

            public float CurrentLineWidth => CurrentLineSize.X;

            public void FinishCurrentLine()
            {
                _totalSize.X = MathF.Max(_totalSize.X, CurrentLineSize.X);
                _totalSize.Y += CurrentLineSize.Y;
                _resultLines.Add(CurrentLineString);
            }

            public void StartNewLine()
            {
                FinishCurrentLine();
                _currentLineFragments.Clear();
            }

            public void AppendCurrentTokenToLineAndClearCurrentToken()
            {
                _currentLineFragments.AddRange(_currentTokenFragments);
                _currentTokenFragments.Clear();
            }

            public float CurrentTokenWidth()
            {
                return CurrentTokenSize.X;
            }

            public void AppendTextToToken(FormattedText.Fragment content)
            {
                _currentTokenFragments.Add(content);
            }

            public bool HasContentInCurrentLine()
            {
                return CurrentLineString.Length > 0;
            }

            public bool IsNewline(FormattedText.Fragment character)
            {
                return character.Text == "\n";
            }

            public bool IsWhiteSpace(FormattedText.Fragment character)
            {
                return string.IsNullOrWhiteSpace(character.Text);
            }
        }

        private class TextStrategy : IStrategy<char>
        {
            private readonly StringBuilder _currentToken = new();
            private readonly float _heightOfOneLine;
            private readonly List<string> _resultLines = new();
            private readonly float _scaleFactor;
            private readonly SpriteFont _spriteFont;

            public TextStrategy(SpriteFont spriteFont, float scaleFactor)
            {
                _heightOfOneLine = spriteFont.LineSpacing * scaleFactor;
                _spriteFont = spriteFont;
                _scaleFactor = scaleFactor;
                Height = _heightOfOneLine;
            }

            private float Height { get; set; }
            private float MaxWidth { get; set; }
            private StringBuilder CurrentLine { get; } = new();
            private float Width => MathF.Max(MaxWidth, CurrentLineWidth);

            public RestrictedString Result => new(_resultLines.ToArray(),
                new Vector2(Width, Height));

            public float CurrentLineWidth { get; private set; }

            public void FinishCurrentLine()
            {
                MaxWidth = MathF.Max(MaxWidth, CurrentLineWidth);
                CurrentLineWidth = 0;
                _resultLines.Add(CurrentLine.ToString());
            }

            public void StartNewLine()
            {
                FinishCurrentLine();
                Height += _heightOfOneLine;
                CurrentLine.Clear();
            }

            public void AppendCurrentTokenToLineAndClearCurrentToken()
            {
                CurrentLineWidth += CurrentTokenWidth();
                CurrentLine.Append(_currentToken.ToString());
                _currentToken.Clear();
            }

            public float CurrentTokenWidth()
            {
                return _spriteFont.MeasureString(_currentToken).X * _scaleFactor;
            }

            public void AppendTextToToken(char content)
            {
                _currentToken.Append(content);
            }

            public bool HasContentInCurrentLine()
            {
                return CurrentLine.Length > 0;
            }

            public bool IsNewline(char character)
            {
                return character == '\n';
            }

            public bool IsWhiteSpace(char character)
            {
                return char.IsWhiteSpace(character);
            }
        }
    }
}
