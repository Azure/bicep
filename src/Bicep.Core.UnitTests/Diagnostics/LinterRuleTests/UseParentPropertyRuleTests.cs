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

    [TestMethod]
    public void Code_fix_handles_parent_name_expression() => AssertCodeFix(@"
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
    public void Code_fix_handles_parent_name_interpolated() => AssertCodeFix(@"
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
    public void Code_fix_handles_parent_name_interpolated_complex() => AssertCodeFix(@"
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
    public void Code_fix_handles_child_name_uninterpolated() => AssertCodeFix(@"
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
    public void Code_fix_handles_whitespace_name_interpolated_complex() => AssertCodeFix(@"
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
}
