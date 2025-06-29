// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseParentPropertyRuleTests : LinterRuleTestsBase
{
    private void AssertCodeFix(string inputFile, string resultFile)
        => AssertCodeFix(UseParentPropertyRule.Code, "Use parent property", inputFile, resultFile);

    private void AssertNoDiagnostics(string inputFile)
        => AssertLinterRuleDiagnostics(UseParentPropertyRule.Code, inputFile, [], new Options(OnCompileErrors.Ignore, IncludePosition.None));

    [TestMethod]
    public void Codefix_handles_parent_name_expression() => AssertCodeFix(@"
param stgName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: stgName
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: '${stgName}/bl|ah'
}
", @"
param stgName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: stgName
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: stg
  name: 'blah'
}
");

    [TestMethod]
    public void Codefix_handles_parent_name_interpolated() => AssertCodeFix(@"
param stgName string
param blobName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: '${stgName}'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: '${stgName|}/${blobName}'
}
", @"
param stgName string
param blobName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: '${stgName}'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: stg
  name: blobName
}
");

    [TestMethod]
    public void Codefix_handles_parent_name_interpolated_complex() => AssertCodeFix(@"
param stgName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: '${stgName}-${toLower(blah)}-foo'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: '${stgNam|e}-${toLower(blah)}-foo/asd-${abc}-${def}'
}
", @"
param stgName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: '${stgName}-${toLower(blah)}-foo'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: stg
  name: 'asd-${abc}-${def}'
}
");

    [TestMethod]
    public void Codefix_handles_child_name_uninterpolated() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'abc'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: 'a|bc/def'
}
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'abc'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: stg
  name: 'def'
}
");

    [TestMethod]
    public void Codefix_handles_whitespace_name_interpolated_complex() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: '${stgName /* sdfsad */}-${toLower(blah)}-foo'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: '${stgNam|e}-${toLower(blah)}-foo/asd-${abc}-${def}'
}
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: '${stgName /* sdfsad */}-${toLower(blah)}-foo'
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: stg
  name: 'asd-${abc}-${def}'
}
");

    [TestMethod]
    public void Rule_ignores_parent_resources_with_loops() => AssertNoDiagnostics(@"
resource sdf 'Microsoft.Storage/storageAccounts@2022-09-01' = [for item in range(0, 10): {
  name: 'abc${item}'
}]

resource sdf2 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = [for item in range(0, 10): {
  name: 'abc${item}/asdf'
}]
");

    [TestMethod]
    public void Codefix_simplifies_interpolated_string_values() => AssertCodeFix(@"
var a = 'blah'

resource parent 'Microsoft.Network/networkInterfaces@2022-07-01' = {
  name: 'parent'
}

resource child 'Microsoft.Network/networkInterfaces/ipConfigurations@2022-07-01' existing = {
  name: 'par|ent/${a}'
}
", @"
var a = 'blah'

resource parent 'Microsoft.Network/networkInterfaces@2022-07-01' = {
  name: 'parent'
}

resource child 'Microsoft.Network/networkInterfaces/ipConfigurations@2022-07-01' existing = {
  parent: parent
  name: a
}
");

    [TestMethod]
    public void Codefix_doesnt_simplify_interpolated_non_string_values() => AssertCodeFix(@"
var a = 100

resource parent 'Microsoft.Network/networkInterfaces@2022-07-01' = {
  name: 'parent'
}

resource child 'Microsoft.Network/networkInterfaces/ipConfigurations@2022-07-01' existing = {
  name: 'par|ent/${a}'
}
", @"
var a = 100

resource parent 'Microsoft.Network/networkInterfaces@2022-07-01' = {
  name: 'parent'
}

resource child 'Microsoft.Network/networkInterfaces/ipConfigurations@2022-07-01' existing = {
  parent: parent
  name: '${a}'
}
");

    [TestMethod]
    public void Codefix_handles_parent_dot_name_expression() => AssertCodeFix(@"
param stgName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: stgName
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: '${stg.name}/bl|ah'
}
", @"
param stgName string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: stgName
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: stg
  name: 'blah'
}
");

    [TestMethod] // https://github.com/Azure/bicep/issues/16455
    public void Rule_ignores_grandparent_resources() => AssertNoDiagnostics(@"
param storageAccountName string = toLower(take('stg${uniqueString(resourceGroup().id, deployment().name)}', 24))
param blobContainerName string = 'blobcontainer${uniqueString(resourceGroup().id, deployment().name)}'

param fileShareName string = 'fileshare${uniqueString(resourceGroup().id, deployment().name)}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: '${storageAccount.name}/defa|ult/${blobContainerName}'
  properties: {
    publicAccess: 'None'
  }
}

resource fileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2022-09-01' = {
  name: '${storageAccount.name}/default/${fileShareName}'
  properties: {
    shareQuota: 100
  }
}");
}
