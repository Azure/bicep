// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit;

public sealed partial class ExternalInputFunctionReferenceVisitor : AstVisitor
{
    private readonly SemanticModel semanticModel;
    private readonly ImmutableDictionary<FunctionCallSyntaxBase, ExternalInputInfo>.Builder externalInputReferences;
    private readonly ExpressionConverter expressionConverter;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.expressionConverter = new ExpressionConverter(new EmitterContext(semanticModel));
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
        //if (SemanticModelHelper.TryGetFunctionInNamespace(semanticModel, SystemNamespaceType.BuiltInName, functionCallSyntax) is not { } functionCall)
        //{
        //    return;
        //}

        if (semanticModel.GetSymbolInfo(functionCallSyntax) is not FunctionSymbol functionSymbol)
        {
            return;
        }

        // TODO in the extension namespace implementation:
        // Ideally extension namespace function authors shouldn't need to set this flag in the types.json at all if they have externalInputs in the "evaluatesTo" property
        // Consider using visitor pattern to determine this automatically
        if (!functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresExternalInput))
        {
            return;
        }

        // External input functions should lower to the same IR, i.e. externalInput('<kind>', <config>)
        var intermediate = expressionConverter.ConvertToIntermediateExpression(functionCallSyntax);
        if (intermediate is not FunctionCallExpression functionExpression || functionExpression.Parameters.Length < 1)
        {
            return;
        }

        if (functionExpression.Parameters[0] is not StringLiteralExpression kindExpression)
        {
            return;
        }

        Expression? configExpression = null;
        if (functionExpression.Parameters.Length > 1)
        {
            configExpression = functionExpression.Parameters[1];
        }
        var index = this.externalInputReferences.Count;
        var definitionKey = GetExternalInputDefinitionName(kindExpression.Value, index);
        externalInputReferences.TryAdd(functionCallSyntax, new(kindExpression, configExpression, definitionKey));
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

public record ExternalInputInfo(Expression Kind, Expression? Config, string DefinitionKey);

public record ExternalInputReferences(
    ImmutableDictionary<FunctionCallSyntaxBase, ExternalInputInfo> ExternalInputInfoBySyntax
);
