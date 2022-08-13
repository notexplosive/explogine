using FluentAssertions;
using NotCore;
using Xunit;

namespace NotCoreTests;

public class TestCommandLineParameters
{
    [Fact]
    public void happy_path()
    {
        var args = new CommandLineArguments("--snazziness=4", "--roll", "--take=never");
        args.BindToParameter(new CommandLineInt("snazziness"));
        args.BindToParameter(new CommandLineBool("roll"));
        args.BindToParameter(new CommandLineString("take"));
        
        args.GetCommandLineValue<CommandLineInt>("snazziness")!.Value.Should().Be(4);
        args.GetCommandLineValue<CommandLineBool>("roll")!.Value.Should().BeTrue();
        args.GetCommandLineValue<CommandLineString>("take")!.Value.Should().Be("never");
    }

    [Fact]
    public void unset_value_is_default()
    {
        var args = new CommandLineArguments("--snazziness=4", "--roll", "--take=never");

        args.GetCommandLineValue<CommandLineBool>("neversetbool")!.Value.Should().BeFalse();
    }
}