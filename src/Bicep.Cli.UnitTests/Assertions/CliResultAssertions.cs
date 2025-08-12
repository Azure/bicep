// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Analyzers.Linter.Rules;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.Cli.UnitTests.Assertions;

public static class CliResultExtensions
{
    public static CliResultAssertions Should(this CliResult result) => new(result);
}

public class CliResultAssertions : ReferenceTypeAssertions<CliResult, CliResultAssertions>
{
    public CliResultAssertions(CliResult instance)
        : base(instance)
    {
    }

    protected override string Identifier => "result";

    public AndConstraint<CliResultAssertions> Succeed(string because = "", params object[] becauseArgs)
    {
        Subject.ExitCode.Should().Be(0, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> Fail(string because = "", params object[] becauseArgs)
    {
        Subject.ExitCode.Should().NotBe(0, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> HaveStdout(string stdout, string because = "", params object[] becauseArgs)
    {
        Subject.Stdout.Should().Be(stdout, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> HaveStdoutMatch(string stdout, string because = "", params object[] becauseArgs)
    {
        Subject.Stdout.Should().Match(stdout, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> NotHaveStdout(string because = "", params object[] becauseArgs)
    {
        Subject.Stdout.Should().BeEmpty(because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> HaveStderr(string stderr, string because = "", params object[] becauseArgs)
    {
        Subject.Stderr.Should().Be(stderr, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> HaveStderrMatch(string stderr, string because = "", params object[] becauseArgs)
    {
        Subject.Stderr.Should().Match(stderr, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> NotHaveStderrMatch(string stderr, string because = "", params object[] becauseArgs)
    {
        Subject.Stderr.Should().NotMatch(stderr, because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> NotHaveStderr(string because = "", params object[] becauseArgs)
    {
        Subject.Stderr.Should().BeEmpty(because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> HaveCompileError(string? errorCode = null, string because = "", params object[] becauseArgs)
    {
        var errorMatch = errorCode is { } ? $"Error {errorCode}" : "Error";
        Subject.Should().HaveStderrMatch($"*{errorMatch}*", because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> NotHaveCompileError(string? errorCode = null, string because = "", params object[] becauseArgs)
    {
        var errorMatch = errorCode is { } ? $"Error {errorCode}" : "Error";
        Subject.Should().NotHaveStderrMatch($"*{errorMatch}*", because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> HaveRuleFailure(string linterRuleCode, string because = "", params object[] becauseArgs)
    {
        Subject.Should().HaveStderrMatch($"*{linterRuleCode}*", because, becauseArgs);

        return new(this);
    }

    public AndConstraint<CliResultAssertions> NotHaveRuleFailure(string linterRuleCode, string because = "", params object[] becauseArgs)
    {
        Subject.Should().NotHaveStderrMatch($"*{linterRuleCode}*", because, becauseArgs);

        return new(this);
    }
}
