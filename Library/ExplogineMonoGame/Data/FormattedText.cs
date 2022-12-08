using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public readonly struct FormattedText : IEnumerable<FormattedText.FormattedGlyph>
{
    private readonly List<FormattedGlyph> _letterPositions = new();

    public FormattedText(IFontGetter fontLike, string text, Rectangle rectangle, Alignment alignment) : this(
        new IFragment[] {new Fragment(fontLike.GetFont(), text)}, rectangle, alignment)
    {
    }

    public FormattedText(IFragment[] fragments, Rectangle rectangle, Alignment alignment)
    {
        Rectangle = rectangle;

        var (lines, restrictedSize) = RestrictedStringBuilder.FromFragments(fragments, rectangle.Width);
        var restrictedBounds =
            RectangleF.FromSizeAlignedWithin(rectangle, restrictedSize, alignment.JustVertical());

        var verticalSpaceUsedByPreviousLines = 0f;
        foreach (var fragmentLine in lines)
        {
            var actualLineSize = fragmentLine.Size;
            var availableBoundForLine = new RectangleF(
                restrictedBounds.TopLeft + new Vector2(0, verticalSpaceUsedByPreviousLines),
                new Vector2(rectangle.Width, actualLineSize.Y));
            var actualLineBounds =
                RectangleF.FromSizeAlignedWithin(availableBoundForLine, actualLineSize, alignment);

            verticalSpaceUsedByPreviousLines += actualLineSize.Y;

            var letterPosition = Vector2.Zero;
            foreach (var letterFragment in fragmentLine)
            {
                var letterSize = letterFragment.Size;
                var position = actualLineBounds.TopLeft + letterPosition + fragmentLine.Size.JustY() -
                               letterSize.JustY();
                AddLetter(new FormattedGlyph(position, letterFragment));
                letterPosition += letterSize.JustX();
            }
        }
    }

    public Rectangle Rectangle { get; }

    private void AddLetter(FormattedGlyph formattedGlyph)
    {
        _letterPositions.Add(formattedGlyph);
    }
    
    public interface IGlyphData
    {
        public Vector2 Size { get; }
    }

    /// <summary>
    /// Fragments are input (and sometimes output)
    /// </summary>
    public interface IFragment
    {
    }
    
    public readonly record struct FormattedGlyph(Vector2 Position, IGlyphData Data);

    public readonly record struct Fragment(Font Font, string Text, Color? Color = null) : IFragment
    {
        public int NumberOfChars => Text.Length;
        public Vector2 Size => Font.MeasureString(Text);
    }

    public readonly record struct FragmentChar(Font Font, char Text, Color? Color = null) : IGlyphData
    {
        public Vector2 Size => Font.MeasureString(Text.ToString());
    }
    
    public readonly record struct FragmentImage(Texture2D Texture, Rectangle SourceRect, float ScaleFactor = 1f, Color? Color = null) : IFragment, IGlyphData
    {
        public Vector2 Size => SourceRect.Size.ToVector2() * ScaleFactor;
    }

    public readonly record struct FragmentLine(ImmutableArray<IGlyphData> Fragments) : IEnumerable<IGlyphData>
    {
        public Vector2 Size
        {
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

        public IEnumerator<IGlyphData> GetEnumerator()
        {
            foreach (var item in Fragments)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public IEnumerator<FormattedGlyph> GetEnumerator()
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
