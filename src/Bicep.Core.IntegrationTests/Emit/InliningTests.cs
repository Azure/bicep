// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Emit;

[TestClass]
public class InliningTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Existing_resource_runtime_names_should_be_inlined()
    {
        var result = CompilationHelper.Compile("""
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            output foo string = sa2.name
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.foo.value", "[reference(resourceId('Microsoft.Storage/storageAccounts', 'foo'), '2025-01-01').accessTier]");
    }

    [TestMethod]
    public void Existing_resource_declaration_should_not_be_emitted_when_name_is_runtime_expression()
    {

        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext, SymbolicNameCodegenEnabled: true)),
            """
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            output foo string = sa2.name
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().NotHaveValueAtPath("$.resources.sa2");
        result.Template.Should().HaveValueAtPath("$.outputs.foo.value", "[reference('sa').accessTier]");
    }

    [TestMethod]
    public void Variables_that_refer_to_existing_resource_runtime_names_should_be_inlined()
    {
        var result = CompilationHelper.Compile("""
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            var foo = sa2.name

            output foo string = foo
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().NotHaveValueAtPath("$.variables");
        result.Template.Should().HaveValueAtPath("$.outputs.foo.value", "[reference(resourceId('Microsoft.Storage/storageAccounts', 'foo'), '2025-01-01').accessTier]");
    }

    [TestMethod]
    public void SkipInline_decision_should_not_void_previous_inline_decision()
    {
        var result = CompilationHelper.Compile("""
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            var foo = '${sa.properties.accessTier}_${sa.name}'

            output foo string = foo
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().NotHaveValueAtPath("$.variables");
        result.Template.Should().HaveValueAtPath("$.outputs.foo.value", "[format('{0}_{1}', reference(resourceId('Microsoft.Storage/storageAccounts', 'foo'), '2025-01-01').accessTier, 'foo')]");
    }

    [TestMethod]
    public void Existing_resources_with_existing_resource_parents_with_runtime_names_should_be_inlined()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext, SymbolicNameCodegenEnabled: true, ResourceInfoCodegenEnabled: true)),
            """
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier

              resource blobs 'blobServices' existing = {
                name: 'default'

                resource container 'containers' existing = {
                  name: 'container'
                }
              }
            }

            output foo string = sa2::blobs::container.id
            output bar string = sa.id
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().NotHaveValueAtPath("$.resources.sa2");
        result.Template.Should().NotHaveValueAtPath("$.resources['sa2::blobs']");
        result.Template.Should().NotHaveValueAtPath("$.resources['sa2::blobs::container']");
        result.Template.Should().HaveValueAtPath("$.outputs.foo.value", "[resourceId('Microsoft.Storage/storageAccounts/blobServices/containers', reference('sa').accessTier, 'default', 'container')]");
        result.Template.Should().HaveValueAtPath("$.outputs.bar.value", "[resourceInfo('sa').id]");
    }

    [TestMethod]
    public void Existing_resources_with_existing_resource_scopes_with_runtime_names_should_be_inlined()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext, SymbolicNameCodegenEnabled: true, ResourceInfoCodegenEnabled: true)),
            """
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            resource saLock 'Microsoft.Authorization/locks@2020-05-01' existing = {
              name: 'saLock'
              scope: sa2
            }

            output foo string = saLock.id
            output bar string = sa.id
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().NotHaveValueAtPath("$.resources.sa2");
        result.Template.Should().NotHaveValueAtPath("$.resources.saLock");
        result.Template.Should().HaveValueAtPath("$.outputs.foo.value", "[extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', reference('sa').accessTier), 'Microsoft.Authorization/locks', 'saLock')]");
        result.Template.Should().HaveValueAtPath("$.outputs.bar.value", "[resourceInfo('sa').id]");
    }

    [TestMethod]
    public void Access_to_non_resourceInfo_properties_of_existing_resources_with_existing_resource_scopes_with_runtime_names_should_be_blocked()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext, SymbolicNameCodegenEnabled: true, ResourceInfoCodegenEnabled: true)),
            """
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            output foo string = sa2.properties.accessTier
            """);

        result.Template.Should().BeNull();
        result.Should().HaveDiagnostics(
        [
            ("BCP307", DiagnosticLevel.Error, "The expression cannot be evaluated, because the identifier properties of the referenced existing resource including \"name\" cannot be calculated at the start of the deployment. In this situation, the accessible properties of \"sa2\" include \"apiVersion\", \"id\", \"name\", \"type\"."),
        ]);
    }

    [TestMethod]
    public void should_block_scoping_deployed_resource_to_existing_resource_with_runtime_name()
    {
        var result = CompilationHelper.Compile("""
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            resource saLock 'Microsoft.Authorization/locks@2020-05-01' = {
              name: 'saLock'
              scope: sa2
              properties: {
                level: 'CanNotDelete'
              }
            }
            """);

        result.Template.Should().BeNull();
        result.Should().HaveDiagnostics(
        [
            ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"scope\" property of the \"Microsoft.Authorization/locks\" type, which requires a value that can be calculated at the start of the deployment. Properties of sa2 which can be calculated at the start include \"apiVersion\", \"type\"."),
        ]);
    }

    [TestMethod]
    public void should_block_child_deployed_resource_of_existing_resource_parent_with_runtime_name()
    {
        var result = CompilationHelper.Compile("""
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            resource blobs 'Microsoft.Storage/storageAccounts/blobServices@2025-01-01' = {
              name: 'default'
              parent: sa2
            }
            """);

        result.Template.Should().BeNull();
        result.Should().HaveDiagnostics(
        [
            ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"parent\" property of the \"Microsoft.Storage/storageAccounts/blobServices\" type, which requires a value that can be calculated at the start of the deployment. Properties of sa2 which can be calculated at the start include \"apiVersion\", \"type\"."),
        ]);
    }

    [TestMethod]
    public void Existing_resources_with_runtime_names_cannot_declare_explicit_dependencies()
    {
        var result = CompilationHelper.Compile("""
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource blobs 'Microsoft.Storage/storageAccounts/blobServices@2025-01-01' = {
              name: 'default'
              parent: sa
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
              dependsOn: [
                blobs
              ]
            }
            """);

        result.Template.Should().BeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(
        [
            ("BCP433", DiagnosticLevel.Error, "The resource \"sa2\" cannot declare explicit dependencies because its identifier properties including \"name\" cannot be calculated at the start of the deployment."),
        ]);
    }

    [TestMethod]
    public void Existing_resources_with_runtime_names_cannot_be_depended_on()
    {
        var result = CompilationHelper.Compile("""
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: 'foo'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource sa2 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: sa.properties.accessTier
            }

            resource blobs 'Microsoft.Storage/storageAccounts/blobServices@2025-01-01' = {
              name: 'default'
              parent: sa
              dependsOn: [
                sa2
              ]
            }
            """);

        result.Template.Should().BeNull();
        result.Should().HaveDiagnostics(
        [
            ("BCP434", DiagnosticLevel.Error, "The resource \"blobs\" cannot declare an explicit dependency on \"sa2\" because the identifier properties of the latter including \"name\" cannot be calculated at the start of the deployment."),
        ]);
    }
}
