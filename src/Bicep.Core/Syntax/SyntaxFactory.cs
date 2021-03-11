﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static Token CreateToken(TokenType tokenType, string text = "")
            => new Token(tokenType, EmptySpan, string.IsNullOrEmpty(text) ? TryGetTokenText(tokenType) : text, EmptyTrivia, EmptyTrivia);

        public static IdentifierSyntax CreateIdentifier(string text)
            => new IdentifierSyntax(CreateToken(TokenType.Identifier, text));

        public static Token NewlineToken => CreateToken(TokenType.NewLine, Environment.NewLine);
        public static Token AtToken => CreateToken(TokenType.At, "@");
        public static Token LeftBraceToken => CreateToken(TokenType.LeftBrace, "{");
        public static Token RightBraceToken => CreateToken(TokenType.RightBrace, "}");
        public static Token LeftParenToken => CreateToken(TokenType.LeftParen, "(");
        public static Token RightParenToken => CreateToken(TokenType.RightParen, ")");
        public static Token LeftSquareToken => CreateToken(TokenType.LeftSquare, "[");
        public static Token RightSquareToken => CreateToken(TokenType.RightSquare, "]");
        public static Token CommaToken => CreateToken(TokenType.Comma, ",");
        public static Token DotToken => CreateToken(TokenType.Dot, ".");
        public static Token QuestionToken => CreateToken(TokenType.Question, "?");
        public static Token ColonToken => CreateToken(TokenType.Colon, ":");
        public static Token SemicolonToken => CreateToken(TokenType.Semicolon, ";");
        public static Token AssignmentToken => CreateToken(TokenType.Assignment, "=");
        public static Token PlusToken => CreateToken(TokenType.Plus, "+");
        public static Token MinusToken => CreateToken(TokenType.Minus, "-");
        public static Token AsteriskToken => CreateToken(TokenType.Asterisk, "*");
        public static Token SlashToken => CreateToken(TokenType.Slash, "/");
        public static Token ModuloToken => CreateToken(TokenType.Modulo, "%");
        public static Token ExclamationToken => CreateToken(TokenType.Exclamation, "!");
        public static Token LessThanToken => CreateToken(TokenType.LessThan, "<");
        public static Token GreaterThanToken => CreateToken(TokenType.GreaterThan, ">");
        public static Token LessThanOrEqualToken => CreateToken(TokenType.LessThanOrEqual, "<=");
        public static Token GreaterThanOrEqualToken => CreateToken(TokenType.GreaterThanOrEqual, ">=");
        public static Token EqualsToken => CreateToken(TokenType.Equals, "==");
        public static Token NotEqualsToken => CreateToken(TokenType.NotEquals, "!=");
        public static Token EqualsInsensitiveToken => CreateToken(TokenType.EqualsInsensitive, "=~");
        public static Token NotEqualsInsensitiveToken => CreateToken(TokenType.NotEqualsInsensitive, "!~");
        public static Token LogicalAndToken => CreateToken(TokenType.LogicalAnd, "&&");
        public static Token LogicalOrToken => CreateToken(TokenType.LogicalOr, "||");
        public static Token TrueKeywordToken => CreateToken(TokenType.TrueKeyword, "true");
        public static Token FalseKeywordToken => CreateToken(TokenType.FalseKeyword, "false");
        public static Token NullKeywordToken => CreateToken(TokenType.NullKeyword, "null");

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
                LeftBraceToken,
                children,
                RightBraceToken);
        }

        public static ArrayItemSyntax CreateArrayItem(SyntaxBase value)
            => new ArrayItemSyntax(value);

        public static ForSyntax CreateRangedForSyntax(string indexIdentifier, SyntaxBase count, SyntaxBase body)
        {
            // generates "range(0, <count>)"
            var rangeSyntax = new FunctionCallSyntax(
                CreateIdentifier("range"),
                LeftParenToken,
                new FunctionArgumentSyntax[] {
                    new FunctionArgumentSyntax(
                        new IntegerLiteralSyntax(CreateToken(TokenType.Integer, "0"), 0),
                        CommaToken),
                    new FunctionArgumentSyntax(count, null),
                },
                RightParenToken);

            return CreateForSyntax(indexIdentifier, rangeSyntax, body);
        }

        public static ForSyntax CreateForSyntax(string indexIdentifier, SyntaxBase inSyntax, SyntaxBase body)
        {
            // generates "[for <identifier> in <inSyntax>: <body>]"
            return new(
                LeftSquareToken,
                CreateToken(TokenType.Identifier, "for"),
                new LocalVariableSyntax(new IdentifierSyntax(CreateToken(TokenType.Identifier, indexIdentifier))),
                CreateToken(TokenType.Identifier, "in"),
                inSyntax,
                ColonToken,
                body,
                RightSquareToken);
        }

        public static ArraySyntax CreateArray(IEnumerable<SyntaxBase> items)
        {
            var children = new List<SyntaxBase> { NewlineToken };

            foreach (var item in items)
            {
                children.Add(CreateArrayItem(item));
                children.Add(NewlineToken);
            }

            return new ArraySyntax(
                LeftSquareToken,
                children,
                RightSquareToken);
        }

        public static SyntaxBase CreateObjectPropertyKey(string text)
        {
            if (Regex.IsMatch(text, "^[a-zA-Z][a-zA-Z0-9_]*$"))
            {
                return CreateIdentifier(text);
            }

            return CreateStringLiteral(text);
        }

        public static IntegerLiteralSyntax CreateIntegerLiteral(long value) =>
            new IntegerLiteralSyntax(CreateToken(TokenType.Integer, value.ToString()), value);

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

        public static StringSyntax CreateInterpolatedKey(SyntaxBase syntax)
        {
            var startToken = CreateStringInterpolationToken(true, false, "");
            var endToken = CreateStringInterpolationToken(false, true, "");

            return new StringSyntax(
                new [] { startToken, endToken },
                syntax.AsEnumerable(),
                new [] { "", "" });
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

        public static FunctionCallSyntax CreateFunctionCall(string functionName, params SyntaxBase[] argumentExpressions)
        {
            var arguments = argumentExpressions.Select((expression, i) => new FunctionArgumentSyntax(
                expression,
                i < argumentExpressions.Length - 1 ? CommaToken : null));

            return new FunctionCallSyntax(
                CreateIdentifier(functionName),
                LeftParenToken,
                arguments,
                RightParenToken);
        }

        public static DecoratorSyntax CreateDecorator(string functionName, params SyntaxBase[] argumentExpressions)
        {
            return new DecoratorSyntax(AtToken, CreateFunctionCall(functionName, argumentExpressions));
        }

        private static string EscapeBicepString(string value)
            => value
            .Replace("\\", "\\\\") // must do this first!
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("${", "\\${")
            .Replace("'", "\\'");

        private static string TryGetTokenText(TokenType tokenType) => tokenType switch
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
            _ => ""
        };
    }
}
