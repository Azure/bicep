// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
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
            => type switch {
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

            if (lineLogs.Count() > truncate)
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
    }
}
