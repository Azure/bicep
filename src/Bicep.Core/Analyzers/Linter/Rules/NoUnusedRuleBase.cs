// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules;

public abstract class NoUnusedRuleBase : LinterRuleBase
{
    protected NoUnusedRuleBase(string code, string description, DiagnosticStyling diagnosticStyling, Uri? docUri = null, DiagnosticLevel diagnosticLevel = DiagnosticLevel.Warning) :
        base(code, description, docUri, diagnosticLevel, diagnosticStyling)
    {
    }

    protected AnalyzerFixableDiagnostic CreateRemoveUnusedDiagnosticForSpan(DiagnosticLevel diagnosticLevel, string name, IdentifierSyntax nameSyntax, SyntaxBase declaringSyntax, ProgramSyntax programSyntax)
    {
        var span = GetSpanForRow(programSyntax, declaringSyntax);
        var codeFix = new CodeFix(GetCodeFixDescription(name), true, CodeFixKind.QuickFix, new CodeReplacement(span, String.Empty));
        var fixableDiagnosticForSpan = CreateFixableDiagnosticForSpan(diagnosticLevel, nameSyntax.Span, codeFix, name);
        return fixableDiagnosticForSpan;
    }

    private static TextSpan GetSpanForRow(ProgramSyntax programSyntax, SyntaxBase declaringSyntax)
    {
        // Find the first & last token in the statement
        var startToken = declaringSyntax.TryFindMostSpecificNodeInclusive(declaringSyntax.Span.Position, x => x is Token) as Token;
        var endToken = declaringSyntax.TryFindMostSpecificNodeInclusive(declaringSyntax.Span.Position + declaringSyntax.Span.Length, x => x is Token) as Token;

        // This shouldn't happen, but if it does - fall back to just replacing the statement only
        if (startToken is null || endToken is null)
        {
            return declaringSyntax.Span;
        }

        // If we have leading or trailing trivia (whitespace or comments), take the outermost trivia
        var startPosSpan = startToken.LeadingTrivia.FirstOrDefault()?.Span ?? startToken.Span;
        var endPosSpan = endToken.TrailingTrivia.LastOrDefault()?.Span ?? endToken.Span;

        // If we have a trailing newline, include it in the calculation so that it is removed
        var followingToken = programSyntax.TryFindMostSpecificNodeInclusive(endPosSpan.Position + endPosSpan.Length, x => x is Token);
        if (followingToken is Token { Type: TokenType.NewLine } newLineToken)
        {
            endPosSpan = newLineToken.Span;
        }

        return TextSpan.Between(startPosSpan, endPosSpan);
    }

    /// <summary>
    /// Abstract method each rule must implement to get description of code fix
    /// </summary>
    abstract protected string GetCodeFixDescription(string name);
}
