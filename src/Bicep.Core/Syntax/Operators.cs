// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
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
            [TokenType.LessThan] = BinaryOperator.LessThan,
            [TokenType.LessThanOrEqual] = BinaryOperator.LessThanOrEqual,
            [TokenType.GreaterThan] = BinaryOperator.GreaterThan,
            [TokenType.GreaterThanOrEqual] = BinaryOperator.GreaterThanOrEqual,
            [TokenType.Plus] = BinaryOperator.Add,
            [TokenType.Minus] = BinaryOperator.Subtract,
            [TokenType.Asterisk] = BinaryOperator.Multiply,
            [TokenType.Slash] = BinaryOperator.Divide,
            [TokenType.Modulo] = BinaryOperator.Modulo,
            [TokenType.DoubleQuestion] = BinaryOperator.Coalesce
        }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<BinaryOperator, string> BinaryOperatorToText = new Dictionary<BinaryOperator, string>
        {
            [BinaryOperator.LogicalOr] = "||",
            [BinaryOperator.LogicalAnd] = "&&",
            [BinaryOperator.Equals] = "==",
            [BinaryOperator.NotEquals] = "!=",
            [BinaryOperator.EqualsInsensitive] = "=~",
            [BinaryOperator.NotEqualsInsensitive] = "!~",
            [BinaryOperator.LessThan] = "<",
            [BinaryOperator.LessThanOrEqual] = "<=",
            [BinaryOperator.GreaterThan] = ">",
            [BinaryOperator.GreaterThanOrEqual] = ">=",
            [BinaryOperator.Add] = "+",
            [BinaryOperator.Subtract] = "-",
            [BinaryOperator.Multiply] = "*",
            [BinaryOperator.Divide] = "/",
            [BinaryOperator.Modulo] = "%",
            [BinaryOperator.Coalesce] = "??"
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

