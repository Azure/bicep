// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests;

public partial class CodeActionTests : CodeActionTestBase
{
    private async Task<string> ApplyUndefinedSymbolCodeFix(string bicepFileContents, string missingName, string actionType)
    {
        var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
        var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
        var uri = documentUri.ToUriEncoded();

        var files = new Dictionary<Uri, string> { [uri] = bicepFileContents };

        var compilation = Services.BuildCompilation(files, uri);
        var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
        diagnostics.Should().ContainSingle(d => d.Code == "BCP057");
        TestContext.WriteLine("Diagnostics: " + string.Join(", ", diagnostics.Select(d =>
        {
            var end = d.Span.Position + d.Span.Length;
            return $"{d.Code}@{d.Span.Position}-{end}";
        })));

        var bcp057 = diagnostics.Single(d => d.Code == "BCP057");
        var diagnosticRange = bcp057.ToRange(compilation.SourceFileGrouping.EntryPoint.LineStarts);
        TestContext.WriteLine($"Diagnostic range: {diagnosticRange.Start.Line}:{diagnosticRange.Start.Character}-{diagnosticRange.End.Line}:{diagnosticRange.End.Character}");

        var helper = await ServerWithBuiltInTypes.GetAsync();
        await helper.OpenFileOnceAsync(TestContext, bicepFileContents, documentUri);

        var codeActions = await helper.Client.RequestCodeAction(new CodeActionParams
        {
            TextDocument = new TextDocumentIdentifier(documentUri),
            Range = diagnosticRange,
        });

        codeActions.Should().NotBeNull();
        TestContext.WriteLine("Returned code actions: " + string.Join(", ", codeActions!.Select(x => x.CodeAction?.Title ?? "<command>")));
        var expectedTitle = actionType == "parameter"
            ? $"Create parameter '{missingName}'"
            : $"Create variable '{missingName}'";
        var codeAction = codeActions!.SingleOrDefault(x => x.CodeAction?.Title == expectedTitle);
        codeAction.Should().NotBeNull($"Expected to find '{expectedTitle}' code action");

        var bicepFile = new LanguageClientFile(documentUri, bicepFileContents);
        return LspRefactoringHelper.ApplyCodeAction(bicepFile, codeAction!.CodeAction!).Text;
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateParameterAndVariableQuickFixes()
    {
        const string missingName = "storageAccountName";
        var bicepFileContents = $"output out string = {missingName}";
        var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
        var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
        var uri = documentUri.ToUriEncoded();

        var files = new Dictionary<Uri, string>
        {
            [uri] = bicepFileContents,
        };

        var compilation = Services.BuildCompilation(files, uri);
        var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
        diagnostics.Should().ContainSingle(d => d.Code == "BCP057");
        TestContext.WriteLine("Diagnostics: " + string.Join(", ", diagnostics.Select(d => d.Code)));

        var bcp057 = diagnostics.Single(d => d.Code == "BCP057");
        var diagnosticRange = bcp057.ToRange(compilation.SourceFileGrouping.EntryPoint.LineStarts);

        var helper = await ServerWithBuiltInTypes.GetAsync();
        await helper.OpenFileOnceAsync(TestContext, bicepFileContents, documentUri);

        var codeActions = await helper.Client.RequestCodeAction(new CodeActionParams
        {
            TextDocument = new TextDocumentIdentifier(documentUri),
            Range = diagnosticRange,
        });

        codeActions.Should().NotBeNull();
        TestContext.WriteLine("Returned code actions: " + string.Join(", ", codeActions!.Select(x => x.CodeAction?.Title ?? "<command>")));

        var createParam = codeActions!.Single(x => x.CodeAction?.Title == $"Create parameter '{missingName}'");
        var createVar = codeActions!.Single(x => x.CodeAction?.Title == $"Create variable '{missingName}'");

        var bicepFile = new LanguageClientFile(documentUri, bicepFileContents);

        // Test parameter creation
        var paramResult = LspRefactoringHelper.ApplyCodeAction(bicepFile, createParam.CodeAction!);
        paramResult.Should().HaveSourceText($$"""
            param storageAccountName string

            output out string = storageAccountName
            """);

        // Test variable creation (fresh bicepFile for clean state)
        bicepFile = new LanguageClientFile(documentUri, bicepFileContents);
        var varResult = LspRefactoringHelper.ApplyCodeAction(bicepFile, createVar.CodeAction!);
        varResult.Should().HaveSourceText($$"""
            var storageAccountName = ''

            output out string = storageAccountName
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInConditionShouldInferBoolParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource st 'Microsoft.Storage/storageAccounts@2022-09-01' = {
              name: 'st'
              location: 'westus'
              publicNetworkAccess: enablePrivateEndpoint ? 'Disabled' : 'Enabled'
            }
            """, "enablePrivateEndpoint", "parameter");

        result.Should().Be("""
            param enablePrivateEndpoint bool

            resource st 'Microsoft.Storage/storageAccounts@2022-09-01' = {
              name: 'st'
              location: 'westus'
              publicNetworkAccess: enablePrivateEndpoint ? 'Disabled' : 'Enabled'
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateVariableWithBoolInitializer()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource st 'Microsoft.Storage/storageAccounts@2022-09-01' = {
              name: 'st'
              location: 'westus'
              publicNetworkAccess: enablePrivateEndpoint ? 'Disabled' : 'Enabled'
            }
            """, "enablePrivateEndpoint", "variable");

        result.Should().Be("""
            var enablePrivateEndpoint = false

            resource st 'Microsoft.Storage/storageAccounts@2022-09-01' = {
              name: 'st'
              location: 'westus'
              publicNetworkAccess: enablePrivateEndpoint ? 'Disabled' : 'Enabled'
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInResourceIfConditionShouldInferBoolParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource pe 'Microsoft.Network/privateEndpoints@2025-01-01' = if (enablePrivateEndpoint) {
              name: 'pe'
              location: 'westus'
            }
            """, "enablePrivateEndpoint", "parameter");

        result.Should().Be("""
            param enablePrivateEndpoint bool

            resource pe 'Microsoft.Network/privateEndpoints@2025-01-01' = if (enablePrivateEndpoint) {
              name: 'pe'
              location: 'westus'
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInResourceIfConditionShouldOfferBoolVariable()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource pe 'Microsoft.Network/privateEndpoints@2025-01-01' = if (enablePrivateEndpoint) {
              name: 'pe'
              location: 'westus'
            }
            """, "enablePrivateEndpoint", "variable");

        result.Should().Be("""
            var enablePrivateEndpoint = false

            resource pe 'Microsoft.Network/privateEndpoints@2025-01-01' = if (enablePrivateEndpoint) {
              name: 'pe'
              location: 'westus'
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateVariableWithIntInitializer()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output total int = replicas + 2
            """, "replicas", "variable");

        result.Should().Be("""
            var replicas = 0

            output total int = replicas + 2
            """);
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateVariableWithObjectInitializer()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource st 'Microsoft.Storage/storageAccounts@2022-09-01' = {
              name: 'st'
              location: 'westus'
              sku: sku
            }
            """, "sku", "variable");

        result.Should().Be("""
            var sku = {}

            resource st 'Microsoft.Storage/storageAccounts@2022-09-01' = {
              name: 'st'
              location: 'westus'
              sku: sku
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateVariableWithTypedObjectProperties()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            type ConfigType = {
              enabled: bool
              count: int
              name: string
            }

            output out ConfigType = config
            """, "config", "variable");

        result.Should().Be("""
            type ConfigType = {
              enabled: bool
              count: int
              name: string
            }

            var config = { count: 0, enabled: false, name: '' }

            output out ConfigType = config
            """);
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateVariableWithUnionTypeInitializer()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            type StorageSkuType = 'Standard_LRS' | 'Standard_GRS' | 'Premium_LRS'

            output sku StorageSkuType = storageType
            """, "storageType", "variable");

        result.Should().Be("""
            type StorageSkuType = 'Standard_LRS' | 'Standard_GRS' | 'Premium_LRS'

            var storageType = 'Premium_LRS'

            output sku StorageSkuType = storageType
            """);
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateParameterWithResourceDerivedType()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
param storageAccountName string
param location string

resource st 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: storagesku
}
""", "storagesku", "parameter");

        result.Should().Be("""
param storageAccountName string
param location string
param storagesku resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.sku

resource st 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: storagesku
}
""");
    }

    [TestMethod]
    public async Task UndefinedNameShouldOfferCreateVariableWithArrayInitializer()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output out array = myItems
            """, "myItems", "variable");

        result.Should().Be("""
            var myItems = []

            output out array = myItems
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInArithmeticShouldInferIntParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output total int = replicas + 2
            """, "replicas", "parameter");

        result.Should().Be("""
            param replicas int

            output total int = replicas + 2
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInOutputWithUserDefinedTypeShouldInferNamedTypeParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            type myType = {
              name: string
              numbers: int
            }

            output myOutput myType = myParam
            """, "myParam", "parameter");

        result.Should().Be("""
            type myType = {
              name: string
              numbers: int
            }

            param myParam myType

            output myOutput myType = myParam
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInOutputWithUserDefinedTypeShouldOfferVariableWithTypedObjectInitializer()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            type myType = {
              name: string
              numbers: int
            }

            output myOutput myType = myParam
            """, "myParam", "variable");

        result.Should().Be("""
            type myType = {
              name: string
              numbers: int
            }

            var myParam = { name: '', numbers: 0 }

            output myOutput myType = myParam
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInResourceShouldInferResourceInputParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'mystorageacct123'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: storageAccountProps
            }
            """, "storageAccountProps", "parameter");

        result.Should().Be("""
            param storageAccountProps resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.properties

            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'mystorageacct123'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: storageAccountProps
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInNestedResourcePropertiesShouldInferResourceInputParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'mystorageacct123'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                encryption: enc
              }
            }
            """, "enc", "parameter");

        result.Should().Be("""
            param enc resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.properties.encryption

            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'mystorageacct123'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                encryption: enc
              }
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedWithLogicalNotOperatorShouldInferBoolParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output isDisabled bool = !isEnabled
            """, "isEnabled", "parameter");

        result.Should().Be("""
            param isEnabled bool

            output isDisabled bool = !isEnabled
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedWithLogicalAndOperatorShouldInferBoolParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            param enableFeatureA bool

            output bothEnabled bool = enableFeatureA && enableFeatureB
            """, "enableFeatureB", "parameter");

        result.Should().Be("""
            param enableFeatureA bool
            param enableFeatureB bool

            output bothEnabled bool = enableFeatureA && enableFeatureB
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInStringInterpolationShouldInferStringParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output greeting string = 'Hello, ${userName}!'
            """, "userName", "parameter");

        result.Should().Be("""
            param userName string

            output greeting string = 'Hello, ${userName}!'
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInStringInterpolationShouldOfferStringVariable()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output greeting string = 'Hello, ${userName}!'
            """, "userName", "variable");

        result.Should().Be("""
            var userName = ''

            output greeting string = 'Hello, ${userName}!'
            """);
    }

    [TestMethod]
    public async Task UndefinedNameWithTypedStringArrayOutputShouldInferStringArrayParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output items string[] = myStrings
            """, "myStrings", "parameter");

        result.Should().Be("""
            param myStrings string[]

            output items string[] = myStrings
            """);
    }

    [TestMethod]
    public async Task UndefinedNameWithTypedIntArrayOutputShouldInferIntArrayParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            output nums int[] = myNumbers
            """, "myNumbers", "parameter");

        result.Should().Be("""
            param myNumbers int[]

            output nums int[] = myNumbers
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInDeeplyNestedResourcePropertyShouldInferFullPath()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'mystorageacct123'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                networkAcls: {
                  defaultAction: action
                }
              }
            }
            """, "action", "parameter");

        result.Should().Be("""
            param action resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.properties.networkAcls.defaultAction

            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'mystorageacct123'
              location: resourceGroup().location
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
              properties: {
                networkAcls: {
                  defaultAction: action
                }
              }
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInForLoopShouldInferArrayParameter()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource storageAccounts 'Microsoft.Storage/storageAccounts@2023-01-01' = [for item in myItems: {
              name: item
              location: 'westus'
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }]
            """, "myItems", "parameter");

        result.Should().Be("""
            param myItems array

            resource storageAccounts 'Microsoft.Storage/storageAccounts@2023-01-01' = [for item in myItems: {
              name: item
              location: 'westus'
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }]
            """);
    }

    [TestMethod]
    public async Task UndefinedNameUsedInResourceNamePropertyShouldInferResourceInputName()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: storageAccountName
              location: 'westus'
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            """, "storageAccountName", "parameter");

        result.Should().Be("""
            param storageAccountName resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name

            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: storageAccountName
              location: 'westus'
              sku: {
                name: 'Standard_LRS'
              }
              kind: 'StorageV2'
            }
            """);
    }

    [TestMethod]
    public async Task UndefinedNameWithNullableUserDefinedTypeShouldPreserveNullability()
    {
        var result = await ApplyUndefinedSymbolCodeFix("""
            type myType = {
              name: string
              count: int
            }

            output myOutput myType? = myParam
            """, "myParam", "parameter");

        result.Should().Be("""
            type myType = {
              name: string
              count: int
            }

            param myParam myType?

            output myOutput myType? = myParam
            """);
    }
}

