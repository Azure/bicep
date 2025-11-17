// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics
{
    [TestClass]
    public class SymbolContextTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        [TestMethod]
        public void LockedModeShouldBlockAccess()
        {
            const string expectedMessage = "Properties of the symbol context should not be accessed until name binding is completed.";

            var compilation = Services.BuildCompilation("");
            var model = compilation.GetEntrypointSemanticModel();
            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var cyclesBySymbol = new Dictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>>();
            var context = new SymbolContext(model, model.SourceFileGrouping, compilation, model.ArtifactReferenceFactory, model.SourceFile);

            FluentActions.Invoking(() => context.TypeManager)
                .Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);
            FluentActions.Invoking(() => context.Binder)
                .Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);

            context.Unlock();
            context.TypeManager.Should().NotBeNull();
            context.Binder.Should().NotBeNull();
        }

        [TestMethod]
        public void TestHoverOnQuotedPropertyReturnsPropertySymbol()
        {
            var text = @"
resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'mystorageaccount'
  location: 'eastus'
  sku: {
    'name': 'Standard_LRS'
  }
}
";
            var skuIndex = text.IndexOf("sku:");
            var namePropertyIndex = text.IndexOf("'name'", skuIndex);
            var cursor = namePropertyIndex + 2;

            var compilation = CompilationHelper.Compile(text).Compilation;
            var model = compilation.GetEntrypointSemanticModel();

            var programSyntax = model.SourceFile.ProgramSyntax;

            var node = programSyntax.TryFindMostSpecificNodeInclusive(
                cursor,
                n => n is not IdentifierSyntax && n is not Token && n is not AliasAsClauseSyntax);

            node.Should().NotBeNull($"cursor at position {cursor} should find a node");
            var symbol = model.GetSymbolInfo(node!);

            symbol.Should().NotBeNull("hovering over a quoted property name should return a symbol");
            symbol.Should().BeOfType<PropertySymbol>("quoted property name should resolve to PropertySymbol");
            symbol!.Name.Should().Be("name");
        }
    }
}
