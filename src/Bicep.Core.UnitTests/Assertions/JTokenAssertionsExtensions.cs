// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using JsonDiffPatchDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class JTokenAssertionsExtensions
    {
        public static JTokenAssertions Should(this JToken instance)
        {
            return new JTokenAssertions(instance); 
        }

        public static AndConstraint<JTokenAssertions> EqualWithJsonDiffOutput(this JTokenAssertions instance, TestContext testContext, JToken expected, string expectedLocation, string actualLocation, string because = "", params object[] becauseArgs)
        {
            const int truncate = 100;
            var diff = new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(instance.Subject, expected);

            // JsonDiffPatch.Diff returns null if there are no diffs
            var lineLogs = (diff?.ToString() ?? string.Empty)
                .Split('\n')
                .Take(truncate);

            if (lineLogs.Count() > truncate)
            {
                lineLogs = lineLogs.Concat(new[] { "...truncated..." });
            }

            var testPassed = diff is null;

            if (!testPassed && BaselineHelper.ShouldSetBaseline(testContext))
            {
                BaselineHelper.SetBaseline(actualLocation, expectedLocation);
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(testPassed)
                .FailWith(@"
Found diffs between actual and expected:
{0}
View this diff with:

git diff --color-words --no-index {1} {2}

Windows copy command:
copy /y {1} {2}

Unix copy command:
cp {1} {2}
", string.Join('\n', lineLogs), actualLocation, expectedLocation);

            return new AndConstraint<JTokenAssertions>(instance);
        }
    }
}