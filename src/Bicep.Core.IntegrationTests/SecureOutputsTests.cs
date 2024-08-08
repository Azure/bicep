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

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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
                @secure()
                param foo string
 
                module mod 'module.bicep' = {
                  name: 'mod'
                  params: {
                    foo: foo
                  }
                }
 
                output bar string = mod.outputs.bar
                "),
            ("module.bicep", @"
                @secure()
                param foo string
 
                @secure()
                output bar string = foo
            ")
        );

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.outputs['bar'].value", "[listOutputsWithSecureValues(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').bar]");
    }
}
