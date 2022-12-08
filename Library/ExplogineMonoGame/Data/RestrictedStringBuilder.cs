using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public static class RestrictedStringBuilder
{
    public static RestrictedString<string> FromText(string text, float restrictedWidth, SpriteFont spriteFont,
        float scaleFactor)
    {
        return RestrictedString<string>.ExecuteStrategy(new TextStrategy(spriteFont, scaleFactor),
            text.ToCharArray(),
            restrictedWidth);
    }

    public static RestrictedString<FormattedText.FragmentLine> FromFragments(FormattedText.Fragment[] fragments,
        float restrictedWidth)
    {
        var combinedText = string.Empty;
        foreach (var fragment in fragments)
        {
            combinedText += fragment.Text;
        }

        var currentFragmentIndex = 0;
        var charIndexWithinCurrentFragment = 0;
        var lettersAsFragments = new FormattedText.FragmentChar[combinedText.Length];
        for (var i = 0; i < lettersAsFragments.Length; i++)
        {
            var currentFragment = fragments[currentFragmentIndex];
            lettersAsFragments[i] =
                new FormattedText.FragmentChar(currentFragment.Font, combinedText[i], currentFragment.Color);
            charIndexWithinCurrentFragment++;
            if (charIndexWithinCurrentFragment > currentFragment.NumberOfChars - 1)
            {
                currentFragmentIndex++;
                charIndexWithinCurrentFragment = 0;
            }
        }

        return RestrictedString<FormattedText.FragmentLine>.ExecuteStrategy<FormattedText.FragmentChar>(new FragmentStrategy(),
            lettersAsFragments,
            restrictedWidth);
    }

    public interface IStrategy<in TChar, TString>
    {
        RestrictedString<TString> Result { get; }
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

    public class FragmentStrategy : IStrategy<FormattedText.FragmentChar, FormattedText.FragmentLine>
    {
        private readonly List<FormattedText.FragmentChar> _currentLineFragments = new();
        private readonly List<FormattedText.FragmentChar> _currentTokenFragments = new();
        private readonly List<FormattedText.FragmentLine> _resultLines = new();
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

        public RestrictedString<FormattedText.FragmentLine> Result => new(_resultLines.ToArray(), _totalSize);

        public float CurrentLineWidth => CurrentLineSize.X;

        public void FinishCurrentLine()
        {
            _totalSize.X = MathF.Max(_totalSize.X, CurrentLineSize.X);
            _totalSize.Y += CurrentLineSize.Y;
            _resultLines.Add(new FormattedText.FragmentLine(_currentLineFragments.ToArray()));
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

        public void AppendTextToToken(FormattedText.FragmentChar content)
        {
            _currentTokenFragments.Add(content);
        }

        public bool HasContentInCurrentLine()
        {
            return CurrentLineString.Length > 0;
        }

        public bool IsNewline(FormattedText.FragmentChar character)
        {
            return character.Text == '\n';
        }

        public bool IsWhiteSpace(FormattedText.FragmentChar character)
        {
            return char.IsWhiteSpace(character.Text);
        }
    }

    private class TextStrategy : IStrategy<char, string>
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

        public RestrictedString<string> Result => new(_resultLines.ToArray(),
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
