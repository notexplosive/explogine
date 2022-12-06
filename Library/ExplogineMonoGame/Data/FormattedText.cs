using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public readonly struct FormattedText : IEnumerable<FormattedText.LetterPosition>
{
    private readonly List<LetterPosition> _letterPositions = new();

    public FormattedText(IFontGetter fontLike, string text, Rectangle rectangle, Alignment alignment)
    {
        Rectangle = rectangle;
        
        var font = fontLike.GetFont();
        var (lines, restrictedSize) = font.GetRestrictedString(text, rectangle.Width);

        var restrictedBounds =
            RectangleF.FromSizeAlignedWithin(rectangle, restrictedSize, alignment.JustVertical());

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

    public Rectangle Rectangle { get; }

    private void AddLetter(LetterPosition letterPosition)
    {
        _letterPositions.Add(letterPosition);
    }

    public readonly record struct LetterPosition(char Letter, Vector2 Position, Font Font);

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
