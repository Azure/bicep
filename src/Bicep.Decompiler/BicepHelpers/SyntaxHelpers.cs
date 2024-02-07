// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Decompiler.BicepHelpers
{
    public static class SyntaxHelpers
    {
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
            "dateTimeToEpoch",
            "dateTimeFromEpoch",
            "utcNow",
            "newGuid",
            "subscription",
            "resourceGroup",
            "deployment",
            "environment",
            "managementGroupResourceId",
            "resourceId",
            "subscriptionResourceId",
            "tenantResourceId",
            "extensionResourceId",
            "providers",
            "pickZones",
            "reference",
            "items",
        }.ToDictionary(x => x, StringComparer.OrdinalIgnoreCase);

        private static readonly IReadOnlyDictionary<string, TokenType> BinaryOperatorReplacements = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            ["add"] = TokenType.Plus,
            ["sub"] = TokenType.Minus,
            ["mul"] = TokenType.Asterisk,
            ["div"] = TokenType.Slash,
            ["mod"] = TokenType.Modulo,
            ["less"] = TokenType.LeftChevron,
            ["lessOrEquals"] = TokenType.LessThanOrEqual,
            ["greater"] = TokenType.RightChevron,
            ["greaterOrEquals"] = TokenType.GreaterThanOrEqual,
            ["equals"] = TokenType.Equals,
            ["and"] = TokenType.LogicalAnd,
            ["or"] = TokenType.LogicalOr,
            ["coalesce"] = TokenType.DoubleQuestion,
            // not an actual ARM template function but is used by Bicep to simplify the expression when decompiling
            ["notEquals"] = TokenType.NotEquals,
        };

        private static readonly IReadOnlyDictionary<string, TokenType> EmptyFunctionKeywordReplacements = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            ["null"] = TokenType.NullKeyword,
            ["true"] = TokenType.TrueKeyword,
            ["false"] = TokenType.FalseKeyword,
        };

        public static string CorrectWellKnownFunctionCasing(string functionName)
        {
            if (WellKnownFunctions.TryGetValue(functionName, out var correctedFunctionName))
            {
                return correctedFunctionName;
            }

            return functionName;
        }

        public static TokenType? TryGetBinaryOperatorReplacement(string functionName)
            => BinaryOperatorReplacements.TryGetValue(functionName, out var tokenType) ? tokenType : null;

        public static TokenType? TryGetEmptyFunctionKeywordReplacement(string functionName)
            => EmptyFunctionKeywordReplacements.TryGetValue(functionName, out var tokenType) ? tokenType : null;

        public static Token CreatePlaceholderToken(TokenType tokenType, string trailingComment)
        {
            var trailingTrivia = new SyntaxTrivia(SyntaxTriviaType.MultiLineComment, TextSpan.Nil, $"/* {trailingComment} */");

            return SyntaxFactory.CreateFreeformToken(tokenType, "?", SyntaxFactory.EmptyTrivia, SyntaxFactory.SingleSpaceTrivia.Append(trailingTrivia));
        }
    }
}
