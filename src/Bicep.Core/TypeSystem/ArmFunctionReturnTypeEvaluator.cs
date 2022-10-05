// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        out IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> diagnosticBuilders,
        IEnumerable<TypeSymbol> operandTypes,
        IEnumerable<FunctionArgument>? prefixArgs = default)
    {
        var operandTypesArray = operandTypes.ToImmutableArray();
        var prefixArgsArray = prefixArgs?.ToImmutableArray() ?? ImmutableArray<FunctionArgument>.Empty;

        List<DiagnosticBuilder.DiagnosticBuilderDelegate> builderDelegates = new();
        diagnosticBuilders = builderDelegates;
        List<ITypeReference> possibleReturnTypes = new();
        var (argumentPermutations, permutationCount) = Permute(
            Enumerable.Range(0, operandTypesArray.Length).ToDictionary(i => i, i => ToJTokens(operandTypesArray[i]).ToImmutableArray()));

        for (int i = 0; i < permutationCount; i++)
        {
            if (EvaluatePermutation(argumentPermutations, i, armFunctionName, prefixArgsArray, out var builderDelegate) is TypeSymbol converted)
            {
                possibleReturnTypes.Add(converted);
            } else
            {
                possibleReturnTypes.Add(nonLiteralReturnType);
            }

            if (builderDelegate is not null)
            {
                builderDelegates.Add(builderDelegate);
            }
        }

        return TypeHelper.TryCollapseTypes(possibleReturnTypes) ?? nonLiteralReturnType;
    }

    private static (Dictionary<K, ImmutableArray<V>> permutations, int permutationCount) Permute<K, V>(IReadOnlyDictionary<K, ImmutableArray<V>> toPermute) where K : notnull
    {
        Dictionary<K, ImmutableArray<V>> permutations = new();
        var repeatedSequenceLength = 1;
        var targetCount = toPermute.Aggregate(1, (acc, x) => acc * x.Value.Length);
        foreach (var (key, conversions) in toPermute)
        {
            if (targetCount > conversions.Length)
            {
                var sequenceToRepeat = conversions.SelectMany(e => Enumerable.Repeat(e, repeatedSequenceLength)).ToImmutableArray();
                repeatedSequenceLength = sequenceToRepeat.Length;
                var repetitionsRequired = targetCount / sequenceToRepeat.Length;
                permutations[key] = sequenceToRepeat.SelectMany(e => Enumerable.Repeat(e, repetitionsRequired)).ToImmutableArray();
            } else
            {
                permutations[key] = conversions;
            }
        }

        return (permutations, targetCount);
    }


    private static TypeSymbol? EvaluatePermutation(IReadOnlyDictionary<int, ImmutableArray<JToken?>> argumentPermutations,
        int permutationToConvert,
        string armFunctionName,
        ImmutableArray<FunctionArgument> prefixArgs,
        out DiagnosticBuilder.DiagnosticBuilderDelegate? builderFunc)
    {
        var args = new FunctionArgument[prefixArgs.Length + argumentPermutations.Count];
        for (int j = 0; j < prefixArgs.Length; j++)
        {
            args[j] = prefixArgs[j];
        }

        for (int j = 0; j < argumentPermutations.Count; j++)
        {
            var arg = argumentPermutations[j][permutationToConvert];
            if (arg is null)
            {
                // if any of the input types is non-literal, we can't produce a literal return type
                builderFunc = null;
                return null;
            }

            args[j + prefixArgs.Length] = new(arg);
        }

        if (EvaluateOperatorAsArmFunction(armFunctionName, out var result, out builderFunc, args))
        {
            if (TryCastToLiteral(result) is {} literalType) {
                return literalType;
            }
        }

        return null;
    }

    private static IEnumerable<JToken?> ToJTokens(TypeSymbol typeSymbol) => typeSymbol switch {
        BooleanLiteralType booleanLiteral => new[] { new JValue(booleanLiteral.Value) },
        IntegerLiteralType integerLiteral => new [] { new JValue(integerLiteral.Value) },
        StringLiteralType stringLiteral => new [] { new JValue(stringLiteral.RawStringValue) },
        ObjectType objectType => ToJTokens(objectType),
        UnionType unionType => unionType.Members.SelectMany(m => ToJTokens(m.Type)),
        _ => new JToken?[] { null },
    };

    private static IEnumerable<JToken?> ToJTokens(ObjectType objectType)
    {
        var (permutations, permutationCount) = Permute(objectType.Properties.ToDictionary(kvp => kvp.Key, kvp => ToJTokens(kvp.Value.TypeReference.Type).ToImmutableArray()));

        var unsuccessfulConversionYielded = false;
        for (int i = 0; i < permutationCount; i++)
        {
            var convertedPermutation = ConvertObjectPermutation(permutations, i);
            if (convertedPermutation is JToken successfulConversion)
            {
                yield return successfulConversion;
            } else if (!unsuccessfulConversionYielded)
            {
                // Flatten out the resultant union type by yielding a null conversion result at most once
                yield return convertedPermutation;
            }
        }
    }

    private static JToken? ConvertObjectPermutation(IReadOnlyDictionary<string, ImmutableArray<JToken?>> permutations, int permutationToConvert)
    {
        var target = new JObject();
        foreach (var (key, propertyVariants) in permutations)
        {
            // If the property could not be converted to a literal in this permutation, then neither can this permutation of the object
            if (propertyVariants[permutationToConvert] is not JToken convertedProperty)
            {
                return null;
            }

            target[key] = convertedProperty;
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
        // TODO: JArrays cannot be converted to type literals in the absence of a tuple type
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
