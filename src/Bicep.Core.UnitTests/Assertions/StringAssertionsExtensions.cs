// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Assertions
{
    public static partial class StringAssertionsExtensions
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

        private static Regex CamelCasingRegex = new(@"^[a-z][a-zA-Z0-9]*$");
        private static Regex KebabCasingRegex = new(@"^[a-z][a-z0-9-]*[a-z0-9]$");

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

        public static AndConstraint<StringAssertions> EqualWithLineByLineDiff(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
        {
            var lineDiff = CalculateDiff(expected, instance.Subject);
            var hasNewlineDiffsOnly = lineDiff is null && !expected.Equals(instance.Subject, System.StringComparison.Ordinal);
            var testPassed = lineDiff is null && !hasNewlineDiffsOnly;

            var output = new StringBuilder();
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(testPassed)
                .FailWith(
                    """
                    Expected strings to be equal{reason}, but they are not.
                    ===== DIFF (--actual, ++expected) =====
                    {4}
                    ===== ACTUAL (length {0}) =====
                    {1}
                    ===== EXPECTED (length {2}) =====
                    {3}
                    ===== END =====
                    """,
                    instance.Subject.Length,
                    instance.Subject,
                    expected.Length,
                    expected,
                    lineDiff ?? "differences in newlines only");

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> EqualWithLineByLineDiffOutput(this StringAssertions instance, TestContext testContext, string expected, string expectedPath, string actualPath, string because = "", params object[] becauseArgs)
        {
            var lineDiff = CalculateDiff(expected, instance.Subject);
            var hasNewlineDiffsOnly = lineDiff is null && !expected.Equals(instance.Subject, System.StringComparison.Ordinal);
            var testPassed = lineDiff is null && !hasNewlineDiffsOnly;

            var isBaselineUpdate = !testPassed && BaselineHelper.ShouldSetBaseline(testContext);
            if (isBaselineUpdate)
            {
                BaselineHelper.SetBaseline(actualPath, expectedPath);
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(testPassed)
                .FailWith(
                    BaselineHelper.GetAssertionFormatString(isBaselineUpdate),
                    lineDiff ?? "differences in newlines only",
                    BaselineHelper.GetAbsolutePathRelativeToRepoRoot(actualPath),
                    BaselineHelper.GetAbsolutePathRelativeToRepoRoot(expectedPath));

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> BeEquivalentToIgnoringNewlines(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
        {
            var normalizedActual = StringUtils.ReplaceNewlines(instance.Subject, "\n");
            var normalizedExpected = StringUtils.ReplaceNewlines(expected, "\n");

            normalizedActual.Should().BeEquivalentTo(normalizedExpected, because, becauseArgs);

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> BeEquivalentToIgnoringTrailingWhitespace(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
        {
            var normalizedActual = string.Join("\n", StringUtils.SplitOnNewLine(instance.Subject).Select(x => x.TrimEnd()));
            var normalizedExpected = string.Join("\n", StringUtils.SplitOnNewLine(expected).Select(x => x.TrimEnd()));

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

        public static AndConstraint<StringAssertions> EqualIgnoringTrailingWhitespace(this StringAssertions instance, string expected, string because = "", params object[] becauseArgs)
        {
            var normalizedActual = string.Join("\n", instance.Subject.ReplaceLineEndings("\n").Split("\n").Select(x => x.TrimEnd()));
            var normalizedExpected = string.Join("\n", expected.ReplaceLineEndings("\n").Split("\n").Select(x => x.TrimEnd()));

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
        /// Compares two strings after normalizing by removing all whitespace
        /// </summary>
        public static AndConstraint<StringAssertions> EqualIgnoringWhitespace(this StringAssertions instance, string? expected, string because = "", params object[] becauseArgs)
        {
            var actualStringWithoutWhitespace = instance.Subject is null ? null : new Regex("\\s*").Replace(instance.Subject, "");
            var expectedStringWithoutWhitespace = expected is null ? null : new Regex("\\s*").Replace(expected, "");

            using (new AssertionScope())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(string.Equals(expectedStringWithoutWhitespace, actualStringWithoutWhitespace, StringComparison.Ordinal))
                    .FailWith("Expected {context:string} to be {0}{reason} when ignoring whitespace, but found {1}.  See next message for details.", expected, instance.Subject);

                actualStringWithoutWhitespace.Should().Be(expectedStringWithoutWhitespace);
            }

            return new AndConstraint<StringAssertions>(instance);
        }

        /// <summary>
        /// Compares two strings after normalizing by pretty-printing both strings as Bicep
        /// </summary>
        public static AndConstraint<StringAssertions> EqualIgnoringBicepFormatting(this StringAssertions instance, string? expected, string because = "", params object[] becauseArgs)
        {
            var actualStringFormatted = instance.Subject is null ? null : PrettyPrintAsBicep(instance.Subject);
            var expectedStringFormatted = expected is null ? null : PrettyPrintAsBicep(expected);

            using (new AssertionScope())
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(string.Equals(expectedStringFormatted, actualStringFormatted, StringComparison.Ordinal))
                    .FailWith("Expected {context:string} to be {0}{reason} when ignoring Bicep formatting, but found {1}.  See next message for details.", expected, instance.Subject);

                actualStringFormatted.Should().Be(expectedStringFormatted);
            }

            return new AndConstraint<StringAssertions>(instance);

            static string PrettyPrintAsBicep(string s) => PrettyPrinterV2.PrintValid(CompilationHelper.Compile(s).SourceFile.ProgramSyntax, PrettyPrinterV2Options.Default);
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

        public static AndConstraint<StringAssertions> BeInCamelCasing(this StringAssertions instance, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(CamelCasingRegex.Match(instance.Subject).Success)
                .FailWith("Expected {0} to be in kebab casing (e.g. 'thisIsCamelCasing')", instance.Subject);

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> BeInKebabCasing(this StringAssertions instance, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(KebabCasingRegex.Match(instance.Subject).Success)
                .FailWith("Expected {0} to be in kebab casing (e.g. 'this-is-kebab-casing')", instance.Subject);

            return new AndConstraint<StringAssertions>(instance);
        }

        public static AndConstraint<StringAssertions> OnlyContainLFNewline(this StringAssertions instance, string because = "", params object[] becauseArgs) =>
            instance.HaveConsistentNewlines("\n", because, becauseArgs);

        public static AndConstraint<StringAssertions> HaveConsistentNewlines(this StringAssertions instance, string newline, string because = "", params object[] becauseArgs)
        {
            var newlines = NewlineSequence().Matches(instance.Subject).Select(x => x.Value).Distinct().ToArray();

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(newlines.Length == 1 && newlines[0] == newline)
                .FailWith("Expected all newlines in {0} to be {1}, but found inconsistent newlines.", instance.Subject, newline);

            return new(instance);
        }

        private static bool Contains(string actual, string expected, StringComparison comparison)
        {
            return (actual ?? string.Empty).Contains(expected ?? string.Empty, comparison);
        }

        [GeneratedRegex("\r\n|\r|\n")]
        private static partial Regex NewlineSequence();
    }
}
