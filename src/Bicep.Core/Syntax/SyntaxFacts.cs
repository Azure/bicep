// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

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
            TokenType.LeftChevron => "<",
            TokenType.RightChevron => ">",
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
            TokenType.Ellipsis => "...",
            TokenType.Hat => "^",
            TokenType.Dollar => "$",
            _ => null,
        };

        public static CommentStickiness GetCommentStickiness(TokenType type) => type switch
        {
            // Any token that can have leading comments attached
            // to it has leading comment stickiness.
            //
            // The NewLine token is included because there may
            // exists dangling comments that are not attached
            // to a statement or an expression.
            //
            // The Pipe token is included because union types can have
            // a optional leading pipe , for example:
            //
            // type MyString = /* leading comment */ | 'foo' | 'bar'
            TokenType.NewLine or
            TokenType.Pipe or
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
            //
            // Trailing comment is sticky to Question because it can be used as 1) a nullable type marker
            // after a type identifier; 2) a placeholder with a trailing TODO comment in decompiled code.
            TokenType.RightParen or
            TokenType.RightSquare or
            TokenType.RightBrace or
            TokenType.StringRightPiece or
            TokenType.Question => CommentStickiness.Trailing,

            // The following tokens may have comments attached
            // to both sides. Exclamation is included because
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

        public static bool HasCommentOrDirective(this Token token) => token.LeadingTrivia.Any(x => !x.IsWhitespace()) || token.TrailingTrivia.Any(x => !x.IsWhitespace());

        public static bool HasTrailingLineComment(this Token token) => token.TrailingTrivia.Any(IsSingleLineComment);

        public static bool IsKeyword(this Token token, string keyword) => token.IsOf(TokenType.Identifier) && IdentifierEquals(token.Text, keyword);

        public static bool IsOf(this Token token, TokenType type) => token.Type == type;

        public static bool IsOf(this SyntaxTrivia trivia, SyntaxTriviaType type) => trivia.Type == type;

        public static bool IsSingleLineComment(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.SingleLineComment;

        public static bool IsMultiLineComment(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.MultiLineComment;

        public static bool IsComment(this SyntaxTrivia? trivia) => IsSingleLineComment(trivia) || IsMultiLineComment(trivia);

        public static bool IsDirective(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.DisableNextLineDiagnosticsDirective;

        public static bool IsWhitespace(this SyntaxTrivia? trivia) => trivia?.Type == SyntaxTriviaType.Whitespace;

        public static bool NameEquals(this ISymbolReference symbolRef, string compareTo) => IdentifierEquals(symbolRef.Name.IdentifierName, compareTo);

        public static bool NameEquals(this IdentifierSyntax identifier, string compareTo) => IdentifierEquals(identifier.IdentifierName, compareTo);

        private static bool IdentifierEquals(string x, string y) => LanguageConstants.IdentifierComparer.Equals(x, y);
    }
}
