// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit;

public class ExternalInputInvocationValidator : AstVisitor
{
    private readonly SemanticModel model;
    private readonly IDiagnosticWriter diagnosticWriter;
    private ImmutableHashSet<DeclaredFunctionSymbol> visitedFunctions;

    public ExternalInputInvocationValidator(SemanticModel model, IDiagnosticWriter diagnosticWriter)
    {
        this.model = model;
        this.diagnosticWriter = diagnosticWriter;
        this.visitedFunctions = [];
    }

    public static void Validate(SemanticModel model, IDiagnosticWriter diagnosticWriter)
    {
        // externalInput invocation validation is only relevant for .bicep files
        if (model.SourceFile.FileKind != BicepSourceFileKind.BicepFile)
        {
            return;
        }
        
        var visitor = new ExternalInputInvocationValidator(model, diagnosticWriter);
        visitor.Visit(model.SourceFile.ProgramSyntax);
    }

    public override void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax)
    {
    }
    public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
    {
        ValidateFunctionCall(syntax);
        base.VisitFunctionCallSyntax(syntax);
    }

    public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
    {
        ValidateFunctionCall(syntax);
        base.VisitInstanceFunctionCallSyntax(syntax);
    }

    private void ValidateFunctionCall(FunctionCallSyntaxBase syntax)
    {
        void ValidateDeclaredFunction(DeclaredFunctionSymbol declaredFunction)
        {
            if (visitedFunctions.Contains(declaredFunction))
            {
                return;
            }

            // save previous state
            var previousVisitedFunctions = visitedFunctions;

            visitedFunctions = visitedFunctions.Add(declaredFunction);
            var lambda = declaredFunction.DeclaringFunction.Lambda;

            switch (lambda)
            {
                case TypedLambdaSyntax typedLambda:
                    Visit(typedLambda.Body);
                    break;
                // TODO: is this needed?
                case LambdaSyntax untypedLambda:
                    Visit(untypedLambda.Body);
                    break;
            }

            visitedFunctions = previousVisitedFunctions;
        }

        if (model.GetSymbolInfo(syntax) is not IFunctionSymbol symbol)
        {
            return;
        }

        switch (symbol)
        {
            case FunctionSymbol functionSymbol when functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ExternalInput):
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).ExternalInputFunctionInvocationNotAllowed(functionSymbol.Name));
                break;
            case DeclaredFunctionSymbol declaredFunctionSymbol:
                ValidateDeclaredFunction(declaredFunctionSymbol);
                break;
        }
    }
}
