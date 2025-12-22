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

public class ExternalInputInvocationValidator(SemanticModel model, IDiagnosticWriter diagnosticWriter) : AstVisitor
{
    private ImmutableStack<string> referenceStack = [];

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


    public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
    {
        var previousStack = this.referenceStack;
        this.referenceStack = this.referenceStack.Push(syntax.Name.IdentifierName);
        base.VisitVariableDeclarationSyntax(syntax);
        this.referenceStack = previousStack;
    }

    public override void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax)
    {
        // external input invocation is allowed inside function declarations
        // so we do not need to validate the body of the function
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
            var lambda = declaredFunction.DeclaringFunction.Lambda;
            var previousStack = this.referenceStack;
            this.referenceStack = this.referenceStack.Push(declaredFunction.Name);
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
            this.referenceStack = previousStack;
        }

        if (model.GetSymbolInfo(syntax) is not IFunctionSymbol symbol)
        {
            return;
        }

        switch (symbol)
        {
            case FunctionSymbol functionSymbol when functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ParamFileImportableOnly):
                var accessChain = this.BuildAccessChain();
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).ParamImportableFunctionOnlyInvocationNotAllowed(functionSymbol.Name, accessChain));
                break;
            case DeclaredFunctionSymbol declaredFunctionSymbol:
                ValidateDeclaredFunction(declaredFunctionSymbol);
                break;
        }
    }

    private ImmutableArray<string> BuildAccessChain() => [.. this.referenceStack.Reverse()];
}
