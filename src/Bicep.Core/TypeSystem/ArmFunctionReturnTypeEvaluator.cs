// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.TypeSystem;

public static class ArmFunctionReturnTypeEvaluator
{
    public static TypeSymbol Evaluate(
        string armFunctionName,
        TypeSymbol nonLiteralReturnType,
        out DiagnosticBuilder.DiagnosticBuilderDelegate? builderFunc,
        params TypeSymbol[] operandTypes)
    {
        var args = new FunctionArgument[operandTypes.Length];
        for (int i = 0; i < operandTypes.Length; i++)
        {
            if (TryConvertToFunctionArgument(operandTypes[i]) is not {} arg)
            {
                // if any of the input types is non-literal, we can't produce a literal return type
                builderFunc = default;
                return nonLiteralReturnType;
            }

            args[i] = arg;
        }

        return Evaluate(armFunctionName, nonLiteralReturnType, out builderFunc, args);
    }

    public static TypeSymbol Evaluate(
        string armFunctionName,
        TypeSymbol nonLiteralReturnType,
        out DiagnosticBuilder.DiagnosticBuilderDelegate? builderFunc,
        params FunctionArgument[] arguments)
    {
        if (EvaluateOperatorAsArmFunction(armFunctionName, out var result, out builderFunc, arguments))
        {
            if (TryCastToLiteral(result) is {} literalType) {
                return literalType;
            }

            // TODO add a diagnostic for this (highly unexpected!) code path
            return nonLiteralReturnType;
        }

        return nonLiteralReturnType;
    }

    /// <summary>
    /// Attempt to convert a Bicep TypeSymbol into an ARM function argument. This operation will only succeed if the
    /// provided symbol is a literal type.
    /// </summary>
    public static FunctionArgument? TryConvertToFunctionArgument(TypeSymbol typeSymbol) => ToJToken(typeSymbol) is {} jtoken ? new(jtoken) : default;

    private static JToken? ToJToken(TypeSymbol typeSymbol) => typeSymbol switch {
        BooleanLiteralType booleanLiteral => booleanLiteral.Value,
        IntegerLiteralType integerLiteral => integerLiteral.Value,
        StringLiteralType stringLiteral => stringLiteral.RawStringValue,
        ObjectType objectType => ToJToken(objectType),
        // TODO add conversion for TupleType => JArray
        _ => default,
    };

    private static JToken? ToJToken(ObjectType objectType)
    {
        var target = new JObject();

        foreach (var (name, type) in objectType.Properties)
        {
            if (ToJToken(type.TypeReference.Type) is {} jtoken)
            {
                target[name] = jtoken;
            }
            else
            {
                return default;
            }
        }

        return target;
    }

    private static bool EvaluateOperatorAsArmFunction(string armFunctionName,
        [NotNullWhen(true)] out JToken? result,
        [NotNullWhen(false)] out DiagnosticBuilder.DiagnosticBuilderDelegate? builderFunc,
        params FunctionArgument[] arguments)
    {
        try {
            result = ExpressionBuiltInFunctions.Functions.EvaluateFunction(armFunctionName, arguments, new());
            builderFunc = default;
            return true;
        }
        catch (Exception e)
        {
            // The ARM function invoked will almost certainly fail at runtime, but there's a chance a fix has been
            // deployed to ARM since this version of Bicep was released. Given that context, this failure will only
            // be reported as a warning, and the fallback type will be used.
            builderFunc = b => b.ArmFunctionLiteralTypeConversionFailedWithMessage(
                string.Join(", ", arguments.Select(a => a.Token.ToString())),
                armFunctionName,
                e.Message);
            result = default;
            return false;
        }
    }

    private static TypeSymbol? TryCastToLiteral(JToken token) => token switch {
        JValue { Value: bool boolValue } => new BooleanLiteralType(boolValue),
        JValue { Value: long longValue } => new IntegerLiteralType(longValue),
        JValue { Value: ulong ulongValue } when ulongValue <= long.MaxValue => new IntegerLiteralType((long) ulongValue),
        JValue { Value: char charValue } => new StringLiteralType(charValue.ToString()),
        JValue { Value: string stringValue } => new StringLiteralType(stringValue),
        JObject jObject => TryCastToLiteral(jObject),
        // TODO add conversion for JArray => TupleType
        _ => default,
    };

    private static TypeSymbol? TryCastToLiteral(JObject jObject)
    {
        List<TypeProperty> convertedProperties = new();
        foreach (var prop in jObject.Properties())
        {
            if (TryCastToLiteral(prop.Value) is TypeSymbol propType)
            {
                convertedProperties.Add(new(prop.Name, propType, TypePropertyFlags.Required|TypePropertyFlags.DisallowAny));
            } else
            {
                return default;
            }
        }

        return new ObjectType("inferredType", TypeSymbolValidationFlags.Default, convertedProperties, additionalPropertiesType: default);
    }
}
