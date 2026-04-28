// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoModuleNameRuleTests : LinterRuleTestsBase
{
    private static readonly ServiceBuilder Services = new ServiceBuilder().WithConfiguration(BicepTestConstants.BuiltInConfigurationWithStableAnalyzers);

    private void AssertCodeFix(string inputFile, string resultFile)
    {
        var (file, cursor) = ParserHelper.GetFileWithSingleCursor(inputFile, '|');
        var result = CompilationHelper.Compile(Services, file);

        using (new AssertionScope().WithVisualCursor(result.Compilation.GetEntrypointSemanticModel().SourceFile, cursor))
        {
            var matchingDiagnostics = result.Diagnostics
                .Where(x => x.Source == DiagnosticSource.CoreLinter)
                .Where(x => x.Span.IsOverlapping(cursor));

            matchingDiagnostics.Should().ContainSingle(x => x.Code == NoModuleNameRule.Code);
            var diagnostic = matchingDiagnostics.Single(x => x.Code == NoModuleNameRule.Code);

            diagnostic.Fixes.Should().ContainSingle(x => x.Title == "Remove module name");
            var fix = diagnostic.Fixes.Single(x => x.Title == "Remove module name");
            fix.Kind.Should().Be(CodeFixKind.QuickFix);

            fix.Should().HaveResult(file, resultFile);
        }
    }

    private void CompileAndTest(string text, int expectedDiagnosticCount, Options? options = null)
    {
        AssertLinterRuleDiagnostics(NoModuleNameRule.Code, text, expectedDiagnosticCount, options);
    }

    [TestMethod]
    public void Module_with_explicit_name_should_produce_diagnostic()
    {
        CompileAndTest("""
            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
              name: 'foo'
              params: {}
            }
            """, 1, new Options(OnCompileErrors.Ignore));
    }

    [TestMethod]
    public void Module_without_name_should_not_produce_diagnostic()
    {
        CompileAndTest("""
            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
              params: {}
            }
            """, 0, new Options(OnCompileErrors.Ignore));
    }

    [TestMethod]
    public void Multiple_modules_with_names_should_produce_multiple_diagnostics()
    {
        CompileAndTest("""
            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = {
              name: 'foo'
              params: {}
            }

            module bar 'br/public:avm/res/network/virtual-network:0.1.8' = {
              name: 'bar'
              params: {}
            }
            """, 2, new Options(OnCompileErrors.Ignore));
    }

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
    {
        CompileAndTest("""
            param count int

            module foo 'br/public:avm/res/network/virtual-network:0.1.8' = [for i in range(0, count): {
              name: 'foo-${i}'
              params: {}
            }]
            """, 1, new Options(OnCompileErrors.Ignore));
    }
}
