// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Syntax
{
    public static class SyntaxFacts
    {
        public static bool IsFreeform(TokenType type) => type switch
        {
            TokenType.NewLine or
            TokenType.Identifier or
            TokenType.Integer or
            TokenType.StringLeftPiece or
            TokenType.StringMiddlePiece or
            TokenType.StringRightPiece or
            TokenType.StringComplete or
            TokenType.MultilineString or
            TokenType.Unrecognized => true,
            _ => false,
        };

        public static string? GetText(TokenType type) => type switch
        {
            TokenType.At => "@",
            TokenType.LeftBrace => "{",
            TokenType.RightBrace => "}",
            TokenType.LeftParen => "(",
            TokenType.RightParen => ")",
            TokenType.LeftSquare => "[",
            TokenType.RightSquare => "]",
            TokenType.Comma => ",",
            TokenType.Dot => ".",
            TokenType.Question => "?",
            TokenType.Colon => ":",
            TokenType.Semicolon => ";",
            TokenType.Assignment => "=",
            TokenType.Plus => "+",
            TokenType.Minus => "-",
            TokenType.Asterisk => "*",
            TokenType.Slash => "/",
            TokenType.Modulo => "%",
            TokenType.Exclamation => "!",
            TokenType.LessThan => "<",
            TokenType.GreaterThan => ">",
            TokenType.LessThanOrEqual => "<=",
            TokenType.GreaterThanOrEqual => ">=",
            TokenType.Equals => "==",
            TokenType.NotEquals => "!=",
            TokenType.EqualsInsensitive => "=~",
            TokenType.NotEqualsInsensitive => "!~",
            TokenType.LogicalAnd => "&&",
            TokenType.LogicalOr => "||",
            TokenType.TrueKeyword => "true",
            TokenType.FalseKeyword => "false",
            TokenType.NullKeyword => "null",
            TokenType.EndOfFile => "",
            TokenType.DoubleQuestion => "??",
            TokenType.DoubleColon => "::",
            TokenType.Arrow => "=>",
            TokenType.Pipe => "|",
            TokenType.WithKeyword => "with",
            TokenType.AsKeyword => "as",
            _ => null,
        };
    }
}
