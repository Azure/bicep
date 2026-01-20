// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Intermediate;
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
        if (semanticModel.GetSymbolInfo(functionCallSyntax) is not FunctionSymbol functionSymbol)
        {
            return;
        }

        if (!functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresExternalInput))
        {
            return;
        }

        // External input functions should lower to the same IR, i.e. externalInput('<kind>', <config>)
        try
        {
            var intermediate = expressionConverter.ConvertExpression(functionCallSyntax);
            if (intermediate is not FunctionExpression functionExpression || functionExpression.Parameters.Length < 1)
            {
                return;
            }

            if (functionExpression.Parameters[0] is not JTokenExpression kindExpression)
            {
                return;
            }

            LanguageExpression? configExpression = null;
            if (functionExpression.Parameters.Length > 1)
            {
                configExpression = functionExpression.Parameters[1];
            }

            var index = this.externalInputReferences.Count;
            var definitionKey = GetExternalInputDefinitionName(kindExpression.Value.ToString(), index);
            externalInputReferences.TryAdd(functionCallSyntax, new(kindExpression, configExpression, definitionKey));
        }
        catch (Exception)
        {
            // we may get an exception during expression conversion e.g. due to invalid syntax.
            // diagnostics for such will be reported elsewhere.
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

public record ExternalInputInfo(LanguageExpression Kind, LanguageExpression? Config, string DefinitionKey);

public record ExternalInputReferences(
    ImmutableDictionary<FunctionCallSyntaxBase, ExternalInputInfo> ExternalInputInfoBySyntax
);
