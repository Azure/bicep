// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests.Utils;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        private static string? CalculateDiff(string expected, string actual, bool ignoreWhiteSpace = false, bool ignoreCase = false, int truncate = 100)
        {
            var diff = InlineDiffBuilder.Diff(expected, actual, ignoreWhiteSpace: ignoreWhiteSpace, ignoreCase: ignoreCase);

            var lineLogs = diff.Lines
                .Where(line => line.Type != ChangeType.Unchanged)
                .Select(line => $"[{line.Position}] {GetDiffMarker(line.Type)} {EscapeWhitespace(line.Text)}")
                .Take(truncate);

            if (lineLogs.Count() >= truncate)
            {
                lineLogs = lineLogs.Concat(new[] { "...truncated..." });
            }

            if (!diff.HasDifferences)
            {
                return null;
            }

            return string.Join('\n', lineLogs);
        }

        public static AndConstraint<StringAssertions> EqualWithLineByLineDiffOutput(this StringAssertions instance, TestContext testContext, string expected, string expectedLocation, string actualLocation, string because = "", params object[] becauseArgs)
        {
            var lineDiff = CalculateDiff(expected, instance.Subject);
            var testPassed = lineDiff is null;

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
                    lineDiff,
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

        /// <summary>
        /// Compares two strings after normalizing by unindenting lines until the least indented line is flushed left, similar to
        /// YAML blocks of text.
        /// </summary>
        public static AndConstraint<StringAssertions> EqualIgnoringMinimumIndent(this StringAssertions instance, string? expected, string because = "", params object[] becauseArgs)
        {
            var normalizedActual = instance.Subject is null ? null : StringTestUtils.Unindent(StringUtils.ReplaceNewlines(instance.Subject, "\n"));
            var normalizedExpected = expected is null ? null : StringTestUtils.Unindent(StringUtils.ReplaceNewlines(expected, "\n"));

            normalizedActual.Should().Be(normalizedExpected, because, becauseArgs);

            return new AndConstraint<StringAssertions>(instance);
        }

        /// <summary>
        /// Compares two strings after normalizing by removing whitespace from the beginning and ending of all lines
        /// </summary>
        public static AndConstraint<StringAssertions> EqualTrimmedLines(this StringAssertions instance, string? expected, string because = "", params object[] becauseArgs)
        {
            var normalizedActual = instance.Subject is null ? null : StringTestUtils.TrimAllLines(StringUtils.ReplaceNewlines(instance.Subject, "\n"));
            var normalizedExpected = expected is null ? null : StringTestUtils.TrimAllLines(StringUtils.ReplaceNewlines(expected, "\n"));

            normalizedActual.Should().Be(normalizedExpected, because, becauseArgs);

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> ContainIgnoringNewlines(this StringAssertions instance, string expected)
        {
            var normalizedActual = StringUtils.ReplaceNewlines(instance.Subject, "\n");
            var normalizedExpected = StringUtils.ReplaceNewlines(expected, "\n");

            normalizedActual.Should().Contain(normalizedExpected);

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> BeEquivalentToPath(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                instance.Subject.Should().BeEquivalentTo(expected, because, becauseArgs);
            }
            else
            {
                instance.Subject.Should().Be(expected, because, becauseArgs);
            }

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
