// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection.Metadata;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit;

public sealed class ExternalInputFunctionReferenceVisitor : AstVisitor
{
    private readonly SemanticModel semanticModel;
    private ParameterAssignmentSyntax? targetParameterAssignment;
    private readonly ImmutableHashSet<ParameterAssignmentSyntax>.Builder parametersContainingExternalInput;
    private readonly ImmutableDictionary<FunctionCallSyntax, int>.Builder externalInputReferences;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.externalInputReferences = ImmutableDictionary.CreateBuilder<FunctionCallSyntax, int>();
        this.parametersContainingExternalInput = ImmutableHashSet.CreateBuilder<ParameterAssignmentSyntax>();
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

            default:
                return;
        }

        base.VisitVariableAccessSyntax(syntax);
    }

    public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
    {
        if (string.Equals(syntax.Name.IdentifierName, "externalInput", LanguageConstants.IdentifierComparison))
        {
            this.externalInputReferences.TryAdd(syntax, this.externalInputReferences.Count);
            if (this.targetParameterAssignment is not null)
            {
                this.parametersContainingExternalInput.Add(this.targetParameterAssignment);
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

        return new ExternalInputReferences(
            ParametersReferences: visitor.parametersContainingExternalInput.ToImmutable(),
            ExternalInputIndexMap: visitor.externalInputReferences.ToImmutable()
        );
    }
}

public record ExternalInputReferences(
        // parameters that contain external input function calls
    ImmutableHashSet<ParameterAssignmentSyntax> ParametersReferences,
    // map of external input function calls to unique indexes to be used to construct externalInput definition in parameters.json
    ImmutableDictionary<FunctionCallSyntax, int> ExternalInputIndexMap
);
