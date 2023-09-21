// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.UnitTests.Assertions;

public static class CliResultAssertionsExtensions
{
    public static CliResultAssertions Should(this CliResult result) => new(result);

    public static AndConstraint<CliResultAssertions> Succeed(this CliResultAssertions instance, string because = "", params object[] becauseArgs)
    {
        instance.Subject.ExitCode.Should().Be(0, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<CliResultAssertions> Fail(this CliResultAssertions instance, string because = "", params object[] becauseArgs)
    {
        instance.Subject.ExitCode.Should().NotBe(0, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<CliResultAssertions> HaveStdout(this CliResultAssertions instance, string stdout, string because = "", params object[] becauseArgs)
    {
        instance.Subject.Stdout.Should().Be(stdout, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<CliResultAssertions> HaveStdoutMatch(this CliResultAssertions instance, string stdout, string because = "", params object[] becauseArgs)
    {
        instance.Subject.Stdout.Should().Match(stdout, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<CliResultAssertions> NotHaveStdout(this CliResultAssertions instance, string because = "", params object[] becauseArgs)
        => HaveStdout(instance, "", because, becauseArgs);

    public static AndConstraint<CliResultAssertions> HaveStderr(this CliResultAssertions instance, string stderr, string because = "", params object[] becauseArgs)
    {
        instance.Subject.Stderr.Should().Be(stderr, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<CliResultAssertions> HaveStderrMatch(this CliResultAssertions instance, string stderr, string because = "", params object[] becauseArgs)
    {
        instance.Subject.Stderr.Should().Match(stderr, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<CliResultAssertions> NotHaveStderr(this CliResultAssertions instance, string because = "", params object[] becauseArgs)
        => HaveStderr(instance, "", because, becauseArgs);
}
