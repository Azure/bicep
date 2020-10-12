// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class SymbolContextTests
    {
        [TestMethod]
        public void LockedModeShouldBlockAccess()
        {
            const string expectedMessage = "Properties of the symbol context should not be accessed until name binding is completed.";

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(""));
            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var cyclesBySymbol = new Dictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>>();
            var context = new SymbolContext(new TypeManager(TestResourceTypeProvider.Create(), bindings, cyclesBySymbol, new SyntaxHierarchy()), bindings, compilation);

            Action byName = () =>
            {
                var tm = context.TypeManager;
            };
            byName.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);

            Action byNode = () =>
            {
                var b = context.Bindings;
            };
            byNode.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);

            context.Unlock();
            context.TypeManager.Should().NotBeNull();
            context.Bindings.Should().NotBeNull();
        }
    }
}
