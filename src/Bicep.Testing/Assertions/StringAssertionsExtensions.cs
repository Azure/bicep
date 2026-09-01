// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.Testing.Assertions;

public static class StringAssertionsExtensions
{
    public static AndConstraint<StringAssertions> BeValidBicepText(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
    {
        ValidateBicepText(instance, because, becauseArgs);
        instance.Be(expected, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<StringAssertions> BeValidBicepTextIgnoringNewlines(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
    {
        ValidateBicepText(instance, because, becauseArgs);
        instance.BeEquivalentToIgnoringNewlines(expected, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<StringAssertions> BeEquivalentToIgnoringNewlines(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
    {
        var normalizedActual = instance.Subject.ReplaceLineEndings("\n");
        var normalizedExpected = expected.ReplaceLineEndings("\n");

        normalizedActual.Should().Be(normalizedExpected, because, becauseArgs);

        return new(instance);
    }

    private static void ValidateBicepText(StringAssertions instance, string because, object[] becauseArgs)
    {
        TestParser.Parse(instance.Subject, out var syntaxErrors);
        syntaxErrors.Should().BeEmpty(because, becauseArgs);
    }
}
