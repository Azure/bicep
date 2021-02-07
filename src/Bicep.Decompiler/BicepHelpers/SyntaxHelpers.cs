// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Decompiler.BicepHelpers
{
    public static class SyntaxHelpers
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
            var children = new List<SyntaxBase>();
            children.Add(SyntaxHelpers.NewlineToken);

            foreach (var property in properties)
            {
                children.Add(property);
                children.Add(SyntaxHelpers.NewlineToken);
            }

            return new ObjectSyntax(
                SyntaxHelpers.CreateToken(TokenType.LeftBrace, "{"),
                children,
                SyntaxHelpers.CreateToken(TokenType.RightBrace, "}"));
        }

        public static ArrayItemSyntax CreateArrayItem(SyntaxBase value)
            => new ArrayItemSyntax(value);

        public static ArraySyntax CreateArray(IEnumerable<SyntaxBase> items)
        {
            var children = new List<SyntaxBase>();
            children.Add(SyntaxHelpers.NewlineToken);

            foreach (var item in items)
            {
                children.Add(CreateArrayItem(item));
                children.Add(SyntaxHelpers.NewlineToken);
            }

            return new ArraySyntax(
                SyntaxHelpers.CreateToken(TokenType.LeftSquare, "["),
                children,
                SyntaxHelpers.CreateToken(TokenType.RightSquare, "]"));
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

        public static StringSyntax CreateInterpolatedKey(SyntaxBase syntax)
        {
            var startToken = CreateStringInterpolationToken(true, false, "");
            var endToken = CreateStringInterpolationToken(false, true, "");

            return new StringSyntax(
                new [] { startToken, endToken },
                syntax.AsEnumerable(),
                new [] { "", "" });
        }

        public static string EscapeBicepString(string value)
            => value
            .Replace("\\", "\\\\") // must do this first!
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("${", "\\${")
            .Replace("'", "\\'");

        public static Token? TryGetBinaryOperatorReplacement(string bannedFunctionName) => bannedFunctionName switch
        {
            "add" => CreateToken(TokenType.Plus, "+"),
            "sub" => CreateToken(TokenType.Minus, "-"),
            "mul" => CreateToken(TokenType.Asterisk, "*"),
            "div" => CreateToken(TokenType.Slash, "/"),
            "mod" => CreateToken(TokenType.Modulo, "%"),
            "less" => CreateToken(TokenType.LessThan, "<"),
            "lessOrEquals" => CreateToken(TokenType.LessThanOrEqual, "<="),
            "greater" => CreateToken(TokenType.GreaterThan, ">"),
            "greaterOrEquals" => CreateToken(TokenType.GreaterThanOrEqual, ">="),
            "equals" => CreateToken(TokenType.Equals, "=="),
            "and" => CreateToken(TokenType.LogicalAnd, "&&"),
            "or" => CreateToken(TokenType.LogicalOr, "||"),
            "coalesce" => CreateToken(TokenType.DoubleQuestion, "??"),
            _ => null,
        };

        private static readonly IReadOnlyDictionary<string, string> WellKnownFunctions = new[]
        {
            "any",
            "concat",
            "format",
            "base64",
            "padLeft",
            "replace",
            "toLower",
            "toUpper",
            "length",
            "split",
            "add",
            "sub",
            "mul",
            "div",
            "mod",
            "string",
            "int",
            "uniqueString",
            "guid",
            "trim",
            "uri",
            "substring",
            "take",
            "skip",
            "empty",
            "contains",
            "intersection",
            "union",
            "first",
            "last",
            "indexOf",
            "lastIndexOf",
            "startsWith",
            "endsWith",
            "min",
            "max",
            "range",
            "base64ToString",
            "base64ToJson",
            "uriComponentToString",
            "uriComponent",
            "dataUriToString",
            "dataUri",
            "array",
            "createArray",
            "coalesce",
            "bool",
            "less",
            "lessOrEquals",
            "greater",
            "greaterOrEquals",
            "equals",
            "json",
            "not",
            "and",
            "or",
            "if",
            "dateTimeAdd",
            "utcNow",
            "newGuid",
            "subscription",
            "resourceGroup",
            "deployment",
            "environment",
            "resourceId",
            "subscriptionResourceId",
            "tenantResourceId",
            "extensionResourceId",
            "providers",
            "pickZones",
            "reference",
        }.ToDictionary(x => x, StringComparer.OrdinalIgnoreCase);

        public static string CorrectWellKnownFunctionCasing(string functionName)
        {
            if (WellKnownFunctions.TryGetValue(functionName, out var correctedFunctionName))
            {
                return correctedFunctionName;
            }

            return functionName;
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
    }
}