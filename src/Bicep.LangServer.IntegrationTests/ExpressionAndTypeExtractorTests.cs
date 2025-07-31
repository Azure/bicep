// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.PrettyPrintV2;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Refactor;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Humanizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class ExpressionAndTypeExtractorTests : CodeActionTestBase
{
    private const string ExtractToVariableTitle = "Extract variable";
    private const string ExtractToParameterTitle = "Extract parameter";
    private const string ExtractToTypeTitle = "Create user-defined type for ";
    private const string PostExtractionCommandName = "bicep.internal.postExtraction";
    private const string Tab = "\t";

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        """
            type superComplexType = {
                p: string
                i: 123 || 456
            }

            param p { *: superComplexType } = {
                a: <<{ p: 'mystring', i: 123 }>>
            }
            """,
        """
            type superComplexType = {
                p: string
                i: 123 | 456
            }

            param a object = { p: 'mystring', i: 123 }

            param p { *: superComplexType } = {
                a: a
            }
            """,
        """
            type superComplexType = {
                p: string
                i: 123 | 456
            }

            param a { i: 123 | 456, p: string } = { p: 'mystring', i: 123 }

            param p { *: superComplexType } = {
                a: a
            }
            """)]
    [DataRow(
        """
            var blah = |[{foo: 'bar'}, {foo: 'baz'}]
            """,
        """
            param newParameter array = [{ foo: 'bar' }, { foo: 'baz' }]

            var blah = newParameter
            """,
        """
            param newParameter { foo: string }[] = [{ foo: 'bar' }, { foo: 'baz' }]

            var blah = newParameter
            """)]
    [DataRow(
        """
            param p1 { intVal: int} = { intVal:123}
            output o object = <<p1>>
            """,
        """
            param p1 { intVal: int} = { intVal:123}
            param newParameter object = p1
            output o object = newParameter
            """,
        """
            param p1 { intVal: int} = { intVal:123}
            param newParameter { intVal: int } = p1
            output o object = newParameter
            """)]
    [DataRow(
        """
            param p2 'foo' || 'bar'
            param v1 int = |p2
            """,
        """
            param p2 'foo' | 'bar'
            param newParameter string = p2
            param v1 int = newParameter
            """,
        """
            param p2 'foo' | 'bar'
            param newParameter 'bar' | 'foo' = p2
            param v1 int = newParameter
            """)]
    [DataRow(
        """
            param p1 { intVal: int}
            output o = <<p1>>
            """,
        """
            param p1 { intVal: int}
            param newParameter object = p1
            output o = newParameter
            """,
        """
            param p1 { intVal: int}
            param newParameter { intVal: int } = p1
            output o = newParameter
            """)]
    public async Task BicepDiscussion(string fileWithSelection, string expectedLooseParamText, string expectedMediumParamText)
    {
        await RunExtractToParameterTest(fileWithSelection, expectedLooseParamText, expectedMediumParamText, "IGNORE");
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        """
            var a = '|b'
            """,
        """
            var newVariable = 'b'

            var a = newVariable
            """,
        """
            param newParameter string = 'b'

            var a = newParameter
            """,
        null,
        null)]
    [DataRow(
        """
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
            """,
        null,
        null)]
    [DataRow(
        """
            var a = 1 + 2
            var b = '${a}|{a}'
            """,
        """
            var a = 1 + 2
            var newVariable = '${a}{a}'
            var b = newVariable
            """,
        """
            var a = 1 + 2
            param newParameter string = '${a}{a}'
            var b = newParameter
            """,
        null,
        null,
        DisplayName = "Full interpolated string")]
    [DataRow(
        """
            // comment 1
            @secure()
            // comment 2
            param a = '|a'
            """,
        """
            // comment 1
            var newVariable = 'a'

            @secure()
            // comment 2
            param a = newVariable
            """,
        """
            // comment 1
            param newParameter string = 'a'

            @secure()
            // comment 2
            param a = newParameter
            """,
        null,
        null,
        DisplayName = "Preceding lines")]
    [DataRow(
        """
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
        null,
        null,
        DisplayName = "Inside a data structure")]
    [DataRow(
        """
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
            @description('Required. Gets or sets the location of the resource. This will be one of the supported and registered Azure Geo Regions (e.g. West US, East US, Southeast Asia, etc.). The geo region of a resource cannot be changed once it is created, but if an identical geo region is specified on update, the request will succeed.')
            param location string = 'westus'

            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                name: 'name'
                location: location
                kind: 'StorageV2'
                sku: {
                name: 'Premium_LRS'
                }
            }
            """,
        null,
        null)]
    public async Task Basics(string fileWithSelection, string? expectedVarText, string? expectedLooseParamText, string? expectedMediumParamText, string? expectedResourceDerivedParamText)
    {
        await RunExtractToVariableAndParameterTest(fileWithSelection, expectedVarText, expectedLooseParamText, expectedMediumParamText, expectedResourceDerivedParamText);
    }

    ////////////////////////////////////////////////////////////////////

    [DataRow(
       "var v = |null",
       """
            var newVariable = null

            var v = newVariable
            """,
       """
            param newParameter object? /* null */ = null

            var v = newParameter
            """,
       null,
       null)]
    [DataTestMethod]
    public async Task NullType(string fileWithSelection, string? expectedVarText, string? expectedLooseParamText, string? expectedMediumParamText, string? expectedResourceDerivedParamText)
    {
        await RunExtractToVariableAndParameterTest(fileWithSelection, expectedVarText, expectedLooseParamText, expectedMediumParamText, expectedResourceDerivedParamText);
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        """
            var a = '|b'
            """,
        """
            param newParameter string = 'b'

            var a = newParameter
            """,
        null // no second option
        )]
    [DataRow(
        """
            var a = |{a: 'b'}
            """,
        """
            param newParameter object = { a: 'b' }

            var a = newParameter
            """,
        """
            param newParameter { a: string } = { a: 'b' }

            var a = newParameter
            """)]
    public async Task ShouldOfferTwoParameterExtractions_IffTheExtractedTypesAreDifferent(string fileWithSelection, string? expectedLooseParamText, string? expectedMediumParamText)
    {
        await RunExtractToParameterTest(fileWithSelection, expectedLooseParamText, expectedMediumParamText, "IGNORE");
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        """
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
    [DataRow(
        """
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
            var id7 = 'gatewayId'
            param id2 string = 'hello'
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

    ////////////////////////////////////////////////////////////////////

    [DataRow(
        "var v = {| 'An*a#and a\\'B': 'a b' }",
        """
            param newParameter object = { 'An*a#and a\'B': 'a b' }

            var v = newParameter
            """)]
    [DataRow(
        "var v = { 'An a and 1 B': 'a| b' }",
        """
            param An_a_and_1_B string = 'a b'

            var v = { 'An a and 1 B': An_a_and_1_B }
            """)]
    [DataRow(
        "var v = { 'A*b#and 1\\'C ': 'b| c' }",
        """
            param A_b_and_1_C_ string = 'b c'

            var v = { 'A*b#and 1\'C ': A_b_and_1_C_ }
            """)]
    [DataRow(
        "var v = { '': |'empty' }",
        """
            param _ string = 'empty'

            var v = { '': _ }
            """)]
    [DataRow(
        "var v = { '  ': |'spaces' }",
        """
            param ___ string = 'spaces'

            var v = { '  ': ___ }
            """)]
    [DataRow(
        """
            var ___ = 1
            var ___2 = 2
            var v = { '  ': |'spaces' }
            """,
        """
            var ___ = 1
            var ___2 = 2
            param ___3 string = 'spaces'
            var v = { '  ': ___3 }
            """)]
    [DataRow(
        "var v = { '99': '|Luftballoons' }",
        """
            param _99 string = 'Luftballoons'

            var v = { '99': _99 }
            """)]
    [DataTestMethod]
    public async Task WeirdNames(string fileWithSelection, string expectedText)
    {
        await RunExtractToParameterTest(fileWithSelection, expectedText, "IGNORE", "IGNORE");
    }

    ////////////////////////////////////////////////////////////////////

    [TestMethod]
    public async Task ShouldHandleArrays()
    {
        await RunExtractToVariableAndParameterTest(
            """
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
                """,
            """
                param newParameter int[] = [1, 2, 3]

                resource subnets 'Microsoft.Network/virtualNetworks/subnets@2024-01-01' = [
                  for (item, index) in newParameter: {
                    name: 'subnet${index}'
                    properties: {
                      addressPrefix: '10.0.${index}.0/24'
                    }
                  }
                ]
                """,
            null);
    }

    ////////////////////////////////////////////////////////////////////

    [TestMethod]
    public async Task ShouldHandleObjects()
    {
        await RunExtractToVariableAndParameterTest("""
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
            @description('Describes the properties of a Virtual Machine Extension.')
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
            """,
        """
            param _artifactsLocation string
            param  _artifactsLocationSasToken string
            @description('Describes the properties of a Virtual Machine Extension.')
            param properties {
              autoUpgradeMinorVersion: bool?
              forceUpdateTag: string?
              instanceView: {
                name: string?
                statuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                substatuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                type: string?
                typeHandlerVersion: string?
              }?
              protectedSettings: object? /* any */
              publisher: string?
              settings: object? /* any */
              type: string?
              typeHandlerVersion: string?
            } = {
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
            @description('Describes the properties of a Virtual Machine Extension.')
            param properties resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties = {
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

    ////////////////////////////////////////////////////////////////////

    [DataRow(
        """
            var i = <<1>>
            """,
        """
            param newParameter int = 1

            var i = newParameter
            """,
        null,
        null,
        DisplayName = "Literal integer")]
    [DataRow(
        """
            param i int = 1
            var j = <<i>> + 1
            """,
        """
            param i int = 1
            param newParameter int = i
            var j = newParameter + 1
            """,
        null,
        null,
        DisplayName = "int parameter reference")]
    [DataRow(
        """
            param i int = 1
            var j = <<i + 1>>
            """,
        """
            param i int = 1
            param newParameter int = i + 1
            var j = newParameter
            """,
        null,
        null,
        DisplayName = "int expression with param")]
    [DataRow(
        """
            param i string
            var j = <<concat(i, i)>>
            """,
        """
            param i string
            param newParameter string = concat(i, i)
            var j = newParameter
            """,
        null,
        null,
        DisplayName = "strings concatenated")]
    [DataRow(
        """
            param i string = 'i'
            var i2 = 'i2'
            var j = <<'{i}{i2}'>>
            """,
        """
            param i string = 'i'
            param newParameter string = '{i}{i2}'
            var i2 = 'i2'
            var j = newParameter
            """,
        null,
        null,
        DisplayName = "strings concatenated")]
    [DataRow(
        """
            var p = <<[ 1, 2, 3 ]>>
            """,
        """
            param newParameter array = [1, 2, 3]

            var p = newParameter
            """,
        """
            param newParameter int[] = [1, 2, 3]

            var p = newParameter
            """,
        null,
        DisplayName = "array literal")]
    [DataRow(
        """
            var p = <<{ a: 1, b: 'b' }>>
            """,
        """
            param newParameter object = { a: 1, b: 'b' }

            var p = newParameter
            """,
        """
            param newParameter { a: int, b: string } = { a: 1, b: 'b' }

            var p = newParameter
            """,
        null,
        DisplayName = "object literal with literal types")]
    [DataRow(
        """
            var p = { a: <<1>>, b: 'b' }
            """,
        """
            param a int = 1

            var p = { a: a, b: 'b' }
            """,
        null,
        null,
        DisplayName = "property value from object literal")]
    [DataRow(
        """
            var o1 = { a: 1, b: 'b' }
            var a = <<o1.a>>
            """,
        """
            var o1 = { a: 1, b: 'b' }
            param o1A int = o1.a
            var a = o1A
            """,
        null,
        null,
        DisplayName = "referenced property value from object literal")]
    [DataRow(
        """
            param p 'a'||'b' = 'a'
            var v = <<p>>
            """,
        """
            param p 'a'|'b' = 'a'
            param newParameter string = p
            var v = newParameter
            """,
        """
            param p 'a'|'b' = 'a'
            param newParameter 'a' | 'b' = p
            var v = newParameter
            """,
        null,
        DisplayName = "string literal union")]
    [DataRow(
        """
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
        """
            var a = {
                int: 1
            }
            param newParameter { int: int } = a
            var b = newParameter.int
            """,
        null,
        DisplayName = "object properties")]
    [DataRow(
        """
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
            param newParameter object = p
            var v = newParameter.o.i2
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
        null,
        DisplayName = "custom object type, whole object")]
    [DataRow(
        """
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
            param pO object = p.o
            var v = pO.i2
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
        null,
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
        null,
        null,
        DisplayName = "resource types undefined 1")]
    [DataRow(
        """
            param p1 'abc'||'def'
            resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
                unknownProperty: |p1
            }
            """,
        """
            param p1 'abc'|'def'
            param unknownProperty string = p1
            resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
                unknownProperty: unknownProperty
            }
            """,
        """
            param p1 'abc'|'def'
            param unknownProperty 'abc' | 'def' = p1
            resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
                unknownProperty: unknownProperty
            }
            """,
        null,
        DisplayName = "resource properties unknown property, follows expression's inferred type")]
    [DataRow("""
            var foo = <<{ intVal: 2 }>>
            """,
        """
            param newParameter object = { intVal: 2 }

            var foo = newParameter
            """,
        """
            param newParameter { intVal: int } = { intVal: 2 }

            var foo = newParameter
            """,
        null)]
    [DataRow(
        """
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
            @description('The resource name')
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
        null,
        null,
        DisplayName = "resource types - string property")]
    [DataRow(
        """
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
            @description('The SKU name. Required for account creation; optional for update. Note that in older versions, SKU name was called accountType.')
            param name string = 'Premium_LRS'

            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                name: 'name'
                location: 'location'
                kind: 'StorageV2'
                sku: {
                    name: name
                }
            }
            """,
        """
            @description('The SKU name. Required for account creation; optional for update. Note that in older versions, SKU name was called accountType.')
            param name string /* 'Premium_LRS' | 'Premium_ZRS' | 'Standard_GRS' | 'Standard_GZRS' | 'Standard_LRS' | 'Standard_RAGRS' | 'Standard_RAGZRS' | 'Standard_ZRS' | string */ = 'Premium_LRS'

            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                name: 'name'
                location: 'location'
                kind: 'StorageV2'
                sku: {
                    name: name
                }
            }
            """,
        null,
        DisplayName = "resource properties - string union")]
    [DataRow(
        """
            param p int?
            var v = |p
            """,
        """
            param p int?
            param newParameter int = p
            var v = newParameter
            """,
        null,
        null,
        DisplayName = "nullable types")]
    [DataRow(
        """
            param whoops int = 'not an int'
            var v = <<p + 1>>
            """,
        """
            param whoops int = 'not an int'
            param newParameter object? /* unknown */ = p + 1
            var v = newParameter
            """,
        null,
        null,
        DisplayName = "error types")]
    [DataRow(
        """
            param p1 { a: { b: string } }
            var v = |p1
            """,
        """
            param p1 { a: { b: string } }
            param newParameter object = p1
            var v = newParameter
            """,
        """
            param p1 { a: { b: string } }
            param newParameter { a: { b: string } } = p1
            var v = newParameter
            """,
        null)]
    [DataTestMethod]
    public async Task Params_InferType(string fileWithSelection, string expectedMediumParameterText, string expectedStrictParameterText, string? expectedResourceDerivedParameterText)
    {
        await RunExtractToParameterTest(fileWithSelection, expectedMediumParameterText, expectedStrictParameterText, expectedResourceDerivedParameterText);
    }

    ////////////////////////////////////////////////////////////////////

    [TestMethod]
    public async Task IfJustPropertyNameSelected_ThenExtractPropertyValue()
    {
        await RunExtractToParameterTest("""
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
            @description('Json formatted public settings for the extension.')
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
            """,
        "IGNORE",
        "IGNORE");
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        """
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
        DisplayName = "Full property value as array, pick up property name")]
    public async Task ShouldPickUpPropertyName_ButOnlyIfFullPropertyValue(string fileWithSelection, string? expectedVarText)
    {
        await RunExtractToVariableTest(fileWithSelection, expectedVarText);
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        "var a = resourceGroup().locati|on",
        """
            var resourceGroupLocation = resourceGroup().location

            var a = resourceGroupLocation
            """)]
    [DataRow(
        "var a = abc|().bcd",
        """
            var newVariable = abc()

            var a = newVariable.bcd
            """)]
    [DataRow(
        "var a = abc.bcd.|def",
        """
            var bcdDef = abc.bcd.def

            var a = bcdDef
            """)]
    [DataRow(
        "var a = abc.b|cd",
        """
            var abcBcd = abc.bcd

            var a = abcBcd
            """)]
    [DataRow(
        "var a = abc.bc|d",
        """
            var abcBcd = abc.bcd

            var a = abcBcd
            """)]
    [DataRow(
        "var a = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob|",
        """
            var primaryEndpointsBlob = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob

            var a = primaryEndpointsBlob
            """)]
    [DataRow(
        "var a = reference(storageAccount.id, '2018-02-01').prim|aryEndpoints.blob",
        """
            var referencePrimaryEndpoints = reference(storageAccount.id, '2018-02-01').primaryEndpoints

            var a = referencePrimaryEndpoints.blob
            """)]
    [DataRow(
        "var a = a.b.|c.d.e",
        """
            var bC = a.b.c

            var a = bC.d.e
            """)]
    // CONSIDER: ideally this would work
    //[DataRow(
    //    """
    //        resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
    //          |name: '${storagePrefix}${uniqueString(resourceGroup().id)}'
    //        }
    //        """,
    //    """
    //        var stgName = '${storagePrefix}${uniqueString(resourceGroup().id)}'
    //        resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
    //          name: stgName
    //        }
    //        """)]
    public async Task ShouldPickUpNameFromPropertyAccess_UpToTwoLevels(string fileWithSelection, string? expectedVariableText)
    {
        await RunExtractToVariableTest(fileWithSelection, expectedVariableText);
    }

    [DataRow(
        """
            var a = 1 + |2
            """,
        """
            var newVariable = 2

            var a = 1 + newVariable
            """)]
    [DataRow(
        """
            var a = <<1 + 2>>
            """,
        """
            var newVariable = 1 + 2

            var a = newVariable
            """)]
    [DataRow(
        """
            var a = <<1 +>> 2
            """,
        """
            var newVariable = 1 + 2

            var a = newVariable
            """)]
    [DataRow(
        """
            var a = 1 |+ 2
            """,
        """
            var newVariable = 1 + 2

            var a = newVariable
            """)]
    [DataRow(
        "var a = |1+2",
        """
            var newVariable = 1

            var a = newVariable+2
            """)]

    //// TODO: what's expected behavior?
    //[DataRow(
    //    "var a = 1|+2",
    //    """
    //        var newVariable = 1+2
    //
    //        var a = newVariable
    //        """)]
    //[DataRow(
    //    "var a = 1| +2",
    //    """
    //        var newVariable = 1+2
    //
    //        var a = newVariable
    //        """)]
    //[DataRow(
    //    "var a = 1 | +2",
    //    """
    //        var newVariable = 1+2
    //
    //        var a = newVariable
    //        """)]
    //[DataRow(
    //    "var a = 1 |+2",
    //    """
    //        var newVariable = 1+2
    //
    //        var a = newVariable
    //        """)]

    // TODO: BUG: should be picking up just "2"
    //[DataRow(
    //    "var a = 1+|2",
    //    """
    //        var newVariable = 2
    //
    //        var a = 1+newVariable
    //        """")]
    // TODO: BUG: should be picking up just 2
    //[DataRow(
    //    "var a = 1+<<2>>",
    //    """
    //        var newVariable = 2
    //
    //        var a = 1+newVariable
    //        """)]

    [DataRow(
        "var a = <<1>>+2",
        """
            var newVariable = 1

            var a = newVariable+2
            """)]

    // TODO: bug
    //[DataRow(
    //    "var a = 1+<<2>>",
    //    """
    //        var newVariable = 2
    //
    //        var a = 1+newVariable
    //        """)]
    //[DataRow(
    //    "var a = << 1 >>+2",
    //    """
    //        var newVariable = 1
    //
    //        var a = newVariable+2
    //        """)]
    //[DataRow(
    //    "var a = 1+<< 2>>",
    //    """
    //        var newVariable = 2
    //
    //        var a = 1+newVariable
    //        """)]
    [DataRow(
        "var a = 1+2|",
        """
            var newVariable = 2

            var a = 1+newVariable
            """)]

    [DataRow(
        "var a = <<1+2>>",
        """
            var newVariable = 1 + 2

            var a = newVariable
            """)]
    [DataRow(
        "var a = 1<<+2>>",
        """
            var newVariable = 1 + 2

            var a = newVariable
            """)]
    [DataRow(
        """
            var a = 1 <<+ 2 + 3 >>+ 4
            """,
        """
            var newVariable = 1 + 2 + 3 + 4

            var a = newVariable
            """)]
    [DataRow(
        """
            param p1 int = 1 + |2
            """,
        """
            var newVariable = 2

            param p1 int = 1 + newVariable
            """)]
    // TODO: bug
    //[DataRow(
    //    "var blah1 = [<<{ foo: 'bar' }>>, { foo: 'baz' }]",
    //    """
    //        var newVariable = { foo: 'bar' }
    //
    //        var blah1 = [newVariable, { foo: 'baz' }]",
    //        """
    //    )]
    [DataTestMethod]
    public async Task ShouldExpandSelectedExpressionsInALogicalWay_Expressions(string lineWithSelection, string expectedNewVarDeclaration)
    {
        await RunExtractToVariableTest(lineWithSelection, expectedNewVarDeclaration);
    }

    ////////////////////////////////////////////////////////////////////

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
    public async Task ShouldExpandSelectedExpressionsInALogicalWay_ObjectProperties(string lineWithSelection, string expectedNewVarDeclaration, string expectedModifiedLine)
    {
        await RunExtractToVarSingleLineTest(
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
            expectedModifiedLine);
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        "storageUri: reference(stora<<geAccount.i>>d, '2018-02-01').primaryEndpoints.blob",
        "var storageAccountId = storageAccount.id",
        "param storageAccountId string = storageAccount.id",
        null,
        "storageUri: reference(storageAccountId, '2018-02-01').primaryEndpoints.blob"
        )]
    [DataRow(
        "storageUri: refer<<ence(storageAccount.id, '2018-02-01').primaryEndpoints.bl>>ob",
        "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
        """
            @description('Uri of the storage account to use for placing the console output and screenshot.')
            param storageUri object? /* unknown */ = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob
            """,
        null,
        "storageUri: storageUri"
        )]
    [DataRow(
        "storageUri: reference(storageAccount.id, '2018-02-01').primar<<yEndpoints.blob>>",
        "var storageUri = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob",
        """
            @description('Uri of the storage account to use for placing the console output and screenshot.')
            param storageUri object? /* unknown */ = reference(storageAccount.id, '2018-02-01').primaryEndpoints.blob
            """,
        null,
        "storageUri: storageUri"
        )]
    public async Task IfThereIsASelection_ThenPickUpEverythingInTheSelection_AfterExpanding(string lineWithSelection, string expectedNewVarDeclaration, string expectedNewParamLooseDeclaration, string expectedNewParamMediumDeclaration, string expectedModifiedLine)
    {
        await RunExtractToVarAndParamOnSingleLineTest(
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
            expectedNewParamLooseDeclaration,
            expectedNewParamMediumDeclaration,
            expectedModifiedLine);
    }

    ////////////////////////////////////////////////////////////////////

    [DataRow(
        """
            var storagePrefix = ''
            resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
              name: '${storagePrefix}${u<<niqueString(resourceGroup().id)>>}'
            }
            """,
        """
            var storagePrefix = ''
            var newVariable = uniqueString(resourceGroup().id)
            resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
              name: '${storagePrefix}${newVariable}'
            }
            """)]
    // TODO: bug
    //[DataRow(
    //    """
    //        var storagePrefix = ''
    //        resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
    //          name: '${storagePrefix}${<<uniqueString(resourceGroup().id)>>}'
    //        }
    //        """,
    //    """
    //        var storagePrefix = ''
    //        var newVariable = uniqueString(resourceGroup().id)
    //        resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
    //          name: '${storagePrefix}${newVariable}'
    //        }
    //        """)]
    //[DataRow(
    //    """
    //        var storagePrefix = ''
    //        resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
    //          name: '${storagePrefix}${|uniqueString(resourceGroup().id)}'
    //        }
    //        """,
    //    """
    //        var storagePrefix = ''
    //        var newVariable = uniqueString(resourceGroup().id)
    //        resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
    //          name: '${storagePrefix}${newVariable}'
    //        }
    //        """)]
    [DataTestMethod]
    public async Task IfThereIsASelection_ThenPickUpEverythingInTheSelection_AfterExpanding_StringExtrapolation(string fileWithSelection, string expectedVariableText)
    {
        await RunExtractToVariableTest(fileWithSelection, expectedVariableText);
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        """
            // My comment here
            resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
                name: 'testResource/cassandraKeyspace'
                properties: {
                resource: {
                    id: 'id'
                }
                <<options>>: {}
                }
            }
            """,
        "IGNORE",
        """
            // My comment here
            @description('A key-value pair of options to be applied for the request. This corresponds to the headers sent with the request.')
            param options { autoscaleSettings: { maxThroughput: int? }?, throughput: int? } = {}

            resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
                name: 'testResource/cassandraKeyspace'
                properties: {
                resource: {
                    id: 'id'
                }
                options: options
                }
            }
            """,
        """
            // My comment here
            @description('A key-value pair of options to be applied for the request. This corresponds to the headers sent with the request.')
            param options resourceInput<'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15'>.properties.options = {}

            resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
                name: 'testResource/cassandraKeyspace'
                properties: {
                resource: {
                    id: 'id'
                }
                options: options
                }
            }
            """,
        DisplayName = "Resource property description")]
    [DataRow(
        """
            type t = {
                @description('My string\'s field')
                myString: string

                @description('''
            My int's field
            is very long
            ''')
                myInt: int
            }

            param p t = {
                myString: |'hello'
                myInt: 42
            }
            """,
        """
            type t = {
                @description('My string\'s field')
                myString: string

                @description('''
            My int's field
            is very long
            ''')
                myInt: int
            }

            @description('My string\'s field')
            param myString string = 'hello'

            param p t = {
                myString: myString
                myInt: 42
            }
            """,
        null,
        null,
        DisplayName = "Apostrophe in description")]
    [DataRow(
        """
            type t = {
                @description('My string\'s field')
                myString: string

                @description('''
            My int's field
            is very long
            ''')
                myInt: int
            }

            param p t = {
                myString: 'hello'
                myInt: |42
            }
            """,
        """
            type t = {
                @description('My string\'s field')
                myString: string

                @description('''
            My int's field
            is very long
            ''')
                myInt: int
            }

            @description('My int\'s field\nis very long\n')
            param myInt int = 42

            param p t = {
                myString: 'hello'
                myInt: myInt
            }
            """,
        null,
        null,
        DisplayName = "multiline description")]
    public async Task Params_ShouldPickUpDescriptions(string fileWithSelection, string expectedLooseParamText, string? expectedMediumParamText, string? expectedResourceDerivedParamText)
    {
        await RunExtractToParameterTest(fileWithSelection, expectedLooseParamText, expectedMediumParamText, expectedResourceDerivedParamText);
    }

    ////////////////////////////////////////////////////////////////////

    [DataTestMethod]
    [DataRow(
        """
            var v = <<1>>
            """,
        """
            var newVariable = 1

            var v = newVariable
            """,
        """
            param newParameter int = 1

            var v = newParameter
            """,
        DisplayName = "Extracting at top of file -> insert at top")]
    [DataRow(
        """
            @secure()
            var v1 = [
                1
                2
            ]
            @secure()
            param p1 array = [
                1
                2
            ]
            var v = |1
            """,
        """
            @secure()
            var v1 = [
                1
                2
            ]
            var newVariable = 1
            @secure()
            param p1 array = [
                1
                2
            ]
            var v = newVariable
            """,
        """
            @secure()
            var v1 = [
                1
                2
            ]
            @secure()
            param p1 array = [
                1
                2
            ]
            param newParameter int = 1
            var v = newParameter
            """,
        DisplayName = "Handle multi-line existing declarations")]
    [DataRow(
        """
            metadata firstLine = 'first line'
            metadata secondLine = 'second line'

            // Some comments
            var v = <<1>>
            """,
        """
            metadata firstLine = 'first line'
            metadata secondLine = 'second line'

            // Some comments
            var newVariable = 1

            var v = newVariable
            """,
        """
            metadata firstLine = 'first line'
            metadata secondLine = 'second line'

            // Some comments
            param newParameter int = 1

            var v = newParameter
            """,
        DisplayName = "No existing params/vars above -> insert right before extraction line")]
    [DataRow(
        """
            param location string
            param resourceGroup string
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2

            metadata line = 'line'

            var v = <<1>>
            """,
        """
            param location string
            param resourceGroup string
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2
            var newVariable = 1

            metadata line = 'line'

            var v = newVariable
            """,
        """
            param location string
            param resourceGroup string
            param newParameter int = 1
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2

            metadata line = 'line'

            var v = newParameter
            """,
        DisplayName = "Existing params and vars at top of file -> param and var inserted after their corresponding existing declarations")]
    [DataRow(
        """
            // location comment
            param location string
            // rg comment
            param resourceGroup string
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2

            metadata line = 'line'

            var v = <<1>>
            """,
        """
            // location comment
            param location string
            // rg comment
            param resourceGroup string
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2
            var newVariable = 1

            metadata line = 'line'

            var v = newVariable
            """,
        """
            // location comment
            param location string
            // rg comment
            param resourceGroup string
            param newParameter int = 1
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2

            metadata line = 'line'

            var v = newParameter
            """,
        DisplayName = "Existing params and vars at top of file -> param and var inserted after their corresponding existing declarations")]
    [DataRow(
        """
            param location string
            param resourceGroup string
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2

            metadata line = 'line'

            param location2 string
            param resourceGroup2 string
            var simpleCalculation2 = 1 + 1
            var complexCalculation2 = simpleCalculation * 2

            metadata line2 = 'line2'

            var v = <<1>>
            """,
        """
            param location string
            param resourceGroup string
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2

            metadata line = 'line'

            param location2 string
            param resourceGroup2 string
            var simpleCalculation2 = 1 + 1
            var complexCalculation2 = simpleCalculation * 2
            var newVariable = 1

            metadata line2 = 'line2'

            var v = newVariable
            """,
        """
            param location string
            param resourceGroup string
            var simpleCalculation = 1 + 1
            var complexCalculation = simpleCalculation * 2

            metadata line = 'line'

            param location2 string
            param resourceGroup2 string
            param newParameter int = 1
            var simpleCalculation2 = 1 + 1
            var complexCalculation2 = simpleCalculation * 2

            metadata line2 = 'line2'

            var v = newParameter
            """,
        DisplayName = "Existing params and vars in multiple places in file -> insert after closest existing declarations above extraction")]
    [DataRow(
        """
            param location string

            resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: location
            }

            resource windowsVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
              parent: virtualMachine
              name: 'name'
              location: location
              properties: {
                publisher: 'Microsoft.Compute'
                type: 'CustomScriptExtension'
                typeHandlerVersion: '1.10'
                autoUpgradeMinorVersion: true
                settings: {
                  fileUris: [
                    'fileUris'
                  ]
                }
                <<protectedSettings>>: {
                  commandToExecute: 'loadTextContent(\'files/my script.ps1\')'
                }
              }
            }
            """,
        "IGNORE",
        """
            param location string

            @description('The extension can contain either protectedSettings or protectedSettingsFromKeyVault or no protected settings at all.')
            param protectedSettings object = {
              commandToExecute: 'loadTextContent(\'files/my script.ps1\')'
            }

            resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: location
            }

            resource windowsVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
              parent: virtualMachine
              name: 'name'
              location: location
              properties: {
                publisher: 'Microsoft.Compute'
                type: 'CustomScriptExtension'
                typeHandlerVersion: '1.10'
                autoUpgradeMinorVersion: true
                settings: {
                  fileUris: [
                    'fileUris'
                  ]
                }
                protectedSettings: protectedSettings
              }
            }
            """,
        DisplayName = "get the rename position correct")]
    public async Task InsertAfterExistingDeclarations(string fileWithSelection, string expectedVarText, string? expectedParamText)
    {
        await RunExtractToVariableAndParameterTest(fileWithSelection.ReplaceNewlines("\n"), expectedVarText, expectedParamText, "IGNORE", "IGNORE");
        await RunExtractToVariableAndParameterTest(fileWithSelection.ReplaceNewlines("\r\n"), expectedVarText, expectedParamText, "IGNORE", "IGNORE");
    }

    [DataRow(
        """
            metadata hello = true
            // Hello
            @description('hello')
            /*
              there
            */
            @export()
            @allowed([1, 2])
            param p int = <<1>>
            """,
        "IGNORE",
        """
            metadata hello = true
            // Hello
            param newParameter int = 1
            @description('hello')
            /*
              there
            */
            @export()
            @allowed([1, 2])
            param p int = newParameter
            """)]
    [TestMethod]
    public async Task VarsAndParams_ShouldInsertBeforeStatementTrivia(string fileWithSelection, string expectedVarText, string? expectedParamText)
    {
        await RunExtractToVariableAndParameterTest(fileWithSelection.ReplaceNewlines("\n"), expectedVarText, expectedParamText, "IGNORE", "IGNORE");
        await RunExtractToVariableAndParameterTest(fileWithSelection.ReplaceNewlines("\r\n"), expectedVarText, expectedParamText, "IGNORE", "IGNORE");
    }

    // We try to imitate the behavior of the existing declarations when inserting new declarations.
    // Logic matrix:
    //   Has an existing declaration
    //   Has a newline before the existing declaration (or at beginning of file)
    //   Has a newline after the existing declaration (or at end of file)
    //   Is there already a newline after the new declaration?
    //
    // ==== No existing declaration
    [DataRow(
        """
            output o = <<1>>
            """,
        """
            var newVariable = 1

            output o = newVariable
            """)]
    [DataRow(
        """

            output o = <<2>>
            """,
        """

            var newVariable = 2

            output o = newVariable
            """)]
    [DataRow(
        """


            output o = <<3>>
            """,
        """


            var newVariable = 3

            output o = newVariable
            """)]
    [DataRow(
        """
            param o1 object = { a: 1, b: 'b' }
            var a = <<o1.a>>
            """,
        """
            param o1 object = { a: 1, b: 'b' }
            var o1A = o1.a
            var a = o1A
            """)]
    [DataRow(
        """
            param o1 object = { a: 1, b: 'b' }

            param a int = <<o1.a>>
            """,
        """
            param o1 object = { a: 1, b: 'b' }

            var o1A = o1.a

            param a int = o1A
            """)]
    [DataRow(
        """
            param o1 object = { a: 1, b: 'b' }
            param a int = <<o1.a>>
            """,
        """
            param o1 object = { a: 1, b: 'b' }
            var o1A = o1.a
            param a int = o1A
            """)]
    // ==== Single existing declaration
    [DataRow(
        """
            var v1 = 1
            output o = <<10>>
            """,
        """
            var v1 = 1
            var newVariable = 10
            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1

            output o = <<11>>
            """,
        """
            var v1 = 1

            var newVariable = 11

            output o = newVariable
            """)]
    [DataRow(
        """

            var v1 = 1

            output o = <<12>>
            """,
        """

            var v1 = 1

            var newVariable = 12

            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1
            output o = <<13>>
            """,
        """
            var v1 = 1
            var newVariable = 13
            output o = newVariable
            """)]
    [DataRow(
        """

            var v1 = 1

            output o = <<14>>
            """,
        """

            var v1 = 1

            var newVariable = 14

            output o = newVariable
            """)]
    [DataRow(
        """

            var v1 = 1
            output o = <<15>>
            """,
        """

            var v1 = 1

            var newVariable = 15
            output o = newVariable
            """)]
    // ==== Multiple existing declarations
    [DataRow(
        """
            var v1 = 1
            var v2 = 2
            output o = <<20>>
            """,
        """
            var v1 = 1
            var v2 = 2
            var newVariable = 20
            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1

            var v2 = 2
            output o = <<21>>
            """,
        """
            var v1 = 1

            var v2 = 2

            var newVariable = 21
            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1

            var v2 = 2

            output o = <<22>>
            """,
        """
            var v1 = 1

            var v2 = 2

            var newVariable = 22

            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1

            var v2 = 2


            output o = <<23>>
            """,
        """
            var v1 = 1

            var v2 = 2

            var newVariable = 23


            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1


            var v2 = 2



            output o = <<24>>
            """,
        """
            var v1 = 1


            var v2 = 2

            var newVariable = 24



            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1


            var v2 = 2
            var v3 = 3



            output o = <<25>>
            """,
        """
            var v1 = 1


            var v2 = 2
            var v3 = 3
            var newVariable = 25



            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1


            var v2 = 2



            var v3 = 3



            output o = <<26>>
            """,
        """
            var v1 = 1


            var v2 = 2



            var v3 = 3

            var newVariable = 26



            output o = newVariable
            """)]
    // ==== Leading and trailing trivia
    [DataRow(
        """
            var v1 = 1
            // comment
            @description('v1')
            /* comment

            */
            var v2 = [
                1,
                2,
                3
            ]
            // comment
            output o = <<30>>
            """,
        """
            var v1 = 1
            // comment
            @description('v1')
            /* comment

            */
            var v2 = [
                1,
                2,
                3
            ]
            var newVariable = 30
            // comment
            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1

            // comment
            @description('v1')
            /* comment

            */
            var v2 = [
                1,
                2,
                3
            ]

            // comment
            output o = <<31>>
            """,
        """
            var v1 = 1

            // comment
            @description('v1')
            /* comment

            */
            var v2 = [
                1,
                2,
                3
            ]

            var newVariable = 31

            // comment
            output o = newVariable
            """)]
    [DataRow(
        """
            var v1 = 1

            // comment
            @description('v1')
            /* comment

            */
            var v2 = [
                1,
                2,
                3
            ]
            // comment
            output o = <<32>>
            """,
        """
            var v1 = 1

            // comment
            @description('v1')
            /* comment

            */
            var v2 = [
                1,
                2,
                3
            ]

            var newVariable = 32
            // comment
            output o = newVariable
            """)]
    [TestMethod]
    public async Task ImitateExistingBlankLineStyle(string fileWithSelection, string expectedVarText)
    {
        await RunExtractToVariableTest(fileWithSelection, expectedVarText);
    }

    [TestMethod]
    public async Task Params_StrictTyping_ShouldHandleEntireResource()
    {
        await RunExtractToParameterTest(
            """
                resource windowsVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = |{
                  parent: virtualMachine
                  name: 'name'
                  location: location
                  properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.10'
                    autoUpgradeMinorVersion: true
                    settings: {
                      fileUris: [
                        'fileUris'
                      ]
                    }
                    protectedSettings: {
                      commandToExecute: 'loadTextContent(\'files/my script.ps1\')'
                    }
                  }
                }
                """,
            "IGNORE",
            // TODO: dependsOn shouldn't be required and others... The specified "object" declaration is missing the following required properties: "asserts", "dependsOn", "eTag", "extendedLocation", "identity", "kind", "managedBy", "managedByExtended", "plan", "scale", "sku", "zones"
            //    why not same as ResourcePropertyTypes in TypeStringifierTests?
            """
                param newParameter {
                  asserts: object
                  dependsOn: (object /* module[] | (resource | module) | resource[] */)[]
                  eTag: string
                  extendedLocation: {
                    name: string
                    type: string /* 'ArcZone' | 'CustomLocation' | 'EdgeZone' | 'NotSpecified' | string */
                  }
                  identity: {
                    delegatedResources: object
                    identityIds: string[]
                    principalId: string
                    tenantId: string
                    type: string /* 'Actor' | 'None' | 'NotSpecified' | 'SystemAssigned' | 'UserAssigned' | string */
                    userAssignedIdentities: object
                  }
                  kind: string
                  location: string
                  managedBy: string
                  managedByExtended: string[]
                  name: string
                  parent: object? /* Microsoft.Compute/virtualMachines */
                  plan: object
                  properties: {
                    autoUpgradeMinorVersion: bool?
                    enableAutomaticUpgrade: bool?
                    forceUpdateTag: string?
                    instanceView: {
                      name: string?
                      statuses: {
                        code: string?
                        displayStatus: string?
                        level: ('Error' | 'Info' | 'Warning')?
                        message: string?
                        time: string?
                      }[]?
                      substatuses: {
                        code: string?
                        displayStatus: string?
                        level: ('Error' | 'Info' | 'Warning')?
                        message: string?
                        time: string?
                      }[]?
                      type: string?
                      typeHandlerVersion: string?
                    }?
                    protectedSettings: object? /* any */
                    publisher: string?
                    settings: object? /* any */
                    type: string?
                    typeHandlerVersion: string?
                  }?
                  scale: { capacity: int, maximum: int, minimum: int }
                  sku: { capacity: int, family: string, model: string, name: string, size: string, tier: string }
                  tags: object?
                  zones: string[]
                } = {
                  parent: virtualMachine
                  name: 'name'
                  location: location
                  properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.10'
                    autoUpgradeMinorVersion: true
                    settings: {
                      fileUris: [
                        'fileUris'
                      ]
                    }
                    protectedSettings: {
                      commandToExecute: 'loadTextContent(\'files/my script.ps1\')'
                    }
                  }
                }

                resource windowsVMExtensions 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = newParameter
                """,
            null // resource-derived types not allowed for the entire resource
        );
    }

    [TestMethod]
    public async Task ObjectTypeWithOnlyReadonlyProperties_ShouldBeObject_NotEmptyCurlyBraces()
    {
        // verify propertiesPrimaryEndpoints is typed as "object", not "{}"
        await RunExtractToParameterTest(
            """
                resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
                  name: '${storagePrefix}${uniqueString(resourceGroup().id)}'
                  location: location
                  sku: {
                    name: storageSKU
                  }
                  kind: 'StorageV2'
                  properties: {
                    supportsHttpsTrafficOnly: true
                  }
                }

                output storageEndpoint object = stg.properties.|primaryEndpoints
                """,
            """
                resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
                  name: '${storagePrefix}${uniqueString(resourceGroup().id)}'
                  location: location
                  sku: {
                    name: storageSKU
                  }
                  kind: 'StorageV2'
                  properties: {
                    supportsHttpsTrafficOnly: true
                  }
                }

                param propertiesPrimaryEndpoints object = stg.properties.primaryEndpoints

                output storageEndpoint object = propertiesPrimaryEndpoints
                """,
            null,
            null);
    }

    [TestMethod]
    public async Task CreateType()
    {
        await RunExtractToTypeTest(
            """
                resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
                  name: '${storagePrefix}${uniqueString(resourceGroup().id)}'
                  location: location
                  sku: {
                    name: storageSKU
                  }
                  kind: 'StorageV2'
                  properties: {
                    supportsHttpsTrafficOnly: true
                  }
                }

                output storageEndpoint object = stg.|properties.primaryEndpoints
                """,
            """
                resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
                  name: '${storagePrefix}${uniqueString(resourceGroup().id)}'
                  location: location
                  sku: {
                    name: storageSKU
                  }
                  kind: 'StorageV2'
                  properties: {
                    supportsHttpsTrafficOnly: true
                  }
                }

                type stgProperties = {
                  accessTier: ('Cool' | 'Hot')?
                  allowBlobPublicAccess: bool?
                  allowSharedKeyAccess: bool?
                  azureFilesIdentityBasedAuthentication: {
                    activeDirectoryProperties: {
                      azureStorageSid: string
                      domainGuid: string
                      domainName: string
                      domainSid: string
                      forestName: string
                      netBiosDomainName: string
                    }?
                    directoryServiceOptions: string /* 'AADDS' | 'AD' | 'None' | string */
                  }?
                  customDomain: { name: string, useSubDomainName: bool? }?
                  encryption: {
                    keySource: string /* 'Microsoft.Keyvault' | 'Microsoft.Storage' | string */
                    keyvaultproperties: { keyname: string?, keyvaulturi: string?, keyversion: string? }?
                    services: { blob: { enabled: bool? }?, file: { enabled: bool? }? }?
                  }?
                  isHnsEnabled: bool?
                  largeFileSharesState: (string /* 'Disabled' | 'Enabled' | string */)?
                  minimumTlsVersion: (string /* 'TLS1_0' | 'TLS1_1' | 'TLS1_2' | string */)?
                  networkAcls: {
                    bypass: (string /* 'AzureServices' | 'Logging' | 'Metrics' | 'None' | string */)?
                    defaultAction: 'Allow' | 'Deny'
                    ipRules: { action: string?, value: string }[]?
                    virtualNetworkRules: {
                      action: string?
                      id: string
                      state: ('deprovisioning' | 'failed' | 'networkSourceDeleted' | 'provisioning' | 'succeeded')?
                    }[]?
                  }?
                  supportsHttpsTrafficOnly: bool?
                }

                output storageEndpoint object = stg.properties.primaryEndpoints
                """);
    }

    [TestMethod]
    public async Task CreateType_ShouldntBeOfferedForSimpleTypes()
    {
        await RunExtractToTypeTest(
            """
                resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
                  name: '${storagePrefix}${uniqueString(resourceGroup().id)}'
                  location: location
                  sku: {
                    name: storageSKU
                  }
                  kind: 'StorageV2'
                  properties: {
                    supportsHttpsTrafficOnly: true
                  }
                }

                output storageEndpoint object = stg.properties.|primaryEndpoints
                """,
            null);
    }

    [DataTestMethod]
    [DataRow(
        """
            resource nsg 'Microsoft.Network/networkSecurityGroups@2023-09-01' = {
              name: 'nsg'
              location: 'westus'
              properties: |{
                securityRules: [{
                  id: 'id1'
                  name: 'name1'
                  type: 'type1'
                  properties: {
                    access: 'Allow'
                    direction: 'Inbound'
                    priority: 4096
                    protocol: 'Tcp'
                  }
                }]
              }
            }
            """,
        """
            @description('Properties of the network security group.')
            param properties resourceInput<'Microsoft.Network/networkSecurityGroups@2023-09-01'>.properties = {
              securityRules: [
                {
                  id: 'id1'
                  name: 'name1'
                  type: 'type1'
                  properties: {
                    access: 'Allow'
                    direction: 'Inbound'
                    priority: 4096
                    protocol: 'Tcp'
                  }
                }
              ]
            }

            resource nsg 'Microsoft.Network/networkSecurityGroups@2023-09-01' = {
              name: 'nsg'
              location: 'westus'
              properties: properties
            }
            """
        )]
    [DataRow(
        """
            resource nsg 'Microsoft.Network/networkSecurityGroups@2023-09-01' = |{
              name: 'nsg'
              location: 'westus'
              properties: {
                securityRules: [{
                  id: 'id1'
                  name: 'name1'
                  type: 'type1'
                  properties: {
                    access: 'Allow'
                    direction: 'Inbound'
                    priority: 4096
                    protocol: 'Tcp'
                  }
                }]
              }
            }
            """,
        null)]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              properties: {
                urlPathMaps: [
                  {
                    properties: {
                      pathRules: [
                        {
                          name: 'name'
                          properties: {
                            paths: [
                              'path'
                            ]
                            |backendAddressPool: {
                              id: 'id'
                            }
                          }
                        }
                      ]
                    }
                  }
                ]
              }
            }
            """,
        """
           @description('Backend address pool resource of URL path map path rule.')
           param backendAddressPool resourceInput<'Microsoft.Network/applicationGateways@2020-11-01'>.properties.urlPathMaps[*].properties.pathRules[*].properties.backendAddressPool = {
             id: 'id'
           }

           resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
             properties: {
               urlPathMaps: [
                 {
                   properties: {
                     pathRules: [
                       {
                         name: 'name'
                         properties: {
                           paths: [
                             'path'
                           ]
                           backendAddressPool: backendAddressPool
                         }
                       }
                     ]
                   }
                 }
               ]
             }
           }
           """
        )]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              properties: {
                urlPathMaps: [
                  {
                    properties: {
                      pathRules: [
                        {
                          name: 'name'
                          properties: {
                            paths: [
                              'path'
                            ]
                            backendAddressPool: {
                              |id: 'id'
                            }
                          }
                        }
                      ]
                    }
                  }
                ]
              }
            }
            """,
        null // Just a simple string property
        )]
    public async Task ResourceDerivedTypes(string fileWithSelection, string? expectedResourceDerivedText)
    {
        await RunExtractToVariableAndParameterTest(fileWithSelection, "IGNORE", "IGNORE", "IGNORE", expectedResourceDerivedText);
    }

    [TestMethod]
    public void TestCheckLineContent()
    {
        var bicep = $$"""

                COMMENT // hello, cruel world
                COMMENT /* cruel, cruel world
                COMMENT
                COMMENT */

                CONTENT @description('description')
                CONTENT @allowed([
                    CONTENT 'abc'
                    CONTENT 'def'
                    CONTENT COMMENT 'ghi' // comment
                    CONTENT COMMENT /* comment */ 'jkl'
                CONTENT ]
                CONTENT )
                CONTENT COMMENT param p1 string = /*comment*/ 'abc'
                CONTENT param p2 object = {
                    CONTENT a: 'abc'
                    COMMENT // hi
                    CONTENT COMMENT b: /*comment*/ 'bcd'

                    CONTENT b: [
                        CONTENT 1, 2

                        CONTENT 3
                        CONTENT 4
                CONTENT  ]
                CONTENT}


                CONTENT COMMENT /*hello*/ resource stg 'Microsoft.Storage/storageAccounts@2019-04-01' = {
                  CONTENT name: '${storagePrefix}${uniqueString(resourceGroup().id)}'
                  CONTENT location: location
                  CONTENT sku: {
                    CONTENT name: storageSKU
                  CONTENT }

                  CONTENT kind: 'StorageV2'
                  CONTENT properties: {
                    COMMENT // supportsHttpsTrafficOnly: true
                  CONTENT }
                CONTENT }
                {{Tab}}   {{Tab}}
                CONTENT output storageEndpoint object = stg.properties.|primaryEndpoints

                COMMENT // Comments are not empty

                COMMENT /* Not even
                COMMENT
                COMMENT this one
                COMMENT
                COMMENT */

                CONTENT         param p2 string
                CONTENT param p3 int            {{Tab}}
                """;

        (bool hasContent, bool hasComments)[] expected = bicep.Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(x => (x.Contains("CONTENT"), x.Contains("COMMENT"))).ToArray();
        bicep = bicep.Replace("COMMENT", "").Replace("CONTENT", "");
        var result = Core.UnitTests.Utils.CompilationHelper.Compile(bicep);
        var lineStarts = result.SourceFile.LineStarts;
        var programSyntax = result.SourceFile.ProgramSyntax;

        using (new AssertionScope())
        {
            for (int line = 0; line < lineStarts.Length; ++line)
            {
                var span = TextCoordinateConverter.GetLineSpan(lineStarts, programSyntax.GetEndPosition(), line);
                var text = bicep.Substring(span.Position, span.Length);
                var (hasContent, hasComments) = ExpressionAndTypeExtractor.CheckLineContent(result.SourceFile.LineStarts, result.SourceFile.ProgramSyntax, line);
                // Console.WriteLine($"{line}: content={hasContent},comments={hasComments}: \"{text.Trim()}\"");

                hasContent.Should().Be(expected[line].hasContent, "line at {0} should {1} have content (text=\"{2}\")", line, expected[line].hasContent ? "" : "not", text);
                hasComments.Should().Be(expected[line].hasComments, "line at {0} should {1} have comments (text=\"{2}\")", line, expected[line].hasComments ? "" : "not", text);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////

    [DataRow(
        """
            var v = 1
            """,
        0)]
    [DataRow(
        """

            var v = 2
            """,
        1)]
    [DataRow(
        """

            // comment
            var v = 3
            """,
        1)]
    [DataRow(
        """

            /* comment */
            var v = 4
            """,
        1)]
    [DataRow(
        """

            /* comment

            */
            var v = 5
            """,
        1)]
    [DataRow(
        """
            /* comment

            */
            var v = 6
            """,
        0)]
    [DataRow(
        """
            /* comment

            */
            @description('description')
            var v = 7
            """,
        0)]
    [DataRow(
        """
            @description('description')
            var v = 8
            """,
        0)]
    [DataRow(
        """

            @description('description')
            var v = 9
            """,
        1)]
    [DataRow(
        """

            @description('description') // comment
            var v = 10
            """,
        1)]
    [DataRow(
        """
            param p1 string
            @description('description') // comment
            var v = 11
            """,
        1)]
    [DataRow(
        """
            param p1 string
            @description('description') /* comment
            */
            var v = 12
            """,
        1)]
    [DataRow(
        """
            param p1 string
            @allowed([1, 2])
            // hello
            /*
            there
            */
            @description('description') /* comment
            */
            var v = 13
            """,
        1)]
    [DataRow(
        """
            param p1 string
            @allowed([1, 2]) // hi
            // hello
            /*
            there
            */
            /* comment */ @description('description') /* comment */
            /*

            */
            var v = [
                14
            ]
            """,
        1)]
    [DataRow(
        """
            /* hello there */ param p1 string // hello
            @allowed([1, 2]) // hi
            // hello
            /*
            there
            */
            /* comment */ @description('description') /* comment */
            /*

            */
            var v = [
                15
            ]
            """,
        1)]
    [DataRow(
        """
            /* hello there */
            @allowed([1, 2]) // hi
            // hello
            /*
            there
            */
            /* comment */ @description('description') /* comment */
            /*

            */
            var v = [
                16
            ]
            """,
        0)]
    [DataTestMethod]
    public void TestGetFirstLineOfStatementIncludingComments(string bicep, int expected)
    {
        // Find the variable declaration line
        var result = Core.UnitTests.Utils.CompilationHelper.Compile(bicep);
        var varDeclaration = result.BicepFile.ProgramSyntax.Declarations.OfType<VariableDeclarationSyntax>().Single();

        var actual = ExpressionAndTypeExtractor.GetFirstLineOfStatementIncludingComments(result.SourceFile.LineStarts, result.SourceFile.ProgramSyntax, varDeclaration);
        actual.Should().Be(expected);
    }

    #region Support

    private async Task RunExtractToVarSingleLineTest(
        string inputTemplateWithSelection,
        string expectedOutputTemplate,
        string lineWithSelection,
        string? expectedNewVarDeclaration,
        string expectedModifiedLine)
    {
        await RunExtractToVariableTest(
            inputTemplateWithSelection.Replace("LINEWITHSELECTION", lineWithSelection),
            expectedOutputTemplate.Replace("EXPECTEDNEWDECLARATION", expectedNewVarDeclaration)
                .Replace("EXPECTEDMODIFIEDLINE", expectedModifiedLine));
    }

    private async Task RunExtractToParamSingleLineTest(
        string inputTemplateWithSelection,
        string expectedOutputTemplate,
        string lineWithSelection,
        string expectedNewParamLooseDeclaration,
        string? expectedNewParamMediumDeclaration,
        string expectedModifiedLine)
    {
        await RunExtractToParameterTest(
            inputTemplateWithSelection.Replace("LINEWITHSELECTION", lineWithSelection),
            expectedOutputTemplate.Replace("EXPECTEDNEWDECLARATION", expectedNewParamLooseDeclaration)
                .Replace("EXPECTEDMODIFIEDLINE", expectedModifiedLine),
            expectedNewParamMediumDeclaration == null ? null :
                expectedOutputTemplate.Replace("EXPECTEDNEWDECLARATION", expectedNewParamMediumDeclaration)
                    .Replace("EXPECTEDMODIFIEDLINE", expectedModifiedLine),
            "IGNORE"
        );
    }

    private async Task RunExtractToVarAndParamOnSingleLineTest(
        string inputTemplateWithSelection,
        string expectedOutputTemplate,
        string lineWithSelection,
        string expectedNewVarDeclaration,
        string expectedNewParamLooseDeclaration,
        string? expectedNewParamMediumDeclaration,
        string expectedModifiedLine
        )
    {
        await RunExtractToVarSingleLineTest(
            inputTemplateWithSelection,
            expectedOutputTemplate,
            lineWithSelection,
            expectedNewVarDeclaration,
            expectedModifiedLine);

        await RunExtractToParamSingleLineTest(
            inputTemplateWithSelection,
            expectedOutputTemplate,
            lineWithSelection,
            expectedNewParamLooseDeclaration,
            expectedNewParamMediumDeclaration,
            expectedModifiedLine);
    }

    private async Task RunExtractToVariableAndParameterTest(string fileWithSelection, string? expectedVariableText, string? expectedLooseParamText, string? expectedMediumParamText, string? expectedResourceDerivedText)
    {
        await RunExtractToVariableTest(
            fileWithSelection,
            expectedVariableText);
        await RunExtractToParameterTest(
            fileWithSelection,
            expectedLooseParamText,
            expectedMediumParamText,
            expectedResourceDerivedText);
    }

    private async Task RunExtractToVariableTest(string fileWithSelection, string? expectedText)
    {
        (var codeActions, var bicepFile) = await GetCodeActionsForSyntaxTest(fileWithSelection);
        var extractedVar = codeActions.FirstOrDefault(x => x.Title.StartsWith(ExtractToVariableTitle));

        if (expectedText == null)
        {
            extractedVar.Should().BeNull("expected no code action for extract var");
        }
        else if (expectedText != "IGNORE")
        {
            extractedVar.Should().NotBeNull("expected an action to extract to variable");
            extractedVar!.Kind.Should().Be(CodeActionKind.RefactorExtract);

            extractedVar.Command.Should().NotBeNull();
            extractedVar.Command!.Name.Should().Be(PostExtractionCommandName);
            var updatedFile = ApplyCodeAction(bicepFile, extractedVar);
            updatedFile.Should().HaveSourceText(expectedText, "extract to variable should match expected outcome");
        }
    }

    private async Task RunExtractToTypeTest(string fileWithSelection, string? expectedText)
    {
        (var codeActions, var bicepFile) = await GetCodeActionsForSyntaxTest(fileWithSelection);
        var extractedType = codeActions.FirstOrDefault(x => x.Title.StartsWith(ExtractToTypeTitle));

        if (expectedText == null)
        {
            extractedType.Should().BeNull("expected no code action for create type");
        }
        else if (expectedText != "IGNORE")
        {
            extractedType.Should().NotBeNull("expected an action to create new type");
            extractedType!.Kind.Should().Be(CodeActionKind.RefactorExtract);

            extractedType.Command.Should().NotBeNull();
            extractedType.Command!.Name.Should().Be(PostExtractionCommandName);

            var updatedFile = ApplyCodeAction(bicepFile, extractedType);
            updatedFile.Should().HaveSourceText(expectedText, "create new type should match expected outcome");
        }
    }

    // expectedMediumParameterText can be "SAME" or "IGNORE"
    // null means that there is no menu item for that option
    private async Task RunExtractToParameterTest(string fileWithSelection, string? expectedLooseParameterText, string? expectedMediumParameterText, string? expectedResourceDerivedText)
    {
        if (expectedMediumParameterText == "SAME")
        {
            expectedMediumParameterText = expectedLooseParameterText;
        }

        (var codeActions, var bicepFile) = await GetCodeActionsForSyntaxTest(fileWithSelection);
        var extractedParamFixes = codeActions.Where(x => x.Title.StartsWith(ExtractToParameterTitle)).ToArray();
        extractedParamFixes.Length.Should().BeLessThanOrEqualTo(3);

        if (expectedLooseParameterText == null)
        {
            extractedParamFixes.Should().BeEmpty("expected no code actions to extract parameter");
            expectedMediumParameterText.Should().BeNull();
        }
        else
        {
            if (expectedLooseParameterText != "IGNORE")
            {
                extractedParamFixes.Should().HaveCountGreaterThanOrEqualTo(1).Should().NotBeNull("expected at least one code action to extract to parameter");
                var looseFix = extractedParamFixes[0];
                looseFix.Kind.Should().Be(CodeActionKind.RefactorExtract);

                looseFix.Command.Should().NotBeNull();
                looseFix.Command!.Name.Should().Be(PostExtractionCommandName);

                var updatedFileLoose = ApplyCodeAction(bicepFile, looseFix);
                updatedFileLoose.Should().HaveSourceText(expectedLooseParameterText, "extract to param with loose typing should match expected outcome");
            }

            if (expectedMediumParameterText == null)
            {
                extractedParamFixes.Length.Should().Be(1, "expected only one code action to extract parameter (as loosely typed - which means the medium-strict version was the same as the loose version)");
            }
            else
            {
                if (expectedMediumParameterText != "IGNORE")
                {
                    extractedParamFixes.Length.Should().BeGreaterThanOrEqualTo(2, "expected a second option to extract to parameter");

                    var mediumFix = extractedParamFixes[1];
                    mediumFix.Kind.Should().Be(CodeActionKind.RefactorExtract);

                    mediumFix.Command.Should().NotBeNull();
                    mediumFix.Command!.Name.Should().Be(PostExtractionCommandName);

                    var updatedFileMedium = ApplyCodeAction(bicepFile, mediumFix);
                    updatedFileMedium.Should().HaveSourceText(expectedMediumParameterText, "extract to param with medium-strict typing should match expected outcome");
                }
            }

            var resourceDerivedFix = extractedParamFixes.Where(fix => fix.Title.Contains("resourceInput<")).SingleOrDefault();
            if (expectedResourceDerivedText == null)
            {
                resourceDerivedFix.Should().BeNull("expected no code actions to extract parameter with resource-derived type");
            }
            else if (expectedResourceDerivedText != "IGNORE")
            {
                resourceDerivedFix.Should().NotBeNull("expected a code action to extract to parameter with resource-derived type");
                resourceDerivedFix!.Kind.Should().Be(CodeActionKind.RefactorExtract);

                resourceDerivedFix.Command.Should().NotBeNull();
                resourceDerivedFix.Command!.Name.Should().Be(PostExtractionCommandName);

                var updatedFileResourceDerived = ApplyCodeAction(bicepFile, resourceDerivedFix);
                updatedFileResourceDerived.Should().HaveSourceText(expectedResourceDerivedText, "extract to param with resource-derived typing should match expected outcome");
            }

        }
    }
}

#endregion
