// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;

namespace Bicep.Testing.Assertions.Json;

public static class JTokenAssertionsExtensions
{
    public static JTokenAssertions Should(this JToken? instance) => new(instance);

    public static AndConstraint<JTokenAssertions> DeepEqual(this JTokenAssertions instance, JToken expected, string because = "", params object[] becauseArgs)
    {
        var diff = new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(instance.Subject, expected);

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(diff is null)
            .FailWith("Expected {0} but got {1}. Differences: {2}", expected.ToString(), instance.Subject?.ToString(), diff?.ToString());

        return new(instance);
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

        return new(instance);
    }

    public static AndConstraint<JTokenAssertions> HaveJsonAtPath(this JTokenAssertions instance, string jtokenPath, string json, string because = "", params object[] becauseArgs) =>
        instance.HaveValueAtPath(jtokenPath, JToken.Parse(json), because, becauseArgs);

    public static AndConstraint<JTokenAssertions> NotHaveValueAtPath(this JTokenAssertions instance, string jtokenPath, string because = "", params object[] becauseArgs)
    {
        var valueAtPath = instance.Subject?.SelectToken(jtokenPath);

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(valueAtPath is null)
            .FailWith("Expected value at path {0} to be null{reason}, but it was {1}", jtokenPath, valueAtPath?.ToString());

        return new(instance);
    }

    public static AndConstraint<JTokenAssertions> NotHaveValue(this JTokenAssertions instance, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(instance.Subject is null)
            .FailWith("Expected value to be null{reason}, but it was {0}", instance.Subject?.ToString());

        return new(instance);
    }
}

public class JTokenAssertions(JToken? subject) : ReferenceTypeAssertions<JToken?, JTokenAssertions>(subject)
{
    protected override string Identifier => nameof(JToken);
}
