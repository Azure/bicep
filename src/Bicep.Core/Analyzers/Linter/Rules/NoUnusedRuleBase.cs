// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules;

public abstract class NoUnusedRuleBase : LinterRuleBase
{
    private readonly string type;

    protected NoUnusedRuleBase(string type, string code, string description, Uri? docUri = null, DiagnosticLevel diagnosticLevel = DiagnosticLevel.Warning, DiagnosticLabel? diagnosticLabel = null) :
        base(code, description, docUri, diagnosticLevel, diagnosticLabel)
    {
        this.type = type;
    }

    protected AnalyzerFixableDiagnostic GetFixableDiagnosticForSpan(string name, IdentifierSyntax nameSyntax, SyntaxBase declaringSyntax, ImmutableArray<int> lineStarts)
    {
        var span = GetSpanForRow(declaringSyntax, nameSyntax, lineStarts);
        var codeFix = new CodeFix($"Remove unused {type}", true, CodeFixKind.QuickFix, new CodeReplacement(span, String.Empty));
        var fixableDiagnosticForSpan = CreateFixableDiagnosticForSpan(nameSyntax.Span, codeFix, name);
        return fixableDiagnosticForSpan;
    }

    private static TextSpan GetSpanForRow(SyntaxBase declaringSyntax, IdentifierSyntax identifierSyntax, ImmutableArray<int> lineStarts)
    {
        var spanPosition = declaringSyntax.Span.Position;
        var (line, _) = TextCoordinateConverter.GetPosition(lineStarts, identifierSyntax.Span.Position);
        if (lineStarts.Length <= line + 1)
        {
            return declaringSyntax.Span;
        }

        var nextLineStart = lineStarts[line + 1];
        var span = new TextSpan(spanPosition, nextLineStart - spanPosition);
        return span;
    }
}
