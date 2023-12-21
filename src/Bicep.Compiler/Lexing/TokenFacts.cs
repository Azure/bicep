// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;

namespace Bicep.Compiler.Lexing;

public static class TokenFacts
{
    public static string? TryGetText(TokenKind kind) => kind switch
    {
        TokenKind.OpenParen => "(",
        TokenKind.CloseParen => ")",
        TokenKind.OpenBrace => "{",
        TokenKind.CloseBrace => "}",
        TokenKind.OpenBracket => "[",
        TokenKind.CloseBracket => "]",
        TokenKind.Assignment => "=",
        TokenKind.Comma => ",",
        TokenKind.Plus => "+",
        TokenKind.Minus => "-",
        TokenKind.Asterisk => "*",
        TokenKind.Slash => "/",
        TokenKind.Colon => ":",
        TokenKind.Exclamation => "!",
        TokenKind.Question => "?",
        TokenKind.LessThan => "<",
        TokenKind.GreaterThan => ">",
        TokenKind.Equals => "==",
        TokenKind.NotEquals => "!=",
        TokenKind.LessThanOrEquals =>  "<=",
        TokenKind.GreaterThanOrEquals => ">=",
        TokenKind.EqualsInsensitive => "=~",
        TokenKind.NotEqualsInsensitive => "!~",
        TokenKind.LogicalAnd => "&&",
        TokenKind.LogicalOr => "||",

        _ => null,
    };
}
