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
        public static bool HasFreeFromText(TokenType type) => type switch
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

        public static CommentStickiness GetCommentStickiness(TokenType type) => type switch
        {
            // Any token that can have leading comments attached
            // to it has leading comment stickiness.
            // The NewLine token is included because there may
            // exists dangling comments that are not attached
            // to a statment or an expression.
            TokenType.NewLine or
            TokenType.At or
            TokenType.Minus or
            TokenType.EndOfFile or
            TokenType.LeftParen or
            TokenType.LeftSquare or
            TokenType.LeftBrace or
            TokenType.StringLeftPiece => CommentStickiness.Leading,

            // Tokens like right brackets and StringRightPiece can
            // only appear at the right of an expression so they
            // have trailing comment stickiness. Right brackets may
            // also have dangling leading comments attached, for example:
            //   var foo = [
            //     true
            //     /* dangling comment */ ]
            TokenType.RightParen or
            TokenType.RightSquare or
            TokenType.RightBrace or
            TokenType.StringRightPiece => CommentStickiness.Trailing,

            // The following tokens may have comments attached
            // to both sides. Exlamation is included because
            // it can have leading comments when used as a
            // not operator before an exapression, and it can
            // have trailing comments when used as a non-null
            // assertion operator.
            TokenType.Exclamation or
            TokenType.FalseKeyword or
            TokenType.TrueKeyword or
            TokenType.NullKeyword or
            TokenType.StringComplete or
            TokenType.Integer or
            TokenType.Identifier => CommentStickiness.Bidirectional,

            // It does not make sense for the rest of the tokens
            // to have comments attached to them.
            _ => CommentStickiness.None,
        };

        public static bool IsOf(this SyntaxTrivia trivia, SyntaxTriviaType type) => trivia.Type == type;
    }
}
