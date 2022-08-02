// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.Parsing;
using System.Collections.Generic;
using System;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class StringAssertionsExtensions
    {
        private static string EscapeWhitespace(string input)
            => input
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");

        private static string GetDiffMarker(ChangeType type)
            => type switch
            {
                ChangeType.Inserted => "++",
                ChangeType.Modified => "//",
                ChangeType.Deleted => "--",
                _ => "  ",
            };

        public static AndConstraint<StringAssertions> EqualWithLineByLineDiffOutput(this StringAssertions instance, TestContext testContext, string expected, string expectedLocation, string actualLocation, string because = "", params object[] becauseArgs)
        {
            const int truncate = 100;
            var diff = InlineDiffBuilder.Diff(instance.Subject, expected, ignoreWhiteSpace: false, ignoreCase: false);

            var lineLogs = diff.Lines
                .Where(line => line.Type != ChangeType.Unchanged)
                .Select(line => $"[{line.Position}] {GetDiffMarker(line.Type)} {EscapeWhitespace(line.Text)}")
                .Take(truncate);

            if (lineLogs.Count() >= truncate)
            {
                lineLogs = lineLogs.Concat(new[] { "...truncated..." });
            }

            var testPassed = !diff.HasDifferences;
            var isBaselineUpdate = !testPassed && BaselineHelper.ShouldSetBaseline(testContext);
            if (isBaselineUpdate)
            {
                BaselineHelper.SetBaseline(actualLocation, expectedLocation);
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(testPassed)
                .FailWith(
                    BaselineHelper.GetAssertionFormatString(isBaselineUpdate),
                    string.Join('\n', lineLogs),
                    BaselineHelper.GetAbsolutePathRelativeToRepoRoot(actualLocation),
                    BaselineHelper.GetAbsolutePathRelativeToRepoRoot(expectedLocation));

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> BeEquivalentToIgnoringNewlines(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
        {
            var normalizedActual = StringUtils.ReplaceNewlines(instance.Subject, "\n");
            var normalizedExpected = StringUtils.ReplaceNewlines(expected, "\n");

            normalizedActual.Should().BeEquivalentTo(normalizedExpected, because, becauseArgs);

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> EqualIgnoringNewlines(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
        {
            var normalizedActual = StringUtils.ReplaceNewlines(instance.Subject, "\n");
            var normalizedExpected = StringUtils.ReplaceNewlines(expected, "\n");

            normalizedActual.Should().Be(normalizedExpected, because, becauseArgs);

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> HaveLengthLessThanOrEqualTo(this StringAssertions instance, int maxLength, string because = "", params object[] becauseArgs)
        {
            int length = instance.Subject.Length;
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(length <= maxLength)
                .FailWith("Expected {0} to have length less than or equal to {1}, but it has length {2}", instance.Subject, maxLength, length);

            return new AndConstraint<StringAssertions>(instance);
        }

        // Adds StringComparison to StringAssertions.NotContainAny
        public static AndConstraint<StringAssertions> NotContainAny(this StringAssertions instance, IEnumerable<string> values, StringComparison stringComparison, string because = "", params object[] becauseArgs)
        {
            IEnumerable<string> enumerable = values.Where((string v) => Contains(instance.Subject, v, stringComparison));
            Execute.Assertion.ForCondition(!enumerable.Any()).BecauseOf(because, becauseArgs).FailWith("Did not expect {context:string} {0} to contain any of the strings: {1}{reason}.",
                instance.Subject, enumerable);
            return new AndConstraint<StringAssertions>(instance);
        }

        private static bool Contains(string actual, string expected, StringComparison comparison)
        {
            return (actual ?? string.Empty).Contains(expected ?? string.Empty, comparison);
        }
    }
}
