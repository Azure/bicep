// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Bicep.Core.UnitTests.Syntax
{
    [TestClass]
    public class SyntaxFactsTests
    {
        [TestMethod]
        public void GetText_FreeformTokenType_ReturnsNull()
        {
            var texts = Enum.GetValues<TokenType>()
                .Where(SyntaxFacts.HasFreeFromText)
                .Select(SyntaxFacts.GetText);

            texts.Should().AllBe(null);
        }

        [TestMethod]
        public void GetText_NonFreeformTokenType_ReturnsText()
        {
            var texts = Enum.GetValues<TokenType>()
                .Where(x => !SyntaxFacts.HasFreeFromText(x))
                .Select(SyntaxFacts.GetText);

            texts.Should().AllSatisfy(x => x.Should().NotBe(null));
        }
    }
}
