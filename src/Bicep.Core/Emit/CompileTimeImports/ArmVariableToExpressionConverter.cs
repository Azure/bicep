// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit.CompileTimeImports;

internal class ArmVariableToExpressionConverter
{
    private const string VariablesFunctionName = "variables";
    private const string CopyIndexFunctionName = "copyindex";

    private readonly TemplateVariablesEvaluator evaluator;
    private readonly ImmutableDictionary<string, string> variableNameToSymbolNameMapping;
    private readonly SyntaxBase? sourceSyntax;
    private string? activeCopyLoopName;

    internal ArmVariableToExpressionConverter(TemplateVariablesEvaluator evaluator, ImmutableDictionary<string, string> variableNameToSymbolNameMapping, SyntaxBase? sourceSyntax)
    {
        this.evaluator = evaluator;
        this.variableNameToSymbolNameMapping = variableNameToSymbolNameMapping;
        this.sourceSyntax = sourceSyntax;
    }

    internal DeclaredVariableExpression ConvertToExpression(string convertedSymbolName, string originalName)
        => new(sourceSyntax,
            convertedSymbolName,
            ConvertToExpression(originalName),
            // Variables cannot have descriptions in an ARM template -- this is only supported in Bicep
            Description: null,
            // An imported variable is never automatically re-exported
            Exported: null);

    private Expression ConvertToExpression(string originalName)
    {
        if (evaluator.TryGetUnevaluatedDeclaringToken(originalName) is JToken singularDeclaration)
        {
            return ConvertToExpression(ExpressionsEngine.ParseLanguageExpressionsRecursive(singularDeclaration), singularDeclaration);
        }

        if (evaluator.TryGetUnevaluatedCopyDeclaration(originalName) is not {} copyDeclaration)
        {
            throw new InvalidOperationException($"Variable {originalName} was not found in template.");
        }

        this.activeCopyLoopName = originalName;
        var expression = new ForLoopExpression(sourceSyntax,
            new FunctionCallExpression(sourceSyntax, "range", ImmutableArray.Create(ExpressionFactory.CreateIntegerLiteral(0, sourceSyntax),
                ConvertToExpression(ExpressionsEngine.ParseLanguageExpressionsRecursive(copyDeclaration.CountToken), copyDeclaration.CountToken))),
            ConvertToExpression(ExpressionsEngine.ParseLanguageExpressionsRecursive(copyDeclaration.ValueItemToken), copyDeclaration.ValueItemToken),
            null,
            null);
        this.activeCopyLoopName = null;
        return expression;
    }

    private Expression ConvertToExpression(IReadOnlyDictionary<JToken, LanguageExpression> parsedExpressions, JToken toConvert)
    {
        if (parsedExpressions.TryGetValue(toConvert, out var armExpression))
        {
            return ConvertToExpression(armExpression);
        }

        return toConvert switch
        {
            JObject objectToCovert => ExpressionFactory.CreateObject(
                objectToCovert.Properties().Select(jProperty => new ObjectPropertyExpression(sourceSyntax,
                    ConvertToExpression(parsedExpressions, jProperty.Name),
                    ConvertToExpression(parsedExpressions, jProperty.Value))),
                sourceSyntax),
            JArray arrayToConvert => ExpressionFactory.CreateArray(
                arrayToConvert.Select(item => ConvertToExpression(parsedExpressions, item)),
                sourceSyntax),
            _ => toConvert.Type switch
            {
                JTokenType.Integer => ExpressionFactory.CreateIntegerLiteral(toConvert.ToObject<long>(), sourceSyntax),
                // there's no Bicep expression that corresponds to .Float, so use a `json('<float>')` function expression
                JTokenType.Float => new FunctionCallExpression(sourceSyntax,
                    "json",
                    ImmutableArray.Create<Expression>(ExpressionFactory.CreateStringLiteral(toConvert.ToString()))),
                JTokenType.Boolean => ExpressionFactory.CreateBooleanLiteral(toConvert.ToObject<bool>(), sourceSyntax),
                JTokenType.Null => new NullLiteralExpression(sourceSyntax),
                // everything else (.String, .Date, .Uri, etc.) is some specialization of string
                _ => ExpressionFactory.CreateStringLiteral(toConvert.ToString(), sourceSyntax),
            },
        };
    }

    private Expression ConvertToExpression(LanguageExpression armExpression) => armExpression switch
    {
        JTokenExpression jTokenExpression => ConvertToExpression(ImmutableDictionary<JToken, LanguageExpression>.Empty, jTokenExpression.Value),
        FunctionExpression functionExpression => ConvertToExpression(functionExpression),
        _ => throw new InvalidOperationException($"Encountered an unrecognized LanguageExpression of type {armExpression.GetType().Name}"),
    };

    private Expression ConvertToExpression(FunctionExpression func)
    {
        if (func.Properties?.LastOrDefault() is LanguageExpression outermostPropertyAccess)
        {
            var baseExpression = ConvertToExpression(new FunctionExpression(func.Function,
                func.Parameters,
                func.Properties.Take(func.Properties.Length - 1).ToArray()));

            return ConvertToExpression(outermostPropertyAccess) switch
            {
                StringLiteralExpression stringLiteralPropertyName when Lexer.IsValidIdentifier(stringLiteralPropertyName.Value) => new PropertyAccessExpression(sourceSyntax, baseExpression, stringLiteralPropertyName.Value, AccessExpressionFlags.None),
                Expression otherwise => new ArrayAccessExpression(sourceSyntax, baseExpression, otherwise, AccessExpressionFlags.None),
            };
        }

        return func.Function.ToLowerInvariant() switch
        {
            VariablesFunctionName => ConvertToExpression(func.Parameters.Single()) switch
            {
                StringLiteralExpression constantVariableName => new SynthesizedVariableReferenceExpression(sourceSyntax,
                    variableNameToSymbolNameMapping[constantVariableName.Value]),
                // if the argument to variables() was itself a runtime-evaluated expression, just treat this as a function call
                Expression otherwise => new FunctionCallExpression(sourceSyntax, VariablesFunctionName, ImmutableArray.Create(otherwise)),
            },
            CopyIndexFunctionName when evaluator.Evaluate(func.Parameters[0]) is TemplateVariablesEvaluator.EvaluatedValue { Value: JValue { Value: string copyIndexName } } &&
                StringComparer.OrdinalIgnoreCase.Equals(this.activeCopyLoopName, copyIndexName) => func.Parameters.Skip(1).FirstOrDefault() switch
                {
                    LanguageExpression startIndexExpression => new BinaryExpression(sourceSyntax,
                        BinaryOperator.Add,
                        new CopyIndexExpression(sourceSyntax, variableNameToSymbolNameMapping[activeCopyLoopName]),
                        ConvertToExpression(startIndexExpression)),
                    _ => new CopyIndexExpression(sourceSyntax, variableNameToSymbolNameMapping[activeCopyLoopName]),
                },
            // this is less robust than decompilation analysis (e.g., the "add" function will not be transformed to a binary expression), but since this expression is produced only to be lightly manipulated and recompiled to ARM JSON, it's fine to be lax here
            // this choice should be revisited if this converter is used outside of the Bicep.Core.Emit namespace
            string otherwise => new FunctionCallExpression(sourceSyntax, otherwise, func.Parameters.Select(ConvertToExpression).ToImmutableArray()),
        };
    }
}
