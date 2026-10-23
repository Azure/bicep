// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Testing.Utilities;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Bicep.Testing.Assertions.Json;

public static class JTokenAssertionsExtensions
{
    public static AndConstraint<JTokenAssertions> HaveJsonAtPath(this JTokenAssertions instance, string jtokenPath, string json, string because = "", params object[] becauseArgs) =>
        instance.HaveValueAtPath(jtokenPath, JToken.Parse(json), because, becauseArgs);
}