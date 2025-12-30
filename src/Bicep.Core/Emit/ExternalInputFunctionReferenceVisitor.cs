// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
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
    private readonly ExpressionConverter expressionConverter;
    private readonly TemplateExpressionEvaluationHelper evaluationHelper;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.expressionConverter = new ExpressionConverter(new EmitterContext(semanticModel));
        this.evaluationHelper = new TemplateExpressionEvaluationHelper();
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
                        parameterAssignment.DeclaringParameterAssignment.Accept(visitor);
                        break;

                    case VariableSymbol variableDeclaration:
                        variableDeclaration.DeclaringVariable.Accept(visitor);
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

        if (!functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ExternalInput))
        {
            return;
        }

        try
        {
            // Extract the 'kind' and 'config' from the evaluated representation
            var evaluated = this.expressionConverter.ConvertExpression(functionCall);
            if (evaluated is not FunctionExpression functionExpression || functionExpression.Parameters.Length < 1)
            {
                return;
            }

            var evalContext = this.evaluationHelper.EvaluationContext;
            var kind = functionExpression.Parameters[0].EvaluateExpression(evalContext).ToString();
            JToken? config = null;
            if (functionExpression.Parameters.Length > 1)
            {
                config = functionExpression.Parameters[1].EvaluateExpression(evalContext);
            }

            var index = this.externalInputReferences.Count;
            var definitionKey = GetExternalInputDefinitionName(kind, index);
            this.externalInputReferences.TryAdd(functionCall, new(kind, config, definitionKey));
        }
        catch (Exception)
        {
            // Swallow any exceptions during evaluation to avoid impacting the compilation flow.
            // Syntax errors will be reported elsewhere.
            // Any evaluation errors will be reported in ParameterAssignmentEvaluator.
            return;
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

public record ExternalInputInfo(string Kind, JToken? Config, string DefinitionKey);

public record ExternalInputReferences(
    ImmutableDictionary<FunctionCallSyntaxBase, ExternalInputInfo> ExternalInputInfoBySyntax
);
