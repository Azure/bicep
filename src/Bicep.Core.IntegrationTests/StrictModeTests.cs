// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class StrictModeTests
{
    [TestMethod]
    public void Test_Issue746()
    {
        var result = CompilationHelper.Compile(@"
var l = l
param l
");

        result.Template.Should().NotHaveValue();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
            ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
            ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
            ("BCP279", DiagnosticLevel.Error, "Expected a type at this location. Please specify a valid type expression or one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
        });
    }
}