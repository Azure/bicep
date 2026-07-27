// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Testing.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Emit
{
    [TestClass]
    public class InlineDependencyVisitorTests
    {
        private static TestCompiler CreateCompiler() => TestCompiler.ForInMemoryCompilation()
            .WithEmptyAzResources();

        private const string Text = @"
var things = ''
var keys = listKeys('fake','fake')
var indirection = concat(things, keys)

var runtimeLoop = [for (item, index) in []: indirection]
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
";
        [TestMethod]
        public async Task VisitorShouldCalculateInliningInBulk()
        {
            var compilation = (await CreateCompiler().CompileWithoutRestore(Text)).Compilation;

            var inlineVariables = InlineDependencyVisitor.GetSymbolsToInline(compilation.GetEntrypointSemanticModel()).VariablesToInline;

            inlineVariables.Should().Contain(new[]
            {
                GetVariableSymbolByName(compilation, "keys"),
                GetVariableSymbolByName(compilation, "indirection"),
                GetVariableSymbolByName(compilation, "runtimeLoop"),
                GetVariableSymbolByName(compilation, "runtimeLoop2")
            });
        }

        [DataRow("things")]
        [DataTestMethod]
        public async Task VisitorShouldProduceNoChainForNonInlinedVariables(string variableName)
        {
            var compilation = (await CreateCompiler().CompileWithoutRestore(Text)).Compilation;
            VariableDeclarationSyntax variable = GetVariableByName(compilation, variableName);

            InlineDependencyVisitor.ShouldInlineVariable(compilation.GetEntrypointSemanticModel(), variable, out var chain).Should().BeFalse();
            chain.Should().BeEmpty();
        }

        [DataRow("keys", "")]
        [DataRow("indirection", "keys")]
        [DataRow("runtimeLoop", "indirection,keys")]
        [DataRow("runtimeLoop2", "indirection,keys")]
        [DataTestMethod]
        public async Task VisitorShouldProduceCorrectChainForInlinedVariables(string variableName, string expectedChain)
        {
            var compilation = (await CreateCompiler().CompileWithoutRestore(Text)).Compilation;
            VariableDeclarationSyntax variable = GetVariableByName(compilation, variableName);

            InlineDependencyVisitor.ShouldInlineVariable(compilation.GetEntrypointSemanticModel(), variable, out var chain).Should().BeTrue();
            chain.Should().NotBeNull();

            var actualChain = string.Join(',', (IEnumerable<string>)chain!);
            actualChain.Should().Be(expectedChain);
        }

        private static VariableDeclarationSyntax GetVariableByName(Compilation compilation, string variableName) =>
            compilation.SourceFileGrouping.EntryPoint.ProgramSyntax.Declarations
                .OfType<VariableDeclarationSyntax>()
                .First(v => string.Equals(v.Name.IdentifierName, variableName, LanguageConstants.IdentifierComparison));

        private static VariableSymbol GetVariableSymbolByName(Compilation compilation, string variableName) =>
            compilation.GetEntrypointSemanticModel().Root.Declarations
                .OfType<VariableSymbol>()
                .First(v => string.Equals(v.Name, variableName, LanguageConstants.IdentifierComparison));
    }
}
