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

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Runtime value 'sa.properties.allowBlobPublicAccess' will reduce the precision of what-if analysis for module 'mod'");
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

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Runtime value 'storageAccount.properties.dnsEndpointType' will reduce the precision of what-if analysis for module 'mod'");
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

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Runtime value 'createOutput.outputs.nameParam' will reduce the precision of what-if analysis for module 'createSA'");
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

            result.Diagnostics.Should().ContainSingleDiagnostic("what-if-short-circuiting", DiagnosticLevel.Warning, "Runtime value 'storageAccount.properties.dnsEndpointType' will reduce the precision of what-if analysis for module 'mod'");
        }
    }
}
