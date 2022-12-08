using System;
using System.Collections;
using System.Collections.Generic;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly struct FormattedText : IEnumerable<FormattedText.LetterPosition>
{
    private readonly List<LetterPosition> _letterPositions = new();

    public FormattedText(IFontGetter fontLike, string text, Rectangle rectangle, Alignment alignment) : this(
        new[] {new Fragment(fontLike.GetFont(), text)}, rectangle, alignment)
    {
    }

    public FormattedText(Fragment[] fragments, Rectangle rectangle, Alignment alignment)
    {
        Rectangle = rectangle;
        
        var (lines, restrictedSize) = RestrictedStringBuilder.FromFragments(fragments, rectangle.Width);
        var restrictedBounds =
            RectangleF.FromSizeAlignedWithin(rectangle, restrictedSize, alignment.JustVertical());

        var verticalSpaceUsedByPreviousLines = 0f;
        foreach (var fragmentLine in lines)
        {
            var actualLineSize = MeasureFragmentLine(fragmentLine);
            var availableBoundForLine = new RectangleF(
                restrictedBounds.TopLeft + new Vector2(0, verticalSpaceUsedByPreviousLines),
                new Vector2(rectangle.Width, actualLineSize.Y));
            var actualLineBounds =
                RectangleF.FromSizeAlignedWithin(availableBoundForLine, actualLineSize, alignment);

            verticalSpaceUsedByPreviousLines += actualLineSize.Y;

            var letterPosition = Vector2.Zero;
            foreach (var letterFragment in fragmentLine)
            {
                var letterSize = letterFragment.Font.MeasureString(letterFragment.Text.ToString());
                
                AddLetter(new LetterPosition(letterFragment.Text, actualLineBounds.TopLeft + letterPosition + fragmentLine.Size.JustY() - letterSize.JustY(), letterFragment.Font, letterFragment.Color));
                letterPosition += letterSize.JustX();
            }
        }
    }

    private Vector2 MeasureFragmentLine(FragmentLine fragmentLine)
    {
        var width = 0f;
        var height = 0f;
        foreach (var fragment in fragmentLine)
        {
            width += fragment.Size.X;
            height = MathF.Max(height, fragment.Size.Y);
        }

        return new Vector2(width, height);
    }

    public Rectangle Rectangle { get; }

    private void AddLetter(LetterPosition letterPosition)
    {
        _letterPositions.Add(letterPosition);
    }

    public readonly record struct LetterPosition(char Letter, Vector2 Position, Font Font, Color? Color);

    public readonly record struct Fragment(Font Font, string Text, Color? Color = null)
    {
        public int NumberOfChars => Text.Length;
        public Vector2 Size => Font.MeasureString(Text);
    }
    
    public readonly record struct FragmentChar(Font Font, char Text, Color? Color = null)
    {
        public Vector2 Size => Font.MeasureString(Text.ToString());
    }

    public readonly record struct FragmentLine(NotNullArray<FragmentChar> Fragments) : IEnumerable<FragmentChar>
    {
        public IEnumerator<FragmentChar> GetEnumerator()
        {
            return Fragments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public Vector2 Size {
            get
            {
                var size = new Vector2();
                foreach (var fragment in Fragments)
                {
                    size.X += fragment.Size.X;
                    size.Y = MathF.Max(size.Y, fragment.Size.Y);
                }

                return size;
            }
        }
    }

    public IEnumerator<LetterPosition> GetEnumerator()
    {
        foreach (var letterPosition in _letterPositions)
        {
            yield return letterPosition;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
