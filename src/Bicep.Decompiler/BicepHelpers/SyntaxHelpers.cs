// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static IReadOnlyDictionary<string, TokenType> BinaryOperatorReplacements = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            ["add"] = TokenType.Plus,
            ["sub"] = TokenType.Minus,
            ["mul"] = TokenType.Asterisk,
            ["div"] = TokenType.Slash,
            ["mod"] = TokenType.Modulo,
            ["less"] = TokenType.LessThan,
            ["lessOrEquals"] = TokenType.LessThanOrEqual,
            ["greater"] = TokenType.GreaterThan,
            ["greaterOrEquals"] = TokenType.GreaterThanOrEqual,
            ["equals"] = TokenType.Equals,
            ["and"] = TokenType.LogicalAnd,
            ["or"] = TokenType.LogicalOr,
            ["coalesce"] = TokenType.DoubleQuestion,
        };

        public static string CorrectWellKnownFunctionCasing(string functionName)
        {
            if (WellKnownFunctions.TryGetValue(functionName, out var correctedFunctionName))
            {
                return correctedFunctionName;
            }

            return functionName;
        }

        public static TokenType? TryGetBinaryOperatorReplacement(string bannedFunctionName)
            => BinaryOperatorReplacements.TryGetValue(bannedFunctionName, out var tokenType) ? tokenType : null;
    }
}