// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoUnusedTypesRuleTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void TypeNameInFormattedMessage()
        {
            var ruleToTest = new NoUnusedTypesRule();
            ruleToTest.GetMessage(nameof(ruleToTest)).Should().Be($"Type \"{nameof(ruleToTest)}\" is declared but never used.");
        }

        private void CompileAndTest(string text, params string[] unusedTypes)
        {
            CompileAndTest(text, new(OnCompileErrors.IncludeErrors), unusedTypes);
        }

        private void CompileAndTest(string text, Options options, params string[] unusedTypes)
        {
            AssertLinterRuleDiagnostics(NoUnusedTypesRule.Code, text, diags =>
                {
                    if (unusedTypes.Any())
                    {
                        var rule = new NoUnusedTypesRule();
                        string[] expectedMessages = unusedTypes.Select(p => rule.GetMessage(p)).ToArray();
                        diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                    }
                    else
                    {
                        diags.Should().BeEmpty();
                    }
                },
                options);
        }

        [DataRow(@"
            type unusedType = string
            type usedType = int
            param foo usedType
            output bar usedType = foo
            ",
            "unusedType")]
        [DataRow(@"
            type type1 = string
            type type2 = int
            type type3 = bool
            ",
            "type1", "type2", "type3")]
        [DataRow(@"
            type usedType = string
            param foo usedType
            output bar usedType = foo
            ")]
        [DataRow(@"
            type baseType = string
            type derivedType = baseType
            param foo derivedType
            output bar derivedType = foo
            ")]
        [DataTestMethod]
        public void TestRule(string text, params string[] unusedTypes)
        {
            CompileAndTest(text, unusedTypes);
        }

        [TestMethod]
        public void Exported_types_are_not_reported_as_unused()
        {
            CompileAndTest("""
                @export()
                type foo = string
                """,
                new Options(OnCompileErrors.IncludeErrors));
        }

        [DataRow(@"
            // Syntax errors
            type =
            type a
            ",
            "a")]
        [DataTestMethod]
        public void SyntaxErrors(string text, params string[] unusedTypes)
        {
            CompileAndTest(text, new(OnCompileErrors.Ignore), unusedTypes);
        }

        [DataRow(@"type")] // Don't show as unused - no type name
        [DataRow(@"type // whoops")] // Don't show as unused - no type name
        [DataTestMethod]
        public void Errors(string text, params string[] unusedTypes)
        {
            CompileAndTest(text, new(OnCompileErrors.Ignore), unusedTypes);
        }

        [TestMethod]
        public void Codefix_removes_unused_type() => AssertCodeFix(
            NoUnusedTypesRule.Code,
            "Remove unused type unusedType",
            """
            type unus|edType = string
            """,
            "");

        [TestMethod]
        public void Codefix_removes_only_the_unused_type() => AssertCodeFix(
            NoUnusedTypesRule.Code,
            "Remove unused type unusedType",
            """
            type unus|edType = string
            type usedType = int
            param foo usedType
            output bar usedType = foo
            """,
            """
            type usedType = int
            param foo usedType
            output bar usedType = foo
            """);
    }
}
