// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection.Metadata;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Emit;

public sealed class ExternalInputFunctionReferenceVisitor : AstVisitor
{
    private readonly SemanticModel semanticModel;
    private ParameterAssignmentSyntax? targetParameterAssignment;
    private VariableDeclarationSyntax? targetVariableDeclaration;
    private readonly ImmutableHashSet<ParameterAssignmentSyntax>.Builder parametersContainingExternalInput;
    private readonly ImmutableHashSet<VariableDeclarationSyntax>.Builder variablesContainingExternalInput;
    private readonly ImmutableDictionary<FunctionCallSyntax, int>.Builder externalInputReferences;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.externalInputReferences = ImmutableDictionary.CreateBuilder<FunctionCallSyntax, int>();
        this.parametersContainingExternalInput = ImmutableHashSet.CreateBuilder<ParameterAssignmentSyntax>();
        this.variablesContainingExternalInput = ImmutableHashSet.CreateBuilder<VariableDeclarationSyntax>();
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
        if (string.Equals(syntax.Name.IdentifierName, LanguageConstants.ExternalInputBicepFunctionName, LanguageConstants.IdentifierComparison))
        {
            this.externalInputReferences.TryAdd(syntax, this.externalInputReferences.Count);
            if (this.targetParameterAssignment is not null)
            {
                this.parametersContainingExternalInput.Add(this.targetParameterAssignment);
            }
            
            if (this.targetVariableDeclaration is not null)
            {
                this.variablesContainingExternalInput.Add(this.targetVariableDeclaration);
            }
        }

        base.VisitFunctionCallSyntax(syntax);
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
}

public record ExternalInputReferences(
    // parameters that contain external input function calls
    ImmutableHashSet<ParameterAssignmentSyntax> ParametersReferences,
    // variables that contain external input function calls
    ImmutableHashSet<VariableDeclarationSyntax> VariablesReferences,
    // map of external input function calls to unique indexes to be used to construct externalInput definition in parameters.json
    ImmutableDictionary<FunctionCallSyntax, int> ExternalInputIndexMap
);
