using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Refactor;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class SyntaxForTypeTests
    {
        // asdfg allowed values

        // Data row args:
        //   - input: type definition for type 't'
        //   - expected: result of calling with loose strictness
        //   - expected: result of calling with medium strictness
        //   - expected: result of calling with strict strictness

        [DataTestMethod]
        [DataRow(
            "type t = 123",
            "int",
            "int",
            "123")]
        [DataRow(
            "type t = 'abc'",
            "string",
            "string",
            "'abc'")]
        [DataRow(
            "type t = true",
            "bool",
            "bool",
            "true")]
        public void LiteralTypes(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        [DataTestMethod]
        [DataRow(
            "type t = int",
            "int",
            "int",
            "int")]
        [DataRow(
            "type t = string",
            "string",
            "string",
            "string")]
        [DataRow(
            "type t = bool",
            "bool",
            "bool",
            "bool")]
        [DataRow(
            "type t = null|'abc'",
            "string",
            "'abc' | null",
            "'abc' | null")]
        public void SimpleTypes(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        [DataTestMethod]
        [DataRow(
            "type t = {a:123,b:'abc'}",
            "object",
            "{ a: int, b: string }",
            "{ a: 123, b: 'abc' }")]
        [DataRow(
            "type t = { 'my type': 'my string' }",
            "object",
            "{ 'my type': string }",
            "{ 'my type': 'my string' }")]
        [DataRow(
            "type t = { 'true': true }",
            "object",
            "{ 'true': bool }",
            "{ 'true': true }")]
        [DataRow(
            "type t = object",
            "object",
            "object",
            "object")]
        public void ObjectTypes(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        [DataTestMethod]
        [DataRow(
            "type t = 'abc' | 'def' | 'ghi'",
            "string",
            "'abc' | 'def' | 'ghi'",
            "'abc' | 'def' | 'ghi'")]
        [DataRow(
            "type t = 1 | 2 | 3 | -1",
            "int",
            "-1 | 1 | 2 | 3",
            "-1 | 1 | 2 | 3")]
        [DataRow(
            "type t = true|false",
            "bool",
            "false | true",
            "false | true")]
        public void UnionTypes(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        [DataTestMethod]
        [DataRow(
            "type t = [int, string]",
            "array",
            "[int, string]",
            "[int, string]")]
        [DataRow(
            "type t = [123, 'abc' | 'def']",
            "array",
            "[int, 'abc' | 'def']",
            "[123, 'abc' | 'def']")]
        public void TupleTypes(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        [DataTestMethod]
        [DataRow(
            """
                type t1 = { a: 'abc', b: 123 }
                type t = { a: t1, b: [t1, t1] }
                """,
            "object",
            // TODO: "{ a: t1, b: [t1, t1] }", asdfg
            "{ a: { a: string, b: int }, b: [{ a: string, b: int }, { a: string, b: int }] }",
            // TODO: "{ a: t1, b: [t1, t1] }"
            "{ a: { a: 'abc', b: 123 }, b: [{ a: 'abc', b: 123 }, { a: 'abc', b: 123 }] }")]
        public void NestedNamedTypes(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        [DataTestMethod]
        [Ignore] //asdfg
        [DataRow(
            "type t = [string, t?]",
            "array",
            "[string, t?]",
            "[string, t?]")]
        public void RecursiveTypes(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        [DataTestMethod]
        [DataRow(
            "type t = string[]",
            "array",
            "string[]",
            "string[]")]
        [DataRow(
            "type t = 'abc'[]",
            "array",
            "string[]",
            "'abc'[]")]
        [DataRow(
            "type t = ('abc'|'def')[]",
            "array",
            "('abc' | 'def')[]",
            "('abc' | 'def')[]")]
        public void TypedArrays(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            RunGetSyntaxForTypeTest(input, expectedLooseSyntax, expectedStrictMediumSyntax, expectedStrictSyntax);
        }

        private static void RunGetSyntaxForTypeTest(string input, string expectedLooseSyntax, string expectedStrictMediumSyntax, string expectedStrictSyntax)
        {
            var compilationResult = CompilationHelper.Compile(input);
            var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
            var declarationSyntax = semanticModel.Root.TypeDeclarations[0].DeclaringSyntax;
            var declaredType = semanticModel.GetTypeInfo(semanticModel.Root.TypeDeclarations.Single(t => t.Name == "t").Value);

            var looseSyntax = SyntaxForType.GetSyntaxStringForType(declaredType, SyntaxForType.Strictness.Loose);
            var mediumStrictSyntax = SyntaxForType.GetSyntaxStringForType(declaredType, SyntaxForType.Strictness.Medium);
            var strictSyntax = SyntaxForType.GetSyntaxStringForType(declaredType, SyntaxForType.Strictness.Strict);

            using (new AssertionScope())
            {
                CompilationHelper.Compile($"type t = {expectedLooseSyntax}").Diagnostics.Should().NotHaveAnyDiagnostics("expected loose syntax should be error-free");
                CompilationHelper.Compile($"type t = {expectedStrictMediumSyntax}").Diagnostics.Should().NotHaveAnyDiagnostics("expected medium strictness syntax should be error-free");
                CompilationHelper.Compile($"type t = {expectedStrictSyntax}").Diagnostics.Should().NotHaveAnyDiagnostics("expected strict syntax should be error-free");
            }

            using (new AssertionScope())
            {
                looseSyntax.Should().Be(expectedLooseSyntax);
                mediumStrictSyntax.Should().Be(expectedStrictMediumSyntax);
                strictSyntax.Should().Be(expectedStrictSyntax);
            }

            var typeName = declaredType.Name;

            // Note: TODO: This might not be true for all cases, might have to remove this check
            if (!typeName.Contains("t1")) // doesn't currently work for recursive and named types
            {
                typeName.Should().EqualIgnoringWhitespace(expectedStrictSyntax, "the resulting strict syntax should be the same as the type's Name property");
            }
        }
    }
}
