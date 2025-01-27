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
    public void Test_Issue2163_Deployments_Secure_Outputs_E2E()
    {
        // https://github.com/Azure/bicep/issues/2163
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext, SecureOutputsEnabled: true)),
            ("main.bicep", @"
                module foo 'foo.bicep' = {
                  name: 'foo'
                }
 
                output myOutput string = foo.outputs.secureOutput
            "),
            ("foo.bicep", @"
                @secure()
                output secureOutput string = '***secret***'
            ")
        );

        result.Diagnostics.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Test_Issue2163_Deployments_Secure_Outputs_Expect_Error_SecureOutputsNotEnabled()
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
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
            ("BCP129", DiagnosticLevel.Error, "Function \"secure\" cannot be used as an output decorator.")
        });
    }

    [TestMethod]
    public void Test_Issue2163_Deployments_Secure_Outputs_Decorator_Translates_To_SecureString_And_SecureObject()
    {
        // https://github.com/Azure/bicep/issues/2163
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext, SecureOutputsEnabled: true)),
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
    }

    [TestMethod]
    public void Test_Issue2163_Deployments_Secure_Outputs_Call_Correct_API()
    {
        // https://github.com/Azure/bicep/issues/2163
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext, SecureOutputsEnabled: true)),
            ("main.bicep", @"
                module foo 'foo.bicep' = {
                  name: 'foo'
                }

                resource key 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = {
                  name: 'secrets/mysecret'
                  properties: {
                    value: foo.outputs.secureOutput
                  }
                }

                module bar 'bar.bicep' = {
                  name: 'bar'
                }

                module baz 'baz.bicep' = {
                  name: 'baz'
                  params: {
                    secureInput: foo.outputs.secureOutput
                  }
                }

                output outputNormalVal string = foo.outputs.normalOutput
                output outputSecureVal string = foo.outputs.secureOutput
                output outputNormalVal2 string = bar.outputs.normalOutput
                output outputNormalVal3 string = baz.outputs.normalOutput
            "),
            ("foo.bicep", @"
                @secure()
                output secureOutput string = '***secret***'
                output normalOutput string = 'normal'
            "),
            ("bar.bicep", @"
                output normalOutput string = 'normal'
            "),
            ("baz.bicep", @"
                @secure()
                param secureInput string

                output normalOutput string = 'normal'"
            )
        );

        result.Diagnostics.Should().NotHaveAnyDiagnostics();

        // Verify referencing secure output in a resource property will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.resources[0].properties.value", "[listOutputsWithSecureValues(resourceId('Microsoft.Resources/deployments', 'foo'), '2022-09-01').secureOutput]");

        // Verify referencing secure output will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.outputs['outputSecureVal'].value", "[listOutputsWithSecureValues(resourceId('Microsoft.Resources/deployments', 'foo'), '2022-09-01').secureOutput]");

        // Verify referencing normal value from a deployment which contains secure outputs will be translated to listOutputsWithSecureValues function
        result.Template.Should().HaveValueAtPath("$.outputs['outputNormalVal'].value", "[listOutputsWithSecureValues(resourceId('Microsoft.Resources/deployments', 'foo'), '2022-09-01').normalOutput]");

        // Verify referencing normal value from a deployment which does NOT contain any secure outputs will be translated to normal reference function
        result.Template.Should().HaveValueAtPath("$.outputs['outputNormalVal2'].value", "[reference(resourceId('Microsoft.Resources/deployments', 'bar'), '2022-09-01').outputs.normalOutput.value]");

        // Verify referencing normal value from a deployment which does NOT contain any secure outputs but secure parameters will be translated to normal reference function
        result.Template.Should().HaveValueAtPath("$.outputs['outputNormalVal3'].value", "[reference(resourceId('Microsoft.Resources/deployments', 'baz'), '2022-09-01').outputs.normalOutput.value]");
    }
}
