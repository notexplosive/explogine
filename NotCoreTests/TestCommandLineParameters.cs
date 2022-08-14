using System;
using FluentAssertions;
using NotCore;
using Xunit;

namespace NotCoreTests;

public class TestCommandLineParameters
{
    [Fact]
    public void happy_path()
    {
        var args = new ParsedCommandLineArguments("--level=4", "--roll", "--foo=bar");
        args.AddParameter<int>("level");
        args.AddParameter<bool>("roll");
        args.AddParameter<string>("foo");

        args.GetValue<int>("level").Should().Be(4);
        args.GetValue<bool>("roll").Should().BeTrue();
        args.GetValue<string>("foo").Should().Be("bar");
    }

    [Fact]
    public void user_provides_arg_that_is_not_used()
    {
        var args = new ParsedCommandLineArguments("--nudge=mega");

        var act = () => { args.GetValue<string>("nudge"); };
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ask_for_bound_but_unset_parameter()
    {
        var args = new ParsedCommandLineArguments("--level=4", "--roll", "--take=never");
        args.AddParameter<bool>("unset");
        args.AddParameter<string>("strong");

        args.GetValue<bool>("unset").Should().BeFalse();
        args.GetValue<string>("strong").Should().BeEmpty();
    }

    [Fact]
    public void ask_for_value_that_is_not_set_or_bound()
    {
        var args = new ParsedCommandLineArguments("--level=4");
        var func = () => { args.GetValue<bool>("never_set"); };
        func.Should().Throw<Exception>();
    }

    [Fact]
    public void asked_for_wrong_type()
    {
        var args = new ParsedCommandLineArguments("--level=4");
        args.AddParameter<int>("level");

        var action = () => { args.GetValue<bool>("level"); };
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void same_parameter_bound_twice()
    {
        var args = new ParsedCommandLineArguments("--level=4");

        var act = () =>
        {
            args.AddParameter<int>("level");
            args.AddParameter<int>("level");
        };

        act.Should().Throw<Exception>();
    }
}
