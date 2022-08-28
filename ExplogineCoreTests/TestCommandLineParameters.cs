using System;
using ExplogineCore;
using FluentAssertions;
using Xunit;

namespace ExplogineCoreTests;

public class TestCommandLineParameters
{
    [Fact]
    public void happy_path()
    {
        var commandLine = new CommandLineParameters("--level=4", "--roll", "--foo=bar");
        commandLine.RegisterParameter<int>("level");
        commandLine.RegisterParameter<bool>("roll");
        commandLine.RegisterParameter<string>("foo");

        commandLine.Args.GetValue<int>("level").Should().Be(4);
        commandLine.Args.GetValue<bool>("roll").Should().BeTrue();
        commandLine.Args.GetValue<string>("foo").Should().Be("bar");
    }

    [Fact]
    public void user_provides_arg_that_is_not_used()
    {
        var commandLine = new CommandLineParameters("--nudge=mega");

        var act = () => { commandLine.Args.GetValue<string>("nudge"); };
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ask_for_bound_but_unset_parameter()
    {
        var args = new CommandLineParameters("--level=4", "--roll", "--take=never");
        args.RegisterParameter<bool>("unset");
        args.RegisterParameter<string>("strong");

        args.Args.GetValue<bool>("unset").Should().BeFalse();
        args.Args.GetValue<string>("strong").Should().BeEmpty();
    }

    [Fact]
    public void ask_for_value_that_is_not_set_or_bound()
    {
        var commandLine = new CommandLineParameters("--level=4");
        var func = () => { commandLine.Args.GetValue<bool>("never_set"); };
        func.Should().Throw<Exception>();
    }

    [Fact]
    public void asked_for_wrong_type()
    {
        var parameters = new CommandLineParameters("--level=4");
        parameters.RegisterParameter<int>("level");

        var action = () => { parameters.Args.GetValue<bool>("level"); };
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void same_parameter_bound_twice()
    {
        var commandLineParams = new CommandLineParameters("--level=4");

        var act = () =>
        {
            commandLineParams.RegisterParameter<int>("level");
            commandLineParams.RegisterParameter<int>("level");
        };

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ignore_capital_letters_when_appropriate()
    {
        var parameters = new CommandLineParameters("--Mode=eDiTor");
        parameters.RegisterParameter<string>("mode");

        parameters.Args.GetValue<string>("MODE").Should().Be("eDiTor");
    }
}
