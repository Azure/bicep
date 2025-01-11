// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem.Types;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.TypeSystem;

public static class ArmFunctionReturnTypeEvaluator
{
    public static TypeSymbol? TryEvaluate(
        string armFunctionName,
        out IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> diagnosticBuilders,
        IEnumerable<TypeSymbol> operandTypes,
        IEnumerable<FunctionArgument>? prefixArgs = default)
    {
        var operandTypesArray = operandTypes.ToImmutableArray();
        var prefixArgsArray = prefixArgs?.ToImmutableArray() ?? [];

        List<DiagnosticBuilder.DiagnosticBuilderDelegate> builderDelegates = new();
        diagnosticBuilders = builderDelegates;

        var args = new FunctionArgument[prefixArgsArray.Length + operandTypesArray.Length];
        for (int i = 0; i < prefixArgsArray.Length; i++)
        {
            args[i] = prefixArgsArray[i];
        }

        for (int i = 0; i < operandTypesArray.Length; i++)
        {
            if (ToJToken(operandTypesArray[i]) is not JToken converted)
            {
                // if any of the input types is non-literal, we can't produce a literal return type
                return null;
            }

            args[i + prefixArgsArray.Length] = new(converted);
        }

        if (EvaluateOperatorAsArmFunction(armFunctionName, out var result, out var builderFunc, args))
        {
            if (TypeHelper.TryCreateTypeLiteral(result) is { } literalType)
            {
                return literalType;
            }
        }
        else
        {
            builderDelegates.Add(builderFunc);
        }

        return null;
    }

    private static JToken? ToJToken(TypeSymbol typeSymbol) => typeSymbol switch
    {
        BooleanLiteralType booleanLiteral => booleanLiteral.Value,
        IntegerLiteralType integerLiteral => integerLiteral.Value,
        StringLiteralType stringLiteral => stringLiteral.RawStringValue,
        NullType => JValue.CreateNull(),
        ObjectType objectType => ToJToken(objectType),
        TupleType tupleType => ToJToken(tupleType),
        // This converter does not handle union types, as a union conversion will take m^n times as many computations,
        // where m == the average number of union type members defined for each function argument and n == the number of
        // function arguments. E.g, the return type of concat('foo'|'bar', 'fizz'|'buzz', 'snap'|'crackle') should be a
        // union type with 8 (2^3) members. Given the size of some enums defined by Azure services (such as the set of
        // all Azure regions or the set of all compute VM SKUs), the computational complexity of this evaluator could
        // spiral out of control pretty easily (and the resultant return type wouldn't be very useful).
        _ => null,
    };

    private static JToken? ToJToken(ObjectType objectType)
    {
        // If an object allows additional properties, then it cannot be cast to a literal
        if (objectType.AdditionalProperties is not null)
        {
            return null;
        }

        var target = new JObject();
        foreach (var (key, property) in objectType.Properties)
        {
            if (ToJToken(property.TypeReference.Type) is not JToken converted)
            {
                return null;
            }

            target[key] = converted;
        }

        return target;
    }

    private static JToken? ToJToken(TupleType tupleType)
    {
        var target = new JArray();
        foreach (var item in tupleType.Items)
        {
            if (ToJToken(item.Type) is not JToken converted)
            {
                return null;
            }

            target.Add(converted);
        }

        return target;
    }

    private static bool EvaluateOperatorAsArmFunction(string armFunctionName,
        [NotNullWhen(true)] out JToken? result,
        [NotNullWhen(false)] out DiagnosticBuilder.DiagnosticBuilderDelegate? builderFunc,
        params FunctionArgument[] arguments)
    {
        try
        {
            result = ExpressionBuiltInFunctions.Functions.EvaluateFunction(armFunctionName, arguments, new TemplateExpressionEvaluationHelper().EvaluationContext);
            builderFunc = default;
            return true;
        }
        catch (Exception e)
        {
            // The ARM function invoked will almost certainly fail at runtime, but there's a chance a fix has been
            // deployed to ARM since this version of Bicep was released. Given that context, this failure will only
            // be reported as a warning, and the fallback type will be used.
            builderFunc = b => b.ArmFunctionLiteralTypeConversionFailedWithMessage(
                string.Join(", ", arguments.Select(a => a.TryGetToken()?.ToString())),
                armFunctionName,
                e.Message);
            result = default;
            return false;
        }
    }
}
