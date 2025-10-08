// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Cli.Helpers.Repl;

public static class StructuralCompletenessHelper
{
    public static bool IsStructurallyComplete(string text)
    {
        var program = new ReplParser(text).Program();
        var completenessVisitor = new StructuralCompletenessVisitor();
        program.Accept(completenessVisitor);
        return completenessVisitor.IsComplete;
    }

    private sealed class StructuralCompletenessVisitor : CstVisitor
    {
        public bool IsComplete { get; private set; } = true;

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            IsComplete = false;
            // don't visit children - we already know it's incomplete
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            if (syntax.Value is SkippedTriviaSyntax)
            {
                IsComplete = false;
                return;
            }

            base.VisitVariableDeclarationSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            if (syntax.CloseParen is SkippedTriviaSyntax)
            {
                IsComplete = false;
                return;
            }

            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            if (syntax.CloseParen is SkippedTriviaSyntax)
            {
                IsComplete = false;
                return;
            }

            base.VisitParenthesizedExpressionSyntax(syntax);
        }
    }
}
