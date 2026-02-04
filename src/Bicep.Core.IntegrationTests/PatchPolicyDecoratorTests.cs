// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class PatchPolicyDecoratorTests
{
    public TestContext TestContext { get; set; } = null!;

    private ServiceBuilder GetServices(bool patchPolicyEnabled = true) =>
        new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, PatchPolicyEnabled: patchPolicyEnabled));

    [TestMethod]
    public void PatchPolicy_Decorator_Should_Compile_Successfully()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @patchPolicy({
              properties: {
                displayName: 'Updated Display Name'
              }
            })
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void PatchPolicy_Decorator_Should_Emit_Method_Property()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @patchPolicy({
              properties: {
                displayName: 'Updated Display Name'
              }
            })
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();

        var resource = result.Template!.SelectToken("$.resources[0]");
        resource.Should().NotBeNull();
        resource!["method"]?.ToString().Should().Be("PATCH");
    }

    [TestMethod]
    public void PatchPolicy_Decorator_Should_Emit_Properties_From_Decorator_Body()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @patchPolicy({
              identity: {
                type: 'UserAssigned'
              }
              properties: {
                displayName: 'Updated Display Name'
              }
            })
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();

        var resource = result.Template!.SelectToken("$.resources[0]");
        resource.Should().NotBeNull();

        // Check that the method is PATCH
        resource!["method"]?.ToString().Should().Be("PATCH");

        // Check that the identity from decorator body is emitted
        resource["identity"]?["type"]?.ToString().Should().Be("UserAssigned");

        // Check that the properties from decorator body are emitted
        resource["properties"]?["displayName"]?.ToString().Should().Be("Updated Display Name");
    }

    [TestMethod]
    public void PatchPolicy_Decorator_Should_Not_Appear_In_Options()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @patchPolicy({
              properties: {
                displayName: 'Updated Display Name'
              }
            })
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();

        var resource = result.Template!.SelectToken("$.resources[0]");
        resource.Should().NotBeNull();

        // patchPolicy should NOT appear in @options
        resource!["@options"]?.SelectToken("$.patchPolicy").Should().BeNull();
    }

    [TestMethod]
    public void PatchPolicy_Decorator_Without_Properties_Should_Produce_Error()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @patchPolicy()
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP071", DiagnosticLevel.Error, "Expected 1 argument, but got 0."),
        });
    }

    [TestMethod]
    public void PatchPolicy_Decorator_With_Invalid_Parameter_Type_Should_Produce_Error()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @patchPolicy('invalid')
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP070", DiagnosticLevel.Error, "Argument of type \"'invalid'\" is not assignable to parameter of type \"object\"."),
        });
    }

    [TestMethod]
    public void PatchPolicy_Decorator_Can_Be_Used_With_Other_Decorators()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @description('My storage account')
            @patchPolicy({
              properties: {
                displayName: 'Updated Display Name'
              }
            })
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void PatchPolicy_Decorator_On_Non_Resource_Should_Produce_Error()
    {
        var services = GetServices();
        var result = CompilationHelper.Compile(services, """
            @patchPolicy({
              value: 'test'
            })
            param myParam string
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP125", DiagnosticLevel.Error, "Function \"patchPolicy\" cannot be used as a parameter decorator."),
        });
    }

    [TestMethod]
    public void PatchPolicy_Decorator_Should_Not_Be_Available_When_Feature_Disabled()
    {
        var services = GetServices(patchPolicyEnabled: false);
        var result = CompilationHelper.Compile(services, """
            @patchPolicy({
              properties: {
                displayName: 'Updated Display Name'
              }
            })
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        // When feature is disabled, patchPolicy should not be recognized
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP057", DiagnosticLevel.Error, "The name \"patchPolicy\" does not exist in the current context."),
        });
    }
}
