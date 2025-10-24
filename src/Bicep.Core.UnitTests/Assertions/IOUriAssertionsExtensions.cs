// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Assertions;

public static class IOUriAssertionsExtensions
{
    public static IOUriAssertions Should(this IOUri instance) => new(instance);

    public static AndConstraint<IOUriAssertions> Exist(
        this IOUriAssertions instance,
        TestContext testContext,
        string content,
        string because = "",
        params object[] becauseArgs)
    {
        var testPassed = File.Exists(instance.Subject.GetFilePath());
        var isBaselineUpdate = !testPassed && BaselineHelper.ShouldSetBaseline(testContext);

        if (isBaselineUpdate)
        {
            if (Path.GetDirectoryName(instance.Subject.GetFilePath()) is string dirName)
            {
                Directory.CreateDirectory(dirName);
            }
            File.WriteAllText(instance.Subject.GetFilePath(), content);
        }

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(testPassed)
            .FailWith($"A baseline file should be present at {instance.Subject.GetFilePath()}.");

        return new AndConstraint<IOUriAssertions>(instance);
    }

    public static AndConstraint<IOUriAssertions> NotExist(
        this IOUriAssertions instance,
        TestContext testContext,
        string because = "",
        params object[] becauseArgs)
    {
        var testPassed = !File.Exists(instance.Subject.GetFilePath());
        var isBaselineUpdate = !testPassed && BaselineHelper.ShouldSetBaseline(testContext);

        if (isBaselineUpdate)
        {
            File.Delete(instance.Subject.GetFilePath());
        }

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(testPassed)
            .FailWith($"No baseline file should be present at {instance.Subject.GetFilePath()}.");

        return new AndConstraint<IOUriAssertions>(instance);
    }
}
