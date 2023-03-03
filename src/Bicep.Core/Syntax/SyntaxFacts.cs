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

        public static CommentStickiness GetCommentStickiness(this TokenType tokenType) => tokenType switch
        {
            TokenType.NewLine or
            TokenType.Comma => CommentStickiness.None,

            TokenType.FalseKeyword or
            TokenType.TrueKeyword or
            TokenType.NullKeyword or
            TokenType.StringComplete or
            TokenType.Integer or
            TokenType.Identifier => CommentStickiness.Bidirectional,

            TokenType.RightParen or
            TokenType.RightSquare or
            TokenType.RightBrace or
            TokenType.StringRightPiece => CommentStickiness.Trailing,

            _ => CommentStickiness.Leading,
        };

        public static bool IsOf(this Token token, TokenType type) => token.Type == type;

        public static bool IsOneOf(this Token token, TokenType firstType, TokenType secondType, params TokenType[] types) =>
            types.Append(firstType).Append(secondType).Any(x => token.Type == x);

        public static bool IsMultiLineNewLine(this Token token) => token.IsOf(TokenType.NewLine) && StringUtils.CountNewlines(token.Text) > 1;

        public static bool HasProperties(this ObjectSyntax syntax) => syntax.Properties.Any();
    }
}
