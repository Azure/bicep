// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ExistingResourceTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Implicit_dependencies_on_existing_resources_are_reflected_in_symbolic_name_template()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            """
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: replace(deployedSa.name, '1', '2')
            }

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                allowSharedKeyAccess: existingSa.properties.allowSharedKeyAccess
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.newSa.dependsOn", """["existingSa"]""");
        result.Template.Should().HaveJsonAtPath("$.resources.existingSa.dependsOn", """["deployedSa"]""");
    }

    [TestMethod]
    public void Explicit_dependencies_on_existing_resources_are_reflected_in_symbolic_name_template()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            """
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: replace(deployedSa.name, '1', '2')
            }

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              dependsOn: [
                existingSa
              ]
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.newSa.dependsOn", """["existingSa"]""");
        result.Template.Should().HaveJsonAtPath("$.resources.existingSa.dependsOn", """["deployedSa"]""");
    }

    [TestMethod]
    public void Implicit_dependencies_on_existing_resources_are_expressed_as_direct_dependencies_on_transitive_dependencies_in_non_symbolic_name_template()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: false)),
            """
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: replace(deployedSa.name, '1', '2')
            }

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                allowSharedKeyAccess: existingSa.properties.allowSharedKeyAccess
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.GetProperty("resources").Should().BeOfType<JArray>()
            .Subject.Count.Should().Be(2);
        result.Template.Should().HaveJsonAtPath("$.resources[1].dependsOn", """["[resourceId('Microsoft.Storage/storageAccounts', 'account1')]"]""");
    }

    [TestMethod]
    public void Explicit_dependencies_on_existing_resources_are_expressed_as_direct_dependencies_on_transitive_dependencies_in_non_symbolic_name_template()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: false)),
            """
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: replace(deployedSa.name, '1', '2')
            }

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              dependsOn: [
                existingSa
              ]
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.GetProperty("resources").Should().BeOfType<JArray>()
            .Subject.Count.Should().Be(2);
        result.Template.Should().HaveJsonAtPath("$.resources[1].dependsOn", """["[resourceId('Microsoft.Storage/storageAccounts', 'account1')]"]""");
    }

    [TestMethod]
    public void Implicit_dependencies_on_deployed_resource_identifying_properties_are_expressed_in_symbolic_name_template()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            """
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            
            resource secondDeployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: replace(deployedSa.name, '1', '2')
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: replace(secondDeployedSa.name, '2', '3')
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.newSa.dependsOn", """["secondDeployedSa"]""");
        result.Template.Should().HaveJsonAtPath("$.resources.secondDeployedSa.dependsOn", """["deployedSa"]""");
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Implicit_dependencies_on_existing_resource_identifying_properties_are_expressed_as_direct_dependencies_on_transitive_dependencies_in_symbolic_name_template(bool useArrayAccess)
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            $$"""
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            
            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: replace(deployedSa.name, '1', '2')
            }

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                allowSharedKeyAccess: bool(existingSa{{(useArrayAccess ? "['name']" : ".name")}})
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.newSa.dependsOn", """["deployedSa"]""");
        result.Template.Should().HaveJsonAtPath("$.resources.existingSa.dependsOn", """["deployedSa"]""");
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Implicit_dependencies_on_existing_resource_collection_identifying_properties_are_expressed_as_direct_dependencies_on_transitive_dependencies_in_symbolic_name_template(bool useArrayAccess)
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            $$"""
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            
            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = [for i in range(0, 1): {
              name: '${replace(deployedSa.name, '1', '2')}_${i}'
            }]

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                allowSharedKeyAccess: bool(existingSa[0]{{(useArrayAccess ? "['name']" : ".name")}})
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.newSa.dependsOn", """["deployedSa"]""");
        result.Template.Should().HaveJsonAtPath("$.resources.existingSa.dependsOn", """["deployedSa"]""");
    }

    [TestMethod]
    public void Existing_resource_function_calls_are_expressed_as_direct_dependencies_on_transitive_dependencies_in_symbolic_name_template()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            """
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            
            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: replace(deployedSa.name, '1', '2')
            }

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                allowSharedKeyAccess: !empty(existingSa.listKeys().keys)
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.newSa.dependsOn", """["deployedSa"]""");
        result.Template.Should().HaveJsonAtPath("$.resources.existingSa.dependsOn", """["deployedSa"]""");
    }

    [TestMethod]
    public void Existing_resource_collection_function_calls_are_expressed_as_direct_dependencies_on_transitive_dependencies_in_symbolic_name_template()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            """
            resource deployedSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account1'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            
            resource existingSa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = [for i in range(0, 1): {
              name: '${replace(deployedSa.name, '1', '2')}_${i}'
            }]

            resource newSa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'account3'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                allowSharedKeyAccess: !empty(existingSa[0].listKeys().keys)
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.newSa.dependsOn", """["deployedSa"]""");
        result.Template.Should().HaveJsonAtPath("$.resources.existingSa.dependsOn", """["deployedSa"]""");
    }

    [TestMethod]
    public void Using_an_existing_resource_as_an_explicit_parent_does_not_generate_an_explicit_dependency()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            """
            resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-08-01' existing = {
              name: 'vnet'
            }

            resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-08-01' existing = {
              parent: virtualNetwork
              name: 'subnet'
            }

            resource sa 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'storage'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                networkAcls: {
                  defaultAction: 'Deny'
                  virtualNetworkRules: [
                    {
                      action: 'Allow'
                      id: subnet.id
                    }
                  ]
                }
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotHaveValueAtPath("$.resources.sa.dependsOn");
    }
}
