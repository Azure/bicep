// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem.Types;
using Newtonsoft.Json.Linq;

using ExpressionEvaluationContext = Azure.Deployments.Expression.Intermediate.ExpressionEvaluationContext;

namespace Bicep.Core.TypeSystem;

public static class ArmFunctionReturnTypeEvaluator
{
    private static readonly PositionalMetadata UnknownPosition = new();
    private static readonly ExpressionEvaluationContext BuiltInsOnly = new([ExpressionBuiltInFunctions.Functions]);

    public static TypeSymbol? TryEvaluate(
        string armFunctionName,
        out IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> diagnosticBuilders,
        IEnumerable<TypeSymbol> operandTypes)
    {
        List<DiagnosticBuilder.DiagnosticBuilderDelegate> builderDelegates = new();
        diagnosticBuilders = builderDelegates;

        var argsBuilder = ImmutableArray.CreateBuilder<ITemplateLanguageExpression>();

        foreach (var operandType in operandTypes)
        {
            if (ToLanguageExpression(operandType) is not ITemplateLanguageExpression converted)
            {
                // if any of the input types is non-literal, we can't produce a literal return type
                return null;
            }

            argsBuilder.Add(converted);
        }

        if (EvaluateOperatorAsArmFunction(armFunctionName, argsBuilder.ToImmutable(), out var result, out var builderFunc))
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

    private static ITemplateLanguageExpression? ToLanguageExpression(TypeSymbol typeSymbol) => typeSymbol switch
    {
        BooleanLiteralType booleanLiteral => new BooleanExpression(booleanLiteral.Value, position: null),
        IntegerLiteralType integerLiteral => new IntegerExpression(integerLiteral.Value, position: null),
        StringLiteralType stringLiteral => stringLiteral.RawStringValue.AsExpression(),
        NullType => new NullExpression(position: null),
        ObjectType objectType => ToLanguageExpression(objectType),
        TupleType tupleType => ToLanguageExpression(tupleType),
        // This converter does not handle union types, as a union conversion will take m^n times as many computations,
        // where m == the average number of union type members defined for each function argument and n == the number of
        // function arguments. E.g, the return type of concat('foo'|'bar', 'fizz'|'buzz', 'snap'|'crackle') should be a
        // union type with 8 (2^3) members. Given the size of some enums defined by Azure services (such as the set of
        // all Azure regions or the set of all compute VM SKUs), the computational complexity of this evaluator could
        // spiral out of control pretty easily (and the resultant return type wouldn't be very useful).
        _ => null,
    };

    private static ObjectExpression? ToLanguageExpression(ObjectType objectType)
    {
        // If an object allows additional properties, then it cannot be cast to a literal
        if (objectType.AdditionalProperties is not null)
        {
            return null;
        }

        var properties = new KeyValuePair<ITemplateLanguageExpression, ITemplateLanguageExpression>[objectType.Properties.Count];
        int i = 0;
        foreach (var (key, property) in objectType.Properties)
        {
            if (ToLanguageExpression(property.TypeReference.Type) is not ITemplateLanguageExpression converted)
            {
                return null;
            }

            properties[i++] = new(key.AsExpression(), converted);
        }

        return new(properties, position: null);
    }

    private static ArrayExpression? ToLanguageExpression(TupleType tupleType)
    {
        var items = ImmutableArray.CreateBuilder<ITemplateLanguageExpression>(tupleType.Items.Length);
        foreach (var item in tupleType.Items)
        {
            if (ToLanguageExpression(item.Type) is not ITemplateLanguageExpression converted)
            {
                return null;
            }

            items.Add(converted);
        }

        return new(items.ToImmutable(), position: null);
    }

    private static bool EvaluateOperatorAsArmFunction(
        string armFunctionName,
        ImmutableArray<ITemplateLanguageExpression> arguments,
        [NotNullWhen(true)] out ITemplateLanguageExpression? result,
        [NotNullWhen(false)] out DiagnosticBuilder.DiagnosticBuilderDelegate? builderFunc)
    {
        try
        {
            result = BuiltInsOnly.GetEvaluator(armFunctionName, UnknownPosition)
                .Invoke(BuiltInsOnly, armFunctionName, arguments, UnknownPosition);
            builderFunc = null;
            return true;
        }
        catch (Exception e)
        {
            // The ARM function invoked will almost certainly fail at runtime, but there's a chance a fix has been
            // deployed to ARM since this version of Bicep was released. Given that context, this failure will only
            // be reported as a warning, and the fallback type will be used.
            builderFunc = b => b.ArmFunctionLiteralTypeConversionFailedWithMessage(
                string.Join(", ", arguments.Select(a => a.SerializeToJToken().ToString())),
                armFunctionName,
                e.Message);
            result = null;
            return false;
        }
    }
}
