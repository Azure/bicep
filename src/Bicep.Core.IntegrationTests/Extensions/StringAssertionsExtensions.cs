// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Bicep.Core.IntegrationTests.Extensons
{
    public static class StringAssertionsExtensions 
    {
        private static string GetDiffMarker(ChangeType type)
            => type switch {
                ChangeType.Inserted => "++",
                ChangeType.Modified => "//",
                ChangeType.Deleted => "--",
                _ => "  ",
            };

        public static AndConstraint<StringAssertions> EqualWithLineByLineDiffOutput(this StringAssertions instance, string expected, string sourceLocation, string targetLocation, string because = "", params object[] becauseArgs)
        {
            const int truncate = 100;
            var diff = InlineDiffBuilder.Diff(instance.Subject, expected);

            var lineLogs = diff.Lines
                .Where(line => line.Type != ChangeType.Unchanged)
                .Select(line => $"[{line.Position}] {GetDiffMarker(line.Type)} {OutputHelper.EscapeWhitespace(line.Text)}")
                .Take(truncate);

            if (diff.Lines.Count > truncate)
            {
                lineLogs = lineLogs.Concat(new[] { "...truncated..." });
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!diff.HasDifferences)
                .FailWith(@"
Found diffs between actual and expected:
{0}
View this diff with:

git diff --color-words --no-index {1} {2}

Windows copy command:
copy /y {1} {2}

Unix copy command:
cp {1} {2}
", string.Join('\n', lineLogs), targetLocation, sourceLocation);

            return new AndConstraint<StringAssertions>(instance);
        } 
    }
}
