// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.PrettyPrintV2;
using Bicep.Core.UnitTests.Serialization;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    // TODO:             //asdfg resource/user-defined types


    [TestClass]
    public class ExtractToVariableTests : CodeActionTestBase
    {
        private const string ExtractToVariableTitle = "Create variable";

        [DataTestMethod]
        [DataRow("""
            var a = '|b'
            """,
            """
            var newVariable = 'b'
            var a = newVariable
            """)]
        [DataRow("""
            var a = 'a'
            var b = '|b'
            var c = 'c'
            """,
            """
            var a = 'a'
            var newVariable = 'b'
            var b = newVariable
            var c = 'c'
            """)]
        [DataRow("""
            var a = 1 + |2
            """,
            """
            var newVariable = 2
            var a = 1 + newVariable
            """)]
        [DataRow("""
            var a = <<1 + 2>>
            """,
            """
            var newVariable = 1 + 2
            var a = newVariable
            """)]
        [DataRow("""
            var a = <<1 +>> 2
            """,
            """
            var newVariable = 1 + 2
            var a = newVariable
            """)]
        [DataRow("""
            var a = 1 |+ 2
            """,
            """
            var newVariable = 1 + 2
            var a = newVariable
            """)]
        [DataRow("""
            var a = 1 <<+ 2 + 3 >>+ 4
            """,
            """
            var newVariable = 1 + 2 + 3 + 4
            var a = newVariable
            """)]
        //asdfg issue: should we expand selection?
        [DataRow("""
            param p1 int = 1 + |2
            """,
            """
            var newVariable = 2
            param p1 int = 1 + newVariable
            """)]
        [DataRow("""
            var a = 1 + 2
            var b = '${a}|{a}'
            """,
            """
            var a = 1 + 2
            var newVariable = '${a}{a}'
            var b = newVariable
            """,
            DisplayName = "Full interpolated string")]
        [DataRow("""
            // comment 1
            @secure
            // comment 2
            param a = '|a'
            """,
            """
            // comment 1
            var newVariable = 'a'
            @secure
            // comment 2
            param a = newVariable
            """,
            DisplayName = "Preceding lines")]
        [DataRow("""
            var a = 1
            var b = [
                'a'
                1 + <<2>>
                'c'
            ]
            """,
            """
            var a = 1
            var newVariable = 2
            var b = [
                'a'
                1 + newVariable
                'c'
            ]
            """,
            DisplayName = "Inside a data structure")]
        [DataRow("""
            // My comment here
            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                name: 'name'
                location: |'westus'
                kind: 'StorageV2'
                sku: {
                name: 'Premium_LRS'
                }
            }
            """,
            """
            // My comment here
            var location = 'westus'
            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                name: 'name'
                location: location
                kind: 'StorageV2'
                sku: {
                name: 'Premium_LRS'
                }
            }
            """)]
        public async Task Basics(string fileWithSelection, string expectedText)
        {
            await RunExtractToVariableTest(fileWithSelection, expectedText);
        }

        [DataTestMethod]
        [DataRow("""
            var newVariable = 'newVariable'
            param newVariable2 string = '|newVariable2'
            """,
            """
            var newVariable = 'newVariable'
            var newVariable3 = 'newVariable2'
            param newVariable2 string = newVariable3
            """,
            DisplayName = "Simple naming conflict")
        ]
        [DataRow("""
            var id = [1, 2, 3]
            param id2 string = 'hello'
            resource id6 'Microsoft.Network/virtualNetworks/subnets@2024-01-01' = [
                for (id3, id4) in id: {
                name: 'subnet${id3}'
                properties: {
                    addressPrefix: '10.0.${id4}.0/24'
                    natGateway: {
                    id: '|gatewayId'
                    }
                }
                }
            ]
            output id5 string = id2
            """,
            """
            var id = [1, 2, 3]
            param id2 string = 'hello'
            var id7 = 'gatewayId'
            resource id6 'Microsoft.Network/virtualNetworks/subnets@2024-01-01' = [
                for (id3, id4) in id: {
                name: 'subnet${id3}'
                properties: {
                    addressPrefix: '10.0.${id4}.0/24'
                    natGateway: {
                    id: id7
                    }
                }
                }
            ]
            output id5 string = id2
            """,
            DisplayName = "Complex naming conflicts")]
        public async Task ShouldRenameToAvoidConflicts(string fileWithSelection, string expectedText)
        {
            await RunExtractToVariableTest(fileWithSelection, expectedText);
        }

        [TestMethod]
        public async Task ShouldHandleArrays()
        {
            await RunExtractToVariableTest("""
            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-01-01' = [
              for (item, index) in <<[1, 2, 3]>>: {
                name: 'subnet${index}'
                properties: {
                  addressPrefix: '10.0.${index}.0/24'
                }
              }
            ]
            """,
            """
            var newVariable = [1, 2, 3]
            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-01-01' = [
              for (item, index) in newVariable: {
                name: 'subnet${index}'
                properties: {
                  addressPrefix: '10.0.${index}.0/24'
                }
              }
            ]
            """);
        }

        [TestMethod]
        public async Task ShouldHandleObjects()
        {
            await RunExtractToVariableTest("""
                resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
                  parent: vmName_resource
                  name: 'cse-windows'
                  location: location
                  properties: <<{
                    // Entire properties object selected
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    settings: {
                      fileUris: [
                        uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                      ]
                      commandToExecute: commandToExecute
                    }
                  }>>
                }                
                """,
                """
                var properties = {
                  // Entire properties object selected
                  publisher: 'Microsoft.Compute'
                  type: 'CustomScriptExtension'
                  typeHandlerVersion: '1.8'
                  autoUpgradeMinorVersion: true
                  settings: {
                    fileUris: [
                      uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                    ]
                    commandToExecute: commandToExecute
                  }
                }
                resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
                  parent: vmName_resource
                  name: 'cse-windows'
                  location: location
                  properties: properties
                }                
                """);
        }

        //[DataTestMethod]
        //[DataRow("""
        //    param p1 int = 1 + /*comments1*/|2/*comments2*/
        //    """,
        //    """
        //    var newVariable = /*comments1*/2/*comments2*/
        //    param p1 int = 1 + newVariable
        //    """,
        //    DisplayName = "asdfg bug: Expression with comments")]
        //public async Task ExpressionsWithComments(string fileWithSelection, string expectedText)
        //{
        //    await RunExtractToVariableTest(fileWithSelection, expectedText);
        //}

        [TestMethod]
        public async Task IfJustPropertyNameSelected_ThenExtractPropertyValue()
        {
            await RunExtractToVariableTest("""
                resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
                  parent: vmName_resource
                  name: 'cse-windows'
                  location: location
                  properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    setting|s: { // Property key selected - extract just the value
                      fileUris: [
                        uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                      ]
                      commandToExecute: commandToExecute
                    }
                  }
                }                
                """,
                """
                var settings = {
                  // Property key selected - extract just the value
                  fileUris: [
                    uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                  ]
                  commandToExecute: commandToExecute
                }
                resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
                  parent: vmName_resource
                  name: 'cse-windows'
                  location: location
                  properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    settings: settings
                  }
                }                
                """);
        }

        [DataTestMethod]
        [DataRow("""
            resource vmName_resource 'Microsoft.Compute/virtualMachines@2019-12-01' = {
              name: vmName
              location: location
              properties: {
                osProfile: {
                  computerName: vmName
                  myproperty: {
                    abc: [
                      {
                        def: [
                          'ghi'
                          '|jkl'
                        ]
                      }
                    ]
                  }
                }
              }
            }            
            """,
            """
            var newVariable = 'jkl'
            resource vmName_resource 'Microsoft.Compute/virtualMachines@2019-12-01' = {
              name: vmName
              location: location
              properties: {
                osProfile: {
                  computerName: vmName
                  myproperty: {
                    abc: [
                      {
                        def: [
                          'ghi'
                          newVariable
                        ]
                      }
                    ]
                  }
                }
              }
            }            
            """,
            DisplayName = "Array element, don't pick up property name")]
        [DataRow("""
            resource vmName_resource 'Microsoft.Compute/virtualMachines@2019-12-01' = {
              name: vmName
              location: location
              properties: {
                osProfile: {
                  computerName: vmName
                  myproperty: {
                    abc: <<[
                      {
                        def: [
                          'ghi'
                          'jkl'
                        ]
                      }
                    ]>>
                  }
                }
              }
            }            
            """,
            """
            var abc = [
              {
                def: [
                  'ghi'
                  'jkl'
                ]
              }
            ]
            resource vmName_resource 'Microsoft.Compute/virtualMachines@2019-12-01' = {
              name: vmName
              location: location
              properties: {
                osProfile: {
                  computerName: vmName
                  myproperty: {
                    abc: abc
                  }
                }
              }
            }            
            """,
            DisplayName = "Full property value as array, pick up property name")]
        public async Task PickUpPropertyName_ButOnlyIfFullPropertyValue(string fileWithSelection, string expectedText)
        {
            await RunExtractToVariableTest(fileWithSelection, expectedText);
        }

        [DataTestMethod]
        [DataRow("var a = abc().bc|d",
            """
            var abcBcd = abc().bcd
            var a = abcBcd
            """)]
        [DataRow("var a = abc|().bcd",
            """
            var newVariable = abc()
            var a = newVariable.bcd
            """)]
        [DataRow("var a = abc.bcd.|def",
            """
            var bcdDef = abc.bcd.def
            var a = bcdDef
            """)]
        [DataRow("var a = abc.b|cd",
            """
            var abcBcd = abc.bcd
            var a = abcBcd
            """)]
        [DataRow("var a = abc.bc|d",
            """
            var abcBcd = abc.bcd
            var a = abcBcd
            """)]
        [DataRow("var a = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob|",
            """
            var primaryEndpointsBlob = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob
            var a = primaryEndpointsBlob
            """)]
        [DataRow("var a = reference(storageAccount.id, '2018-02-01').prim|aryEndpoints.blob",
            """
            var referencePrimaryEndpoints = reference(storageAccount.id, '2018-02-01').primaryEndpoints
            var a = referencePrimaryEndpoints.blob
            """)]
        [DataRow("var a = a.b.|c.d.e",
            """
            var bC = a.b.c
            var a = bC.d.e
            """)]
        public async Task PickUpNameFromProperyAccess_UpToTwoLevels(string fileWithSelection, string expectedText)
        {
            await RunExtractToVariableTest(fileWithSelection, expectedText);
        }

        [DataTestMethod]
        //
        // Closest ancestor expression is the top-level expression itself -> offer to update full expression
        //
        [DataRow(
            "storageUri:| reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').primaryEndpoints.|blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').primaryEndpoints.<<blo>>b",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        //
        // Cursor is inside the property name -> offer full expression
        //
        [DataRow(
            "storageUri|: reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        [DataRow(
            "<<storageUri: re>>ference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        [DataRow(
            "<<storageUri: reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob>>",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        //
        // Cursor is inside a subexpression -> only offer to extract that specific subexpression
        //
        // ... reference() call
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').|primaryEndpoints.blob",
            "var referencePrimaryEndpoints = reference(storageAccount.id, '2018-02-01').primaryEndpoints",
            "storageUri: referencePrimaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference|(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: refere<<nce(storageAccount.id, '201>>8-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        //   ... '2018-02-01'
        [DataRow(
            "storageUri: reference(storageAccount.id, |'2018-02-01').primaryEndpoints.blob",
            "var newVariable = '2018-02-01'",
            "storageUri: reference(storageAccount.id, newVariable).primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01|').primaryEndpoints.blob",
            "var newVariable = '2018-02-01'",
            "storageUri: reference(storageAccount.id, newVariable).primaryEndpoints.blob"
            )]
        //   ... storageAccount.id
        [DataRow(
            "storageUri: reference(storageAccount.|id, '2018-02-01').primaryEndpoints.blob",
            "var storageAccountId = storageAccount.id",
            "storageUri: reference(storageAccountId, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.i|d, '2018-02-01').primaryEndpoints.blob",
            "var storageAccountId = storageAccount.id",
            "storageUri: reference(storageAccountId, '2018-02-01').primaryEndpoints.blob"
            )]
        // ... storageAccount
        [DataRow(
            "storageUri: reference(storageAc|count.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = storageAccount",
            "storageUri: reference(newVariable.id, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAc|count.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = storageAccount",
            "storageUri: reference(newVariable.id, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAc|count.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = storageAccount",
            "storageUri: reference(newVariable.id, '2018-02-01').primaryEndpoints.blob"
            )]
        // ... inside reference(x, y) but not inside x or y -> closest enclosing expression is the reference()
        [DataRow(
            "storageUri: reference(storageAccount.id,| '2018-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01' |).primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference|(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        public async Task ShouldExpandSelectedExpressionsInALogicalWay(string lineWithSelection, string expectedVarDefinition, string expectedModifiedLine)
        {
            string inputWithSelection = $$"""
                resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                  properties: {
                    diagnosticsProfile: {
                      bootDiagnostics: {
                        {{lineWithSelection}}
                      }
                    }
                  }
                }
                """;

            string expectedOutput = $$"""
                resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                {{expectedVarDefinition}}
                resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                  properties: {
                    diagnosticsProfile: {
                      bootDiagnostics: {
                        {{expectedModifiedLine}}
                      }
                    }
                  }
                }
                """;
            await RunExtractToVariableTest(inputWithSelection, expectedOutput);
        }

        [DataTestMethod]
        [DataRow(
            "storageUri: reference(stora<<geAccount.i>>d, '2018-02-01').primaryEndpoints.blob",
            "var storageAccountId = storageAccount.id",
            "storageUri: reference(storageAccountId, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: refer<<ence(storageAccount.id, '2018-02-01').primaryEndpoints.bl>>ob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').primar<<yEndpoints.blob>>",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        public async Task IfThereIsASelection_ThenPickUpEverythingInTheSelection_AfterExpanding(string lineWithSelection, string expectedVarDefinition, string expectedModifiedLine)
        {
            string inputWithSelection = $$"""
                resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                  properties: {
                    diagnosticsProfile: {
                      bootDiagnostics: {
                        {{lineWithSelection}}
                      }
                    }
                  }
                }
                """;

            string expectedOutput = $$"""
                resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                {{expectedVarDefinition}}
                resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                  properties: {
                    diagnosticsProfile: {
                      bootDiagnostics: {
                        {{expectedModifiedLine}}
                      }
                    }
                  }
                }
                """;
            await RunExtractToVariableTest(inputWithSelection, expectedOutput);
        }

        private async Task RunExtractToVariableTest(string fileWithSelection, string? expectedText)
        {
            (var codeActions, var bicepFile) = await RunSyntaxTest(fileWithSelection, '|');
            var extract = codeActions.FirstOrDefault(x => x.Title.StartsWith(ExtractToVariableTitle));

            if (expectedText == null)
            {
                extract.Should().BeNull("should not offer any variable extractions");
            }
            else
            {
                extract.Should().NotBeNull("should contain an action to extract to variable");
                extract!.Kind.Should().Be(CodeActionKind.RefactorExtract);

                var updatedFile = ApplyCodeAction(bicepFile, extract);
                updatedFile.Should().HaveSourceText(expectedText);
            }
        }
    }
}
