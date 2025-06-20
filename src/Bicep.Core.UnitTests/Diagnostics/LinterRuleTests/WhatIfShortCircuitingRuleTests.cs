// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class WhatIfShortCircuitingRuleTests : LinterRuleTestsBase
    {
        private static readonly ServiceBuilder Services = new ServiceBuilder()
            .WithRegistration(x => x.AddSingleton(
                IConfigurationManager.WithStaticConfiguration(
                    IConfigurationManager.GetBuiltInConfiguration()
                    .WithAllAnalyzers())));

        private readonly string SAModuleContent = """
            param test string

            resource storageAccountModule 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                name: test
                location: 'eastus'
                sku: {
                    name: 'Standard_LRS'
                }
                kind: 'StorageV2'
                properties: {
                    accessTier: 'Hot'
                }
            }
            """;

        [TestMethod]
        public void WhatIfShortCircuiting_Condition()
        {
            var result = CompilationHelper.Compile(Services,
                [
                    ("mod.bicep", """
                        param condition bool
                        param a string

                        resource sa 'Microsoft.Storage/storageAccounts@2023-05-01' = if (condition) {
                            name: a
                            location: 'eastus'
                            sku: {
                                name: 'Standard_LRS'
                            }
                            kind: 'StorageV2'
                        }
                     """),
                    ("main.bicep", """
                        resource sa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
                            name: 'storageAccount'
                        }
                        module mod 'mod.bicep' = {
                            name: 'mod'
                            params: {
                                condition: sa.properties.allowBlobPublicAccess
                                a: 'abc'
                            }
                        }
                     """)]);

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'condition' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If.");
        }

        [TestMethod]
        public void WhatIfShortCircuiting_NoDiagnostics()
        {
            var result = CompilationHelper.Compile(Services,
                [
                    ("createSA.bicep", SAModuleContent),
                    ("main.bicep", """
                        param input string
                        module creatingSA 'createSA.bicep' = {
                          name: 'creatingSA'
                          params: {
                            test: input
                          }
                        }
                        module creatingSA2 'createSA.bicep' = {
                          name: 'creatingSA2'
                          params: {
                            test: 'value'
                          }
                        }
                    """)]);

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void WhatIfShortCircuiting_Name()
        {
            var result = CompilationHelper.Compile(Services,
                [
                    ("module.bicep", SAModuleContent),
                    ("main.bicep", """
                        resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                            name: 'storageAccountName'
                            location: 'eastus'
                            sku: {
                                name: 'Standard_LRS'
                            }
                            kind: 'StorageV2'
                            properties: {
                                accessTier: 'Hot'
                            }
                        }

                        module mod 'module.bicep' = {
                          name: 'mod'
                          params: {
                            test: storageAccount.properties.dnsEndpointType
                          }
                        }
                    """)]);

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'test' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If.");
        }

        [TestMethod]
        public void WhatIfShortCircuitingNested_Name()
        {
            var result = CompilationHelper.Compile(Services,
                [
                    ("createSA.bicep", SAModuleContent),
                    ("createModule.bicep", """
                        param nameParam string
                        output nameParam string = nameParam
                    """),
                    ("main.bicep", """
                        module createOutput 'createModule.bicep' = {
                          name: 'createOutput'
                          params: {
                            nameParam: 'value'
                          }
                        }

                        module createSA 'createSA.bicep' = {
                          name: 'createSA'
                          params: {
                            test: createOutput.outputs.nameParam
                          }
                        }
                    """)]);

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'test' is used as a resource identifier, API version, or condition in the module 'createSA'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If.");
        }

        [TestMethod]
        public void WhatIfShortCircuiting_Metadata()
        {
            var result = CompilationHelper.Compile(Services,
                [
                    ("module.bicep", """
                        param test string
                        param loc string = resourceGroup().location

                        resource storageAccountModule 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                            name: test
                            location: loc
                            sku: {
                                name: 'Standard_LRS'
                            }
                            kind: 'StorageV2'
                            properties: {
                                accessTier: 'Hot'
                            }
                        }
                    """),
                    ("main.bicep", """
                        param loc string = resourceGroup().location

                        resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                            name: 'storageAccountName'
                            location: loc
                            sku: {
                                name: 'Standard_LRS'
                            }
                            kind: 'StorageV2'
                            properties: {
                                accessTier: 'Hot'
                            }
                        }

                        module mod 'module.bicep' = {
                          name: 'mod'
                          params: {
                            test: storageAccount.properties.dnsEndpointType
                          }
                        }

                        module mod2 'module.bicep' = {
                          name: 'mod2'
                          params: {
                            test: loc
                          }
                        }
                    """)]);

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'test' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If.");
        }

        [TestMethod]
        public void Should_detect_transitive_usage()
        {
            var result = CompilationHelper.Compile(
                Services,
                ("main.bicep", """
                    resource sa 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
                       name: 'acct'
                    }
                    module mod 'mod.bicep' = {
                       name: 'mod'
                       params: {
                           condition: sa.properties.allowBlobPublicAccess
                           name: sa.properties.dnsEndpointType
                       }
                    }
                    """),
                ("mod.bicep", """
                    param condition bool
                    param name string

                    module mod2 'mod2.bicep' = {
                        name: 'mod2'
                        params: {
                            condition: condition
                        }
                    }

                    module mod3 'mod3.bicep' = {
                        name: 'mod3'
                        params: {
                            name: name
                        }
                    }
                    """),
                ("mod2.bicep", """
                    param condition bool

                    resource vnet 'Microsoft.Network/virtualNetworks@2024-07-01' = if (condition) {
                      name: 'vnet'
                    }
                    """),
                ("mod3.bicep", """
                    param name string 
                    
                    resource vnet 'Microsoft.Network/virtualNetworks@2024-07-01' = {
                      name: name
                    }
                    """));

            result.Should().HaveDiagnostics(new[]
            {
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'condition' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'name' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
            });
        }

        [TestMethod]
        public void Should_detect_problematic_parameter_usage_in_json_template_modules()
        {
            var result = CompilationHelper.Compile(
                Services,
                ("main.bicep", """
                    resource sa 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
                       name: 'acct'
                    }
                    module mod 'mod.json' = {
                       name: 'mod'
                       params: {
                           condition: sa.properties.allowBlobPublicAccess
                           nestedCondition: sa.properties.allowBlobPublicAccess
                           name: sa.properties.dnsEndpointType
                           nestedName: sa.properties.dnsEndpointType
                           nestedDeploymentName: sa.properties.dnsEndpointType
                           apiVersion: sa.properties.dnsEndpointType
                           nestedApiVersion: sa.properties.dnsEndpointType
                       }
                    }
                    """),
                ("mod.json", """
                    {
                        "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                        "contentVersion": "1.0.0.0",
                        "parameters": {
                            "condition": {"type": "bool"},
                            "nestedCondition": {"type": "bool"},
                            "name": {"type": "string"},
                            "nestedName": {"type": "string"},
                            "nestedDeploymentName": {"type": "string"},
                            "apiVersion": {"type": "string"},
                            "nestedApiVersion": {"type": "string"},
                        },
                        "resources": [
                            {
                                "type": "Microsoft.Network/virtualNetworks",
                                "apiVersion": "[parameters('apiVersion')]",
                                "name": "[parameters('name')]",
                                "condition": "[parameters('condition')]"
                            },
                            {
                                "type": "Microsoft.Resources/deployments",
                                "apiVersion": "2022-09-01",
                                "name": "[parameters('nestedDeploymentName')]",
                                "properties": {
                                    "mode": "Incremental",
                                    "expressionEvaluationOptions": {
                                        "scope": "Inner"
                                    },
                                    "parameters": {
                                        "condition": {"value": "[parameters('nestedCondition')]"},
                                        "name": {"value": "[parameters('nestedName')]"},
                                        "apiVersion": {"value": "[parameters('nestedApiVersion')]"}
                                    },
                                    "template": {
                                        "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                                        "contentVersion": "1.0.0.0",
                                        "parameters": {
                                            "condition": {"type": "bool"},
                                            "name": {"type": "string"},
                                            "apiVersion": {"type": "string"}
                                        },
                                        "resources": [
                                            {
                                                "type": "Microsoft.Network/virtualNetworks",
                                                "apiVersion": "[parameters('apiVersion')]",
                                                "name": "[parameters('name')]",
                                                "condition": "[parameters('condition')]"
                                            }
                                        ]
                                    }
                                }
                            }
                        ]
                    }
                    """));

            result.Should().HaveDiagnostics(new[]
            {
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'condition' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'nestedCondition' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'name' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'nestedName' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'apiVersion' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'nestedApiVersion' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
            });
        }

        [TestMethod]
        public void Should_detect_usage_via_parameter_default_values()
        {
            var result = CompilationHelper.Compile(
                Services,
                ("main.bicep", """
                    resource sa 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
                       name: 'acct'
                    }
                    module mod 'mod.bicep' = {
                       name: 'mod'
                       params: {
                           condition: sa.properties.allowBlobPublicAccess
                       }
                    }
                    """),
                ("mod.bicep", """
                    param condition bool
                    param modCondition bool = !condition

                    module empty 'empty.bicep' = if (modCondition) {
                        name: 'mod2'
                    }
                    """),
                ("empty.bicep", string.Empty));

            result.Should().HaveDiagnostics(new[]
            {
                ("what-if-short-circuiting", DiagnosticLevel.Warning, "Parameter 'condition' is used as a resource identifier, API version, or condition in the module 'mod'. Providing a runtime value for this parameter will lead to short-circuiting or less precise predictions in What-If."),
            });
        }
    }
}
