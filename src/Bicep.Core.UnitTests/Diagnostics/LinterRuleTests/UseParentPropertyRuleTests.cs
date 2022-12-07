// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseParentPropertyRuleTests : LinterRuleTestsBase
{
    private void AssertCodeFix(string inputFile, string resultFile)
    {
        var (file, cursor) = ParserHelper.GetFileWithSingleCursor(inputFile, '|');
        var result = CompilationHelper.Compile(file);

        using (new AssertionScope().WithVisualCursor(result.Compilation.GetEntrypointSemanticModel().SourceFile, cursor))
        {
            var diagnostic = result.Diagnostics
                .OfType<IBicepAnalyerFixableDiagnostic>()
                .Where(x => x.Code == UseParentPropertyRule.Code)
                .Where(x => x.Span.IsOverlapping(cursor))
                .Single();

            diagnostic.Fixes.Should().HaveCount(1);

            var fix = diagnostic.Fixes.Single();
            fix.Description.Should().Be("Use parent property");
            fix.Kind.Should().Be(CodeFixKind.QuickFix);

            fix.Should().HaveResult(file, resultFile);
        }
    }

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
