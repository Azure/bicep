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
    public void ExternalInput_reference_in_declared_function_lambda_is_allowed()
    {
        var result = CompilationHelper.Compile("""
func foo() any => externalInput('type', 'myInput')
func bar(test string) any => sys.externalInput('type', test)
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void ExternalInput_reference_in_nested_declared_function_lambda_is_allowed()
    {
        var result = CompilationHelper.Compile("""
func foo(test string) any => externalInput('type', test)
func bar() any => foo('myInput')
func baz() any => bar()
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void ExternalInput_reference_in_bicepparam_file_is_allowed()
    {
        var result = CompilationHelper.CompileParams(
            ("parameters.bicepparam", """
using none
param foo = externalInput('type', 'myInput')
var bar = externalInput('type', 'myInput')
"""));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void ExternalInput_reference_in_variable_is_blocked()
    {
        var result = CompilationHelper.Compile("""
var foo = externalInput('type', 'myInput')
""");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void ExternalInput_reference_in_output_is_blocked()
    {
        var result = CompilationHelper.Compile("""
output foo any = externalInput('type', 'myInput')
""");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void ExternalInput_reference_in_param_definition_is_blocked()
    {
        var result = CompilationHelper.Compile("""
param foo any = externalInput('type', 'myInput')
""");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void ExternalInput_reference_in_resource_definition_is_blocked()
    {
        var result = CompilationHelper.Compile("""
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
""");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void ExternalInput_reference_in_object_property_is_blocked()
    {
        var result = CompilationHelper.Compile("""
var obj = {
   prop: externalInput('type', 'myInput')
}
""");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void ExternalInput_reference_in_module_param_is_blocked()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
module myModule 'module.bicep' = {
   name: 'myModuleInstance'
   params: {
       inputParam: externalInput('type', 'myInput')
   }
}
"""),
            ("module.bicep", """
param inputParam any
"""));

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void ExternalInput_reference_in_variable_calling_a_nested_declared_function_lambda_is_blocked()
    {
        var result = CompilationHelper.Compile("""
func foo(test string) any => externalInput('type', test)
func bar() any => foo('myInput')
var baz = bar()
""");

        result.Should().NotGenerateATemplate();
        result.Should().OnlyContainDiagnostic(
            "BCP445",
            DiagnosticLevel.Error,
            "Function \"externalInput\" is not valid at this location. It can only be used within a declared function body."
        );
    }

    [TestMethod]
    public void Declared_function_containing_external_input_referenced_in_bicep_file_is_blocked()
    {
        var result = CompilationHelper.Compile("""
func foo(test string) any => externalInput('type', test)
var result = foo('myInput')
""");

        result.Should().NotGenerateATemplate();
    }
}
