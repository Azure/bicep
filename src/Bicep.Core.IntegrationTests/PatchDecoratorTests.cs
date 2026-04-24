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
public class PatchDecoratorTests
{
    public TestContext TestContext { get; set; } = null!;

    private ServiceBuilder GetServices(bool patchEnabled = true) =>
        new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, PatchEnabled: patchEnabled));

    [TestMethod]
    public void Patch_Decorator_Should_Compile_Successfully()
    {
        var result = CompilationHelper.Compile(
            GetServices(),
            """
            targetScope = 'subscription'

            @patch()
            resource policyDef 'Microsoft.Authorization/policyDefinitions@2021-06-01' = {
              name: 'test-policy'
              properties: {
                displayName: 'Test Policy'
                policyType: 'Custom'
                mode: 'All'
                policyRule: {}
              }
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Patch_Decorator_Should_Emit_Method_PATCH()
    {
        var result = CompilationHelper.Compile(
            GetServices(),
            """
            targetScope = 'subscription'

            @patch()
            resource policyDef 'Microsoft.Authorization/policyDefinitions@2021-06-01' = {
              name: 'test-policy'
              properties: {
                displayName: 'Test Policy'
                policyType: 'Custom'
                mode: 'All'
                policyRule: {}
              }
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
        var template = result.Template;
        template.Should().NotBeNull();

        var resources = template!.SelectToken("resources")!.ToArray();
        resources.Should().HaveCount(1);

        var policyResource = (JObject)resources[0];
        policyResource["method"]?.ToString().Should().Be("PATCH");
    }

    [TestMethod]
    public void Patch_Decorator_Should_Not_Be_Available_When_Feature_Disabled()
    {
        var result = CompilationHelper.Compile(
            GetServices(patchEnabled: false),
            """
            targetScope = 'subscription'

            @patch()
            resource policyDef 'Microsoft.Authorization/policyDefinitions@2021-06-01' = {
              name: 'test-policy'
              properties: {
                displayName: 'Test Policy'
                policyType: 'Custom'
                mode: 'All'
                policyRule: {}
              }
            }
            """);

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP057", DiagnosticLevel.Error, "The name \"patch\" does not exist in the current context."),
        });
    }

    [TestMethod]
    public void Resource_Without_Patch_Decorator_Should_Not_Have_Method()
    {
        var result = CompilationHelper.Compile(
            GetServices(),
            """
            targetScope = 'subscription'

            resource policyDef 'Microsoft.Authorization/policyDefinitions@2021-06-01' = {
              name: 'test-policy'
              properties: {
                displayName: 'Test Policy'
                policyType: 'Custom'
                mode: 'All'
                policyRule: {}
              }
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
        var template = result.Template;
        template.Should().NotBeNull();

        var resources = template!.SelectToken("resources")!.ToArray();
        resources.Should().HaveCount(1);

        var policyResource = (JObject)resources[0];
        // Verify method property is not present
        policyResource.ContainsKey("method").Should().BeFalse();
    }

    [TestMethod]
    public void Patch_Decorator_Should_Work_With_Various_Resource_Types()
    {
        var result = CompilationHelper.Compile(
            GetServices(),
            """
            @patch()
            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'mystorageaccount'
              location: 'eastus'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
              properties: {}
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
        var template = result.Template;
        template.Should().NotBeNull();

        var resources = template!.SelectToken("resources")!.ToArray();
        resources.Should().HaveCount(1);

        var storageResource = (JObject)resources[0];
        storageResource["method"]?.ToString().Should().Be("PATCH");
    }

    [TestMethod]
    public void Patch_Decorator_Should_Not_Accept_Arguments()
    {
        var result = CompilationHelper.Compile(
            GetServices(),
            """
            targetScope = 'subscription'

            @patch({})
            resource policyDef 'Microsoft.Authorization/policyDefinitions@2021-06-01' = {
              name: 'test-policy'
              properties: {
                displayName: 'Test Policy'
                policyType: 'Custom'
                mode: 'All'
                policyRule: {}
              }
            }
            """);

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP071", DiagnosticLevel.Error, "Expected 0 arguments, but got 1."),
        });
    }
}