// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseResourceSymbolReferenceRuleTests : LinterRuleTestsBase
{
    private void AssertCodeFix(string inputFile, string resultFile)
        => AssertCodeFix(UseResourceSymbolReferenceRule.Code, "Use a direct resource reference.", inputFile, resultFile);

    private void AssertNoDiagnostics(string inputFile)
        => AssertLinterRuleDiagnostics(UseParentPropertyRule.Code, inputFile, new string[] { }, new Options(OnCompileErrors.Ignore, IncludePosition.None));

    [TestMethod]
    public void Codefix_handles_list_functions_with_reference_based_id_and_apiversion() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stgName'
}

var blah = listKe|ys(stg.id, stg.apiVersion).keys
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stgName'
}

var blah = stg.listKeys().keys
");

    [TestMethod]
    public void Codefix_handles_list_functions_with_reference_based_id_and_apiversion_and_optional_args() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stgName'
}

var blah = listKe|ys(stg.id, stg.apiVersion, { complex: 'properties' }).keys
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stgName'
}

var blah = stg.listKeys(stg.apiVersion,{ complex: 'properties' }).keys
");

    [TestMethod]
    public void Codefix_handles_list_functions_with_resourceId_function() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = listKe|ys(resourceId('Microsoft.Storage/storageAccounts', 'stg${123}Name'), stg.apiVersion).keys
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = stg.listKeys().keys
");

    [TestMethod]
    public void Codefix_handles_reference_functions_with_resourceId_function() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = refer|ence(resourceId('Microsoft.Storage/storageAccounts', 'stg${123}Name')).accessTier
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = stg.properties.accessTier
");

    [TestMethod]
    public void Codefix_handles_reference_functions_with_resourceId_function_and_apiVersion() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = refer|ence(resourceId('Microsoft.Storage/storageAccounts', 'stg${123}Name'), '2022-09-01').accessTier
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = stg.properties.accessTier
");

    [TestMethod]
    public void Codefix_handles_reference_functions_with_resourceId_function_and_full() => AssertCodeFix(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = refer|ence(resourceId('Microsoft.Storage/storageAccounts', 'stg${123}Name'), '2022-09-01', 'Full').sku
", @"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'stg${123}Name'
}

var blah = stg.sku
");

    [TestMethod]
    public void Rule_ignores_optimized_list_functions() => AssertNoDiagnostics(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: stgName
}

var blah = stg.listKeys().keys
");

    [TestMethod]
    public void Rule_ignores_unmatching_list_functions() => AssertNoDiagnostics(@"
resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'foo'
}

var blah = listKe|ys(resourceId('Microsoft.Storage/storageAccounts', 'bar'), stg.apiVersion).keys
");
}
