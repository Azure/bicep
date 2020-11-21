// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void AllUnaryOperatorsShouldValidText()
        {
            RunTextTest(Operators.UnaryOperatorToText);
        }

        [TestMethod]
        public void AllBinaryOperatorsShouldHaveValidText()
        {
            RunTextTest(Operators.BinaryOperatorToText);
        }

        [TestMethod]
        public void AllUnaryOperatorsShouldMapToTokens()
        {
            RunTokenTest(Operators.TokenTypeToUnaryOperator);
        }

        [TestMethod]
        public void AllBinaryOperatorsShouldMapToTokens()
        {
            RunTokenTest(Operators.TokenTypeToBinaryOperator);
        }

        private static void RunTextTest<TEnum>(IDictionary<TEnum,string> data) where TEnum : struct
        {
            foreach (TEnum @operator in GetValues<TEnum>())
            {
                data[@operator].Should().NotBeNullOrWhiteSpace($"because the {@operator} operator should have text");
            }

            var duplicatedTexts = data.Values
                .GroupBy(text => text)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToList();

            duplicatedTexts.Should().BeEmpty($"because operators {duplicatedTexts.ConcatString(", ")} should have unique text");
        }

        private static void RunTokenTest<TEnum>(IDictionary<TokenType, TEnum> data) where TEnum : struct
        {
            data.Values.Should().BeEquivalentTo(GetValues<TEnum>(), "because there should be a mapping from a token type to every operator");
        }

        private static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).OfType<TEnum>();
        }
    }
}

