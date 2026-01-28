// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ResourceMethodPropertyTests
{
    private static ServiceBuilder Services => new();

    [TestMethod]
    public void KnownTopLevelResourceProperties_Should_Include_Hidden_Method_Property()
    {
        var properties = AzResourceTypeProvider.KnownTopLevelResourceProperties()
            .ToDictionary(p => p.Name);

        properties.Should().ContainKey(LanguageConstants.ResourceMethodPropertyName);

        var methodProp = properties[LanguageConstants.ResourceMethodPropertyName];
        methodProp.Flags.Should().HaveFlag(TypePropertyFlags.Hidden, "method should be hidden from IntelliSense");
        methodProp.Flags.Should().HaveFlag(TypePropertyFlags.SystemProperty, "method should be a system property");
    }

    [TestMethod]
    public void Resource_With_Method_Patch_Should_Compile_Successfully()
    {
        var result = CompilationHelper.Compile(Services, """
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              method: 'PATCH'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Resource_With_Method_Put_Should_Compile_Successfully()
    {
        var result = CompilationHelper.Compile(Services, """
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              method: 'PUT'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Resource_With_Method_Patch_Should_Emit_To_Template()
    {
        var result = CompilationHelper.Compile(Services, """
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              method: 'PATCH'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();

        var template = result.Template!;
        var resources = template.SelectToken("$.resources");
        resources.Should().NotBeNull();

        var saResource = template.SelectToken("$.resources.sa") ?? template.SelectToken("$.resources[0]");
        saResource.Should().NotBeNull();
        saResource!["method"]?.ToString().Should().Be("PATCH");
    }

    [TestMethod]
    public void Resource_With_Method_Put_Should_Emit_To_Template()
    {
        var result = CompilationHelper.Compile(Services, """
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              method: 'PUT'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();

        var template = result.Template!;
        var saResource = template.SelectToken("$.resources.sa") ?? template.SelectToken("$.resources[0]");
        saResource.Should().NotBeNull();
        saResource!["method"]?.ToString().Should().Be("PUT");
    }

    [TestMethod]
    public void Resource_With_Invalid_Method_Should_Produce_Error()
    {
        var result = CompilationHelper.Compile(Services, """
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              method: 'DELETE'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP036", DiagnosticLevel.Error, "The property \"method\" expected a value of type \"'PATCH' | 'PUT'\" but the provided value is of type \"'DELETE'\"."),
        });
    }

    [TestMethod]
    public void Resource_With_Invalid_Method_Post_Should_Produce_Error()
    {
        var result = CompilationHelper.Compile(Services, """
            resource sa 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'teststorage'
              location: 'eastus'
              method: 'POST'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP088", DiagnosticLevel.Error, "The property \"method\" expected a value of type \"'PATCH' | 'PUT'\" but the provided value is of type \"'POST'\". Did you mean \"'PUT'\"?"),
        });
    }

    [TestMethod]
    public void Resource_Without_Method_Should_Compile_Successfully()
    {
        var result = CompilationHelper.Compile(Services, """
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
    public void Resource_Without_Method_Should_Not_Emit_Method_In_Template()
    {
        var result = CompilationHelper.Compile(Services, """
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

        var template = result.Template!;
        var saResource = template.SelectToken("$.resources.sa") ?? template.SelectToken("$.resources[0]");

        saResource.Should().NotBeNull();
        saResource!["method"].Should().BeNull();
    }
}
