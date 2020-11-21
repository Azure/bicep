// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Syntax
{
    [TestClass]
    public class SeparatedSyntaxListTests
    {
        [TestMethod]
        public void EmptyListMustHaveZeroLengthSpan()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action wrongSpan = () => new SeparatedSyntaxList(Enumerable.Empty<SyntaxBase>(), Enumerable.Empty<Token>(), new TextSpan(53, 1));
            wrongSpan.Should().Throw<ArgumentException>().WithMessage("The specified span was '[53:54]' but expected a zero-length span.");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(100)]
        [DataTestMethod]
        public void EmptyListMustHaveZeroSeparators(int separatorCount)
        {
            var separators = Enumerable.Repeat(TestSyntaxFactory.CreateToken(TokenType.Colon), separatorCount);

            // ReSharper disable once ObjectCreationAsStatement
            Action wrongSeparators = () => new SeparatedSyntaxList(Enumerable.Empty<SyntaxBase>(), separators, new TextSpan(53, 1));
            wrongSeparators.Should().Throw<ArgumentException>().WithMessage("With zero elements, the number of separators must also be zero.");
        }

        [DataRow(1, 5)]
        [DataRow(2, 0)]
        [DataRow(4, 1)]
        [DataRow(2, 3)]
        [DataTestMethod]
        public void ListMustHaveOneFewerSeparatorThanElements(int elementCount, int separatorCount)
        {
            var elements = Enumerable.Repeat(TestSyntaxFactory.CreateInt(42), elementCount);
            var separators = Enumerable.Repeat(TestSyntaxFactory.CreateToken(TokenType.Colon), separatorCount);

            Action wrongSeparators = () => new SeparatedSyntaxList(elements, separators, new TextSpan(0, 0));
            wrongSeparators.Should().Throw<ArgumentException>().WithMessage($"The number of separators ({separatorCount}) must be the same or one less than the number of elements ({elementCount}).");
        }
    }
}
