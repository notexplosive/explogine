using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.TextFormatting;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestTextFormattingParser
{
    [Fact]
    public void push_one_color_integration()
    {
        var result = Format.StringToInstructions("This should push a [color(ff0000)]color[/color]");
        
        result.Should().HaveCount(4);
        result[0].Should().BeOfType<StringLiteralInstruction>().Which.Text.Should().Be("This should push a ");
        result[1].Should().BeOfType<PushColor>().Which.Color.Should().Be(Color.Red);
        result[2].Should().BeOfType<StringLiteralInstruction>().Which.Text.Should().Be("color");
        result[3].Should().BeOfType<PopColor>();
    }

    [Fact]
    public void push_color()
    {
        new PushColor(new[] {"abcdef"}).Color.Should().Be(new Color(0xab, 0xcd, 0xef, 0xff));
    }
    
    [Fact]
    public void push_font()
    {
        var push = new PushFont(new[] {"fonts/cool-font", "32"});
        var font = push.Font.Should().BeOfType<IndirectFont>();
        font.Which.FontSize.Should().Be(32);
        font.Which.SpriteFontPath.Should().Be("fonts/cool-font");
    }

    [Fact]
    public void image_instruction()
    {
        var image = new ImageLiteralInstruction(new[] {"cool-image", "0.5"});
        image.ScaleFactor.Should().Be(0.5f);
    }
}
