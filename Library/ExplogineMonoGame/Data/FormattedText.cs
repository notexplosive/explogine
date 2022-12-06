using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class FormattedText : IEnumerable<FormattedText.LetterPosition>
{
    private List<LetterPosition> _letterPositions = new();

    public FormattedText(IFont fontLike, string text, Rectangle rectangle, Alignment alignment)
    {
        var font = fontLike.GetFont();
        var restrictedString = font.GetRestrictedString(text, rectangle.Width);
        var lines = restrictedString.Lines;
        
        var restrictedBounds =
            RectangleF.FromSizeAlignedWithin(rectangle, restrictedString.Size, alignment.JustVertical());
        
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
                AddLetter(new LetterPosition(letter, actualLineBounds.TopLeft + letterPosition));
                letterPosition += font.MeasureString(letter.ToString()).JustX();
            }
        }
    }

    private void AddLetter(LetterPosition letterPosition)
    {
        _letterPositions.Add(letterPosition);
    }

    public readonly record struct LetterPosition(char Letter, Vector2 Position);

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
