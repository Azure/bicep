// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Emit;

[TestClass]
public class DependencyInferenceTests
{
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
    public void Extensibility_resources_always_generate_explicit_dependency()
    {
        var result = CompilationHelper.Compile(
            new UnitTests.ServiceBuilder().WithFeatureOverrides(new(ExtensibilityEnabled: true))
                .WithConfigurationPatch(c => c.WithExtensions("""
                    {
                      "az": "builtin:",
                      "kubernetes": "builtin:",
                      "microsoftGraph": "builtin:",
                      "foo": "builtin:",
                      "bar": "builtin:"
                    }
                    """))
                .WithNamespaceProvider(TestExtensibilityNamespaceProvider.CreateWithDefaults()),
            """
            extension bar with {
              connectionString: 'connectionString'
            }

            resource container 'container' existing = {
              name: 'containerName'
            }

            resource sa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: container.name
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveJsonAtPath("$.resources.sa.dependsOn", """["container"]""");
    }
}
