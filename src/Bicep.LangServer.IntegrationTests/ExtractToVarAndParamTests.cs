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
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

//asdfg
/*
 type foo = {
  property: foo?
}*/

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class ExtractToVarAndParamTests : CodeActionTestBase
    {
        private const string ExtractToVariableTitle = "Create variable for";
        private const string ExtractToParameterTitle = "Create parameter for";

        [DataTestMethod]
        [DataRow("""
            var a = '|b'
            """,
            """
            var newVariable = 'b'
            var a = newVariable
            """,
            """
            param newParameter string = 'b'
            var a = newParameter
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
            """,
            """
            var a = 'a'
            param newParameter string = 'b'
            var b = newParameter
            var c = 'c'
            """)]
        [DataRow("""
            var a = 1 + |2
            """,
            """
            var newVariable = 2
            var a = 1 + newVariable
            """,
            """
            param newParameter int = 2
            var a = 1 + newParameter
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
            """
            var a = 1
            param newParameter int = 2
            var b = [
                'a'
                1 + newParameter
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
            """,
            """
            // My comment here
            param location string = 'westus'
            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                name: 'name'
                location: location
                kind: 'StorageV2'
                sku: {
                name: 'Premium_LRS'
                }
            }
            """)]
        public async Task Basics(string fileWithSelection, string? expectedVarText, string? expectedParamText = null)
        {
            await RunExtractToVariableAndOrParameterTest(fileWithSelection, expectedVarText, expectedParamText);
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
            await RunExtractToVariableAndOrParameterTest("""
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
            """,
            """
            param newParameter array = [1, 2, 3]
            resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-01-01' = [
              for (item, index) in newParameter: {
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
            await RunExtractToVariableAndOrParameterTest("""
                    param _artifactsLocation string
                    param  _artifactsLocationSasToken string
                
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
                    param _artifactsLocation string
                    param  _artifactsLocationSasToken string

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
                    """,
                """
                    param _artifactsLocation string
                    param  _artifactsLocationSasToken string

                    param properties object = {
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

        [DataTestMethod]
        //asdfg delete
        [DataRow("""
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: {
                			id: |'virtualNetworksId'
                		}
                	}
                }
                """,
            """
                param id string = 'virtualNetworksId'
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: {
                			id: id
                		}
                	}
                }
                """,
            DisplayName = "resource types 3 asdfg")]


        [DataRow("""
                var i = <<1>>
                """,
            """
                param newParameter int = 1
                var i = newParameter
                """,
            DisplayName = "Literal integer")]
        [DataRow("""
                param i int = 1
                var j = <<i>> + 1
                """,
            """
                param i int = 1
                param newParameter int = i
                var j = newParameter + 1
                """,
            DisplayName = "int parameter reference")]
        [DataRow("""
                param i int = 1
                var j = <<i + 1>>
                """,
            """
                param i int = 1
                param newParameter int = i + 1
                var j = newParameter
                """,
            DisplayName = "int expression with param")]
        [DataRow("""
                param i string
                var j = <<concat(i, i)>>
                """,
            """
                param i string
                param newParameter string = concat(i, i)
                var j = newParameter
                """,
            DisplayName = "strings concatenated")]
        [DataRow("""
                param i string = 'i'
                var i2 = 'i2'
                var j = <<'{i}{i2}'>>
                """,
            """
                param i string = 'i'
                var i2 = 'i2'
                param newParameter string = '{i}{i2}'
                var j = newParameter
                """,
            DisplayName = "strings concatenated")]
        [DataRow("""
                var p = <<[ 1, 2, 3 ]>>
                """,
            """
                param newParameter array = [1, 2, 3]
                var p = newParameter
                """,
            DisplayName = "array literal")]
        [DataRow("""
                var p = <<{ a: 1, b: 'b' }>>
                """,
            """
                param newParameter object = { a: 1, b: 'b' }
                var p = newParameter
                """,
            DisplayName = "object literal")]
        [DataRow("""
                var p = { a: <<1>>, b: 'b' }
                """,
            """
                param a int = 1
                var p = { a: a, b: 'b' }
                """,
            DisplayName = "property value from object literal")]
        [DataRow("""
                var o1 = { a: 1, b: 'b' }
                var a = <<o1.a>>
                """,
            """
                var o1 = { a: 1, b: 'b' }
                param o1A int = o1.a
                var a = o1A
                """,
            DisplayName = "referenced property value from object literal")]
        [DataRow("""
                param p 'a'||'b' = 'a'
                var v = <<p>>
                """,
            """
                param p 'a'|'b' = 'a'
                param newParameter 'a' | 'b' = p
                var v = newParameter
                """,
            DisplayName = "string literal type")] //asdfg correct behavior?
        [DataRow("""
                var a = {
                    int: 1
                }
                var b = a.|int
                """,
            """
                var a = {
                    int: 1
                }
                param aInt int = a.int
                var b = aInt
                """,
            DisplayName = "object properties 1")]
        [DataRow("""
                var a = {
                    int: 1
                }
                var b = |a.int
                """,
            """
                var a = {
                    int: 1
                }
                param newParameter object = a
                var b = newParameter.int
                """,
        DisplayName = "object properties 2")]
        [DataRow("""
                var a = {
                    sku: {
                        name: 'Standard_LRS'
                    }
                }
                var b = a.|sku
                """,
            """
                var a = {
                    sku: {
                        name: 'Standard_LRS'
                    }
                }
                param aSku object = a.sku
                var b = aSku
                """,
            DisplayName = "object properties 3")]
        [DataRow("""
                param p {
                  i: int
                  o: {
                    i2: int
                  }
                } = { i:1, o: { i2: 2} }
                var v = <<p>>.o.i2
                """,
            """
                param p {
                  i: int
                  o: {
                    i2: int
                  }
                } = { i:1, o: { i2: 2} }
                param newParameter { i: int, o: { i2: int } } = p
                var v = newParameter.o.i2
                """,
            DisplayName = "custom object type, whole object")]
        [DataRow("""
                param p {
                  i: int
                  o: {
                    i2: int
                  }
                } = { i:1, o: { i2: 2} }
                var v = p.|o.i2
                """,
            """
                param p {
                  i: int
                  o: {
                    i2: int
                  }
                } = { i:1, o: { i2: 2} }
                param pO { i2: int } = p.o
                var v = pO.i2
                """,
            DisplayName = "custom object type, partial")]
        [DataRow("""
                resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
                  unknownProperty: |123
                }
                """,
            """
                param unknownProperty int = 123
                resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
                  unknownProperty: unknownProperty
                }
                """,
            DisplayName = "resource types undefined 1")]
        [DataRow("""
                param p1 'abc'||'def'
                resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
                  unknownProperty: |p1
                }
                """,
            """
                param p1 'abc'|'def'
                param unknownProperty 'abc' | 'def' = p1
                resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
                  unknownProperty: unknownProperty
                }
                """,
            DisplayName = "resource properties unknown property, follows expression's inferred type")]

        //asdf TODO(??)
        //[DataRow("""
        //        var a = <<aksCluster>>
        //        resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = { }
        //        """,
        //    """
        //        param newParameter resource 'Microsoft.ContainerService/managedClusters@2021-03-01' = aksCluster
        //        var a = newParameter
        //        resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = { }                
        //        """,
        //    DisplayName = "resource type")]


        [DataRow("""
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: {
                			id: |'virtualNetworksId'
                		}
                	}
                }
                """,
            """
                param id string = 'virtualNetworksId'
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: {
                			id: id
                		}
                	}
                }
                """,
            DisplayName = "resource types 3 asdfg")]
        [DataRow("""
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: |{
                			id: virtualNetworksId
                		}
                	}
                }
                """,
            """
                param remoteVirtualNetwork object = {
                  id: virtualNetworksId
                }
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: remoteVirtualNetwork
                	}
                }
                """,
            DisplayName = "resource types - SubResource")]
        [DataRow("""
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: |true
                		remoteVirtualNetwork: {
                			id: virtualNetworksId'
                		}
                	}
                }
                """,
            """
                param allowVirtualNetworkAccess bool = true
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: 'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: allowVirtualNetworkAccess
                		remoteVirtualNetwork: {
                			id: virtualNetworksId'
                		}
                	}
                }
                """,
            DisplayName = "resource types 5 asdfg")]
        //asdfg param ought to be named peeringName instead of name 
        [DataRow("""
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: |'virtualNetwork/name'
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: {
                			id: virtualNetworksId
                		}
                	}
                }
                """,
            """
                param name string = 'virtualNetwork/name'
                resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
                	name: name
                	properties: {
                		allowVirtualNetworkAccess: true
                		remoteVirtualNetwork: {
                			id: virtualNetworksId
                		}
                	}
                }
                """,
            DisplayName = "resource types - string property")]
        [DataRow("""
                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                	name: 'name'
                	location: 'location'
                	kind: 'StorageV2'
                	sku: {
                		name: |'Premium_LRS'
                	}
                }
                """,
            """
                param name 'Premium_LRS' | 'Premium_ZRS' | 'Standard_GRS' | 'Standard_GZRS' | 'Standard_LRS' | 'Standard_RAGRS' | 'Standard_RAGZRS' | 'Standard_ZRS' | string = 'Premium_LRS'
                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                	name: 'name'
                	location: 'location'
                	kind: 'StorageV2'
                	sku: {
                		name: name
                	}
                }
                """,
            DisplayName = "resource properties - string union")]
        [DataRow("""
                param p int?
                var v = |p
                """,
            // CONSIDER: 'int?' would be better
            """
                param p int?
                param newParameter int | null = p
                var v = newParameter
                """,
            DisplayName = "nullable types")]
        [DataRow("""
                param whoops int = 'not an int'
                var v = <<p + 1>>
                """,
            """
                param whoops int = 'not an int'
                param newParameter unknown = p + 1
                var v = newParameter
                """,
            DisplayName = "error types")]

        //asdfg TODO: secure types
        //[DataRow(""" TODO: asdfg
        //    @secure()
        //    param i string = "secure"
        //    var j = <<i>>
        //    """,
        //    """
        //    param i string = "secure"
        //    @secure()
        //    param newParameter string = i
        //    var j = newParameter
        //    """,
        //    DisplayName = "secure string param reference")]
        //asdfg TODO: secure types
        //[DataRow("""
        //    @secure()
        //    param i string = "secure"
        //    var j = <<i>>
        //    """,
        //    """
        //    param i string = "secure"
        //    @secure()
        //    param newParameter string = i
        //    var j = newParameter
        //    """,
        //    DisplayName = "expression with secure string param reference")]
        public async Task Params_CorrectlyInferType(string fileWithSelection, string expectedText)
        {
            await RunExtractToParameterTest(fileWithSelection, expectedText);
        }

        [TestMethod]
        public async Task IfJustPropertyNameSelected_ThenExtractPropertyValue()
        {
            await RunExtractToVariableAndOrParameterTest("""                                
                var isWindowsOS = true
                var provisionExtensions = true
                param _artifactsLocation string
                @secure()
                param _artifactsLocationSasToken string

                resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
                  name: 'cse-windows/extension'
                  location: 'location'
                  properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    setting|s: { // Property key selected - extract just the value
                      fileUris: [
                        uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                      ]
                      commandToExecute: 'commandToExecute'
                    }
                  }
                }
                """,
                """
                var isWindowsOS = true
                var provisionExtensions = true
                param _artifactsLocation string
                @secure()
                param _artifactsLocationSasToken string

                var settings = {
                  // Property key selected - extract just the value
                  fileUris: [
                    uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                  ]
                  commandToExecute: 'commandToExecute'
                }
                resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
                  name: 'cse-windows/extension'
                  location: 'location'
                  properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    settings: settings
                  }
                }
                """,
                """
                var isWindowsOS = true
                var provisionExtensions = true
                param _artifactsLocation string
                @secure()
                param _artifactsLocationSasToken string

                param settings object = {
                  // Property key selected - extract just the value
                  fileUris: [
                    uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                  ]
                  commandToExecute: 'commandToExecute'
                }
                resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
                  name: 'cse-windows/extension'
                  location: 'location'
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
            """
            param newParameter string = 'jkl'
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
                          newParameter
                        ]
                      }
                    ]
                  }
                }
              }
            }            
            """,
            DisplayName = "Array element, don't pick up property name")]
        [DataRow(
            """
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
                """
                param abc array = [
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
        public async Task VarOrParam_PickUpPropertyName_ButOnlyIfFullPropertyValue(string fileWithSelection, string? expectedVarText, string? expectedParamText)
        {
            await RunExtractToVariableAndOrParameterTest(fileWithSelection, expectedVarText, expectedParamText);
        }

        [DataTestMethod]
        [DataRow("var a = resourceGroup().locati|on",
            """
                var resourceGroupLocation = resourceGroup().location
                var a = resourceGroupLocation
                """,
            """
                param resourceGroupLocation string = resourceGroup().location
                var a = resourceGroupLocation
                """)]
        [DataRow("var a = abc|().bcd",
            """
                var newVariable = abc()
                var a = newVariable.bcd
                """,
            null)]
        [DataRow("var a = abc.bcd.|def",
            """
                var bcdDef = abc.bcd.def
                var a = bcdDef
                """,
            null)]
        [DataRow("var a = abc.b|cd",
            """
                var abcBcd = abc.bcd
                var a = abcBcd
                """,
            null)]
        [DataRow("var a = abc.bc|d",
            """
                var abcBcd = abc.bcd
                var a = abcBcd
                """,
            null)]
        [DataRow("var a = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob|",
            """
                var primaryEndpointsBlob = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob
                var a = primaryEndpointsBlob
                """,
            null)]
        [DataRow("var a = reference(storageAccount.id, '2018-02-01').prim|aryEndpoints.blob",
            """
                var referencePrimaryEndpoints = reference(storageAccount.id, '2018-02-01').primaryEndpoints
                var a = referencePrimaryEndpoints.blob
                """,
            """
                param referencePrimaryEndpoints unknown = reference(storageAccount.id, '2018-02-01').primaryEndpoints
                var a = referencePrimaryEndpoints.blob
                """)]
        [DataRow("var a = a.b.|c.d.e",
            """
                var bC = a.b.c
                var a = bC.d.e
                """,
            null)]
        public async Task PickUpNameFromPropertyAccess_UpToTwoLevels(string fileWithSelection, string ?expectedVariableText, string? expectedParameterText)
        {
            await RunExtractToVariableAndOrParameterTest(fileWithSelection, expectedVariableText, expectedParameterText);
        }

        [DataTestMethod]
        //
        // Closest ancestor expression is the top-level expression itself -> offer to update full expression
        //
        [DataRow(
            "storageUri:| reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            null,
            "storageUri: storageUri"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').primaryEndpoints.|blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            null,
            "storageUri: storageUri"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').primaryEndpoints.<<blo>>b",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            null,
            "storageUri: storageUri"
            )]
        //
        // Cursor is inside the property name -> offer full expression
        //
        [DataRow(
            "storageUri|: reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            null,
            "storageUri: storageUri"
            )]
        [DataRow(
            "<<storageUri: re>>ference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            null,
            "storageUri: storageUri"
            )]
        [DataRow(
            "<<storageUri: reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob>>",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            null,
            "storageUri: storageUri"
            )]
        //
        // Cursor is inside a subexpression -> only offer to extract that specific subexpression
        //
        // ... reference() call
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').|primaryEndpoints.blob",
            "var referencePrimaryEndpoints = reference(storageAccount.id, '2018-02-01').primaryEndpoints",
            null,
            "storageUri: referencePrimaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference|(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            null,
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: refere<<nce(storageAccount.id, '201>>8-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            null,
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        //   ... '2018-02-01'
        [DataRow(
            "storageUri: reference(storageAccount.id, |'2018-02-01').primaryEndpoints.blob",
            "var newVariable = '2018-02-01'",
            null,
            "storageUri: reference(storageAccount.id, newVariable).primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01|').primaryEndpoints.blob",
            "var newVariable = '2018-02-01'",
            null,
            "storageUri: reference(storageAccount.id, newVariable).primaryEndpoints.blob"
            )]
        //   ... storageAccount.id
        [DataRow(
            "storageUri: reference(storageAccount.|id, '2018-02-01').primaryEndpoints.blob",
            "var storageAccountId = storageAccount.id",
            null,
            "storageUri: reference(storageAccountId, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.i|d, '2018-02-01').primaryEndpoints.blob",
            "var storageAccountId = storageAccount.id",
            null,
            "storageUri: reference(storageAccountId, '2018-02-01').primaryEndpoints.blob"
            )]
        // ... storageAccount
        [DataRow(
            "storageUri: reference(storageAc|count.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = storageAccount",
            null,
            "storageUri: reference(newVariable.id, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAc|count.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = storageAccount",
            null,
            "storageUri: reference(newVariable.id, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAc|count.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = storageAccount",
            null,
            "storageUri: reference(newVariable.id, '2018-02-01').primaryEndpoints.blob"
            )]
        // ... inside reference(x, y) but not inside x or y -> closest enclosing expression is the reference()
        [DataRow(
            "storageUri: reference(storageAccount.id,| '2018-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            null,
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01' |).primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            null,
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: reference|(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "var newVariable = reference(storageAccount.id, '2018-02-01')",
            null,
            "storageUri: newVariable.primaryEndpoints.blob"
            )]
        public async Task ShouldExpandSelectedExpressionsInALogicalWay(string lineWithSelection, string? expectedNewVarDeclaration, string? expectedNewParamDeclaration, string expectedModifiedLine)
        {
            await RunExtractToVarAndOrParamOnSingleLineTest(
                inputTemplateWithSelection: """
                    resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                    resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                      properties: {
                        diagnosticsProfile: {
                          bootDiagnostics: {
                            LINEWITHSELECTION
                          }
                        }
                      }
                    }
                    """,
                expectedOutputTemplate: """
                    resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                    EXPECTEDNEWDECLARATION
                    resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                      properties: {
                        diagnosticsProfile: {
                          bootDiagnostics: {
                            EXPECTEDMODIFIEDLINE
                          }
                        }
                      }
                    }
                    """,
                lineWithSelection,
                expectedNewVarDeclaration,
                expectedNewParamDeclaration,
                expectedModifiedLine);
        }

        [DataTestMethod]
        [DataRow(
            "storageUri: reference(stora<<geAccount.i>>d, '2018-02-01').primaryEndpoints.blob",
            "var storageAccountId = storageAccount.id",
            "param storageAccountId string = storageAccount.id",
            "storageUri: reference(storageAccountId, '2018-02-01').primaryEndpoints.blob"
            )]
        [DataRow(
            "storageUri: refer<<ence(storageAccount.id, '2018-02-01').primaryEndpoints.bl>>ob",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "param storageUri unknown = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        [DataRow(
            "storageUri: reference(storageAccount.id, '2018-02-01').primar<<yEndpoints.blob>>",
            "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "param storageUri unknown = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
            "storageUri: storageUri"
            )]
        public async Task IfThereIsASelection_ThenPickUpEverythingInTheSelection_AfterExpanding(string lineWithSelection, string expectedNewVarDeclaration, string expectedNewParamDeclaration, string expectedModifiedLine)
        {
            await RunExtractToVarAndOrParamOnSingleLineTest(
                inputTemplateWithSelection: """
                    resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                    resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                      properties: {
                        diagnosticsProfile: {
                          bootDiagnostics: {
                            LINEWITHSELECTION
                          }
                        }
                      }
                    }
                    """,
                expectedOutputTemplate: """
                    resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }

                    EXPECTEDNEWDECLARATION
                    resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
                      properties: {
                        diagnosticsProfile: {
                          bootDiagnostics: {
                            EXPECTEDMODIFIEDLINE
                          }
                        }
                      }
                    }
                    """,
                lineWithSelection,
                expectedNewVarDeclaration,
                expectedNewParamDeclaration,
                expectedModifiedLine);
        }

        private async Task RunExtractToVarAndOrParamOnSingleLineTest(
            string inputTemplateWithSelection,
            string expectedOutputTemplate,
            string lineWithSelection,
            string? expectedNewVarDeclaration,
            string? expectedNewParamDeclaration,
            string expectedModifiedLine
            )
        {
            await RunExtractToVariableTestIf(
                expectedNewVarDeclaration is { },
                    inputTemplateWithSelection.Replace("LINEWITHSELECTION", lineWithSelection),
                    expectedOutputTemplate.Replace("EXPECTEDNEWDECLARATION", expectedNewVarDeclaration)
                        .Replace("EXPECTEDMODIFIEDLINE", expectedModifiedLine));

            await RunExtractToParameterTestIf(
                expectedNewParamDeclaration is { },
                inputTemplateWithSelection.Replace("LINEWITHSELECTION", lineWithSelection),
                expectedOutputTemplate.Replace("EXPECTEDNEWDECLARATION", expectedNewParamDeclaration)
                    .Replace("EXPECTEDMODIFIEDLINE", expectedModifiedLine));
        }

        //private async Task RunExtractToVariableAndOrParameterTest(string fileWithSelection, string expectedTextTemplate, string? expectedNewVarDeclaration, string? expectedNewParamDeclaration)
        //{
        //    await RunExtractToVariableTestIf(
        //        expectedNewVarDeclaration is { },
        //        fileWithSelection,
        //        expectedTextTemplate.Replace("EXPECTEDNEWDECLARATION", expectedNewVarDeclaration));
        //    await RunExtractToParameterTestIf(
        //        expectedNewParamDeclaration is { },
        //        fileWithSelection,
        //        expectedTextTemplate.Replace("EXPECTEDNEWDECLARATION", expectedNewParamDeclaration));
        //}

        private async Task RunExtractToVariableAndOrParameterTest(string fileWithSelection, string? expectedVariableText, string? expectedParameterText)
        {
            await RunExtractToVariableTestIf(
                expectedVariableText is { },
                fileWithSelection,
                expectedVariableText);
            await RunExtractToParameterTestIf(
                expectedParameterText is { },
                fileWithSelection,
                expectedParameterText);
        }

        private async Task RunExtractToVariableTestIf(bool condition, string fileWithSelection, string? expectedText)
        {
            if (condition)
            {
                using (new AssertionScope("extract to var test"))
                {
                    await RunExtractToVariableTest(fileWithSelection, expectedText);
                }
            }
        }

        private async Task RunExtractToParameterTestIf(bool condition, string fileWithSelection, string? expectedText)
        {
            if (condition)
            {
                using (new AssertionScope("extract to param test"))
                {
                    await RunExtractToParameterTest(fileWithSelection, expectedText);
                }
            }
        }

        private async Task RunExtractToVariableTest(string fileWithSelection, string? expectedText)
        {
            (var codeActions, var bicepFile) = await GetCodeActionsForSyntaxTest(fileWithSelection);
            var extractedVar = codeActions.FirstOrDefault(x => x.Title.StartsWith(ExtractToVariableTitle));

            if (expectedText == null)
            {
                extractedVar.Should().BeNull("should not offer to extract variables");
            }
            else
            {
                extractedVar.Should().NotBeNull("should contain an action to extract to variable");
                extractedVar!.Kind.Should().Be(CodeActionKind.RefactorExtract);

                var updatedFile = ApplyCodeAction(bicepFile, extractedVar);
                updatedFile.Should().HaveSourceText(expectedText);
            }
        }

        private async Task RunExtractToParameterTest(string fileWithSelection, string? expectedText)
        {
            (var codeActions, var bicepFile) = await GetCodeActionsForSyntaxTest(fileWithSelection);
            var extractedParam = codeActions.FirstOrDefault(x => x.Title.StartsWith(ExtractToParameterTitle));

            if (expectedText == null)
            {
                extractedParam.Should().BeNull("should not offer to extract parameters");
            }
            else
            {
                extractedParam.Should().NotBeNull("should contain an action to extract to parameter");
                extractedParam!.Kind.Should().Be(CodeActionKind.RefactorExtract);

                var updatedFile = ApplyCodeAction(bicepFile, extractedParam);
                updatedFile.Should().HaveSourceText(expectedText);
            }
        }
    }
}
