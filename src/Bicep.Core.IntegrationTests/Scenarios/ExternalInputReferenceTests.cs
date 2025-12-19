// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.IntegrationTests.Scenarios;

[TestClass]
public class ExternalInputReferenceTests
{
    [TestMethod]
    public void Valid_externalInput_reference_in_declared_function_lambda()
    {
        var result = CompilationHelper.Compile(@"
            func foo() any => externalInput('type', 'myInput')
            func bar(test string) any => sys.externalInput('type', test)
        ");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Invalid_externalInput_reference_in_variable()
    {
        var result = CompilationHelper.Compile(@"
            var foo = externalInput('type', 'myInput')
        ");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void Invalid_externalInput_reference_in_output()
    {
        var result = CompilationHelper.Compile(@"
            output foo any = externalInput('type', 'myInput')
        ");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void Invalid_externalInput_reference_in_param_definition()
    {
        var result = CompilationHelper.Compile(@"
            param foo any = externalInput('type', 'myInput')
        ");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void Invalid_externalInput_reference_in_resource_definition()
    {
        var result = CompilationHelper.Compile(@"
            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                name: 'name'
                location: resourceGroup().location
                kind: 'StorageV2'
                sku: {
                    name: 'Premium_LRS'
                }
                properties: {
                    allowBlobPublicAccess: externalInput('type', 'myInput')
                }
            }
        ");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }
}
