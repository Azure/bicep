// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public static class SyntaxFactory
    {
        public static readonly TextSpan EmptySpan = new TextSpan(0, 0);

        public static readonly IEnumerable<SyntaxTrivia> EmptyTrivia = Enumerable.Empty<SyntaxTrivia>();

        public static Token CreateToken(TokenType tokenType, string text)
            => new Token(tokenType, EmptySpan, text, EmptyTrivia, EmptyTrivia);

        public static IdentifierSyntax CreateIdentifier(string text)
            => new IdentifierSyntax(CreateToken(TokenType.Identifier, text));

        public static Token NewlineToken
            => CreateToken(TokenType.NewLine, Environment.NewLine);

        public static ObjectPropertySyntax CreateObjectProperty(string key, SyntaxBase value)
            => new ObjectPropertySyntax(CreateObjectPropertyKey(key), CreateToken(TokenType.Colon, ":"), value);

        public static ObjectSyntax CreateObject(IEnumerable<ObjectPropertySyntax> properties)
        {
            var children = new List<SyntaxBase> { NewlineToken };

            foreach (var property in properties)
            {
                children.Add(property);
                children.Add(NewlineToken);
            }

            return new ObjectSyntax(
                CreateToken(TokenType.LeftBrace, "{"),
                children,
                CreateToken(TokenType.RightBrace, "}"));
        }

        public static ArrayItemSyntax CreateArrayItem(SyntaxBase value)
            => new ArrayItemSyntax(value);

        public static ArraySyntax CreateArray(IEnumerable<SyntaxBase> items)
        {
            var children = new List<SyntaxBase> { NewlineToken };

            foreach (var item in items)
            {
                children.Add(CreateArrayItem(item));
                children.Add(NewlineToken);
            }

            return new ArraySyntax(
                CreateToken(TokenType.LeftSquare, "["),
                children,
                CreateToken(TokenType.RightSquare, "]"));
        }

        public static SyntaxBase CreateObjectPropertyKey(string text)
        {
            if (Regex.IsMatch(text, "^[a-zA-Z][a-zA-Z0-9_]*$"))
            {
                return CreateIdentifier(text);
            }

            return CreateStringLiteral(text);
        }

        public static StringSyntax CreateStringLiteral(string value)
        {
            return new StringSyntax(CreateStringLiteralToken(value).AsEnumerable(), Enumerable.Empty<SyntaxBase>(), value.AsEnumerable());
        }

        public static StringSyntax CreateStringLiteralWithComment(string value, string comment)
        {
            var trailingTrivia = new SyntaxTrivia(SyntaxTriviaType.MultiLineComment, EmptySpan, $"/*{comment.Replace("*/", "*\\/")}*/");
            var stringToken = new Token(TokenType.StringComplete, EmptySpan, $"'{EscapeBicepString(value)}'", EmptyTrivia, trailingTrivia.AsEnumerable());

            return new StringSyntax(stringToken.AsEnumerable(), Enumerable.Empty<SyntaxBase>(), value.AsEnumerable());
        }

        public static Token CreateStringLiteralToken(string value)
        {
            return CreateToken(TokenType.StringComplete, $"'{EscapeBicepString(value)}'");
        }

        public static Token CreateStringInterpolationToken(bool isStart, bool isEnd, string value)
        {
            if (isStart)
            {
                return CreateToken(TokenType.StringLeftPiece, $"'{EscapeBicepString(value)}${{");
            }

            if (isEnd)
            {
                return CreateToken(TokenType.StringRightPiece, $"}}{EscapeBicepString(value)}'");
            }

            return CreateToken(TokenType.StringMiddlePiece, $"}}{EscapeBicepString(value)}${{");
        }

        private static string EscapeBicepString(string value)
            => value
            .Replace("\\", "\\\\") // must do this first!
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("${", "\\${")
            .Replace("'", "\\'");
    }
}
