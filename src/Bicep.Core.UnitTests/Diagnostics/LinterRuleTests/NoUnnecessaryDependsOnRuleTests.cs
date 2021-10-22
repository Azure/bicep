// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoUnnecessaryDependsOnRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, string[] expectedMessages)
        {
            AssertLinterRuleDiagnostics(NoUnnecessaryDependsOnRule.Code, text, expectedMessages, onCompileErrors);
        }

        [TestMethod]
        public void If_No_Simple_UnnecessaryDependsOn_ShouldPass()
        {
            CompileAndTest(@"
              resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
                name: 'name'
                location: resourceGroup().location
                sku: {
                  name: 'F1'
                  capacity: 1
                }
              }

              resource webApplication 'Microsoft.Web/sites@2018-11-01' = {
                name: 'name'
                location: resourceGroup().location
                properties: {
                  serverFarmId: appServicePlan.id
                }
              }

              resource webApplication2 'Microsoft.Web/sites@2018-11-01' = {
                name: 'name2'
                location: resourceGroup().location
                properties: {
                  serverFarmId: appServicePlan.id
                }
                dependsOn: []
              }

              resource webApplication3 'Microsoft.Web/sites@2018-11-01' = {
                name: 'name3'
                location: resourceGroup().location
                properties: {
                  serverFarmId: appServicePlan.id
                }
                dependsOn: [
                    webApplication
                    webApplication //asdf fail because of duplicate entry?
                    webApplication2
                ]
              }
            ",
              OnCompileErrors.Fail,
              new string[] { }
            );
        }

        // This is the failing example in the docs
        [TestMethod]
        public void If_SimpleUnnecessaryDependsOn_ShouldFail()
        {
            CompileAndTest(
                @"
                resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
                  name: 'name'
                  location: resourceGroup().location
                  sku: {
                    name: 'F1'
                    capacity: 1
                  }
                }

                resource webApplication 'Microsoft.Web/sites@2018-11-01' = {
                  name: 'name'
                  location: resourceGroup().location
                  properties: {
                    serverFarmId: appServicePlan.id
                  }
                  dependsOn: [
                    appServicePlan
                  ]
                }
            ",
              OnCompileErrors.Fail,
              new string[] {
                "Remove unnecessary dependsOn entry 'appServicePlan'."
              }
            );
        }

        [TestMethod]
        public void If_Indirect_UnnecessaryDependsOn_ShouldFail()
        {
            CompileAndTest(
                @"
                resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
                  name: 'name'
                  location: resourceGroup().location
                  sku: {
                    name: 'F1'
                    capacity: 1
                  }
                }

                resource webApplication 'Microsoft.Web/sites@2018-11-01' = {
                  name: 'name'
                  location: resourceGroup().location
                  properties: {
                    serverFarmId: appServicePlan.id
                  }
                  dependsOn: [
                    appServicePlan
                  ]
                }
            ",
              OnCompileErrors.Fail,
              new string[] {
                "Remove unnecessary dependsOn entry 'appServicePlan'."
              });
        }

        [TestMethod]
        public void If_Explicit_DependsOn_ToAncestor_FromNestedChild_ShouldFail()
        {
            CompileAndTest(
                @"
                resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
                  location: resourceGroup().location
                  name: 'myVnet'
                  properties: {
                    addressSpace: {
                      addressPrefixes: [
                        '10.0.0.0/20'
                      ]
                    }
                  }

                  // As nested child
                  resource subnet1 'subnets@2020-06-01' = {
                    name: 'subnet1'
                        properties: {
                        addressPrefix: '10.0.0.0/24'
                    }
                    dependsOn: [
                      vnet
                    ]
                  }
                }
            ",
              OnCompileErrors.Fail,
              new string[] {
                "" +
                "Remove unnecessary dependsOn entry 'vnet'."
              });
        }

        [TestMethod]
        public void If_Explicit_DependsOn_ToAncestor_FromTopLevelChild_ShouldFail()
        {
            CompileAndTest(
               @"
                resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
                  location: resourceGroup().location
                  name: 'myVnet'
                  properties: {
                    addressSpace: {
                      addressPrefixes: [
                        '10.0.0.0/20'
                      ]
                    }
                  }
                }
                // As top-level child
                resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
                  parent: vnet
                  name: 'subnet'
                  properties: {
                    addressPrefix: '10.0.1.0/24'
                  }
                  dependsOn: [
                    vnet
                  ]
                }
            ",
              OnCompileErrors.Fail,
              new string[] {
              "Remove unnecessary dependsOn entry 'vnet'."
              }
            );
        }

        [TestMethod]
        public void If_DuplicateEntries_ShouldFailForEach()
        {
            CompileAndTest(
               @"
                resource vn 'Microsoft.Network/virtualNetworks@2020-06-01' existing =  {
                  name: 'vn'

                  resource subnet1 'subnets@2020-06-01' = {
                    name: 'subnet1'
                    properties: {
                      addressPrefix: '10.0.1.0/24'
                    }
                  }

                  resource subnet2 'subnets@2020-06-01' = {
                    name: 'subnet2'
                    properties: {
                      addressPrefix: '10.0.1.0/24'
                    }
                    dependsOn: [
                      vn
                      subnet1
                      vn
                    ]
                  }
                }
            ",
              OnCompileErrors.Fail,
              new string[] {
                "Remove unnecessary dependsOn entry 'vn'.",
                "Remove unnecessary dependsOn entry 'vn'."
              }
            );
        }

        [TestMethod]
        public void If_Explicit_DependsOn_ToParent_FromGrandChild_UsingColonNotation_ShouldFail()
        {
            CompileAndTest(
                @"
                resource grandparent 'Microsoft.Network/virtualNetworks@2020-06-01' = {
                  location: resourceGroup().location
                  name: 'grandparent'
                  properties: {
                    addressSpace: {
                      addressPrefixes: [
                        '10.0.0.0/20'
                      ]
                    }
                  }
                  resource parent 'subnets@2020-06-01' = {
                    name: 'parent'
                    properties: {
                      addressPrefix: '10.0.1.0/24'
                    }
                    resource grandchild 'DoesntExistButThatsOkay@2020-10-01' = {
                      name: 'grandchild'
                      dependsOn: [
                        grandparent
                        grandparent::parent
                      ]
                      resource greatgrandchild 'DoesntExistButThatsOkay@2020-10-01' = {
                          name: 'greatgrandchild'
                          dependsOn: [
                            grandparent::parent
                            grandparent::parent::grandchild
                          ]
                      }
                    }
                  }
                }
                ",
                OnCompileErrors.Fail,
                new string[] {
                    "Remove unnecessary dependsOn entry 'grandparent'.",
                    "Remove unnecessary dependsOn entry 'parent'.",
                    "Remove unnecessary dependsOn entry 'parent'.",
                    "Remove unnecessary dependsOn entry 'grandchild'.",
            });
        }

        [TestMethod]
        public void If_UnnecessaryReferenceToParent_FromLoop_ToNonLoopedParent_Should_Fail()
        {
            CompileAndTest(@"
                resource vn 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
                  name: 'vn'
                }

                resource blobServices 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' = [for i in range(0, 3): {
                  name: 'blobs${i}'
                  parent: vn
                  dependsOn: [
                    vn
                  ]
                }]
            ",
              OnCompileErrors.Fail,
              new string[] {
                "Remove unnecessary dependsOn entry 'vn'."
              }
            );
        }

        [TestMethod]
        public void If_ReferencesResourceByIndex_Simple_Should_IgnoreAndPass()
        {
            CompileAndTest(@"
              resource vn1 'Microsoft.Network/virtualNetworks@2021-02-01' existing = [for i in range(0, 1): {
                name: 'vn1'
              }]

              resource subnet1 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' = [for i in range(0, 1): {
                name: 'subnet1'
                parent: vn1[i]
                dependsOn: [
                  vn1[i] // IGNORE THIS (DON'T GIVE RULE FAILURE)
                ]
              }]
            ",
              OnCompileErrors.Fail,
              new string[] {
              }
            );
        }

        [TestMethod]
        public void If_ReferencesResourceByIndex_Realistic_Should_IgnoreAndPass()
        {
            CompileAndTest(@"
              param storageAccounts array

              resource storageAccountResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for storageName in storageAccounts: {
                name: storageName
                location: resourceGroup().location
                properties: {
                  supportsHttpsTrafficOnly: true
                }
                kind: 'StorageV2'
                sku: {
                  name: 'Standard_LRS'
                }
              }]

              resource dScript 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
                name: 'scriptWithStorage'
                location: resourceGroup().location
                kind: 'AzureCLI'
                identity: {
                }
                properties: {
                    azCliVersion: '2.0.80'
                    storageAccountSettings: {
                    storageAccountName: storageAccountResources[0].name
                    }
                    retentionInterval: 'P1D'
                }
                dependsOn: [
                    storageAccountResources[0]
                ]
              }
            ",
              OnCompileErrors.Fail,
              new string[] {
              }
            );
        }

        [TestMethod]
        public void If_ReferencesResourceCollection_Should_IgnoreAndPass()
        {
            CompileAndTest(@"
              resource vn4 'Microsoft.Network/virtualNetworks@2021-02-01' existing = [for i in range(0, 1): {
                name: 'vn4${i}'
              }]

              resource subnet4 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' = [for i in range(0, 1): {
                name: 'subnet4'
                parent: vn4[i]
                dependsOn: [
                  vn4 // Depends on the entire collection vn4 of vnets
                ]
              }]
            ",
            OnCompileErrors.Fail,
            new string[] {
            }
          );
        }

        [TestMethod]
        public void If_DependsOnModule_Should_IgnoreAndPass()
        {
            CompileAndTest(@"
              module m1 'module.bicep' = {
                name: 'm1'
                dependsOn: []
              }

              resource vn3 'Microsoft.Network/virtualNetworks@2021-02-01' existing = [for i in range(0, 1): {
                name: 'vn3${i}'
              }]

              resource subnet3 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' = [for i in range(0, 1): {
                name: 'subnet3'
                parent: vn3[i]
                dependsOn: [
                  m1
                ]
              }]
            ",
            OnCompileErrors.Ignore, // Will get an error about not finding the module
            new string[] {
            }
          );
        }

        [TestMethod]
        public void TolerantOfSyntaxErrors_1()
        {
            CompileAndTest(@"
              resource resource1 'Microsoft.Network/virtualNetworks/subnets@2021-03-01' existing = {
                // missing name
              }

              resource resource2 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
                name: resource1.id
              }
            ",
            OnCompileErrors.Ignore,
            new string[] {
            }
          );
        }

        [TestMethod]
        public void TolerantOfSyntaxErrors_2()
        {
            CompileAndTest(@"
              resource aksDefaultPoolSubnet 'Microsoft.Network/virtualNetworks/subnets' existing = {
                parent: virtualNetwork
                name: aksDefaultPoolSubnetName
              }

              resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
                name: guid(aksDefaultPoolSubnet.id, 'Network Contributor')
                scope: aksDefaultPoolSubnet
                properties: {
                  principalId: aksServicePrincipalObjectId
                  roleDefinitionId: '4d97b98b-1d4f-4787-a291-c67834d212e7'
                }
                dependsOn: [
                  virtualNetwork
                  userAssignedIdentities
                ]
              }
            ",
            OnCompileErrors.Ignore,
            new string[] {
            }
          );
        }
    }
}
