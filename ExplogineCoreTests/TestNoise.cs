using ExplogineCore.Data;
using FluentAssertions;
using Xunit;

namespace ExplogineCoreTests;

public class TestNoise
{
    [Fact]
    public void double_is_between_zero_and_one()
    {
        var noise = new Noise(0);

        for (int i = 0; i < 1000; i++)
        {
            (noise.DoubleAt(i) < 1.0).Should().BeTrue($"at {i} we got {noise.DoubleAt(i)} which should be less than 1");
            (noise.DoubleAt(i) > 0.0).Should().BeTrue($"at {i} we got {noise.DoubleAt(i)} which should be more than 0");
        }
    }
}
