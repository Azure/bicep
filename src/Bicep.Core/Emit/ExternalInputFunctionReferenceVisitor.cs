// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit;

public sealed partial class ExternalInputFunctionReferenceVisitor : AstVisitor
{
    private readonly SemanticModel semanticModel;
    // a FunctionSyntax can request multiple external inputs, hence the array as value
    private readonly ImmutableDictionary<FunctionCallSyntaxBase, ImmutableArray<ExternalInputInfo>>.Builder infoBySyntax;
    private readonly ImmutableDictionary<string, ExternalInputInfo>.Builder infoBySerializedExpression;
    private readonly ExpressionConverter expressionConverter;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.expressionConverter = new ExpressionConverter(new EmitterContext(semanticModel));
        this.infoBySyntax = ImmutableDictionary.CreateBuilder<FunctionCallSyntaxBase, ImmutableArray<ExternalInputInfo>>();
        this.infoBySerializedExpression = ImmutableDictionary.CreateBuilder<string, ExternalInputInfo>();
    }

    public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
    {
        VisitFunctionCallSyntaxInternal(syntax);
        base.VisitFunctionCallSyntax(syntax);
    }

    public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
    {
        VisitFunctionCallSyntaxInternal(syntax);
        base.VisitInstanceFunctionCallSyntax(syntax);
    }

    public static ExternalInputReferences CollectExternalInputReferences(SemanticModel model)
    {
        static void ProcessReferences(
            ExternalInputFunctionReferenceVisitor visitor,
            IEnumerable<DeclaredSymbol> declaredSymbols)
        {
            foreach (var symbol in declaredSymbols)
            {
                switch (symbol)
                {
                    case ParameterAssignmentSymbol parameterAssignment:
                        parameterAssignment.DeclaringParameterAssignment.Accept(visitor);
                        break;

                    case VariableSymbol variableDeclaration:
                        variableDeclaration.DeclaringVariable.Accept(visitor);
                        break;
                }
            }
        }

        var visitor = new ExternalInputFunctionReferenceVisitor(model);

        ProcessReferences(visitor, model.Root.ParameterAssignments);
        ProcessReferences(visitor, model.Root.VariableDeclarations);

        if (model.Root.UsingDeclarationSyntax?.Config is { } config)
        {
            // TODO update visitor to improve this
            config.Accept(visitor);
        }

        return new ExternalInputReferences(
            InfoBySyntax: visitor.infoBySyntax.ToImmutable(),
            InfoBySerializedExpression: visitor.infoBySerializedExpression.ToImmutable()
        );
    }

    private void VisitFunctionCallSyntaxInternal(FunctionCallSyntaxBase functionCallSyntax)
    {
        if (semanticModel.GetSymbolInfo(functionCallSyntax) is not FunctionSymbol functionSymbol ||
            !functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresExternalInput))
        {
            return;
        }

        try
        {
            var intermediate = expressionConverter.ConvertExpression(functionCallSyntax);
            if (intermediate is FunctionExpression functionExpression)
            {
                // it's possible the function syntax maps to multiple external inputs, e.g. concat(externalInput('input1'), externalInput('input2'))
                // therefore we need to collect all external inputs found within the reduced expression
                CollectExternalInputs(functionCallSyntax, functionExpression);
            }
        }
        catch (Exception)
        {
            // Exception during expression conversion (e.g., invalid syntax).
            // Diagnostics will be reported elsewhere.
        }
    }

    private void CollectExternalInputs(FunctionCallSyntaxBase sourceSyntax, FunctionExpression functionExpression)
    {
        if (!functionExpression.NameEquals(LanguageConstants.ExternalInputBicepFunctionName))
        {
            foreach (var parameter in functionExpression.Parameters)
            {
                if (parameter is FunctionExpression nestedFunc)
                {
                    CollectExternalInputs(sourceSyntax, nestedFunc);
                }
            }
            return;
        }

        if (functionExpression.Parameters.Length < 1 ||
            functionExpression.Parameters[0] is not JTokenExpression kindExpression)
        {
            return;
        }

        var configExpression = functionExpression.Parameters.Length > 1 ? functionExpression.Parameters[1] : null;

        // externalInput invocations with the same 'kind' and 'config' parameters should map to the same ExternalInputInfo
        var serializedExpr = ExpressionsEngine.SerializeExpression(functionExpression);
        if (infoBySerializedExpression.ContainsKey(serializedExpr))
        {
            return;
        }

        var index = infoBySerializedExpression.Count;
        var definitionKey = GetExternalInputDefinitionName(kindExpression.Value.ToString(), index);
        var externalInputInfo = new ExternalInputInfo(kindExpression, configExpression, definitionKey);

        infoBySerializedExpression.Add(serializedExpr, externalInputInfo);
        infoBySyntax.TryAdd(sourceSyntax, []);
        infoBySyntax[sourceSyntax] = infoBySyntax[sourceSyntax].Add(externalInputInfo);
    }

    private static string GetExternalInputDefinitionName(string kind, int index)
    {
        // The name of the external input definition is a combination of the kind and the index.
        // e.g. 'sys.cli' becomes 'sys_cli_0'
        var sanitizedKind = NonAlphanumericPattern().Replace(kind, "_");
        return $"{sanitizedKind}_{index}";
    }

    [GeneratedRegex(@"\W")]
    private static partial Regex NonAlphanumericPattern();
}

public record ExternalInputInfo(LanguageExpression Kind, LanguageExpression? Config, string DefinitionKey);

public record ExternalInputReferences(
    ImmutableDictionary<FunctionCallSyntaxBase, ImmutableArray<ExternalInputInfo>> InfoBySyntax,
    ImmutableDictionary<string, ExternalInputInfo> InfoBySerializedExpression
);
