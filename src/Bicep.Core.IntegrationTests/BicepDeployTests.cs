// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class BicepDeployTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Bicepdeploy_basic_compilation()
    {
        var result = CompilationHelper.CompileBicepDeploy(
            new ServiceBuilder(),
            ("test.txt", """Hello $NAME!"""),
            ("main.bicepdeploy", """
deploy foo 'main.bicep' {
  params: {
    greeting: replace(loadTextContent('test.txt'), '$NAME', 'Anthony')
  }
}
"""),
            ("main.bicep", """
param greeting string

output greeting string = greeting
"""));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Bicepdeploy_type_validation_works()
    {
        var result = CompilationHelper.CompileBicepDeploy(
            new ServiceBuilder(),
            ("main.bicepdeploy", """
deploy foo 'main.bicep' {
  params: {
    bar: 'bar' // this isn't defined in the main.bicep file
  }
}
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP035", DiagnosticLevel.Error, """The specified "object" declaration is missing the following required properties: "foo"."""),
            ("BCP037", DiagnosticLevel.Error, """The property "bar" is not allowed on objects of type "parameters". Permissible properties include "foo"."""),
        ]);
    }
}