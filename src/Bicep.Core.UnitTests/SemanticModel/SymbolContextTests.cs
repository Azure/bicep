// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
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

            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var cyclesBySyntax = new Dictionary<SyntaxBase, DeclaredSymbol[]>();
            var context = new SymbolContext(new TypeManager(bindings, cyclesBySyntax), bindings);

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
