using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public static class RestrictedStringBuilder
{
    public static RestrictedString<string> FromText(string text, float restrictedWidth, IFont font)
    {
        return RestrictedString<string>.ExecuteStrategy(new TextStrategy(font),
            text.ToCharArray(),
            restrictedWidth);
    }

    public static RestrictedString<FormattedText.GlyphDataLine> FromFragments(FormattedText.IFragment[] fragments,
        float restrictedWidth)
    {
        var lettersAsFragments = RestrictedStringBuilder.BreakFragmentsIntoIndividualLetters(fragments);

        return RestrictedString<FormattedText.GlyphDataLine>.ExecuteStrategy(new FragmentStrategy(),
            lettersAsFragments,
            restrictedWidth);
    }

    private static FormattedText.IGlyphData[] BreakFragmentsIntoIndividualLetters(FormattedText.IFragment[] fragments)
    {
        var combinedText = string.Empty;
        foreach (var fragment in fragments)
        {
            if (fragment is FormattedText.Fragment textFragment)
            {
                combinedText += textFragment.Text;
            }
            else
            {
                // One character placeholder in the combined text string
                combinedText += '#';
            }
        }

        var currentFragmentIndex = 0;
        var charIndexWithinCurrentFragment = 0;
        var lettersAsFragments = new FormattedText.IGlyphData[combinedText.Length];
        for (var i = 0; i < lettersAsFragments.Length; i++)
        {
            var currentFragment = fragments[currentFragmentIndex];
            if (currentFragment is FormattedText.Fragment fragment)
            {
                lettersAsFragments[i] =
                    new FormattedText.CharGlyphData(fragment.Font, combinedText[i], fragment.Color);
                charIndexWithinCurrentFragment++;
                if (charIndexWithinCurrentFragment > fragment.NumberOfChars - 1)
                {
                    currentFragmentIndex++;
                    charIndexWithinCurrentFragment = 0;
                }
            }
            else
            {
                lettersAsFragments[i] = currentFragment.ToGlyphData();
                currentFragmentIndex++;
                charIndexWithinCurrentFragment = 0;
            }
        }

        return lettersAsFragments;
    }

    public interface IStrategy<in TChar, TString>
    {
        RestrictedString<TString> Result { get; }
        float CurrentLineWidth { get; }
        void FinishLine();
        void StartNewLine();
        void FinishToken();
        float CurrentTokenWidth();
        void AppendTextToToken(TChar character);
        bool HasContentInCurrentLine();
        bool IsNewline(TChar character);
        bool IsWhiteSpace(TChar character);
        void AppendManualLinebreak(TChar newlineCharacter);
    }

    public class FragmentStrategy : IStrategy<FormattedText.IGlyphData, FormattedText.GlyphDataLine>
    {
        private readonly List<FormattedText.IGlyphData> _currentLineFragments = new();
        private readonly List<FormattedText.IGlyphData> _currentTokenFragments = new();
        private readonly List<FormattedText.GlyphDataLine> _resultLines = new();
        private Vector2 _totalSize;
        private int _lineNumber;

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

        public RestrictedString<FormattedText.GlyphDataLine> Result => new(_resultLines.ToArray(), _totalSize);

        public float CurrentLineWidth => CurrentLineSize.X;

        public void FinishLine()
        {
            _lineNumber++;
            _totalSize.X = MathF.Max(_totalSize.X, CurrentLineSize.X);
            _totalSize.Y += CurrentLineSize.Y;
            _resultLines.Add(new FormattedText.GlyphDataLine(_currentLineFragments.ToImmutableArray()));
        }

        public void StartNewLine()
        {
            _currentLineFragments.Clear();
        }

        public void FinishToken()
        {
            _currentLineFragments.AddRange(_currentTokenFragments);
            _currentTokenFragments.Clear();
        }

        public void AppendManualLinebreak(FormattedText.IGlyphData newlineCharacter)
        {
            var size = new Vector2(0, newlineCharacter.Size.Y / 2);
            _currentLineFragments.Add(new FormattedText.WhiteSpaceGlyphData(size, newlineCharacter.ScaleFactor, true));
        }

        public float CurrentTokenWidth()
        {
            return CurrentTokenSize.X;
        }

        public void AppendTextToToken(FormattedText.IGlyphData content)
        {
            _currentTokenFragments.Add(content);
        }

        public bool HasContentInCurrentLine()
        {
            return _currentLineFragments.Count > 0;
        }

        public bool IsNewline(FormattedText.IGlyphData character)
        {
            return character is FormattedText.CharGlyphData {Text: '\n'};
        }

        public bool IsWhiteSpace(FormattedText.IGlyphData character)
        {
            if (character is FormattedText.CharGlyphData fragmentChar)
            {
                return char.IsWhiteSpace(fragmentChar.Text);
            }

            return false;
        }
    }

    private class TextStrategy : IStrategy<char, string>
    {
        private readonly StringBuilder _currentToken = new();
        private readonly float _heightOfOneLine;
        private readonly List<string> _resultLines = new();
        private readonly IFont _font;

        public TextStrategy(IFont font)
        {
            _heightOfOneLine = font.Height;
            _font = font;
            Height = _heightOfOneLine;
        }

        private float Height { get; set; }
        private float MaxWidth { get; set; }
        private StringBuilder CurrentLine { get; } = new();
        private float Width => MathF.Max(MaxWidth, CurrentLineWidth);

        public RestrictedString<string> Result => new(_resultLines.ToArray(),
            new Vector2(Width, Height));

        public float CurrentLineWidth { get; private set; }

        public void FinishLine()
        {
            MaxWidth = MathF.Max(MaxWidth, CurrentLineWidth);
            CurrentLineWidth = 0;
            _resultLines.Add(CurrentLine.ToString());
        }

        public void StartNewLine()
        {
            Height += _heightOfOneLine;
            CurrentLine.Clear();
        }

        public void FinishToken()
        {
            CurrentLineWidth += CurrentTokenWidth();
            CurrentLine.Append(_currentToken.ToString());
            _currentToken.Clear();
        }

        public float CurrentTokenWidth()
        {
            return _font.MeasureString(_currentToken.ToString()).X;
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
        
        public void AppendManualLinebreak(char newlineCharacter)
        {
            // does nothing
        }
    }
}
