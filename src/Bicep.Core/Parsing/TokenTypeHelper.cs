// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Parsing;

public static class TokenTypeHelper
{
    public static int GetOperatorPrecedence(TokenType tokenType) => tokenType switch
    {
        // the absolute values are not important here
        TokenType.Modulo or
        TokenType.Asterisk or
        TokenType.Slash => 100,

        TokenType.Plus or
        TokenType.Minus => 90,

        TokenType.RightChevron or
        TokenType.GreaterThanOrEqual or
        TokenType.LeftChevron or
        TokenType.LessThanOrEqual => 80,

        TokenType.Equals or
        TokenType.NotEquals or
        TokenType.EqualsInsensitive or
        TokenType.NotEqualsInsensitive => 70,

        // if we add bitwise operators in the future, they should go here
        TokenType.LogicalAnd => 50,
        TokenType.LogicalOr => 40,
        TokenType.DoubleQuestion => 30,

        _ => -1,
    };
}
