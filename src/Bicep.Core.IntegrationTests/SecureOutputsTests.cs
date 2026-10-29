// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;


[TestClass]
public class SecureOutputsTests
{

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Test_Deployments_Secure_Outputs_Expect_No_Error()
    {
        // https://github.com/Azure/bicep/issues/2163
        var result = CompilationHelper.Compile(
            ("main.bicep", @"
                @secure()
                param myInput string
 
                module foo 'foo.bicep' = {
                  name: 'foo'
                  params: {
                    myInput : myInput
                  }
                }
                @secure()
                output myOutput string = foo.outputs.myOutput
            "),
            ("foo.bicep", @"
                @secure()
                param myInput string
 
                @secure()
                output myOutput string = myInput
            ")
        );
        result.Diagnostics.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Test_Deployments_Secure_Outputs_Expect_Error()
    {
        // Test implicit secure value.
        var result = CompilationHelper.Compile(
            ("main.bicep", @"
                @secure()
                param myInput string

                output myOutput string = myInput
            ")
        );
        result.Diagnostics.Should().ContainDiagnostic("outputs-should-not-contain-secrets", DiagnosticLevel.Warning, "Outputs should not contain secrets. Found possible secret: secure value 'myInput'");

        // Test referencing sub-module secure output value.
        result = CompilationHelper.Compile(
            ("main.bicep", @"
                @secure()
                param myInput string
 
                module foo 'foo.bicep' = {
                  name: 'foo'
                  params: {
                    myInput : myInput
                  }
                }

                output myOutput string = foo.outputs.myOutput
            "),
            ("foo.bicep", @"
                @secure()
                param myInput string

                @secure()
                output myOutput string = myInput
            ")
        );
        result.Diagnostics.Should().ContainDiagnostic("outputs-should-not-contain-secrets", DiagnosticLevel.Warning, "Outputs should not contain secrets. Found possible secret: secure value 'foo.outputs.myOutput'");
    }

    [TestMethod]
    public void Test_Deployments_Secure_Outputs_Decorator_Translates_To_SecureString_And_SecureObject()
    {
        // https://github.com/Azure/bicep/issues/2163
        var result = CompilationHelper.Compile(
            ("main.bicep", @"
                @secure()
                output secureStringOutput string = 'intern project 2024'

                @secure()
                output secureObjectOutput object = {
                  key1: 'value1'
                  key2: 'value2'
                }
            ")
        );

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs['secureStringOutput'].type", "securestring");
        result.Template.Should().HaveValueAtPath("$.outputs['secureObjectOutput'].type", "secureObject");

        result = CompilationHelper.Compile(
            ("main.bicep", @"
                output secureObjectOutput {
                      @secure()
                      foo: string
                      bar: string
                    } = {
                      foo: '***'
                      bar: 'normal'
                    }
            ")
        );

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs['secureObjectOutput'].properties.foo.type", "securestring");
        result.Template.Should().HaveValueAtPath("$.outputs['secureObjectOutput'].properties.bar.type", "string");
        result.Template.Should().HaveValueAtPath("$.outputs['secureObjectOutput'].type", "object");
    }

    [TestMethod]
    public void Test_Deployments_Secure_Outputs_Call_Correct_API()
    {
        // https://github.com/Azure/bicep/issues/2163
        var result = CompilationHelper.Compile(
            ("main.bicep", @"
                module secureOuputs 'secureOuputs.bicep' = {
                  name: 'secureOuputs'
                }

                resource key 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = {
                  name: 'secrets/mysecret'
                  properties: {
                    value: secureOuputs.outputs.secureOutput
                  }
                }

               resource key2 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = {
                  name: 'secrets/mysecret2'
                  properties: {
                    value: implicitSecureOuputWithoutDecorator.outputs.secureObjectOutput.foo
                  }
                }

                module noSecureOutput 'noSecureOutput.bicep' = {
                  name: 'noSecureOutput'
                }

                module noSecureOutputWithSecureParam 'noSecureOutputWithSecureParam.bicep' = {
                  name: 'noSecureOutputWithSecureParam'
                  params: {
                    secureInput: secureOuputs.outputs.secureOutput
                  }
                }

               module implicitSecureOuputWithoutDecorator 'implicitSecureOuputWithoutDecorator.bicep' = {
                  name: 'implicitSecureOuputWithoutDecorator'
                }

                @secure()
                output outputSecureVal string = secureOuputs.outputs.secureOutput
                @secure()
                output outputImplicitSecureObject_secureString string = implicitSecureOuputWithoutDecorator.outputs.secureObjectOutput.foo
                @secure()
                output outputImplicitSecureObject_normalString string = implicitSecureOuputWithoutDecorator.outputs.secureObjectOutput.bar
                output outputNormalVal string = secureOuputs.outputs.normalOutput
                output outputNormalVal2 string = noSecureOutput.outputs.normalOutput
                output outputNormalVal3 string = noSecureOutputWithSecureParam.outputs.normalOutput
            "),
            ("secureOuputs.bicep", @"
                @secure()
                output secureOutput string = '***secret***'
                output normalOutput string = 'normal'
            "),
            ("noSecureOutput.bicep", @"
                output normalOutput string = 'normal'
            "),
            ("noSecureOutputWithSecureParam.bicep", @"
                @secure()
                param secureInput string

                output normalOutput string = 'normal'"
            ),
            ("implicitSecureOuputWithoutDecorator.bicep", @"
                output secureObjectOutput {
                      @secure()
                      foo: string
                      bar: string
                    } = {
                      foo: '***'
                      bar: 'normal'
                    }
            ")
        );

        result.Diagnostics.Should().NotHaveAnyDiagnostics();

        // Verify referencing secure output in a resource property will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.resources['key'].properties.value", "[listOutputsWithSecureValues('secureOuputs', '2025-04-01').secureOutput]");

        // Verify referencing implicit secure output in a resource property will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.resources['key2'].properties.value", "[listOutputsWithSecureValues('implicitSecureOuputWithoutDecorator', '2025-04-01').secureObjectOutput.foo]");

        // Verify referencing secure output will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.outputs['outputSecureVal'].value", "[listOutputsWithSecureValues('secureOuputs', '2025-04-01').secureOutput]");

        // Verify referencing normal value from a deployment which contains secure outputs will be translated to reference function
        result.Template.Should().HaveValueAtPath("$.outputs['outputNormalVal'].value", "[reference('secureOuputs').outputs.normalOutput.value]");

        // Verify referencing normal value from a deployment which does NOT contain any secure outputs will be translated to normal reference function
        result.Template.Should().HaveValueAtPath("$.outputs['outputNormalVal2'].value", "[reference('noSecureOutput').outputs.normalOutput.value]");

        // Verify referencing normal value from a deployment which does NOT contain any secure outputs but secure parameters will be translated to normal reference function
        result.Template.Should().HaveValueAtPath("$.outputs['outputNormalVal3'].value", "[reference('noSecureOutputWithSecureParam').outputs.normalOutput.value]");

        // Verify referencing secure output in a resource property will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.outputs['outputSecureVal'].value", "[listOutputsWithSecureValues('secureOuputs', '2025-04-01').secureOutput]");

        // Verify referencing a normal string in an implicit secure output object will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.outputs['outputImplicitSecureObject_normalString'].value", "[listOutputsWithSecureValues('implicitSecureOuputWithoutDecorator', '2025-04-01').secureObjectOutput.bar]");

        // Verify referencing a secure string in an implicit secure output object will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.outputs['outputImplicitSecureObject_secureString'].value", "[listOutputsWithSecureValues('implicitSecureOuputWithoutDecorator', '2025-04-01').secureObjectOutput.foo]");

    }

    [TestMethod]
    public void Test_Deployments_Secure_Outputs_Using_Language_Version_2_0()
    {
        // secure parameter does NOT use language version 2.0
        var result = CompilationHelper.Compile(
            ("main.bicep", @"
                @secure()
                param secureInput string
            ")
        );
        result.Template?.Root.ToString().Should().NotContain("\"languageVersion\": \"2.0\"");

        // Generated template uses language version 2.0 when secure output is declared.
        result = CompilationHelper.Compile(
            ("main.bicep", @"
                @secure()
                param myInput string
 
                @secure()
                output myOutput string = myInput
            ")
        );
        result.Template?.Root.ToString().Should().Contain("\"languageVersion\": \"2.0\"");

        // Generated template uses language version 2.0 when secure output is declared in modules.
        result = CompilationHelper.Compile(
            ("main.bicep", @"
                module foo 'foo.bicep' = {
                  name: 'foo'
                  params: {
                    myInput : 'password'
                  }
                }

                module bar 'bar.bicep' = {
                  name: 'bar'
                  params: {
                    secureInput : foo.outputs.myOutput
                  }
                }
            "),
            ("foo.bicep", @"
                @secure()
                param myInput string
 
                @secure()
                output myOutput string = myInput
            "),
            ("bar.bicep", @"
                @secure()
                param secureInput string
            ")
        );
        result.Template?.Root.ToString().Should().Contain("\"languageVersion\": \"2.0\"");
    }

    [TestMethod]
    public void Non_sensitive_outputs_of_module_with_secure_outputs_should_use_reference_function()
    {
        var result = CompilationHelper.Compile(
            ("mod.bicep", """
                @secure()
                output sensitive string = 'foo'

                output notSensitive string = 'bar'
                """),
            ("main.bicep", """
                module mod 'mod.bicep' = {}

                @secure()
                output sensitive string = mod.outputs.sensitive
                output notSensitive string = mod.outputs.notSensitive
                """));

        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.sensitive.value", "[listOutputsWithSecureValues('mod', '2025-04-01').sensitive]");
        result.Template.Should().HaveValueAtPath("$.outputs.notSensitive.value", "[reference('mod').outputs.notSensitive.value]");
    }

    [TestMethod]
    public void Non_sensitive_outputs_of_collection_module_with_secure_outputs_should_use_reference_function()
    {
        var result = CompilationHelper.Compile(
            ("mod.bicep", """
                @secure()
                output sensitive string = 'foo'

                output notSensitive string = 'bar'
                """),
            ("main.bicep", """
                module mod 'mod.bicep' = [for i in range(0, 1): {}]

                @secure()
                output sensitive string = mod[0].outputs.sensitive
                output notSensitive string = mod[0].outputs.notSensitive
                """));

        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.sensitive.value", "[listOutputsWithSecureValues(format('mod[{0}]', 0), '2025-04-01').sensitive]");
        result.Template.Should().HaveValueAtPath("$.outputs.notSensitive.value", "[reference(format('mod[{0}]', 0)).outputs.notSensitive.value]");
    }

    [TestMethod]
    public void Secure_output_access_should_be_blocked_on_reference_via_inlined_variable()
    {
        var result = CompilationHelper.Compile(
            ("mod.bicep", """
                @secure()
                output sensitive string = 'foo'

                output notSensitive string = 'bar'
                """),
            ("main.bicep", """
                param condition bool

                module mod 'mod.bicep' = {}
                module mod2 'mod.bicep' = {}

                var modToUse = condition ? mod : mod2

                @secure()
                output sensitive string = modToUse.outputs.sensitive
                output notSensitive string = modToUse.outputs.notSensitive
                """));

        result.Should().HaveDiagnostics(
        [
            ("BCP426", DiagnosticLevel.Error, "Secure outputs may only be accessed via a direct module reference. Only non-sensitive outputs are supported when dereferencing a module indirectly via a variable or lambda."),
        ]);
    }

    [TestMethod]
    public void Secure_output_access_should_be_blocked_on_reference_via_lambda_variable()
    {
        var result = CompilationHelper.Compile(
            ("mod.bicep", """
                @secure()
                output sensitive string = 'foo'

                output notSensitive string = 'bar'
                """),
            ("main.bicep", """
                module mod 'mod.bicep' = [for i in range(0, 1): {}]

                output out1 array = map(mod, m => m.outputs.sensitive)
                output out2 array = map(mod, m => m.outputs.notSensitive)
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(
        [
            ("BCP426", DiagnosticLevel.Error, "Secure outputs may only be accessed via a direct module reference. Only non-sensitive outputs are supported when dereferencing a module indirectly via a variable or lambda."),
        ]);
    }

    // A compiled module (from a registry, template spec, or local JSON file) records a
    // resource-derived output as its ARM type plus a `__bicep_resource_derived_type!` source.
    // The output stays secure when the ARM type is, matching the same module compiled from source.
    private static string CompiledModuleWithResourceDerivedOutput(string armType, string source, string value) => $$"""
        {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "languageVersion": "2.0",
          "contentVersion": "1.0.0.0",
          "resources": {},
          "outputs": {
            "out": {
              "type": "{{armType}}",
              "value": {{value}},
              "metadata": { "__bicep_resource_derived_type!": { "source": "{{source}}" } }
            }
          }
        }
        """;

    private const string VmAdminUsernameSource = "Microsoft.Compute/virtualMachines@2024-07-01#properties/properties/properties/osProfile/properties/adminUsername";
    private const string VmHardwareProfileSource = "Microsoft.Compute/virtualMachines@2024-07-01#properties/properties/properties/hardwareProfile";

    [TestMethod]
    [DataRow("securestring", "string", VmAdminUsernameSource, "\"foo\"", "[listOutputsWithSecureValues('mod', '2025-04-01').out]")]
    [DataRow("string", "string", VmAdminUsernameSource, "\"foo\"", "[reference('mod').outputs.out.value]")]
    [DataRow("secureObject", "object", VmHardwareProfileSource, "{}", "[listOutputsWithSecureValues('mod', '2025-04-01').out]")]
    [DataRow("object", "object", VmHardwareProfileSource, "{}", "[reference('mod').outputs.out.value]")]
    public void Resource_derived_output_of_compiled_module_should_keep_secure_flag(string armType, string bicepType, string source, string value, string expected)
    {
        var result = CompilationHelper.Compile(
            ("mod.json", CompiledModuleWithResourceDerivedOutput(armType, source, value)),
            ("main.bicep", $$"""
                module mod 'mod.json' = {}

                @secure()
                output out {{bicepType}} = mod.outputs.out
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs.out.value", expected);
    }

    [TestMethod]
    public void Secure_resource_derived_output_of_compiled_module_should_be_blocked_on_reference_via_lambda_variable()
    {
        var result = CompilationHelper.Compile(
            ("mod.json", CompiledModuleWithResourceDerivedOutput("securestring", VmAdminUsernameSource, "\"foo\"")),
            ("main.bicep", """
                module mod 'mod.json' = [for i in range(0, 1): {}]

                output out array = map(mod, m => m.outputs.out)
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(
        [
            ("BCP426", DiagnosticLevel.Error, "Secure outputs may only be accessed via a direct module reference. Only non-sensitive outputs are supported when dereferencing a module indirectly via a variable or lambda."),
        ]);
    }

    [TestMethod]
    public void Non_sensitive_resource_derived_output_of_compiled_module_should_be_allowed_on_reference_via_lambda_variable()
    {
        var result = CompilationHelper.Compile(
            ("mod.json", CompiledModuleWithResourceDerivedOutput("string", VmAdminUsernameSource, "\"foo\"")),
            ("main.bicep", """
                module mod 'mod.json' = [for i in range(0, 1): {}]

                output out array = map(mod, m => m.outputs.out)
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs.out.value", "[map(references('mod'), lambda('m', lambdaVariables('m').outputs.out.value))]");
    }

    [TestMethod]
    public void Secure_resource_derived_output_of_compiled_module_should_force_symbolic_names()
    {
        // A named module and a non-output sink leave nothing else to force symbolic names.
        var result = CompilationHelper.Compile(
            ("mod.json", CompiledModuleWithResourceDerivedOutput("securestring", VmAdminUsernameSource, "\"foo\"")),
            ("main.bicep", """
                module mod 'mod.json' = {
                  name: 'mod'
                }

                resource secret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
                  name: 'kv/s'
                  properties: {
                    value: mod.outputs.out
                  }
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.languageVersion", "2.0");
        result.Template.Should().HaveValueAtPath("$.resources.secret.properties.value", "[listOutputsWithSecureValues('mod', '2025-04-01').out]");
    }
}
