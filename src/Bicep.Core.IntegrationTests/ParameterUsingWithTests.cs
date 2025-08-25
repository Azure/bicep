// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ParameterUsingWithTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Example_with_scope_information_and_deployment()
    {
        var result = CompilationHelper.CompileParams(
            ("parameters.bicepparam", @"
var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  mode: 'deployment'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param foo = 'foo/bar/baz'
"),
            ("main.bicep", @"
param foo string
"));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }
}