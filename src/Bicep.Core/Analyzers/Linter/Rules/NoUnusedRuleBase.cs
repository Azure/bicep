// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules;

public abstract class NoUnusedRuleBase : LinterRuleBase
{
    protected NoUnusedRuleBase(string code, string description, DiagnosticStyling diagnosticStyling) :
        base(code, description, LinterRuleCategory.BestPractice, diagnosticStyling)
    {
    }

    protected Diagnostic CreateRemoveUnusedDiagnosticForSpan(DiagnosticLevel diagnosticLevel, string name, TextSpan nameSpan, SyntaxBase declaringSyntax, ProgramSyntax programSyntax)
    {
        var span = GetSpanForRow(programSyntax, declaringSyntax);
        var codeFix = new CodeFix(GetCodeFixDescription(name), true, CodeFixKind.QuickFix, new CodeReplacement(span, String.Empty));

        return CreateFixableDiagnosticForSpan(diagnosticLevel, nameSpan, codeFix, name);
    }

    protected Diagnostic CreateRemoveUnusedDiagnosticForSpan(DiagnosticLevel diagnosticLevel, string name, TextSpan nameSpan, TextSpan codeFixSpan)
    {
        var codeFix = new CodeFix(GetCodeFixDescription(name), true, CodeFixKind.QuickFix, new CodeReplacement(codeFixSpan, String.Empty));

        return CreateFixableDiagnosticForSpan(diagnosticLevel, nameSpan, codeFix, name);
    }

    protected static bool IsExported(SemanticModel model, StatementSyntax declaringSyntax)
        => SemanticModelHelper.TryGetDecoratorInNamespace(model, declaringSyntax, SystemNamespaceType.BuiltInName, LanguageConstants.ExportPropertyName) is not null;

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
