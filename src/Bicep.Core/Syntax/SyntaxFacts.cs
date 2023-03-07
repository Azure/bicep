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
        public static bool IsSingleLineComment(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.SingleLineComment;

        public static bool IsMultiLineComment(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.MultiLineComment;

        public static bool IsComment(this SyntaxTrivia? trivia) => IsSingleLineComment(trivia) || IsMultiLineComment(trivia);

        public static CommentStickiness GetCommentStickiness(this Token token) => token.Type.GetCommentStickiness();

        public static CommentStickiness GetCommentStickiness(this TokenType type) => type switch
        {
            // Minus is included because negative numbers can have leading comments.
            TokenType.Minus or
            TokenType.EndOfFile or
            TokenType.LeftParen or
            TokenType.LeftSquare or
            TokenType.LeftBrace or
            TokenType.StringLeftPiece => CommentStickiness.Leading,

            TokenType.RightParen or
            TokenType.RightSquare or
            TokenType.RightBrace or
            TokenType.StringRightPiece => CommentStickiness.Trailing,

            TokenType.NewLine or
            TokenType.Exclamation or
            TokenType.FalseKeyword or
            TokenType.TrueKeyword or
            TokenType.NullKeyword or
            TokenType.StringComplete or
            TokenType.Integer or
            TokenType.Identifier => CommentStickiness.Bidirectional,

            _ => CommentStickiness.None,
        };

        public static bool IsOf(this Token token, TokenType type) => token.Type == type;
    }
}
