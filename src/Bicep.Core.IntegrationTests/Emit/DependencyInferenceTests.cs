// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Emit;

[TestClass]
public class DependencyInferenceTests
{
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
              name: '${replace(deployedSa.name, '1', '2')}${i}'
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
              name: '${replace(deployedSa.name, '1', '2')}${i}'
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

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Non_looped_resource_depending_on_looped_existing_resource_should_depend_on_transitive_resource_collections(bool useSymbolicNameCodegen)
    {
        var result = CompilationHelper.Compile(
            new UnitTests.ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: useSymbolicNameCodegen)),
            """
            resource vnets 'Microsoft.Network/virtualNetworks@2024-03-01' = [for i in range(0, 10): {
              name: 'vnet${i}'
            }]

            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-03-01' existing = [for i in range(0, 10): {
              parent: vnets[i]
              name: 'subnet'
            }]

            resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = {
              name: 'vault'
              location: resourceGroup().location
              properties: {
                sku: {
                  name: 'standard'
                  family: 'A'
                }
                tenantId: subscription().tenantId
                networkAcls: {
                  virtualNetworkRules: [for i in range(0, 10): {
                    id: subnets[i].id
                  }]
                }
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        if (useSymbolicNameCodegen)
        {
            result.Template.Should().HaveJsonAtPath("$.resources.vault.dependsOn", """["vnets"]""");
        }
        else
        {
            result.Template.Should().HaveJsonAtPath("$.resources[?(@.name=='vault')].dependsOn", """["vnets"]""");
        }
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Looped_resource_depending_on_looped_existing_resource_should_depend_on_transitive_resource_element(bool useSymbolicNameCodegen)
    {
        var result = CompilationHelper.Compile(
            new UnitTests.ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: useSymbolicNameCodegen)),
            """
            resource vnets 'Microsoft.Network/virtualNetworks@2024-03-01' = [for i in range(0, 2): {
              name: 'vnet${i}'
            }]

            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-03-01' existing = [for j in range(0, 10): {
              parent: vnets[j % 2]
              name: 'subnet'
            }]

            resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = [for k in range(10, 10): {
              name: 'vault${k}'
              location: resourceGroup().location
              properties: {
                sku: {
                  name: 'standard'
                  family: 'A'
                }
                tenantId: subscription().tenantId
                networkAcls: {
                  virtualNetworkRules: [{
                    id: subnets[k - 10].id
                  }]
                }
              }
            }]
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        if (useSymbolicNameCodegen)
        {
            result.Template.Should().HaveJsonAtPath(
                "$.resources.vault.dependsOn",
                """["[format('vnets[{0}]', mod(range(0, 10)[sub(range(10, 10)[copyIndex()], 10)], 2))]"]""");
        }
        else
        {
            result.Template.Should().HaveJsonAtPath(
                "$.resources[?(@.type=='Microsoft.KeyVault/vaults')].dependsOn",
                """["[resourceId('Microsoft.Network/virtualNetworks', format('vnet{0}', range(0, 2)[mod(range(0, 10)[sub(range(10, 10)[copyIndex()], 10)], 2)]))]"]""");
        }
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Looped_resource_depending_on_looped_variable_should_depend_on_transitive_resource_element(bool useSymbolicNameCodegen)
    {
        var result = CompilationHelper.Compile(
            new UnitTests.ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: useSymbolicNameCodegen)),
            """
            resource vnets 'Microsoft.Network/virtualNetworks@2024-03-01' = [for i in range(0, 2): {
              name: 'vnet${i}'
            }]

            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-03-01' existing = [for j in range(0, 10): {
              parent: vnets[j % 2]
              name: 'subnet'
            }]

            resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = [for k in range(10, 10): {
              name: 'vault${k}'
              location: resourceGroup().location
              properties: {
                sku: {
                  name: 'standard'
                  family: 'A'
                }
                tenantId: subscription().tenantId
                networkAcls: {
                  virtualNetworkRules: [{
                    id: subnetIds[k - 10]
                  }]
                }
              }
            }]
            
            var subnetIds = [for l in range(20, 10): subnets[l - 20].id]
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        if (useSymbolicNameCodegen)
        {
            result.Template.Should().HaveJsonAtPath(
                "$.resources.vault.dependsOn",
                """["[format('vnets[{0}]', mod(range(0, 10)[sub(range(20, 10)[sub(range(10, 10)[copyIndex()], 10)], 20)], 2))]"]""");
        }
        else
        {
            result.Template.Should().HaveJsonAtPath(
                "$.resources[?(@.type=='Microsoft.KeyVault/vaults')].dependsOn",
                """["[resourceId('Microsoft.Network/virtualNetworks', format('vnet{0}', range(0, 2)[mod(range(0, 10)[sub(range(20, 10)[sub(range(10, 10)[copyIndex()], 10)], 20)], 2)]))]"]""");
        }
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void CopyIndex_only_appears_in_compiled_expression_if_all_links_in_chain_use_a_loop_variable_reference(bool useSymbolicNameCodegen)
    {
        var result = CompilationHelper.Compile(
            new UnitTests.ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: useSymbolicNameCodegen)),
            """
            resource vnets 'Microsoft.Network/virtualNetworks@2024-03-01' = [for i in range(0, 2): {
              name: 'vnet${i}'
            }]

            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-03-01' existing = [for j in range(0, 10): {
              parent: vnets[0]
              name: 'subnet${j}'
            }]

            resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = [for k in range(10, 10): {
              name: 'vault${k}'
              location: resourceGroup().location
              properties: {
                sku: {
                  name: 'standard'
                  family: 'A'
                }
                tenantId: subscription().tenantId
                networkAcls: {
                  virtualNetworkRules: [{
                    id: subnets[k - 10].id
                  }]
                }
              }
            }]
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        if (useSymbolicNameCodegen)
        {
            result.Template.Should().HaveJsonAtPath("$.resources.vault.dependsOn", """["[format('vnets[{0}]', 0)]"]""");
        }
        else
        {
            result.Template.Should().HaveJsonAtPath(
                "$.resources[?(@.type=='Microsoft.KeyVault/vaults')].dependsOn",
                """["[resourceId('Microsoft.Network/virtualNetworks', format('vnet{0}', range(0, 2)[0]))]"]""");
        }
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void CopyIndex_only_appears_in_compiled_expression_if_all_links_in_chain_use_a_loop_variable_reference_2(bool useSymbolicNameCodegen)
    {
        var result = CompilationHelper.Compile(
            new UnitTests.ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: useSymbolicNameCodegen)),
            """
            resource vnets 'Microsoft.Network/virtualNetworks@2024-03-01' = [for i in range(0, 2): {
              name: 'vnet${i}'
            }]

            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-03-01' existing = [for j in range(0, 10): {
              parent: vnets[j % 2]
              name: 'subnet${j}'
            }]

            resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = [for k in range(10, 10): {
              name: 'vault${k}'
              location: resourceGroup().location
              properties: {
                sku: {
                  name: 'standard'
                  family: 'A'
                }
                tenantId: subscription().tenantId
                networkAcls: {
                  virtualNetworkRules: [{
                    id: subnets[0].id
                  }]
                }
              }
            }]
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        if (useSymbolicNameCodegen)
        {
            result.Template.Should().HaveJsonAtPath("$.resources.vault.dependsOn", """["[format('vnets[{0}]', mod(range(0, 10)[0], 2))]"]""");
        }
        else
        {
            result.Template.Should().HaveJsonAtPath(
                "$.resources[?(@.type=='Microsoft.KeyVault/vaults')].dependsOn",
                """["[resourceId('Microsoft.Network/virtualNetworks', format('vnet{0}', range(0, 2)[mod(range(0, 10)[0], 2)]))]"]""");
        }
    }

    [TestMethod]
    public void Extension_resources_always_generate_explicit_dependency()
    {
        var result = CompilationHelper.Compile(
            new UnitTests.ServiceBuilder()
                .WithConfigurationPatch(c => c.WithExtensions("""
                    {
                      "az": "builtin:",
                      "kubernetes": "builtin:",
                      "microsoftGraph": "builtin:",
                      "foo": "builtin:",
                      "bar": "builtin:"
                    }
                    """))
                .WithNamespaceProvider(TestExtensionsNamespaceProvider.CreateWithDefaults()),
            """
            extension bar with {
              connectionString: 'connectionString'
            }

            resource container 'container' existing = {
              name: 'containerName'
            }

            resource tags 'Microsoft.Resources/tags@2025-04-01' = {
              name: 'default'
              properties: {
                tags: {
                  tag: toLower(container.name)
                }
              }
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.tags.dependsOn", """["container"]""");
    }

    [TestMethod]
    public void Using_an_existing_resource_as_a_scope_does_not_generate_an_explicit_dependency()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(SymbolicNameCodegenEnabled: true)),
            ("main.bicep", """
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2024-07-01' existing = {
                  name: 'rg'
                }

                module empty 'empty.bicep' = {
                  scope: rg
                  name: 'empty'
                }
                """),
            ("empty.bicep", string.Empty));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotHaveValueAtPath("$.resources.empty.dependsOn");
    }
}
