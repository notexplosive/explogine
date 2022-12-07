using System.Collections;
using System.Collections.Generic;
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
        
        var (lines, restrictedSize) = Font.RestrictedString.FromFragments(fragments, rectangle.Width);
        var restrictedBounds =
            RectangleF.FromSizeAlignedWithin(rectangle, restrictedSize, alignment.JustVertical());

        foreach (var fragment in fragments)
        {
            var font = fragment.Font;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var actualLineSize = font.MeasureString(line);
                var availableBoundForLine = new RectangleF(
                    restrictedBounds.TopLeft + new Vector2(0, i * font.FontSize),
                    new Vector2(rectangle.Width, actualLineSize.Y));
                var actualLineBounds =
                    RectangleF.FromSizeAlignedWithin(availableBoundForLine, actualLineSize, alignment);

                var letterPosition = Vector2.Zero;
                foreach (var letter in line)
                {
                    AddLetter(new LetterPosition(letter, actualLineBounds.TopLeft + letterPosition, font));
                    letterPosition += font.MeasureString(letter.ToString()).JustX();
                }
            }
        }
    }

    public Rectangle Rectangle { get; }

    private void AddLetter(LetterPosition letterPosition)
    {
        _letterPositions.Add(letterPosition);
    }

    public readonly record struct LetterPosition(char Letter, Vector2 Position, Font Font);

    public readonly record struct Fragment(Font Font, string Text)
    {
        public int NumberOfChars => Text.Length;
        public Vector2 Size => Font.MeasureString(Text);
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
