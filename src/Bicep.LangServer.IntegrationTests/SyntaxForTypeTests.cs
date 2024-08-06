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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class SyntaxForTypeTests
    {
        // asdfg allowed values

        [DataTestMethod]
        //asdfg
        // object types
        //[DataRow(
        //    "int",
        //    "int",
        //    "int",
        //    "int")]
        //[DataRow(
        //    "123",
        //    "int",
        //    "int",
        //    "123")]
        //[DataRow(
        //    "{a:123,b:'abc'}",
        //    "object",
        //    "{ a: int, b: string }",
        //    "{ a: 123, b: 'abc' }")]
        //[DataRow(
        //    "{ 'my type': 'my string' }",
        //    "object",
        //    "{ 'my type': string }",
        //    "{ 'my type': 'my string' }")]
        //[DataRow(
        //    "{ 'true': true }",
        //    "object",
        //    "{ 'true': bool }",
        //    "{ 'true': true }")]
        //[DataRow(
        //    "object",
        //    "object",
        //    "object",
        //    "object")]
        //[DataRow(
        //    "null",
        //    "null",
        //    "null",
        //    "null")]
        //[DataRow(
        //    "'abc' | 'def' | 'ghi'",
        //    "string",
        //    "'abc' | 'def' | 'ghi'",
        //    "'abc' | 'def' | 'ghi'")]
        //[DataRow(
        //    "1 | 2 | 3 | -1",
        //    "int",
        //    "-1 | 1 | 2 | 3",
        //    "-1 | 1 | 2 | 3")]
        //[DataRow(
        //    "true|false",
        //    "bool",
        //    "false | true",
        //    "false | true")]
        [DataRow(
            "[int, string]",
            "array",
            "[int, string]",
            "[int, string]")]
        [DataRow(
            "[123, 'abc' | 'def']",
            "array",
            "[int, 'abc' | 'def']",
            "[123, 'abc' | 'def']")]

        [DataRow(
            "",
            "",
            "",
            "")]
        public void GetSyntaxForType(string inputType, string expectedLooseSyntax, string expectedMediumSyntax, string expectedStrictSyntax)
        {
            var compilationResult = CompilationHelper.Compile($"type t = {inputType}");
            var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
            var declarationSyntax = semanticModel.Root.TypeDeclarations[0].DeclaringSyntax;
            var declaredType = semanticModel.GetTypeInfo(semanticModel.Root.TypeDeclarations[0].Value);

            var looseSyntax = SyntaxForType.GetSyntaxStringForType(declaredType, SyntaxForType.Strictness.Loose);
            var mediumSyntax = SyntaxForType.GetSyntaxStringForType(declaredType, SyntaxForType.Strictness.Medium);
            var strictSyntax = SyntaxForType.GetSyntaxStringForType(declaredType, SyntaxForType.Strictness.Strict);

            looseSyntax.Should().Be(expectedLooseSyntax);
            mediumSyntax.Should().Be(expectedMediumSyntax);
            strictSyntax.Should().Be(expectedStrictSyntax);

            //Trace.WriteLine("asdfg1 " + declarationSyntax.GetDebuggerDisplay());
            //Trace.WriteLine("asdfg3 " + looseSyntax);
            //Trace.WriteLine("asdfg2 " + declaredType.Name);
            //Trace.WriteLine("asdfg4 " + declaredType.FormatNameForCompoundTypes());

            var nameSyntax = declaredType.Name;
            // doesn't work for {'true': true}: nameSyntax.Should().EqualIgnoringWhitespace(expectedStrictSyntax); //asdfg?
        }
    }
}
