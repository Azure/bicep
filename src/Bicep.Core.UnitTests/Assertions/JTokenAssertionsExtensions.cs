// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.FileSystem;
using FluentAssertions;
using FluentAssertions.Execution;
using JsonDiffPatchDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class JTokenAssertionsExtensions
    {
        public static JTokenAssertions Should(this JToken? instance)
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

            return new AndConstraint<JTokenAssertions>(instance);
        }

        public static AndConstraint<JTokenAssertions> DeepEqual(this JTokenAssertions instance, JToken expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(JToken.DeepEquals(instance.Subject, expected))
                .FailWith("Expected {0} but got {1}", expected.ToString(), instance.Subject?.ToString());

            return new AndConstraint<JTokenAssertions>(instance);
        }

        public static AndConstraint<JTokenAssertions> HaveValueAtPath(this JTokenAssertions instance, string jtokenPath, JToken expected, string because = "", params object[] becauseArgs)
        {
            var valueAtPath = instance.Subject?.SelectToken(jtokenPath);

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(valueAtPath is not null)
                .FailWith("Expected value at path {0} to be {1}{reason} but it was null. Original JSON: {2}", jtokenPath, expected.ToString(), instance.Subject?.ToString());

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(JToken.DeepEquals(valueAtPath, expected))
                .FailWith("Expected value at path {0} to be {1}{reason} but it was {2}", jtokenPath, expected.ToString(), valueAtPath?.ToString());

            return new AndConstraint<JTokenAssertions>(instance);
        }

        public static AndConstraint<JTokenAssertions> NotHaveValueAtPath(this JTokenAssertions instance, string jtokenPath, string because = "", params object[] becauseArgs)
        {
            var valueAtPath = instance.Subject?.SelectToken(jtokenPath);

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(valueAtPath is null)
                .FailWith("Expected value at path {0} to be null{reason}, but it was {1}", jtokenPath, valueAtPath);

            return new AndConstraint<JTokenAssertions>(instance);
        }

        public static AndConstraint<JTokenAssertions> NotHaveValue(this JTokenAssertions instance, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(instance.Subject is null)
                .FailWith("Expected value to be null{reason}, but it was {0}", instance.Subject?.ToString());

            return new AndConstraint<JTokenAssertions>(instance);
        }
    }
}