using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [DataRow(
            "123",
            "123",
            "int",
            "int")]
        [DataRow(
            "{a:int,b:123}",
            "{a:int,b:123}",
            "{a:int,b:int}",
            "object")]
        [DataRow(
            "{'my type': string}",
            "{a:int,b:123}",
            "{a:int,b:int}",
            "object")]
        public void GetSyntaxForType(string inputType, string expectedLooseSyntax, string expectedMediumSyntax, string expectedStrictSyntax)
        {
            var compilationResult = CompilationHelper.Compile($"type t = {inputType}");
            var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
            var declarationSyntax = semanticModel.Root.TypeDeclarations[0].DeclaringSyntax;
            var declaredType = semanticModel.GetTypeInfo(semanticModel.Root.TypeDeclarations[0].Value);

            var looseSyntax = SyntaxForType.GetSyntaxForType(declaredType, SyntaxForType.Strictness.Loose);
            var mediumSyntax = SyntaxForType.GetSyntaxForType(declaredType, SyntaxForType.Strictness.Loose);
            var strictSyntax = SyntaxForType.GetSyntaxForType(declaredType, SyntaxForType.Strictness.Loose);

            //asdfg looseSyntax.Should().Be(expectedLooseSyntax);
            

            Trace.WriteLine("asdfg1 " + declarationSyntax.GetDebuggerDisplay());
            Trace.WriteLine("asdfg3 " + looseSyntax);
            Trace.WriteLine("asdfg2 " + declaredType.Name);
            Trace.WriteLine("asdfg4 " + declaredType.FormatNameForCompoundTypes());
        }
    }
}
