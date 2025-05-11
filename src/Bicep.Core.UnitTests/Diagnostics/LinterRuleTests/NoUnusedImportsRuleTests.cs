// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoUnusedImportsRuleTests : LinterRuleTestsBase
{
    [TestMethod]
    public void ImportNameInFormattedMessage()
    {
        var ruleToTest = new NoUnusedImportsRule();
        ruleToTest.GetMessage(nameof(ruleToTest)).Should().Be($"Import \"{nameof(ruleToTest)}\" is declared but never used.");
    }

    private void CompileAndTest(string text, (string path, string contents)[]? additionalFiles = null, params string[] unusedImports)
    {
        CompileAndTest(text, LinterRuleTestsBase.OnCompileErrors.IncludeErrors, additionalFiles, unusedImports);
    }

    private void CompileAndTest(string text, LinterRuleTestsBase.OnCompileErrors onCompileErrors, (string path, string contents)[]? additionalFiles = null, params string[] unusedImports)
    {
        AssertLinterRuleDiagnostics(NoUnusedImportsRule.Code, text, diags =>
            {
                if (unusedImports.Any())
                {
                    var rule = new NoUnusedImportsRule();
                    string[] expectedMessages = unusedImports.Select(p => rule.GetMessage(p)).ToArray();
                    diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            },
            new Options(onCompileErrors, AdditionalFiles: additionalFiles));
    }

    [DataRow(@"
            import {password} from './mod.bicep'
            var sum = 1 + 3
            output sub int = sum
            ",
        "mod.bicep",
        @"
            @export()
            var password = ''
            ",
        "password")]
    [DataRow(@"
            import {param1, param2, param3} from './mod.bicep'
            var sum = 1 + 3
            output sub int = sum
            ",
        "mod.bicep",
        @"
            @export()
            var param1 = ''
            @export()
            var param2 = ''
            @export()
            var param3 = ''
            ",
        "param1", "param2", "param3")]
    [DataRow(@"
            import {param1, param2, param3} from './mod.bicep'
            var sum = 1 + 3
            output sub int = sum + param2
            ",
        "mod.bicep",
        @"
            @export()
            var param1 = ''
            @export()
            var param2 = ''
            @export()
            var param3 = ''
            ",
        "param1", "param3")]
    [DataRow(@"
            import {param2} from './mod.bicep'
            var sum = 1 + 3
            output sub int = sum + param2
            ",
        "mod.bicep",
        @"
            @export()
            var param2 = 5
            ")]
    [DataRow(@"
            var sum = 1 + 3
            output sub int = sum
            ",
        "mod.bicep",
        "")]
    [DataRow(@"
            // function import
            import { getString, getString2 } from './mod.bicep'

            output outputString string = getString()
            ",
        "mod.bicep",
        @"
        @export()
        func getString() string => 'exported'
        @export()
        func getString2() string => 'Not exported'
        ",
        "getString2")]
    [DataRow(@"
    import * as mod from './mod.bicep'
    ",
        "mod.bicep",
        @"",
        "mod")]
    [DataRow(@"
    import * as mod from './mod.bicep'

    output outputString string = mod.getString()
    ",
        "mod.bicep",
        @"
        @export()
        func getString() string => 'exported'
        ")]
    [DataRow(@"
    import { getString as test } from './mod.bicep'
    ",
        "mod.bicep",
        @"
        @export()
        func getString() string => 'exported'
        ",
        "test")]
    [DataTestMethod]
    public void TestRule(string text, string importFileName, string importFileText, params string[] unusedImports)
    {
        var additionalFiles = new[] { (importFileName, importFileText) };
        CompileAndTest(text, LinterRuleTestsBase.OnCompileErrors.Ignore, additionalFiles, unusedImports);
    }

    [DataRow(@"
            import { namePrefix, location } from './mod.bicep'

            module stgModule './storageAccount.bicep' = {
              name: 'storageDeploy'
              params: {
                storagePrefix: namePrefix
                location: location
              }
            }

            output storageEndpoint object = stgModule.outputs.storageEndpoint
            ",
        "mod.bicep",
        @"
            @export()
            var namePrefix = 'prefix'
            @export()
            var location = 'eastus'
            ")]
    [DataRow(@"
            import { namePrefix, location, unusedparam } from './mod.bicep'

            module stgModule './storageAccount.bicep' = {
              name: 'storageDeploy'
              params: {
                storagePrefix: namePrefix
                location: location
              }
            }

            output storageEndpoint object = stgModule.outputs.storageEndpoint
            ",
        "mod.bicep",
        @"
            @export()
            var namePrefix = 'prefix'
            @export()
            var location = 'eastus'
            @export()
            var unusedparam = 'param'
            ",
        "unusedparam")]
    [DataTestMethod]
    public void Modules(string text, string importFileName, string importFileText, params string[] unusedImports)
    {
        var additionalFiles = new[] { (importFileName, importFileText) };
        CompileAndTest(text, LinterRuleTestsBase.OnCompileErrors.Ignore, additionalFiles, unusedImports);
    }

    [DataRow(@"
            import { location } from './mod.bicep'

            resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = if (false) {
              name: 'myZone'
              location: location
            }
            ",
        "mod.bicep",
        @"
            @export()
            var location = 'eastus'
            ")]
    [DataRow(@"
            import { location, size } from './mod.bicep'

            resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = if (false) {
              name: 'myZone'
              location: location
            }
            ",
        "mod.bicep",
        @"
            @export()
            var location = 'eastus'
            @export()
            var size = 5
            ",
        "size")]
    [DataTestMethod]
    public void Conditions(string text, string importFileName, string importFileText, params string[] unusedImports)
    {
        var additionalFiles = new[] { (importFileName, importFileText) };
        CompileAndTest(text, additionalFiles, unusedImports);
    }

    [DataRow(
        @"
        import { p1, p2, p|3 } from './mod.bicep'
        var used1 = p1
        var used2 = p2
        ",
        @"
        @export()
        var p1 = 'prefix'
        @export()
        var p2 = 'eastus'
        @export()
        var p3 = 'param'
        ",
        @"
        import { p1, p2 } from './mod.bicep'
        var used1 = p1
        var used2 = p2
        ",
        "p3")]
    [DataRow(
        @"
        import { p1, p2, p|3 } from './mod.bicep'
        var used1 = p1
        var used2 = p2
        ",
        @"
        @export()
        var p1 = 'prefix'
        @export()
        var p2 = 'eastus'
        @export()
        var p3 = 'param'
        ",
        @"
        import { p1, p2 } from './mod.bicep'
        var used1 = p1
        var used2 = p2
        ",
        "p3")]
    [DataRow(
        @"
        import { p1, p|2, p3 } from './mod.bicep'
        var used1 = p1
        var used2 = p3
        ",
        @"
        @export()
        var p1 = 'prefix'
        @export()
        var p2 = 'eastus'
        @export()
        var p3 = 'param'
        ",
        @"
        import { p1, p3 } from './mod.bicep'
        var used1 = p1
        var used2 = p3
        ",
        "p2")]
    [DataRow(
        @"
        import { p|1, p2, p3 } from './mod.bicep'
        var used1 = p2
        var used2 = p3
        ",
        @"
        @export()
        var p1 = 'prefix'
        @export()
        var p2 = 'eastus'
        @export()
        var p3 = 'param'
        ",
        @"
        import {  p2, p3 } from './mod.bicep'
        var used1 = p2
        var used2 = p3
        ",
        "p1")]
    [DataRow(
        "import * as mo|d from './mod.bicep'",
        @"
        @export()
        var p1 = 'prefix'
        @export()
        var p2 = 'eastus'
        @export()
        var p3 = 'param'
        ",
        "",
        "mod")]
    [DataRow(
        "import { getStr|ing } from './mod.bicep'",
        @"
        @export()
        func getString() string => 'exported'
        ",
        "import {  } from './mod.bicep'",
        "getString")]
    [DataRow(
        "import { t| } from './mod.bicep'",
        @"
        @export()
        type t = string
        ",
        "import {  } from './mod.bicep'",
        "t")]
    [DataTestMethod]
    public void Codefix_recommends_remove_unused_imports(string text, string importFileText, string expectedResultText, string importToRemove)
    {
        CompilationHelper.InputFile[] additionalFiles =
        [
            new("mod.bicep", importFileText)
        ];

        AssertCodeFix(text, expectedResultText, importToRemove, additionalFiles);
    }


    private static void AssertCodeFix(string inputFile, string resultFile, string importNameToFix, CompilationHelper.InputFile[]? supportingFiles = null)
        => AssertCodeFix(NoUnusedImportsRule.Code, $"Remove unused import {importNameToFix}", inputFile, resultFile, supportingFiles);

    [DataRow(@"import", "mod.bicep", "")] // Don't show as unused - no imported symbol or file name
    [DataRow(@"import {p2} from './mod.bicep'", "mod.bicep", "")] // Don't show as unused - imported symbol not existing
    [DataTestMethod]
    public void Errors(string text, string importFileName, string importFileText, params string[] unusedImports)
    {
        var additionalFiles = new[] { (importFileName, importFileText) };
        CompileAndTest(text, LinterRuleTestsBase.OnCompileErrors.Ignore, additionalFiles);
    }
}
