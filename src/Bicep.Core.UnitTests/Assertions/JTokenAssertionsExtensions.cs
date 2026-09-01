// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Testing.Assertions.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using JsonDiffPatchDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Assertions;

public static class JTokenAssertionsExtensions
{
    public static AndConstraint<JTokenAssertions> EqualWithJsonDiffOutput(this JTokenAssertions instance, TestContext testContext, JToken expected, string expectedLocation, string actualLocation, string because = "", bool validateLocation = true, params object[] becauseArgs)
    {
        var diff = new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(instance.Subject, expected);
        var jsonDiff = diff?.ToString();
        var testPassed = jsonDiff is null;

        if (validateLocation)
        {
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
                    jsonDiff,
                    BaselineHelper.GetAbsolutePathRelativeToRepoRoot(actualLocation),
                    BaselineHelper.GetAbsolutePathRelativeToRepoRoot(expectedLocation));
        }
        else
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(testPassed)
                .FailWith(jsonDiff);
        }

        return new(instance);
    }
}
