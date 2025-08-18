// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Google.Protobuf;
using JsonDiffPatchDotNet;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Extension.UnitTests.Assertions;

public static class IMessageAssertionsExtensions
{
    public static IMessageAssertions Should(this IMessage instance)
    {
        return new IMessageAssertions(instance);
    }

    public static AndConstraint<IMessageAssertions> DeepEqualJson(this IMessageAssertions instance, string json, string because = "", params object[] becauseArgs)
    {
        JsonFormatter formatter = new(JsonFormatter.Settings.Default.WithIndentation());
        formatter.Format(instance.Subject).FromJson<JToken>().Should().DeepEqual(JObject.Parse(json), because, becauseArgs);

        return new(instance);
    }
}
