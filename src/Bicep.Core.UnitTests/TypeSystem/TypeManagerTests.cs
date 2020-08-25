// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class TypeManagerTests
    {
        [TestMethod]
        public void UnlockedModeShouldBlockTypeQueries()
        {
            const string expectedMessage = "Type checks are blocked until name binding is completed.";

            var tm =new TypeManager(new Dictionary<SyntaxBase, Symbol>());

            Action byName = () => tm.GetTypeByName("string");
            byName.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);

            Action byNode = () => tm.GetTypeInfo(TestSyntaxFactory.CreateBool(true), new TypeManagerContext());
            byNode.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);
        }
    }
}

