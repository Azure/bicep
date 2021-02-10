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

        public static string CorrectWellKnownFunctionCasing(string functionName)
        {
            if (WellKnownFunctions.TryGetValue(functionName, out var correctedFunctionName))
            {
                return correctedFunctionName;
            }

            return functionName;
        }

        public static Token? TryGetBinaryOperatorReplacement(string bannedFunctionName) => bannedFunctionName switch
        {
            "add" => SyntaxFactory.CreateToken(TokenType.Plus),
            "sub" => SyntaxFactory.CreateToken(TokenType.Minus),
            "mul" => SyntaxFactory.CreateToken(TokenType.Asterisk),
            "div" => SyntaxFactory.CreateToken(TokenType.Slash),
            "mod" => SyntaxFactory.CreateToken(TokenType.Modulo),
            "less" => SyntaxFactory.CreateToken(TokenType.LessThan),
            "lessOrEquals" => SyntaxFactory.CreateToken(TokenType.LessThanOrEqual),
            "greater" => SyntaxFactory.CreateToken(TokenType.GreaterThan),
            "greaterOrEquals" => SyntaxFactory.CreateToken(TokenType.GreaterThanOrEqual),
            "equals" => SyntaxFactory.CreateToken(TokenType.Equals),
            "and" => SyntaxFactory.CreateToken(TokenType.LogicalAnd),
            "or" => SyntaxFactory.CreateToken(TokenType.LogicalOr),
            _ => null,
        };
    }
}