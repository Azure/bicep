// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Testing.Assertions.Json;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using JsonDiffPatchDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Testing.Baselines;

public static class BaselineAssertionsExtensions
{
    public static AndConstraint<StringAssertions> MatchTextBaseline(this StringAssertions instance, BaselineFile baselineFile, string because = "", params object[] becauseArgs)
    {
        baselineFile.Write(instance.Subject);

        return instance.MatchTextBaseline(
            baselineFile.TestContext,
            baselineFile.EmbeddedFile.Contents,
            baselineFile.EmbeddedFile.RelativeSourcePath,
            baselineFile.OutputFilePath,
            because,
            becauseArgs);
    }

    public static AndConstraint<StringAssertions> MatchJsonBaseline(this StringAssertions instance, BaselineFile baselineFile, string because = "", params object[] becauseArgs)
    {
        baselineFile.Write(instance.Subject);

        JToken.Parse(instance.Subject).Should().MatchJsonBaseline(
            baselineFile.TestContext,
            JToken.Parse(baselineFile.EmbeddedFile.Contents),
            baselineFile.EmbeddedFile.RelativeSourcePath,
            baselineFile.OutputFilePath,
            because,
            validateLocation: true,
            becauseArgs);

        return new(instance);
    }

    public static AndConstraint<StringAssertions> MatchTextBaseline(this StringAssertions instance, TestContext testContext, string expected, string expectedPath, string actualPath, string because = "", params object[] becauseArgs)
    {
        var lineDiff = CalculateDiff(expected, instance.Subject);
        var hasNewlineDiffsOnly = lineDiff is null && !expected.Equals(instance.Subject, StringComparison.Ordinal);
        var testPassed = lineDiff is null && !hasNewlineDiffsOnly;

        var isBaselineUpdate = !testPassed && BaselineUpdate.IsEnabled(testContext);
        if (isBaselineUpdate)
        {
            BaselineUpdate.Apply(actualPath, expectedPath);
        }

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(testPassed)
            .FailWith(
                BaselineUpdate.GetFailureMessage(isBaselineUpdate),
                lineDiff ?? "differences in newlines only",
                TestRepository.GetAbsolutePath(actualPath),
                TestRepository.GetAbsolutePath(expectedPath));

        return new(instance);
    }

    public static AndConstraint<JTokenAssertions> MatchJsonBaseline(this JTokenAssertions instance, BaselineFile baselineFile, string because = "", params object[] becauseArgs)
    {
        baselineFile.Write(instance.Subject?.ToString() ?? "null");

        return instance.MatchJsonBaseline(
            baselineFile.TestContext,
            JToken.Parse(baselineFile.EmbeddedFile.Contents),
            baselineFile.EmbeddedFile.RelativeSourcePath,
            baselineFile.OutputFilePath,
            because,
            validateLocation: true,
            becauseArgs);
    }

    public static AndConstraint<JTokenAssertions> MatchJsonBaseline(this JTokenAssertions instance, TestContext testContext, JToken expected, string expectedLocation, string actualLocation, string because = "", bool validateLocation = true, params object[] becauseArgs)
    {
        var diff = new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(instance.Subject, expected);
        var jsonDiff = diff?.ToString();
        var testPassed = jsonDiff is null;

        if (validateLocation)
        {
            var isBaselineUpdate = !testPassed && BaselineUpdate.IsEnabled(testContext);
            if (isBaselineUpdate)
            {
                BaselineUpdate.Apply(actualLocation, expectedLocation);
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(testPassed)
                .FailWith(
                    BaselineUpdate.GetFailureMessage(isBaselineUpdate),
                    jsonDiff,
                    TestRepository.GetAbsolutePath(actualLocation),
                    TestRepository.GetAbsolutePath(expectedLocation));
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

    private static string? CalculateDiff(string expected, string actual, int truncate = 100)
    {
        var diff = InlineDiffBuilder.Diff(expected, actual);
        var lineLogs = diff.Lines
            .Where(line => line.Type != ChangeType.Unchanged)
            .Select(line => $"[{line.Position}] {GetDiffMarker(line.Type)} {EscapeWhitespace(line.Text)}")
            .Take(truncate);

        if (lineLogs.Count() >= truncate)
        {
            lineLogs = lineLogs.Concat(["...truncated..."]);
        }

        return diff.HasDifferences ? string.Join('\n', lineLogs) : null;
    }

    private static string EscapeWhitespace(string input)
        => input.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");

    private static string GetDiffMarker(ChangeType type)
        => type switch
        {
            ChangeType.Inserted => "++",
            ChangeType.Modified => "//",
            ChangeType.Deleted => "--",
            _ => "  ",
        };
}
