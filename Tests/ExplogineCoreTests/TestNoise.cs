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

        var average = 0.0;

        var iterations = 1000;
        for (var i = 0; i < iterations; i++)
        {
            (noise.DoubleAt(i) < 1.0).Should().BeTrue($"at {i} we got {noise.DoubleAt(i)} which should be less than 1");
            (noise.DoubleAt(i) > 0.0).Should().BeTrue($"at {i} we got {noise.DoubleAt(i)} which should be more than 0");

            average += noise.DoubleAt(i);
        }

        average /= iterations;
        average.Should().BeApproximately(0.5, 0.01);
    }

    [Fact]
    public void probability_is_about_right()
    {
        var noise = new Noise(0);

        var count0 = 0;
        var count1 = 0;
        var count2 = 0;
        var count3 = 0;

        for (var i = 0; i < 1000; i++)
        {
            var j = noise.IntAt(i, 4);

            switch (j)
            {
                case 0:
                    count0++;
                    break;
                case 1:
                    count1++;
                    break;
                case 2:
                    count2++;
                    break;
                case 3:
                    count3++;
                    break;
            }
        }

        count0.Should().BeInRange(220, 280);
        count1.Should().BeInRange(220, 280);
        count2.Should().BeInRange(220, 280);
        count3.Should().BeInRange(220, 280);
    }
}
