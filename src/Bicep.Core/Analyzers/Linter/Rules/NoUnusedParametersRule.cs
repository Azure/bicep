// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedParametersRule : LinterRuleBase
    {
        public new const string Code = "no-unused-params";
        public NoUnusedParametersRule() : base(
            code: Code,
            description: CoreResources.ParameterMustBeUsedRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.ParameterMustBeUsedRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            // parameters must have at least two references
            //  1) One reference will be the the paramater syntax declaration
            //  2) VariableAccessSyntax indicates a reference to the parameter
            var unreferencedParams = model.Root.ParameterDeclarations
                                    .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());

            return unreferencedParams.Select(param => GetFixableDiagnosticForSpan(param.Name, param.NameSyntax, param.DeclaringSyntax, model.SourceFile.LineStarts));
        }

        private AnalyzerFixableDiagnostic GetFixableDiagnosticForSpan(string name, IdentifierSyntax nameSyntax, SyntaxBase declaringSyntax, ImmutableArray<int> lineStarts)
        {
            var span = GetSpanForRow(declaringSyntax, nameSyntax, lineStarts);
            var codeFix = new CodeFix("Remove unused parameter", true, CodeFixKind.QuickFix, new CodeReplacement(span, String.Empty));
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
