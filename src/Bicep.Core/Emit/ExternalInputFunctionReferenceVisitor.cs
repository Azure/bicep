// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public sealed partial class ExternalInputFunctionReferenceVisitor : AstVisitor
{
    private readonly SemanticModel semanticModel;
    private readonly ImmutableDictionary<FunctionCallSyntaxBase, ExternalInputInfo>.Builder externalInputReferences;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.externalInputReferences = ImmutableDictionary.CreateBuilder<FunctionCallSyntaxBase, ExternalInputInfo>();
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
                        // visitor.targetParameterAssignment = parameterAssignment;
                        parameterAssignment.DeclaringParameterAssignment.Accept(visitor);
                        // visitor.targetParameterAssignment = null;
                        break;

                    case VariableSymbol variableDeclaration:
                        // visitor.targetVariableDeclaration = variableDeclaration;
                        variableDeclaration.DeclaringVariable.Accept(visitor);
                        // visitor.targetVariableDeclaration = null;
                        break;
                }
            }
        }

        var visitor = new ExternalInputFunctionReferenceVisitor(model);

        // Process the parameter assignments and variable declarations to find any direct references
        // to the external input function.
        ProcessReferences(visitor, model.Root.ParameterAssignments);
        ProcessReferences(visitor, model.Root.VariableDeclarations);

        if (model.Root.UsingDeclarationSyntax?.Config is { } config)
        {
            // TODO update visitor to improve this
            config.Accept(visitor);
        }

        return new ExternalInputReferences(
            // ParametersReferences: visitor.parametersContainingExternalInput.ToImmutable(),
            // VariablesReferences: visitor.variablesContainingExternalInput.ToImmutable(),
            ExternalInputInfoBySyntax: visitor.externalInputReferences.ToImmutable()
        );
    }

    private void VisitFunctionCallSyntaxInternal(FunctionCallSyntaxBase functionCallSyntax)
    {
        if (SemanticModelHelper.TryGetFunctionInNamespace(semanticModel, SystemNamespaceType.BuiltInName, functionCallSyntax) is not { } functionCall)
        {
            return;
        }

        if (semanticModel.GetSymbolInfo(functionCall) is not FunctionSymbol functionSymbol)
        {
            return;
        }

        var index = this.externalInputReferences.Count;

        if (functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ExternalInput))
        {
            // Extract the 'kind' and 'config' from the evaluated representation 
            var context = new EmitterContext(semanticModel);
            var converter = new ExpressionConverter(context);
            var evaluated = converter.ConvertExpression(functionCall);
            if (evaluated is not FunctionExpression functionExpression || functionExpression.Parameters.Length < 1)
            {
                return;
            }

            if (functionExpression.Parameters[0] is not JTokenExpression kindExpression)
            {
                return;
            }

            var kind = kindExpression.Value.ToString();
            JToken? config = null;
            if (functionExpression.Parameters.Length > 1)
            {
                if (functionExpression.Parameters[1] is JTokenExpression configExpression)
                {
                    config = configExpression.Value;
                }
            }

            var definitionKey = GetExternalInputDefinitionName(kind, index);
            this.externalInputReferences.TryAdd(functionCall, new(kind, config, definitionKey));
        }
    }

    private static string GetExternalInputDefinitionName(string kind, int index)
    {
        // The name of the external input definition is a combination of the kind and the index.
        // e.g. 'sys.cli' becomes 'sys_cli_0'
        var nonAlphanumericPattern = NonAlphanumericPattern();
        var sanitizedKind = nonAlphanumericPattern.Replace(kind, "_");
        return $"{sanitizedKind}_{index}";
    }

    [GeneratedRegex(@"\W", RegexOptions.Compiled)]
    private static partial Regex NonAlphanumericPattern();
}

public record ExternalInputReferences(
    ImmutableDictionary<FunctionCallSyntaxBase, ExternalInputInfo> ExternalInputInfoBySyntax
);

public record ExternalInputInfo(string Kind, JToken? Config, string DefinitionKey);