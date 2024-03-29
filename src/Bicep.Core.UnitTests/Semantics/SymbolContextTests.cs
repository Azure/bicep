// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
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
            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var cyclesBySymbol = new Dictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>>();
            var context = new SymbolContext(compilation, compilation.GetEntrypointSemanticModel());

            Action byName = () =>
            {
                var tm = context.TypeManager;
            };
            byName.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);

            Action byNode = () =>
            {
                var b = context.Compilation;
            };
            byNode.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);

            context.Unlock();
            context.TypeManager.Should().NotBeNull();
            context.Compilation.Should().NotBeNull();
        }
    }
}
