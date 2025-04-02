// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Emit;

public sealed partial class ExternalInputFunctionReferenceVisitor : AstVisitor
{
    private readonly SemanticModel semanticModel;
    private ParameterAssignmentSyntax? targetParameterAssignment;
    private VariableDeclarationSyntax? targetVariableDeclaration;
    private readonly ImmutableHashSet<ParameterAssignmentSymbol>.Builder parametersContainingExternalInput;
    private readonly ImmutableHashSet<VariableSymbol>.Builder variablesContainingExternalInput;
    private readonly ImmutableDictionary<FunctionCallSyntaxBase, string>.Builder externalInputReferences;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.externalInputReferences = ImmutableDictionary.CreateBuilder<FunctionCallSyntaxBase, string>();
        this.parametersContainingExternalInput = ImmutableHashSet.CreateBuilder<ParameterAssignmentSymbol>();
        this.variablesContainingExternalInput = ImmutableHashSet.CreateBuilder<VariableSymbol>();
    }

    public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
    {
        var symbol = semanticModel.GetSymbolInfo(syntax);

        if (symbol is DeclaredSymbol declaredSymbol && declaredSymbol.Type is ErrorType)
        {
            return;
        }

        switch (symbol)
        {
            case ParameterAssignmentSymbol parameterAssignmentSymbol:
                Visit(parameterAssignmentSymbol.DeclaringSyntax);
                break;

            case VariableSymbol variableSymbol:
                Visit(variableSymbol.DeclaringSyntax);
                break;
        }

        base.VisitVariableAccessSyntax(syntax);
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
        var visitor = new ExternalInputFunctionReferenceVisitor(model);

        foreach (var paramAssignment in model.Root.ParameterAssignments)
        {
            var declaringSyntax = paramAssignment.DeclaringParameterAssignment;
            visitor.targetParameterAssignment = declaringSyntax;
            declaringSyntax.Accept(visitor);
        }

        foreach (var variableDeclaration in model.Root.VariableDeclarations)
        {
            var declaringSyntax = variableDeclaration.DeclaringVariable;
            visitor.targetVariableDeclaration = declaringSyntax;
            declaringSyntax.Accept(visitor);
        }

        return new ExternalInputReferences(
            ParametersReferences: visitor.parametersContainingExternalInput.ToImmutable(),
            VariablesReferences: visitor.variablesContainingExternalInput.ToImmutable(),
            ExternalInputIndexMap: visitor.externalInputReferences.ToImmutable()
        );
    }

    private void VisitFunctionCallSyntaxInternal(FunctionCallSyntaxBase functionCallSyntax)
    {
        if (SemanticModelHelper.TryGetNamedFunction(
                semanticModel, 
                SystemNamespaceType.BuiltInName, 
                LanguageConstants.ExternalInputBicepFunctionName, 
                functionCallSyntax) is { } functionCall)
        {
            if (functionCallSyntax.Arguments.Length < 1 || 
                this.semanticModel.GetTypeInfo(functionCallSyntax.Arguments[0]) is not StringLiteralType stringLiteral)
            {
                return;
            }

            var index = this.externalInputReferences.Count;
            var definitionKey = GetExternalInputDefinitionName(stringLiteral.RawStringValue, index);

            this.externalInputReferences.TryAdd(functionCall, definitionKey);

            if (this.targetParameterAssignment is not null)
            {
                var symbol = semanticModel.GetSymbolInfo(this.targetParameterAssignment);
                if (symbol is ParameterAssignmentSymbol parameterAssignmentSymbol)
                {
                    this.parametersContainingExternalInput.Add(parameterAssignmentSymbol);
                }
            }
            
            if (this.targetVariableDeclaration is not null)
            {
                var symbol = semanticModel.GetSymbolInfo(this.targetVariableDeclaration);
                if (symbol is VariableSymbol variableSymbol)
                {
                    this.variablesContainingExternalInput.Add(variableSymbol);
                }
            }
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
    // parameters that contain external input function calls
    ImmutableHashSet<ParameterAssignmentSymbol> ParametersReferences,
    // variables that contain external input function calls
    ImmutableHashSet<VariableSymbol> VariablesReferences,
    // map of external input function calls to unique keys to be used to construct externalInput definition in parameters.json
    ImmutableDictionary<FunctionCallSyntaxBase, string> ExternalInputIndexMap
);
