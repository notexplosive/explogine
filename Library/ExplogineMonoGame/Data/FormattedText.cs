using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public readonly struct FormattedText
{
    private readonly IFragment[] _fragments;

    public FormattedText(IFontGetter indirectFont, string text, Color? color = null) : this(new Fragment(indirectFont.GetFont(), text, color))
    {
    }

    public FormattedText(params IFragment[] fragments)
    {
        _fragments = fragments;
    }

    /// <summary>
    ///     The size of the whole text if it had infinite width
    /// </summary>
    /// <returns></returns>
    public Vector2 OneLineSize()
    {
        var (_, restrictedSize) = RestrictedStringBuilder.FromFragments(_fragments, float.MaxValue);

        // +1 on both sides to round up
        return restrictedSize + new Vector2(1);
    }

    public IEnumerable<FormattedGlyph> GetGlyphs(Rectangle rectangle, Alignment alignment)
    {
        var (lines, restrictedSize) = RestrictedStringBuilder.FromFragments(_fragments, rectangle.Width);
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
                yield return new FormattedGlyph(position, letterFragment);
                letterPosition += letterSize.JustX();
            }
        }
    }

    public interface IGlyphData
    {
        public Vector2 Size { get; }
        float ScaleFactor { get; }
    }

    /// <summary>
    ///     Fragments are input (and sometimes output)
    /// </summary>
    public interface IFragment
    {
        public Vector2 Size { get; }
    }

    public readonly record struct FormattedGlyph(Vector2 Position, IGlyphData Data)
    {
    }

    public readonly record struct Fragment(IFontGetter FontGetter, string Text, Color? Color = null) : IFragment
    {
        public Font Font => FontGetter.GetFont();
        public int NumberOfChars => Text.Length;
        public Vector2 Size => Font.MeasureString(Text);

        public override string ToString()
        {
            return $"{Size} \"{Text}\"";
        }
    }

    public readonly record struct FragmentChar(Font Font, char Text, Color? Color = null) : IGlyphData
    {
        public Vector2 Size => Font.MeasureString(Text.ToString());
        public float ScaleFactor => Font.ScaleFactor;

        public override string ToString()
        {
            return $"{Size} '{Text}'";
        }
    }

    public readonly record struct FragmentImage(Texture2D Texture, Rectangle SourceRect, float ScaleFactor = 1f,
        Color? Color = null) : IFragment, IGlyphData
    {
        public Vector2 Size => SourceRect.Size.ToVector2() * ScaleFactor;

        public override string ToString()
        {
            return $"{Size} (image)";
        }
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

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var fragment in Fragments)
            {
                if (fragment is FragmentChar fragmentChar)
                {
                    result.Append(fragmentChar.Text);
                }

                if (fragment is FragmentImage)
                {
                    result.Append("(image)");
                }
            }

            return $"{Size} {result.ToString()}";
        }
    }
}
