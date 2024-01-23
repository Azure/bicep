// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public static class Operators
    {
        public static readonly ImmutableDictionary<TokenType, BinaryOperator> TokenTypeToBinaryOperator = new Dictionary<TokenType, BinaryOperator>
        {
            [TokenType.LogicalOr] = BinaryOperator.LogicalOr,
            [TokenType.LogicalAnd] = BinaryOperator.LogicalAnd,
            [TokenType.Equals] = BinaryOperator.Equals,
            [TokenType.NotEquals] = BinaryOperator.NotEquals,
            [TokenType.EqualsInsensitive] = BinaryOperator.EqualsInsensitive,
            [TokenType.NotEqualsInsensitive] = BinaryOperator.NotEqualsInsensitive,
            [TokenType.LeftChevron] = BinaryOperator.LessThan,
            [TokenType.LessThanOrEqual] = BinaryOperator.LessThanOrEqual,
            [TokenType.RightChevron] = BinaryOperator.GreaterThan,
            [TokenType.GreaterThanOrEqual] = BinaryOperator.GreaterThanOrEqual,
            [TokenType.Plus] = BinaryOperator.Add,
            [TokenType.Minus] = BinaryOperator.Subtract,
            [TokenType.Asterisk] = BinaryOperator.Multiply,
            [TokenType.Slash] = BinaryOperator.Divide,
            [TokenType.Modulo] = BinaryOperator.Modulo,
            [TokenType.DoubleQuestion] = BinaryOperator.Coalesce
        }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<TokenType, UnaryOperator> TokenTypeToUnaryOperator = new Dictionary<TokenType, UnaryOperator>
        {
            [TokenType.Exclamation] = UnaryOperator.Not,
            [TokenType.Minus] = UnaryOperator.Minus
        }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<UnaryOperator, string> UnaryOperatorToText = new Dictionary<UnaryOperator, string>
        {
            [UnaryOperator.Minus] = "-",
            [UnaryOperator.Not] = "!"
        }.ToImmutableDictionary();
    }
}
