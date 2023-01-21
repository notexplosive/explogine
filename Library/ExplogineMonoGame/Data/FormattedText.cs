using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using ExplogineMonoGame.TextFormatting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public readonly struct FormattedText
{
    private readonly IFragment[] _fragments;

    public FormattedText(IFontGetter indirectFont, string text, Color? color = null) : this(
        new Fragment(indirectFont, text, color))
    {
    }

    public FormattedText(params IFragment[] fragments)
    {
        _fragments = fragments;
    }

    public FormattedText(IFontGetter startingFont, Color startingColor,
        Instruction[] instructions)
    {
        var fragments = new List<IFragment>();
        var fonts = new Stack<IFontGetter>();
        fonts.Push(startingFont);

        var colors = new Stack<Color>();
        colors.Push(startingColor);

        foreach (var instruction in instructions)
        {
            if (instruction is ILiteralInstruction literalInstruction)
            {
                fragments.Add(literalInstruction.GetFragment(fonts.Peek(), colors.Peek()));
            }

            if (instruction is IStackInstruction<Color> colorInstruction)
            {
                colorInstruction.Do(colors);
            }

            if (instruction is IStackInstruction<IFontGetter> fontInstruction)
            {
                fontInstruction.Do(fonts);
            }
        }

        if (colors.Count != 1)
        {
            Client.Debug.LogWarning($"Colors stack was {colors.Count} when it should be 1");
        }

        if (fonts.Count != 1)
        {
            Client.Debug.LogWarning($"Fonts stack was {fonts.Count} when it should be 1");
        }

        _fragments = fragments.ToArray();
    }

    private FormattedText(IFontGetter startingFont, Color startingColor, string formatString) : this(startingFont, startingColor,  Format.StringToInstructions(formatString))
    {
    }

    public int Length
    {
        get
        {
            if (_fragments == null || _fragments.Length == 0)
            {
                return 0;
            }

            var charCount = 0;
            foreach (var fragment in _fragments)
            {
                charCount += fragment.CharCount;
            }

            return charCount;
        }
    }

    public static FormattedText FromFormatString(IFontGetter startingFont, Color startingColor, string formatString)
    {
        return new FormattedText(startingFont, startingColor, formatString);
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

    public IEnumerable<FormattedGlyph> GetGlyphs(RectangleF rectangle, Alignment alignment)
    {
        if (_fragments == null || _fragments.Length == 0)
        {
            yield break;
        }

        var (lines, restrictedSize) = RestrictedStringBuilder.FromFragments(_fragments, rectangle.Width);
        var restrictedBounds =
            RectangleF.FromSizeAlignedWithin(rectangle, restrictedSize, alignment.JustVertical());

        var lineNumber = 0;
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
                var isWhiteSpace = letterFragment is CharGlyphData fragmentChar &&
                                   char.IsWhiteSpace(fragmentChar.Text);

                var glyphData = letterFragment;
                if (isWhiteSpace)
                {
                    // White space still has a size and scale factor, but it's a different type so it's not caught like regular text
                    glyphData = new WhiteSpaceGlyphData(letterFragment.Size, letterFragment.ScaleFactor,
                        WhiteSpaceType.Space);
                }

                yield return new FormattedGlyph(position, glyphData, lineNumber);
                letterPosition += letterSize.JustX();
            }

            lineNumber++;
        }
    }

    /// <summary>
    ///     GlyphData is the guts of the output
    /// </summary>
    public interface IGlyphData
    {
        Vector2 Size { get; }
        float ScaleFactor { get; }
        void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings);
    }

    /// <summary>
    ///     Fragments are input
    /// </summary>
    public interface IFragment
    {
        Vector2 Size { get; }
        IGlyphData ToGlyphData();
        int CharCount { get; }
    }

    /// <summary>
    ///     This is the final output of the FormattedText. These are the things you interface with.
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="Data"></param>
    /// <param name="LineNumber"></param>
    public readonly record struct FormattedGlyph(Vector2 Position, IGlyphData Data, int LineNumber)
    {
        public RectangleF Rectangle => new(Position, Data.Size);

        public void Draw(Painter painter, Vector2 offset, DrawSettings settings)
        {
            Data.OneOffDraw(painter, Position + offset, settings);
        }
    }

    public readonly record struct Fragment(IFontGetter FontGetter, string Text, Color? Color = null) : IFragment
    {
        public IFont Font => FontGetter.GetFont();
        public int CharCount => Text.Length;
        public Vector2 Size => Font.MeasureString(Text);

        public IGlyphData ToGlyphData()
        {
            // Sucks that this is written this way
            throw new Exception(
                $"{nameof(Fragment)} contains more than one character so it cannot be converted to {nameof(IGlyphData)}");
        }

        public override string ToString()
        {
            return $"{Size} \"{Text}\"";
        }
    }

    public readonly record struct CharGlyphData(IFont Font, char Text, Color? Color = null) : IGlyphData
    {
        public Vector2 Size => Font.MeasureString(Text.ToString());
        public float ScaleFactor => Font.ScaleFactor;

        public void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings)
        {
            painter.DrawStringAtPosition(Font, Text.ToString(), position,
                drawSettings with {Color = Color ?? drawSettings.Color});
        }

        public override string ToString()
        {
            return $"{Size} '{Text}'";
        }
    }

    public readonly record struct WhiteSpaceGlyphData(Vector2 Size, float ScaleFactor,
        WhiteSpaceType WhiteSpaceType) : IGlyphData
    {
        public void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings)
        {
            // this function is intentionally left blank
        }

        public override string ToString()
        {
            return $"{Size} (whitespace)";
        }
    }

    public readonly record struct ImageGlyphData(IndirectAsset<ImageAsset> Image,
        float ScaleFactor = 1f, Color? Color = null) : IGlyphData
    {
        public void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings)
        {
            painter.DrawAtPosition(Image.Get().Texture, position, new Scale2D(ScaleFactor),
                drawSettings with {SourceRectangle = Image.Get().SourceRectangle, Color = Color ?? drawSettings.Color});
        }

        public Vector2 Size => Image.Get().SourceRectangle.Size.ToVector2() * ScaleFactor;

        public override string ToString()
        {
            return $"{Size} (image)";
        }
    }

    public readonly record struct FragmentImage(IndirectAsset<ImageAsset> Image, float ScaleFactor = 1f,
        Color? Color = null) : IFragment
    {
        public FragmentImage(Texture2D texture, float scaleFactor = 1f, Color? color = null) : this(
            new ImageAsset(texture, texture.Bounds), scaleFactor, color)
        {
        }

        public Vector2 Size => Image.Get().SourceRectangle.Size.ToVector2() * ScaleFactor;

        public IGlyphData ToGlyphData()
        {
            return new ImageGlyphData(Image, ScaleFactor, Color);
        }

        /// <summary>
        /// An image is always just one character
        /// </summary>
        public int CharCount => 1;

        public override string ToString()
        {
            return $"{Size} (image)";
        }
    }

    public readonly record struct GlyphDataLine(ImmutableArray<IGlyphData> Fragments) : IEnumerable<IGlyphData>
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
                if (fragment is CharGlyphData fragmentChar)
                {
                    result.Append(fragmentChar.Text);
                }

                if (fragment is ImageGlyphData)
                {
                    result.Append("(image)");
                }
            }

            return $"{Size} {result.ToString()}";
        }
    }
}
