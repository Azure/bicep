// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoModuleNameRuleTests : LinterRuleTestsBase
{
    // The rule is off by default; supply a bicepconfig.json to enable it for code-fix tests.
    private static readonly CompilationHelper.InputFile BicepConfigEnablingRule = new(
        "bicepconfig.json",
        """{"analyzers":{"core":{"rules":{"no-module-name":{"level":"warning"}}}}}""");

    private static void AssertCodeFix(string inputFile, string resultFile)
        => AssertCodeFix(NoModuleNameRule.Code, CoreResources.NoModuleNameRule_CodeFix, inputFile, resultFile, [BicepConfigEnablingRule]);

    [TestMethod]
    public void Module_with_explicit_name_should_produce_diagnostic()
        => AssertLinterRuleDiagnostics(NoModuleNameRule.Code, """
            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
              name: 'foo'
              params: {}
            }
            """, 1, new Options(OnCompileErrors.Ignore));

    [TestMethod]
    public void Module_without_name_should_not_produce_diagnostic()
        => AssertLinterRuleDiagnostics(NoModuleNameRule.Code, """
            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
              params: {}
            }
            """, [], new Options(OnCompileErrors.Ignore, IncludePosition.None));

    [TestMethod]
    public void Multiple_modules_with_names_should_produce_multiple_diagnostics()
        => AssertLinterRuleDiagnostics(NoModuleNameRule.Code, """
            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
              name: 'foo'
              params: {}
            }

            module bar 'br/public:avm/res/network/virtual-network:0.1.8' = {
              name: 'bar'
              params: {}
            }
            """, 2, new Options(OnCompileErrors.Ignore));

    [TestMethod]
    public void Codefix_removes_name_property() => AssertCodeFix("""
        module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
          na|me: 'foo'
          params: {}
        }
        """, """
        module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
          params: {}
        }
        """);

    [TestMethod]
    public void Module_collection_with_explicit_name_should_produce_diagnostic()
        => AssertLinterRuleDiagnostics(NoModuleNameRule.Code, """
            param count int

            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = [for i in range(0, count): {
              name: 'foo-${i}'
              params: {}
            }]
            """, 1, new Options(OnCompileErrors.Ignore));
}
