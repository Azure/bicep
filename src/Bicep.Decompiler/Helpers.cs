// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Decompiler
{
    public static class Helpers
    {
        public static readonly TextSpan EmptySpan = new TextSpan(0, 0);

        public static readonly IEnumerable<SyntaxTrivia> EmptyTrivia = Enumerable.Empty<SyntaxTrivia>();

        public static Token CreateToken(TokenType tokenType, string text)
            => new Token(tokenType, EmptySpan, text, EmptyTrivia, EmptyTrivia);

        public static IdentifierSyntax CreateIdentifier(string text)
            => new IdentifierSyntax(CreateToken(TokenType.Identifier, text));

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
            var stringTokens = new [] { CreateStringLiteralToken(value) };
            var rawSegments = Lexer.TryGetRawStringSegments(stringTokens) ?? throw new ArgumentException($"Error parsing string {value}");

            return new StringSyntax(
                stringTokens,
                Enumerable.Empty<SyntaxBase>(),
                rawSegments);
        }

        public static string EscapeBicepString(string value)
            => value
            .Replace("\\", "\\\\") // must do this first!
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("${", "\\${")
            .Replace("'", "\\'");

        private static IReadOnlyDictionary<string, string> WellKnownFunctions = new []
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