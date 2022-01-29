// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedVariablesRule : LinterRuleBase
    {
        public new const string Code = "no-unused-vars";

        public NoUnusedVariablesRule() : base(
            code: Code,
            description: CoreResources.UnusedVariableRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }


        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UnusedVariableRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            // TODO: Performance: Use a visitor to visit VariableAccesssyntax and collects the non-error symbols into a list.
            // Then do a symbol visitor to go through all the symbols that exist and compare.
            // Same issue for unused-params rule.

            // variables must have a reference of type VariableAccessSyntax
            var unreferencedVariables = model.Root.Declarations.OfType<VariableSymbol>()
                .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any())
                .Where(sym => sym.Name != "<missing>");
            foreach (var sym in unreferencedVariables)
            {
                yield return GetFixableDiagnosticForSpan(sym.Name, sym.NameSyntax, sym.DeclaringSyntax, model.SourceFile.LineStarts);
            }

            // TODO: This will not find local variables because they are not in the top-level scope.
            // Therefore this will not find scenarios such as a loop variable that is not used within the loop

            // local variables must have a reference of type VariableAccessSyntax
            var unreferencedLocalVariables = model.Root.Declarations.OfType<LocalVariableSymbol>()
                        .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any())
                        .Where(sym => sym.Name != "<missing>");

            foreach (var sym in unreferencedLocalVariables)
            {
                yield return GetFixableDiagnosticForSpan(sym.Name, sym.NameSyntax, sym.DeclaringSyntax, model.SourceFile.LineStarts);
            }
        }

        private AnalyzerFixableDiagnostic GetFixableDiagnosticForSpan(string name, IdentifierSyntax nameSyntax, SyntaxBase declaringSyntax, ImmutableArray<int> lineStarts)
        {
            var span = GetSpanForRow(declaringSyntax, nameSyntax, lineStarts);
            var codeFix = new CodeFix("Remove unused variable", true, CodeFixKind.QuickFix, new CodeReplacement(span, String.Empty));
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
}
