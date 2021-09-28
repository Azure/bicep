// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Emit
{
    [TestClass]
    public class InlineDependencyVisitorTests
    {
        private const string Text = @"
var things = ''
var keys = listKeys('fake','fake')
var indirection = concat(things, keys)

var runtimeLoop = [for (item, index) in []: indirection]
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
";
        [TestMethod]
        public void VisitorShouldCalculateInliningInBulk()
        {
            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateFromText(Text, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);

            var inlineVariables = InlineDependencyVisitor.GetVariablesToInline(compilation.GetEntrypointSemanticModel());

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
        public void VisitorShouldProduceNoChainForNonInlinedVariables(string variableName)
        {
            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateFromText(Text, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            VariableDeclarationSyntax variable = GetVariableByName(compilation, variableName);

            InlineDependencyVisitor.ShouldInlineVariable(compilation.GetEntrypointSemanticModel(), variable, out var chain).Should().BeFalse();
            chain.Should().BeEmpty();
        }

        [DataRow("keys", "")]
        [DataRow("indirection", "keys")]
        [DataRow("runtimeLoop", "indirection,keys")]
        [DataRow("runtimeLoop2", "indirection,keys")]
        [DataTestMethod]
        public void VisitorShouldProduceCorrectChainForInlinedVariables(string variableName, string expectedChain)
        {
            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateFromText(Text, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
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
