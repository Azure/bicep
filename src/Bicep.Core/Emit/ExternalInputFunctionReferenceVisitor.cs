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
    private readonly ImmutableHashSet<FunctionCallSyntax>.Builder externalInputReferences;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.externalInputReferences = ImmutableHashSet.CreateBuilder<FunctionCallSyntax>();
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
            this.externalInputReferences.Add(syntax);
        }

        base.VisitFunctionCallSyntax(syntax);
    }

    public static ExternalInputReferencesResult CollectExternalInputReferences(SemanticModel model)
    {
        var parameterAssignments = model.Root.ParameterAssignments;
        var paramReferences = ImmutableHashSet.CreateBuilder<ParameterAssignmentSyntax>(); 
        var allFunctionRefs = ImmutableHashSet.CreateBuilder<FunctionCallSyntax>();
        foreach (var parameterAssignment in parameterAssignments)
        {
            var assignmentSyntax = parameterAssignment.DeclaringParameterAssignment;
            var functionRefs = CollectExternalInputReferencesInternal(model, assignmentSyntax);
            if (functionRefs.Count > 0)
            {
                paramReferences.Add(assignmentSyntax);
            }
            allFunctionRefs.UnionWith(functionRefs);
        }

        var externalInputIndexMap = allFunctionRefs
            .Select((functionCall, index) => (functionCall, index))
            .ToImmutableDictionary(pair => pair.functionCall, pair => pair.index);

        return new ExternalInputReferencesResult(
            ParametersReferences: paramReferences.ToImmutable(),
            ExternalInputIndexMap: externalInputIndexMap
        );
    }

    private static ImmutableHashSet<FunctionCallSyntax> CollectExternalInputReferencesInternal(
        SemanticModel model, 
        ParameterAssignmentSyntax parameterAssignment)
    {
        var visitor = new ExternalInputFunctionReferenceVisitor(model);
        visitor.Visit(parameterAssignment);
        return visitor.externalInputReferences.ToImmutable();
    }

    public record ExternalInputReferencesResult(
        // parameters that contain external input function calls
        ImmutableHashSet<ParameterAssignmentSyntax> ParametersReferences,
        // map of external input function calls to unique indexes to be used to construct externalInput definition in parameters.json
        ImmutableDictionary<FunctionCallSyntax, int> ExternalInputIndexMap
    );
}
