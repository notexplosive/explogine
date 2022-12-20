using System.Linq;
using ExplogineMonoGame.Data;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestFormattedText
{
    [Fact]
    public void chars_in_equals_glyphs_out()
    {
        void TestCase(string input)
        {
            var formattedText = new FormattedText(new TestFont(), input);
            var glyphs = formattedText
                .GetGlyphs(new RectangleF(0, 0, float.MaxValue, float.MaxValue), Alignment.TopLeft).ToList();
            glyphs.Count.Should().Be(input.Length, $"\"{input}\" should have {input.Length} glyphs");
        }

        TestCase("There is just one line");
        TestCase("There\nis\none\nline\nper\nword");
        TestCase("There\n\nare\n\ntwo\n\nlines\n\nper\n\nword");
    }

    [Fact]
    public void one_newline_does_right_thing()
    {
        var font = new TestFont();
        var formattedText = new FormattedText(font, "String with\nOne newline");
        var glyphs = formattedText
            .GetGlyphs(new RectangleF(0, 0, float.MaxValue, float.MaxValue), Alignment.TopLeft).ToList();

        // this is a janky way to find the newline but at time of writing this test case the glyph count does not equal char count so there's really no good way to do this.
        foreach (var glyph in glyphs)
        {
            if (glyph.Data is FormattedText.FragmentChar fragmentChar)
            {
                if (fragmentChar.Text == 'O') // the O in "One newline"
                {
                    glyph.Position.Y.Should().Be(font.Height);
                }
            }
        }
    }

    [Fact]
    public void two_newlines_does_right_thing()
    {
        var font = new TestFont();
        var formattedText = new FormattedText(font, "String with\n\nTwo newline");
        var glyphs = formattedText
            .GetGlyphs(new RectangleF(0, 0, float.MaxValue, float.MaxValue), Alignment.TopLeft).ToList();

        foreach (var glyph in glyphs)
        {
            if (glyph.Data is FormattedText.FragmentChar fragmentChar)
            {
                if (fragmentChar.Text == 'T')
                {
                    glyph.Position.Y.Should().Be(font.Height * 2);
                }
            }
        }
    }
}

/// <summary>
///     Acts as a mock font for tests.
///     Maybe we can convert this to VirtualFont and bring it into product code?
/// </summary>
public class TestFont : IFont
{
    public IFont GetFont()
    {
        return this;
    }

    public float ScaleFactor => 1f;
    public float Height => 32;

    public Vector2 MeasureString(string text, float? restrictedWidth = null)
    {
        if (!restrictedWidth.HasValue)
        {
            return new Vector2(text.Length * ScaleFactor, Height);
        }

        return RestrictedStringBuilder.FromText(text, restrictedWidth.Value, this).Size;
    }
}
