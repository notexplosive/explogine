using ExplogineMonoGame.Data;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestColorExtensions
{
    [Fact]
    public void color_hex_and_back_again()
    {
        Color.Red.ToRgbaHex().Should().Be(0xff0000ff);
        ColorExtensions.FromRgbHex(0xff0000).Should().Be(Color.Red);
    }
}
